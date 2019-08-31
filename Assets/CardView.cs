using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    public void TurnFaceUp(float flipSpeed){
        Sequence flipSequence = DOTween.Sequence();
        
        flipSequence
        .Append(this.transform.DORotate(new Vector3(0, -90, 0), flipSpeed / 2f)
            .OnComplete(() => { front.SetActive(true); back.SetActive(false); isFaceUp=true;}))
                .Append(this.transform.DORotate(new Vector3(0, 0, 0), flipSpeed / 2f));
    }

    public void TurnFaceDown(float flipSpeed){
        Sequence flipSequence = DOTween.Sequence();
        
        flipSequence
        .Append(this.transform.DORotate(new Vector3(0, -90, 0), flipSpeed / 2f)
            .OnComplete(() => { front.SetActive(false); back.SetActive(true); isFaceUp=false;}))
                .Append(this.transform.DORotate(new Vector3(0, 0, 0), flipSpeed / 2f));
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
        if(isFaceUp){
            ChangeSortingLayer_Recursive("Default");
            isBeingDragged = false;
            int closestColumn = SolitaireGraphics.Instance.GetClosestColumn(this.transform.position);
            if(closestColumn >= 0){
                GameManager.Instance.NotifyCardDropped_Tableu(cardData, closestColumn);
            } else if(closestColumn >= -4 && closestColumn <= -1){
                //Card dropped on Foundation pile
                int suitIndex = (-closestColumn) + 1; 
                Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];
                GameManager.Instance.NotifyCardDropped_FoundationPile(cardData, suits[suitIndex]);
            }

        }
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
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, speed * Time.deltaTime);
        if(cardAbove != null && cardAbove.isFaceUp){
            targetPosition.z = cardAbove.transform.position.z;
            cardAbove.MoveTowardsPoint_Recursive(targetPosition - offsetToCardAbove, speed * 1.5f);
        }
    }

    public void SetSortingOrderAndZDepth(int cardsBelow){
        int delta = (cardsBelow+1) + (int) this.transform.position.z;
        IncreaseSortingOrder(delta);
        
        var tmpPosition = this.transform.position;
        tmpPosition.z = -(cardsBelow+1);
        this.transform.position = tmpPosition;

        if(cardAbove != null){
            cardAbove.SetSortingOrderAndZDepth(cardsBelow+1);
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
