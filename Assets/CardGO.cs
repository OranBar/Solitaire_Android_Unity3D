using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGO : MonoBehaviour
{
    public int moveSpeed = 2;

    public SpriteRenderer mainSpriteRenderer;

    public SpriteRenderer smallSuit, bigSuit, value;

    public GameObject front, back;

    private List<SpriteRenderer> mySpriteRenderers;


    public CardGO cardAbove, cardBelow;
    public bool isFaceUp;

    private int cardId;
    private static int NEXT_AVAILABLE_CARD_ID=0;
    
    private Vector3 offsetToCardAbove;

    private void AcquireId(){
        this.cardId = CardGO.NEXT_AVAILABLE_CARD_ID;
        CardGO.NEXT_AVAILABLE_CARD_ID++;
    }

    void Awake()
    {
        AcquireId();

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
        }
    }

    void OnMouseUp()
    {
        if(isFaceUp){
            ChangeSortingLayer_Recursive("Default");
        }
    }
    Vector3 currentVelocity;

    private void OnMouseDrag()
    {
        Vector2 targetPosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = new Vector3(targetPosition2D.x, targetPosition2D.y, this.transform.position.z);

        if(isFaceUp){
            MoveTowardsPoint_Recursive(targetPosition, moveSpeed);
        }
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
    
}
