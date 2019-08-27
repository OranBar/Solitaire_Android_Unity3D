using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardColumn[] tableu;
    public List<Card> stockPile;
    public List<Card> wastePile;
    public Dictionary<Suit, List<Card>> suit_to_foundationPile;
    //public something stock;
    public DeckShuffler shuffler;

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
        SetUpTable(7);
    }

    private void SetUpTable(int noOfColumns)
    {
        if(noOfColumns <= 9){
            throw new Exception("Too Many Columns. Please choose a number <= 9");
        }

        shuffler.ShuffleDeck();
        
        //Init Tableu
        tableu = new CardColumn[noOfColumns];
        for (int i = 0; i < noOfColumns; i++)
        {
            CardColumn newCardColumn = new CardColumn();

            List<Card> faceDownCards = shuffler.DrawCards(i);
            newCardColumn.faceDownCards = new Stack<Card>(faceDownCards);

            List<Card> faceUpcard = shuffler.DrawCards(1);
            newCardColumn.faceUpCards = faceUpcard;

            tableu[i] = newCardColumn;
        }
        //Init Stock
        this.stockPile = shuffler.DrawCards(shuffler.GetRemainigCardsCount());
        //Init Foundation Piles
        this.suit_to_foundationPile = new Dictionary<Suit, List<Card>>();
        foreach(Suit suit in Enum.GetValues(typeof(Suit))){
            this.suit_to_foundationPile[suit] = new List<Card>();
        }
    }

    public List<Card> GetFoundationPile(Suit suit){
        return this.suit_to_foundationPile[suit];
    }
}
