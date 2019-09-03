using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public float moveSpeed = 2.5f;

    public SpriteRenderer mainSpriteRenderer;

    public SpriteRenderer smallSuit, bigSuit, value;

    public GameObject front, back;

    private List<SpriteRenderer> mySpriteRenderers;


    public CardView CardAbove{
        get{ return SolitaireGraphics.Instance.GetCardAbove(this);}
    }
    public CardView CardBelow{
        get{ return SolitaireGraphics.Instance.GetCardBelow(this);}
    }
    public bool isFaceUp;

    public Card cardData;
    
    private Vector3 offsetToCardAbove;

    public Vector3 positionBeforeDrag;
    // private Vector3 positionBeforeDrag;

    public GameState CurrGameState{
        get{ return GameManager.Instance.gameState; }
    }

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
        if(PauseMenu.Instance.isPaused){return;}

        if(isFaceUp){
            if(this.cardData.GetZone(CurrGameState) == Zone.Waste){
                if(this.CardAbove != null && this.CardAbove.isFaceUp){
                    return; //You can't take cards from the pile if they are not at the top.
                }
            }
            
            ChangeSortingLayer_Recursive("Selectedcards");
            isBeingDragged = true;
            positionBeforeDrag = this.transform.position;
        }
    }

    void OnMouseUp()
    {
        if(PauseMenu.Instance.isPaused){return;}

        if(isFaceUp){
            if(this.cardData.GetZone(CurrGameState) == Zone.Waste){
                if(this.CardAbove != null && this.CardAbove.isFaceUp){
                    return; //You can't take cards from the pile if they are not at the top.
                }
            }
            ChangeSortingLayer_Recursive("Default");
            isBeingDragged = false;

            TablePosition dropPosition = SolitaireGraphics.Instance.GetTablePosition(this.transform.position);
            
            // Zone dropZone = SolitaireGraphics.Instance.GetDropZone(this.transform.position);
            // int dropIndex = SolitaireGraphics.Instance.GetClosestColumn(this.transform.position);

            
            GameManager.Instance.NotifyCardDropped(cardData, dropPosition);

            // } else if(dropIndex >= -4 && dropIndex <= -1){
            //     //Card dropped on Foundation pile
            //     int suitIndex = (-dropIndex) + 1; 
            //     Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];
            //     GameManager.Instance.NotifyCardDropped_FoundationPile(cardData, suits[suitIndex]);
            // }

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
                transform.position = Vector3.SmoothDamp(transform.position, targetMovePosition.Value, ref currentVelocity, moveSpeed * Time.deltaTime);
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
        // transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, speed * Time.deltaTime);
        MoveToPoint(targetPosition);
        
        if(this.cardData.GetZone(CurrGameState) == Zone.Tableu && CardAbove != null && CardAbove.isFaceUp){
            targetPosition.z = CardAbove.transform.position.z;
            // CardAbove.MoveTowardsPoint_Recursive(targetPosition - offsetToCardAbove, speed * 1.5f);
            StartCoroutine(MoveToPoint_Delayed_Coro(CardAbove, targetPosition - offsetToCardAbove, speed, 0.01f));
        }
    }

    private IEnumerator MoveToPoint_Delayed_Coro(CardView targetCard, Vector3 targetPosition, float speed, float startDelay){
        yield return new WaitForSeconds(startDelay);
        targetCard.MoveToPoint(targetPosition);
    }

    public void SetSortingOrderAndZDepth(int cardsBelow, bool executeRecursively=true){
        // int delta = (cardsBelow+1) + (int) this.transform.position.z;
        SetSortingOrder(cardsBelow);
        
        this.transform.SetZ(-(cardsBelow+1));
        
        if(executeRecursively && CardAbove != null){
            CardAbove.SetSortingOrderAndZDepth(cardsBelow+1);
        }
    }

    public void SetSortingOrder(int cardsBelow){
        for (int i = 0; i < mySpriteRenderers.Count; i++)
        {
            mySpriteRenderers[i].sortingOrder = (cardsBelow*10) + i;
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
        if(this.CardAbove != null){
            this.CardAbove.ChangeSortingLayer_Recursive(newLayerName);
        }
    }

    public void UndoDrag(){
        MoveToPoint(positionBeforeDrag);
    }
    
}
