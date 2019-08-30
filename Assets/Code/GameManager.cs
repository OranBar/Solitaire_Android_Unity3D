using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
//---------------------------
    public ISolitaireGraphics graphics;
    public int columns_count = 7;
//---------------------------

    public CardColumn[] tableu;
    public List<Card> stockPile;
    public List<Card> wastePile;
    public Dictionary<Suit, List<Card>> suit_to_foundationPile;
    //public something stock;
    public DeckShuffler shuffler;

    public List<Move> movesHistory = new List<Move>();

    protected override void InitTon(){ 
        graphics = this.GetComponent<ISolitaireGraphics>();

        Debug.Assert(graphics != null, "Couldn't find class implementing the ISolitaireGraphics on GameObject with GameManager component ("+this.gameObject.name+"", this.gameObject);
    }

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
        graphics.SetupGraphics(tableu, stockPile);        
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

    public void NotifyCardDropped(Card selectedCard, int targetColumn){
        if(targetColumn == SolitaireGraphics.INVALID_COLUMN){
            //Card was dropped on the upper part of the table, between foundation piles and deckPile. 
            IllegalMove move = new IllegalMove(selectedCard);
            graphics.NotifyIllegalMove(move);
            Debug.Log("Illegal move");
        }
        else if(targetColumn >= -4 && targetColumn <=-1){
            //Card was dropped on foundation pile

            Debug.Log("Foundation Pile move");
        }else{
            //Card was dropped on one of the tableu's columns
            CardColumn targetCardColumn = this.tableu[targetColumn];
            Card cardToDropOn = targetCardColumn.faceUpCards.Last();
            if(IsLegalMove(selectedCard, cardToDropOn)){
                //Create move. 
                List<Card> cardsBeingMoved = this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard).ToList();
                Card targetCard = this.tableu[targetColumn].faceUpCards.Last();
                Move move = new Move(cardsBeingMoved, targetCard);
                //Graphics react to move
                graphics.NotifyLegalMove(move);
                //Store move in history
                movesHistory.Add(move);
                Debug.Log("Legal move");
                //Update Game data
            } else{
                //Put card back where it began
                IllegalMove move = new IllegalMove(selectedCard);
                graphics.NotifyIllegalMove(move);
                Debug.Log("Illegal move");
            }
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
