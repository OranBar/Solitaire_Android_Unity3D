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
    public List<ISolitaireEventsHandlers> subscribedInstances = new List<ISolitaireEventsHandlers>();
    public int columns_count = 7;
//---------------------------

    public GameState gameState;
    
    public CardColumn[] Tableu{
        get{return gameState.tableu;}
        set{ gameState.tableu = value;}
    }
    public Stack<Card> StockPile{
        get{return gameState.stockPile;}
        set{ gameState.stockPile = value;}
    }
    public Stack<Card> WastePile{
        get{ return gameState.wastePile; }
        set{ gameState.wastePile = value;}
    }
    public FoundationPile[] FoundationPiles{
        get{return gameState.foundationPiles;}
        set{ gameState.foundationPiles = value;}
    }

    public DeckShuffler shufflerClone;

    public Stack<Move> movesHistory = new Stack<Move>();

    protected override void InitTon(){ }

    public void RegisterSolitaireEventsHandler(ISolitaireEventsHandlers eventHandler){
        subscribedInstances.Add(eventHandler);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
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
        gameState = new GameState();
        this.movesHistory = new Stack<Move>();

        shufflerClone = seed.Clone() as DeckShuffler;

        SetUpTable(columns_count, seed);
        foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
            subscribedHandler.NotifyBeginGame(Tableu, StockPile);
        }
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
        Tableu = new CardColumn[noOfColumns];
        for (int i = 0; i < noOfColumns; i++)
        {
            CardColumn newCardColumn = new CardColumn();

            List<Card> faceDownCards = shuffler.DrawCards(i);
            newCardColumn.faceDownCards = new Stack<Card>(faceDownCards);
            

            List<Card> faceUpCard = shuffler.DrawCards(1);
            newCardColumn.faceUpCards = faceUpCard;

            Tableu[i] = newCardColumn;
        }
        //Init Stock
        StockPile = new Stack<Card>(shuffler.DrawCards(shuffler.GetRemainigCardsCount()));
    }

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

    private void ExecuteMove(Move move)
    {
        Card selectedCard = move.movedCards.First();

        if(move.from.zone == Zone.Tableu){
            int selectedCardColumn = selectedCard.GetColumn(move.gameSnapshot);
            //Update start tableu pile - unreference moved cards
            CardColumn startCardColum = this.Tableu[selectedCardColumn];
            startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == selectedCard).ToList();
       
            //Flip card below if needed
            Card cardToFlip = move.GetCardToFlip();
            if(cardToFlip != null){
                Card faceDownCardToFlip = this.Tableu[selectedCardColumn].faceDownCards.Pop();
                startCardColum.faceUpCards.Add(cardToFlip);
            }
        }
        if(move.from.zone == Zone.Foundation){
            //Update foundation pile - Remove moved card
            FoundationPiles[move.from.index].cards.Pop();
        }
        if(move.from.zone == Zone.Waste){
            //Update waste Pile (linked to stock pile)- Remove selected card
            this.WastePile.Pop();
        }
        //--------------------
        if(move.to.zone == Zone.Tableu){
            //Update destination tableu pile - reference moved cards
            CardColumn targetCardColumn = this.Tableu[move.to.index];
            targetCardColumn.faceUpCards.AddRange(move.movedCards);
        }
        if(move.to.zone == Zone.Foundation){
            //Update foundation pile - Add moved card
            FoundationPiles[move.to.index].cards.Push(selectedCard);
        }
    }

    public Card GetCardAbove(Card card){
        Zone cardZone = card.GetZone(gameState);
        int cardColumnIndex = card.GetColumn(gameState);

        if(cardZone == Zone.Tableu){
            CardColumn cardColumn = this.Tableu[cardColumnIndex];
            
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
        if(cardZone == Zone.Waste){
            var wastePileCards = this.WastePile.Reverse().ToList();
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
        Zone cardZone = card.GetZone(gameState);
        int cardColumnIndex = card.GetColumn(gameState);

        if(cardZone == Zone.Tableu){
            CardColumn cardColumn = this.Tableu[cardColumnIndex];
            
            List<Card> faceUpCards = cardColumn.faceUpCards;
            List<Card> faceDownCards = cardColumn.faceDownCards.Reverse().ToList();
            
            if(faceUpCards.Contains(card)){
                int index = faceUpCards.IndexOf(card);
                if(index-1 >= 0){
                    return faceUpCards[index-1];
                }else{
                    if(faceDownCards.Count > 0){
                        return faceDownCards[0];
                    }
                }
            }

            if(faceDownCards.Contains(card)){
                int index = faceDownCards.IndexOf(card);
                if(index-1 >= 0){
                    return faceDownCards[index-1];
                }else{
                    return null;
                }
            }
        }
        if(cardZone == Zone.Waste){
            var wastePileCards = this.WastePile.Reverse().ToList();
            if(wastePileCards.Contains(card)){
                int index = wastePileCards.IndexOf(card);
                if(index-1 >= 0){
                    return wastePileCards[index-1];
                }else{
                    return null;
                }
            }
        }

        return null;
    }

    public void NotifyCardDropped(Card selectedCard, TablePosition dropPosition){
        bool isValidMove = false;
        Zone selectedCardZone = selectedCard.GetZone(gameState);
        int selectedCardColumnIndex = selectedCard.GetColumn(gameState);

        TablePosition from = new TablePosition(selectedCardZone, selectedCardColumnIndex);

        if(from.zone == dropPosition.zone && from.index == dropPosition.index){
            isValidMove = false;
        }
        else if(dropPosition.zone == Zone.Tableu)
        {
            int targetColumn = dropPosition.index;
            CardColumn targetCardColumn = this.Tableu[targetColumn];
            Card cardToDropOn = targetCardColumn.faceUpCards.LastOrDefault();

            isValidMove = IsLegal_TableuMove(selectedCard, cardToDropOn);
        }
        else if(dropPosition.zone == Zone.Foundation)
        {
            isValidMove = IsLegal_FoundationMove(selectedCard, FoundationPiles[dropPosition.index]);
        } else {
            //Card was dropped on the upper part of the table, between foundation piles and deckPile, or on the same column as it started
            isValidMove = false;
        }

        if(isValidMove){
            List<Card> cardsBeingMoved = new List<Card>();
            cardsBeingMoved.Add(selectedCard);
            //If the moved card comes from the tableu, there might other cards above that need to be moved as well.
            //This doens't happen for moves where the selected card comes from foundation piles, stock pile or waste pile.
           
            if (selectedCardZone == Zone.Tableu)
            {  
                cardsBeingMoved.AddRange(this.Tableu[selectedCardColumnIndex].faceUpCards.SkipWhile(c => c != selectedCard).Skip(1));
            }
            GameState snapshot = gameState.Clone() as GameState;
            Move move = new Move(cardsBeingMoved, from, dropPosition, snapshot);
            //Notify subscribers
            foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
                subscribedHandler.NotifyLegalMove(move);
            }
            //Store move in history
            Debug.Log("Legal move");
            //Update Game data
            ExecuteMove(move);
            movesHistory.Push(move);
        } else{
            IllegalMove move = new IllegalMove(selectedCard);
            //Notify subscribers
            foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
                subscribedHandler.NotifyIllegalMove(move);
            }
            Debug.Log("Illegal move");
        }

    }
    public void NotifyStockMove(){
        
        if(StockPile.Count > 0){
            List<Card> movedCards = new List<Card>();
            movedCards.Add(StockPile.Peek());
            Move move = new Move(movedCards, new TablePosition(Zone.Stock, -1), new TablePosition(Zone.Waste, -1), gameState.Clone() as GameState);

            foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
                subscribedHandler.NotifyFlipStockCardMove(move);
            }
            //Update Game Data
            Card nextCard = StockPile.Pop();
            WastePile.Push(nextCard);
            
            movesHistory.Push(move);

        } else{
            Move move = new Move(this.WastePile.Reverse().ToList(), new TablePosition(Zone.Waste, -1), new TablePosition(Zone.Stock, -1), gameState.Clone() as GameState);

            foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
                subscribedHandler.NotifyRestoreStockpileFromWastePile(new List<Card>(WastePile));
            }
            while(WastePile.Count > 0){
                Card card = WastePile.Pop();
                StockPile.Push(card);
            }

            movesHistory.Push(move);
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

    public int serializedGameToLoad_index = 1;
    [ContextMenu("Load Game")]
    private void LoadSerializedGame(){
        using (FileStream fs = File.Open(dirPathToSerialize+"data"+serializedGameToLoad_index+".dat", FileMode.Open))
        {
            BinaryFormatter b = new BinaryFormatter();  
            this.shufflerClone = b.Deserialize(fs) as DeckShuffler;  
        }

        this.RestartGame();
    }
    
    public void UndoMove(){
        if(movesHistory.Count == 0){
            Debug.LogWarning("Can't undo since no moves have been played yet");
            return;
        }
        Move moveToUndo = movesHistory.Pop();
        this.Tableu = moveToUndo.gameSnapshot.tableu;
        this.StockPile = moveToUndo.gameSnapshot.stockPile;
        this.WastePile = moveToUndo.gameSnapshot.wastePile;
        this.FoundationPiles = moveToUndo.gameSnapshot.foundationPiles;
        foreach(ISolitaireEventsHandlers subscribedHandler in subscribedInstances){
            subscribedHandler.NotifyUndoMove(moveToUndo);
        }

    }
}

