using System;  
using System.IO;  
using System.Runtime.Serialization;  
using System.Runtime.Serialization.Formatters.Binary;  
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
    public Stack<Card> stockPile;
    public Stack<Card> wastePile;
    // public Dictionary<Suit, Stack<Card>> suit_to_foundationPile;
    public FoundationPile[] foundationPiles;
    //public something stock;
    // public DeckShuffler shuffler, shufflerClone;
    public DeckShuffler shufflerClone;

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

    [ContextMenu("InitGame")]
    public void InitGame()
    {
        DeckShuffler shuffler = new DeckShuffler();
        shuffler.ShuffleDeck();

        InitGame(shuffler);     
    }

    public void InitGame(DeckShuffler seed)
    {
        this.stockPile = new Stack<Card>();
        this.wastePile = new Stack<Card>();
        this.movesHistory = new List<Move>();

        Suit[] suits = new Suit[]{Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades};
        foundationPiles = new FoundationPile[suits.Length];
        for (int i = 0; i < suits.Length; i++)
        {
            FoundationPile pile = new FoundationPile(suits[i]);
            foundationPiles[i] = pile;
        }

        shufflerClone = seed.Clone() as DeckShuffler;

        SetUpTable(columns_count, seed);
        graphics.SetupGraphics(tableu, stockPile);        
    }

    public void RestartGame(){
        InitGame(shufflerClone);
    }
    
    private void SetUpTable(int noOfColumns, DeckShuffler shuffler)
    {
        if(noOfColumns > 9){
            throw new Exception("Too Many Columns. Please choose a number <= 9");
        }
        
        //Init Tableu
        tableu = new CardColumn[noOfColumns];
        for (int i = 0; i < noOfColumns; i++)
        {
            CardColumn newCardColumn = new CardColumn();

            List<Card> faceDownCards = shuffler.DrawCards(i);
            newCardColumn.faceDownCards = new Stack<Card>(faceDownCards);
            foreach(var card in faceDownCards){
                card.zone = Zone.Tableu;
                card.column = i;
            }

            List<Card> faceUpCard = shuffler.DrawCards(1);
            newCardColumn.faceUpCards = faceUpCard;
            faceUpCard[0].zone = Zone.Tableu;
            faceUpCard[0].column = i;

            tableu[i] = newCardColumn;
        }
        //Init Stock
        this.stockPile = new Stack<Card>(shuffler.DrawCards(shuffler.GetRemainigCardsCount()));
        foreach(var stockCard in stockPile){
            stockCard.zone = Zone.Waste;
        }
        //Init Foundation Piles
        // this.suit_to_foundationPile = new Dictionary<Suit, List<Card>>();
        // foreach(Suit suit in Enum.GetValues(typeof(Suit))){
        //     this.suit_to_foundationPile[suit] = new List<Card>();
        // }
    }

   
    // public Stack<Card> GetFoundationPile(int column){
        
    // }

    // public Stack<Card> GetFoundationPile(Suit suit){
    //     return this.suit_to_foundationPile[suit];
    // }

    public bool IsLegal_TableuMove(Card draggedCard, Card destinationCard){
        //If destinationCard is null, then we are dropping on an empty column, and we only allow kings
        if(destinationCard == null){
            return draggedCard.value == 13;
        }

        if(draggedCard.suitColor != destinationCard.suitColor){
            if(destinationCard.value == (draggedCard.value + 1)){
                return true;
            }
        }
        return false;
    }

    private bool IsLegal_FoundationMove(Card selectedCard, FoundationPile pile)
    {
        if(selectedCard.suit == pile.suit){
            if(pile.cards.Count == 0){
                return selectedCard.value == 1;
            }
            if(selectedCard.value == (pile.cards.Peek().value + 1)){
                return true;
            }
        }
        return false;
    }

    // public void NotifyCardDropped_FoundationPile(Card cardData, Suit suit)
    // {
    //     Stack<Card> foundationPile = this.foundationPiles[suit];
    //     if(IsLegal_FoundationPileMove(cardData, foundationPile)){
    //         //TODO: Legal foundation pile move
    //         //Create Move
            
    //     } else{
    //         IllegalMove move = new IllegalMove(cardData);
    //         graphics.NotifyIllegalMove(move);
    //         Debug.Log("Illegal move");
    //     }
    // }
    private void ExecuteMove(Move move)
    {
        Card selectedCard = move.movedCards.First();

        if(move.from.zone == Zone.Tableu){
            //Update start tableu pile - unreference moved cards
            CardColumn startCardColum = this.tableu[selectedCard.column];
            startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == selectedCard).ToList();
        }
        if(move.from.zone == Zone.Foundation){
            //Update foundation pile - Remove moved card
            foundationPiles[move.from.index].cards.Pop();
        }
        if(move.from.zone == Zone.Waste){
            //Update waste Pile (linked to stock pile)- Remove selected card
            this.wastePile.Pop();
        }
        //--------------------
        if(move.to.zone == Zone.Tableu){
            //Update destination tableu pile - reference moved cards
            CardColumn targetCardColumn = this.tableu[move.to.index];
            targetCardColumn.faceUpCards.AddRange(move.movedCards);
        }
        if(move.to.zone == Zone.Foundation){
            //Update foundation pile - Add moved card
            foundationPiles[move.to.index].cards.Push(selectedCard);
        }
            
        //Update cards with new zone and column 
        foreach (Card card in move.movedCards)
        {
            card.zone = move.to.zone;
            card.column = move.to.index;
        }
    }

    public Card GetCardAbove(Card card){
        if(card.zone == Zone.Tableu){
            CardColumn cardColumn = this.tableu[card.column];
            
            List<Card> faceUpCards = cardColumn.faceUpCards;
            if(faceUpCards.Contains(card)){
                int index = faceUpCards.IndexOf(card);
                if(index+1 < faceUpCards.Count ){
                    return faceUpCards[index+1];
                }else{
                    return null;
                }
            }

            List<Card> faceDownCards = cardColumn.faceDownCards.Reverse().ToList();
            if(faceDownCards.Contains(card)){
                int index = faceDownCards.IndexOf(card);
                if(index+1 < faceDownCards.Count){
                    return faceDownCards[index+1];
                }else{
                    return null;
                }
            }
        }
        if(card.zone == Zone.Waste){
            var wastePileCards = this.wastePile.Reverse().ToList();
            if(wastePileCards.Contains(card)){
                int index = wastePileCards.IndexOf(card);
                if(index+1 < wastePileCards.Count ){
                    return wastePileCards[index+1];
                }else{
                    return null;
                }
            }
        }

        return null;
    }

    public Card GetCardBelow(Card card){
        if(card.zone == Zone.Tableu){
            CardColumn cardColumn = this.tableu[card.column];
            
            List<Card> faceUpCards = cardColumn.faceUpCards;
            List<Card> faceDownCards = cardColumn.faceDownCards.Reverse().ToList();
            
            if(faceUpCards.Contains(card)){
                int index = faceUpCards.IndexOf(card);
                if(index-1 > 0){
                    return faceUpCards[index-1];
                }else{
                    if(faceDownCards.Count > 0){
                        return faceDownCards[0];
                    }
                }
            }

            if(faceDownCards.Contains(card)){
                int index = faceDownCards.IndexOf(card);
                if(index-1 > 0){
                    return faceDownCards[index-1];
                }else{
                    return null;
                }
            }
        }
        if(card.zone == Zone.Waste){
            var wastePileCards = this.wastePile.Reverse().ToList();
            if(wastePileCards.Contains(card)){
                int index = wastePileCards.IndexOf(card);
                if(index-1 > 0){
                    return wastePileCards[index-1];
                }else{
                    return null;
                }
            }
        }

        return null;
    }


    // public void NotifyCardDropped(Card selectedCard, TablePosition dropPosition){
    //     TablePosition from = new TablePosition(selectedCard.zone, selectedCard.column);
        
    //     List<Card> cardsBeingMoved = new List<Card>();
    //     cardsBeingMoved.Add(selectedCard);
    //     //If the moved card comes from the tableu, there might other cards above that need to be moved as well.
    //     //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
    //     if (selectedCard.zone == Zone.Tableu)
    //     {  //NOW
    //         cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard));
    //     }










    //     Move move = new Move(cardsBeingMoved, from, dropPosition);
    //     graphics.NotifyLegalMove(move);
    //     movesHistory.Add(move);
    // }
















    public void NotifyCardDropped(Card selectedCard, TablePosition dropPosition){
        
        // if(dropPosition.zone == Zone.NotAZone){
        //     //Card was dropped on the upper part of the table, between foundation piles and deckPile, or on the same column as it started
        //     IllegalMove move = new IllegalMove(selectedCard);
        //     graphics.NotifyIllegalMove(move);
        //     Debug.Log("Illegal move");
        // } 
        // else if(dropPosition.zone == Zone.Tableu){
        //     ProcessCardDroppedOnTableu(selectedCard, dropPosition);
        // }else if(dropPosition.zone == Zone.Foundation)
        // {
        //     ProcessCardDroppedOnFoundation(selectedCard, dropPosition);
        // }

        bool isValidMove = false;
        TablePosition from = new TablePosition(selectedCard.zone, selectedCard.column);

        if(from.zone == dropPosition.zone && from.index == dropPosition.index){
            isValidMove = false;
        }
        else if(dropPosition.zone == Zone.Tableu)
        {
            int targetColumn = dropPosition.index;
            CardColumn targetCardColumn = this.tableu[targetColumn];
            Card cardToDropOn = targetCardColumn.faceUpCards.LastOrDefault();

            isValidMove = IsLegal_TableuMove(selectedCard, cardToDropOn);
        }
        else if(dropPosition.zone == Zone.Foundation)
        {
            isValidMove = IsLegal_FoundationMove(selectedCard, foundationPiles[dropPosition.index]);
        } else {
            //Card was dropped on the upper part of the table, between foundation piles and deckPile, or on the same column as it started
            isValidMove = false;
        }

        
        

        if(isValidMove){
            List<Card> cardsBeingMoved = new List<Card>();
            cardsBeingMoved.Add(selectedCard);
            //If the moved card comes from the tableu, there might other cards above that need to be moved as well.
            //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
           
            if (selectedCard.zone == Zone.Tableu)
            {  
                cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard).Skip(1));
            }
            Move move = new Move(cardsBeingMoved, from, dropPosition);
            //Graphics react to move
            graphics.NotifyLegalMove(move);
            //Store move in history
            Debug.Log("Legal move");
            //Update Game data
            if (selectedCard.zone == Zone.Tableu)
            {  //NOW
                // cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard).Skip(1));
                this.tableu[selectedCard.column].faceUpCards = this.tableu[selectedCard.column].faceUpCards.TakeUntil(c => c == selectedCard).ToList();
                if(this.tableu[selectedCard.column].faceDownCards.Count > 0){
                    Card faceDownCardToFlip = this.tableu[selectedCard.column].faceDownCards.Pop();
                    this.tableu[selectedCard.column].faceUpCards.Add(faceDownCardToFlip);
                }
            }
            // CardColumn startCardColum = this.tableu[selectedCard.column];
            // startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == selectedCard).ToList();
            // targetCardColumn.faceUpCards.AddRange(move.movedCards);
            // foreach(Card card in move.movedCards){
            //     card.column = dropPosition.index;
            // }
            ExecuteMove(move);
            movesHistory.Add(move);
        } else{
            IllegalMove move = new IllegalMove(selectedCard);
            graphics.NotifyIllegalMove(move);
            Debug.Log("Illegal move");
        }

    }


    // public void ProcessCardDroppedOnTableu(Card selectedCard, TablePosition dropPosition){
    //     //Card was dropped on one of the tableu's columns
    //     TablePosition from = new TablePosition(selectedCard.zone, selectedCard.column);

    //     int targetColumn = dropPosition.index;
    //     CardColumn targetCardColumn = this.tableu[targetColumn];
    //     Card cardToDropOn = targetCardColumn.faceUpCards.LastOrDefault();    //TODO: thorws sequence empry exception
    //     if(IsLegal_TableuMove(selectedCard, cardToDropOn))
    //     {
    //         //Create move. 
    //         List<Card> cardsBeingMoved = new List<Card>();
    //         cardsBeingMoved.Add(selectedCard);
    //         //If the moved card comes from the tableu, there might other cards above that need to be moved as well.
    //         //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
    //         if (selectedCard.zone == Zone.Tableu)
    //         {  //NOW
    //             cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard));
    //         }

    //         // Card targetCard = this.tableu[targetColumn].faceUpCards.Last();
    //         // Move move = new Move(cardsBeingMoved, targetCard);
    //         // Move move = new Move(cardsBeingMoved, selectedCard.zone, selectedCard.column, Zone.Tableu, targetColumn);
    //         Move move = new Move(cardsBeingMoved, from, dropPosition);
    //         //Graphics react to move
    //         graphics.NotifyLegalMove(move);
    //         //Store move in history
    //         movesHistory.Add(move);
    //         Debug.Log("Legal move");
    //         //Update Game data
    //         // CardColumn startCardColum = this.tableu[selectedCard.column];
    //         // startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == selectedCard).ToList();
    //         // targetCardColumn.faceUpCards.AddRange(move.movedCards);
    //         // foreach(Card card in move.movedCards){
    //         //     card.column = dropPosition.index;
    //         // }
    //         ExecuteMove(selectedCard, dropPosition, move);

    //     }
    //     else
    //     {
    //         //Put card back where it began
    //         IllegalMove move = new IllegalMove(selectedCard);
    //         graphics.NotifyIllegalMove(move);
    //         Debug.Log("Illegal move");
    //     }
    // }


    // private void ProcessCardDroppedOnFoundation(Card selectedCard, TablePosition dropPosition)
    // {
    //     if (IsLegal_FoundationMove(selectedCard, this.foundationPiles[dropPosition.index]))
    //     {
    //         // CardColumn fromCardColumn = this.tableu[targetColumn];


    //         //Create move. 
    //         List<Card> cardsBeingMoved = new List<Card>();
    //         cardsBeingMoved.Add(selectedCard);
    //         //If the moved card comes from the tableu, there might other cards above that need to be moved as well.
    //         //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
    //         if (selectedCard.zone == Zone.Tableu)
    //         {
    //             cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard));
    //         }

    //         // Card targetCard = this.tableu[targetColumn].faceUpCards.Last();
    //         // Move move = new Move(cardsBeingMoved, targetCard);
    //         // Move move = new Move(cardsBeingMoved, selectedCard.zone, selectedCard.column, Zone.Tableu, targetColumn);
    //         TablePosition from = new TablePosition(selectedCard.zone, selectedCard.column);
    //         Move move = new Move(cardsBeingMoved, from, dropPosition);
    //         //Graphics react to move
    //         graphics.NotifyLegalMove(move);
    //         //Store move in history
    //         movesHistory.Add(move);
    //         Debug.Log("Legal move");
    //         //Update Game data


    //         //NOW
    //     }
    // }

    // public void NotifyCardDropped_Tableu(Card selectedCard, int targetColumn){
    //     if(targetColumn == SolitaireGraphics.INVALID_COLUMN || targetColumn == selectedCard.column){
    //         //Card was dropped on the upper part of the table, between foundation piles and deckPile, or on the same column as it started
    //         IllegalMove move = new IllegalMove(selectedCard);
    //         graphics.NotifyIllegalMove(move);
    //         Debug.Log("Illegal move");
    //     } else {
    //         //Card was dropped on one of the tableu's columns
    //         CardColumn targetCardColumn = this.tableu[targetColumn];
    //         Card cardToDropOn = targetCardColumn.faceUpCards.Last();    //TODO: thorws sequence empry exception
    //         if(IsLegal_TableuMove(selectedCard, cardToDropOn)){
    //             //Create move. 
    //             List<Card> cardsBeingMoved = new List<Card>();
    //             cardsBeingMoved.Add(selectedCard);
    //             //If the moved card comes from the tableu, there might other cards above that need to be moved aswell.
    //             //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
    //             if(selectedCard.column >= 0 ){
    //                 cardsBeingMoved.AddRange(this.tableu[selectedCard.column].faceUpCards.SkipWhile(c => c != selectedCard));
    //             }

    //             Card targetCard = this.tableu[targetColumn].faceUpCards.Last();
    //             // Move move = new Move(cardsBeingMoved, targetCard);
    //             Move2 move = new Move2(cardsBeingMoved, selectedCard.zone, selectedCard.column, Zone.Tableu, targetColumn);
    //             //Graphics react to move
    //             graphics.NotifyLegalMove(move);
    //             //Store move in history
    //             movesHistory.Add(move);
    //             Debug.Log("Legal move");
    //             //Update Game data
    //             CardColumn startCardColum = this.tableu[selectedCard.column];
    //             startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == selectedCard).ToList();
    //             targetCardColumn.faceUpCards.AddRange(move.movedCards);
    //             foreach(Card card in move.movedCards){
    //                 card.column = targetCard.column;
    //             }

    //         }else{
    //             //Put card back where it began
    //             IllegalMove move = new IllegalMove(selectedCard);
    //             graphics.NotifyIllegalMove(move);
    //             Debug.Log("Illegal move");
    //         }
    //     }
    // }

    //NOW Update reference to cards moved away from stock (here? maybe not)
    public void NotifyStockMove(){
        if(stockPile.Count > 0){
            graphics.NotifyFlipStockCardMove(stockPile.Peek(), new List<Card>(wastePile));

            //Update Game Data
            Card nextCard = stockPile.Pop();
            wastePile.Push(nextCard);

            // graphics.NotifyFlipStockCardMove(nextCard, wastePile.Count);
        } else{
            graphics.NotifyRestoreStockpileFromWastePile(new List<Card>(wastePile));

            while(wastePile.Count > 0){
                Card card = wastePile.Pop();
                stockPile.Push(card);
            }
        }
    }

    public string dirPathToSerialize;


    [ContextMenu("Serialize Current Game")]
    private void SerializeCurrentGame(){
        string[] filePaths = Directory.GetFiles(dirPathToSerialize, "*.dat");

        using (FileStream fs = File.Create(dirPathToSerialize+"data"+(filePaths.Length+1)+".dat"))
        {
            BinaryFormatter b = new BinaryFormatter();  
            b.Serialize(fs, this.shufflerClone);  
        }
    }

    int serializedGameToLoad_index = 0;
    [ContextMenu("Load Game")]
    private void LoadSerializedGame(){
        using (FileStream fs = File.Open(dirPathToSerialize+"data"+serializedGameToLoad_index+".dat", FileMode.Open))
        {
            BinaryFormatter b = new BinaryFormatter();  
            this.shufflerClone = b.Deserialize(fs) as DeckShuffler;  
        }

        this.RestartGame();
    }
}

