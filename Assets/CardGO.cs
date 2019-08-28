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

    
    private Vector2 offsetToCardAbove;

    void Awake()
    {

        mySpriteRenderers = new List<SpriteRenderer>();
        foreach(var cardObj_child in this.transform.GetAllChildren(true)){
            var spriteRenderer = cardObj_child.GetComponent<SpriteRenderer>();
            mySpriteRenderers.Add(spriteRenderer);
        }

        offsetToCardAbove = new Vector2(0, GameObject.FindObjectOfType<SolitaireGraphics>().faceUp_padding_y);
    }

    float x,y;
    Vector2 currentVelocity;
    bool isBeingDragged = false;

    private void OnMouseDrag()
    {
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref currentVelocity, .2f);
        if(!isFaceUp){
            MoveTowardsPoint_Recursive(targetPosition, moveSpeed);
        }
    }

    private void MoveTowardsPoint_Recursive(Vector2 targetPosition, float speed){
        //TODO: try adding deltaTime
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref currentVelocity, speed * Time.deltaTime);
        if(cardAbove != null){
            cardAbove.MoveTowardsPoint_Recursive(targetPosition - offsetToCardAbove, speed * 1.5f);
        }
    }

    public void IncreaseSortingOrder(int amountToIncrease){
        foreach(var spriteRenderer in mySpriteRenderers){
            spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + amountToIncrease;
        }
    }

    
}
