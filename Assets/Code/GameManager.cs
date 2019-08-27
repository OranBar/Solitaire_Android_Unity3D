using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardColumn[] cardColumns;
    public Shuffler shuffler;

    void Awake()
    {
         
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BeginGame(){
        InitGame();
    }

    private void InitGame()
    {
        SetUpColumns();
    }

    private void SetUpColumns()
    {
        shuffler.ShuffleDeck();

        cardColumns = new CardColumn[8];
        for (int i = 0; i < 7; i++)
        {
            CardColumn newCardColumn = new CardColumn();

            List<Card> faceDownCards = shuffler.DrawCards(i);
            newCardColumn.faceDownCards = new Stack<Card>(faceDownCards);

            List<Card> faceUpcard = shuffler.DrawCards(1);
            newCardColumn.faceUpCards = faceUpcard;

            cardColumns[i] = newCardColumn;
        }
    }
}
