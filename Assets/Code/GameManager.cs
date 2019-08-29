using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
//---------------------------
    public SolitaireGraphics graphics;
    public int columns_count = 7;
//---------------------------

    public CardColumn[] tableu;
    public List<Card> stockPile;
    public List<Card> wastePile;
    public Dictionary<Suit, List<Card>> suit_to_foundationPile;
    //public something stock;
    public DeckShuffler shuffler;

    public List<Move> movesHistory;

    protected override void InitTon(){ }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BeginGame(){
        InitGame();
    }

    [ContextMenu("InitGame")]
    private void InitGame()
    {
        shuffler = new DeckShuffler();

        SetUpTable(columns_count);
        SetUpGraphics();
        
    }
    
    private void SetUpGraphics()
    {
        graphics.SetUpGraphics(this);
    }

    private void SetUpTable(int noOfColumns)
    {
        if(noOfColumns > 9){
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
            foreach(var card in faceDownCards){
                card.column = i;
            }

            List<Card> faceUpCard = shuffler.DrawCards(1);
            newCardColumn.faceUpCards = faceUpCard;
            faceUpCard[0].column = i;

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

    public void NotifyCardDropped(Card card, int targetColumn){
        CardColumn targetCardColumn = this.tableu[targetColumn];
        Card columnCard = targetCardColumn.faceUpCards.Last();
        if(IsLegalMove(card, columnCard)){
            //Create move. 
            //Store move in history
        }
    }

    public bool IsLegalMove(Card draggedCard, Card destinationCard){
        if(draggedCard.suitColor != destinationCard.suitColor){
            if(destinationCard.value == (draggedCard.value + 1)){
                return true;
            }
        }
        return false;
    }

}
