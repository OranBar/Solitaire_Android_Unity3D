using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public int moveSpeed = 2;

    public SpriteRenderer mainSpriteRenderer;

    public SpriteRenderer smallSuit, bigSuit, value;

    public GameObject front, back;

    private List<SpriteRenderer> mySpriteRenderers;


    public CardView cardAbove, cardBelow;
    public bool isFaceUp;

    public Card cardData;
    
    private Vector3 offsetToCardAbove;

    private Vector3 positionBeforeDrag;
    // private Vector3 positionBeforeDrag;

    void Awake()
    {
        mySpriteRenderers = new List<SpriteRenderer>();
        foreach(var cardObj_child in this.transform.GetAllChildren(true)){
            var spriteRenderer = cardObj_child.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null){
                mySpriteRenderers.Add(spriteRenderer);
            }
        }

        offsetToCardAbove = new Vector3(0, GameObject.FindObjectOfType<SolitaireGraphics>().faceUp_padding_y, 0);
    }

    void OnMouseDown()
    {
        if(isFaceUp){
            ChangeSortingLayer_Recursive("Selectedcards");
            isBeingDragged = true;
            positionBeforeDrag = this.transform.position;
        }
    }

    void OnMouseUp()
    {
        ChangeSortingLayer_Recursive("Default");
        isBeingDragged = false;
        int closestColumn = SolitaireGraphics.Instance.GetClosestColumn(this.transform.position);
        GameManager.Instance.NotifyCardDropped(cardData, closestColumn);
    }

    Vector3 currentVelocity;
    bool isBeingDragged=false;
    Vector3? targetMovePosition = null;

    void Update()
    {
        if(isBeingDragged){
            Vector2 targetPosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPosition = new Vector3(targetPosition2D.x, targetPosition2D.y, this.transform.position.z);
            MoveToPoint(targetPosition);
        }
        if(targetMovePosition.HasValue){
            if(Vector3.Distance(this.transform.position, targetMovePosition.Value) > 0.0001f){
                MoveTowardsPoint_Recursive(targetMovePosition.Value, moveSpeed);
            } else {
                targetMovePosition = null;
            }
        }

    }

    public void MoveToPoint(Vector3 point){
        targetMovePosition = point;
    }

    private void MoveTowardsPoint_Recursive(Vector3 targetPosition){
        float speed = this.moveSpeed;
        MoveTowardsPoint_Recursive(targetPosition, speed);
    }

    private void MoveTowardsPoint_Recursive(Vector3 targetPosition, float speed){
        //TODO: try adding deltaTime
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, speed * Time.deltaTime);
        if(cardAbove != null){
            cardAbove.MoveTowardsPoint_Recursive(targetPosition - offsetToCardAbove, speed * 1.5f);
        }
    }

    public void IncreaseSortingOrder(int amountToIncrease){
        foreach(var spriteRenderer in mySpriteRenderers){
            spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + amountToIncrease;
        }
    }

    public void ChangeSortingLayer(string newLayerName){
        foreach(var spriteRenderer in mySpriteRenderers){
            spriteRenderer.sortingLayerName = newLayerName;
        }
    }

    public void ChangeSortingLayer_Recursive(string newLayerName){
        ChangeSortingLayer(newLayerName);
        if(this.cardAbove != null){
            this.cardAbove.ChangeSortingLayer_Recursive(newLayerName);
        }
    }

    public void UndoDrag(){
        MoveToPoint(positionBeforeDrag);
    }
    
}
