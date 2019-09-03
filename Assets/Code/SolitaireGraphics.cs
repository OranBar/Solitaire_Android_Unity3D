using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class SolitaireGraphics : Singleton<SolitaireGraphics>, ISolitaireEventsHandlers
{
//--------------------------- Inspector Variables
    public GameObject cardPrefab, foundationPilePrefab;
    public RectTransform topBar;
    public int x_padding = 5;
    public int y_padding = 50;
    public float faceDown_padding_y, faceUp_padding_y;
    public Stack<Sequence> movesUndo_sequences = new Stack<Sequence>();

    public float stockPile_padding_x = 0.3215505f;

    public AnimationCurve easeFunction;
    public float cardToTableu_animDuration = 0.6f;
    public float flipSpeed = 0.6f;

//---------------------------

    private Vector3 stockPile_pos;

    private Vector3[] foundationPilesPositions;
    private Vector3[] tableuPositions;
    private Dictionary<string, CardView> cardData_to_cardView = new Dictionary<string, CardView>();
    private Transform cardsContainer;


    protected override void InitTon(){ 
        GameManager.Instance.RegisterSolitaireEventsHandler(this);
        DOTween.SetTweensCapacity(500, 50);
    }
    
    public CardView GetCardAbove(CardView cardView)
    {
        Card cardAbove = GameManager.Instance.GetCardAbove(cardView.cardData);
        if(cardAbove == null){
            return null;
        }
        return this.cardData_to_cardView[cardAbove.ToString()];
    }

    public CardView GetCardBelow(CardView cardView)
    {
        Card cardBelow = GameManager.Instance.GetCardBelow(cardView.cardData);
        if(cardBelow == null){
            return null;
        }
        return this.cardData_to_cardView[cardBelow.ToString()];
    }


    public Vector2 ComputeCardSize_ScreenSpace(int noOfColumns, int x_padding, int y_padding){
        int screenWidth = Camera.main.pixelWidth;
        int screenHeight = Camera.main.pixelHeight;

        int spaceNeededForMargins = (noOfColumns+1)*x_padding;

        float cardWidth = (screenWidth - spaceNeededForMargins) / noOfColumns;
        // 2.5 x 3.5 inches is the standard size of poker cards
        float cardHeight = (cardWidth * 3.5f) / 2.5f;

        Vector2 suggestedCardSize = new Vector2(cardWidth, cardHeight);
        
        return suggestedCardSize;
    }

    public Vector2 ComputeCardSize_WorldSpace(int noOfColumns, int x_padding, int y_padding){
        var cardSize = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        Vector2 suggestedCardSize = Camera.main.ScreenToWorldPoint(cardSize) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        return suggestedCardSize;
    }

    public Vector3[] ComputeTableuPositions_Portrait(int noOfColumns, int x_padding, int y_padding, float topBarSize_y){
        Vector3[] tableuPortraitPositions = new Vector3[noOfColumns];
        Vector2 suggestedCardSize_sceenSpace = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        float cardWidth = suggestedCardSize_sceenSpace.x;
        float cardHeight = suggestedCardSize_sceenSpace.y;


        for (int i = 0; i < noOfColumns; i++)
        {
            float x = (x_padding*(i+1)) + (cardWidth*i) + (cardWidth/2);
            float y = ((y_padding*2) + cardHeight/2 + cardHeight);
            Vector2 cardPosition_screen = new Vector3(x, Camera.main.pixelHeight - y,0);

            Vector3 cardPosition_world = Camera.main.ScreenToWorldPoint(cardPosition_screen);
            cardPosition_world.z = 0;
            tableuPortraitPositions[i] = cardPosition_world;
            tableuPortraitPositions[i].y = tableuPortraitPositions[i].y - topBarSize_y;
        }
        return tableuPortraitPositions;
    }
    public static float ScreenSpace_To_WorldSpace(int noOfPixels){
        Vector2 result = Camera.main.ScreenToWorldPoint(new Vector2(noOfPixels, 0)) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        return result.x;
    }

    private GameObject InstantiateAndScale(GameObject prefab, Vector2 suggestedCardSize, Vector3 pos){
        var newGO = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        var cardView = newGO.GetComponent<CardView>();
        var multiplier = (suggestedCardSize.x) / (cardView.mainSpriteRenderer.size.x);
        newGO.transform.localScale = (newGO.transform.localScale) * (multiplier);
        return newGO;
    }

    public void NotifyBeginGame(CardColumn[] tableu, Stack<Card> stockPileCards)
    {
        this.foundationPilesPositions = new Vector3[4];
        this.cardData_to_cardView = new Dictionary<string, CardView>();
        if(this.cardsContainer!=null){
            GameObject.Destroy(cardsContainer.gameObject);
        }

        int columns_count = tableu.Length;
        
        //Create cards container empty gameobject - Stored as class variable for easy access in insantiation methods
        this.cardsContainer = new GameObject("Cards Container").transform;
        
        Vector2 suggestedCardSize = ComputeCardSize_WorldSpace(columns_count, x_padding, y_padding);
        Sequence animSequence = DOTween.Sequence();

        var y_padding_worldSpace = ScreenSpace_To_WorldSpace(y_padding);
        //Compute TopBarOffset
        Vector3 topBarOffset = ComputeTopBarOffset();

        //Compute tableu positions
        this.tableuPositions = ComputeTableuPositions_Portrait(columns_count, x_padding, y_padding, topBarOffset.y);

        //Init stock pile object
        InstantiateStockPile(stockPileCards, suggestedCardSize, y_padding_worldSpace);

        //Place cards on tableu with animations
        float anim_delay = 0f;

        // Loop tableu columns 1 by 1
        for (int i = 0; i < tableu.Length; i++)
        {
            var tableuColumn_pos = tableuPositions[i];

            //Foundation Pile
            if (i < 4)
            {
                Suit[] suits = new Suit[]{Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades};
                GameObject foundationPileGO = InstantiateFoundationPile(suggestedCardSize, y_padding_worldSpace, suits[i], tableuColumn_pos);
                foundationPilesPositions[i] = foundationPileGO.transform.position;
            }

            CardView[] cardPile = new CardView[i];
            //FaceDown Card Pile
            List<Card> faceDownCards_data = new List<Card>(tableu[i].faceDownCards.Reverse());
            for (int ii = 0; ii < i; ii++)
            {
                GameObject faceDownCardGO = InstantiateFaceDownCard(suggestedCardSize, cardPile, faceDownCards_data, ii);

                //Prepare Animation
                if(ii > 0){
                    tableuColumn_pos.y = tableuColumn_pos.y - faceDown_padding_y;
                }
                //If a card is above another, the Z has to reflect that, or the colliders will overlap and steal each others' calls
                tableuColumn_pos.z = faceDownCardGO.transform.position.z; 

                //Build Animation 
                faceDownCardGO.transform.DOMove(tableuColumn_pos, cardToTableu_animDuration).SetDelay(anim_delay);

                anim_delay += 0.15f;
            }
            //FaceUp Cards
            GameObject cardGO = InstantiateFaceUpCard(tableu, suggestedCardSize, i, cardPile);
            CardView cardView = cardGO.GetComponent<CardView>();

            //Prepare Animation
            if(i > 0){
                tableuColumn_pos.y = tableuColumn_pos.y - faceDown_padding_y;
            }
            //If a card is above another, the Z has to reflect that, or the colliders will overlap and steal each others' calls
            tableuColumn_pos.z = cardGO.transform.position.z; 

            //Build Animation Sequence - Move and Flip. It starts automatically
            Sequence moveAndFlipSequence = DOTween.Sequence();
            moveAndFlipSequence
                .PrependInterval(anim_delay)
                .Append(cardGO.transform.DOMove(tableuColumn_pos, cardToTableu_animDuration))
                .OnComplete(() => { cardView.TurnFaceUp(flipSpeed); });
                
            anim_delay += 0.15f;
        }
    }

    private void InstantiateStockPile(Stack<Card> stockPileCards, Vector2 suggestedCardSize, float y_padding_worldSpace)
    {
        this.stockPile_pos = tableuPositions.Last() + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y, 0);
        
        Card[] stockPileCardsArr = stockPileCards.ToArray();

        CardView previousCardView = null;
        for (int i = 0; i < stockPileCards.Count; i++)
        {
            Card stockPileCard = stockPileCardsArr[i];
            GameObject stockPileGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, stockPileCard, i);
            CardView stockPileCardView = stockPileGO.GetComponent<CardView>();

            previousCardView = stockPileCardView;
            stockPileGO.name = stockPileCard.ToString()+"(Stock)";
        }

        //Initialize Object to decect stock touches
        GameObject stockPileClickDetectorGO = new GameObject("stockPileClickDetectorGO");
        stockPileClickDetectorGO.transform.parent = cardsContainer;
        var stockClickDetector = stockPileClickDetectorGO.AddComponent<StockClickDetector>();
        stockClickDetector.cooldown = flipSpeed;
        stockPileClickDetectorGO.AddComponentCopy(previousCardView.GetComponent<BoxCollider2D>());
        stockPileClickDetectorGO.transform.localScale = previousCardView.transform.localScale;
        stockPileClickDetectorGO.transform.position = previousCardView.transform.position;

        //Make sure it's above everything else so it catches the raycastings first.
        stockPileClickDetectorGO.transform.SetZ(-52); 

        //Make fondo of stock object
        var go = InstantiateAndScale(foundationPilePrefab, suggestedCardSize,stockPile_pos);
        go.GetComponent<CardView>().bigSuit.enabled = false;
        go.transform.SetZ(1);
        go.name = "Stock_Fondo";
        go.transform.parent = cardsContainer;
    }

    private GameObject InstantiateFoundationPile(Vector2 suggestedCardSize, float y_padding_worldSpace, Suit suit, Vector3 tableuColumn_pos)
    {
        Vector3 targetPos = tableuColumn_pos + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y, 0);
        GameObject foundationPileGO = this.InstantiateAndScale(foundationPilePrefab, suggestedCardSize, targetPos);
        CardView cardView = foundationPileGO.GetComponent<CardView>();
        
        foundationPileGO.transform.parent = cardsContainer;
        cardView.bigSuit.sprite = SpritesProvider.LoadSuitSprite(suit);
        cardView.SetSortingOrderAndZDepth(-1);
        foundationPileGO.name = "Foundation_" + suit.ToString();

        return foundationPileGO;
    }

    private GameObject InstantiateFaceUpCard(CardColumn[] tableu, Vector2 suggestedCardSize, int column, CardView[] facedownCardPileBelow)
    {
        Card card = tableu[column].GetTopCard();

        GameObject cardGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, card, facedownCardPileBelow.Length);
        CardView cardView = cardGO.GetComponent<CardView>();
        
        cardGO.name = card.ToString();

        return cardGO;
    }

    private GameObject InstantiateFaceDownCard(Vector2 suggestedCardSize, CardView[] cardPile, List<Card> faceDownCards_data, int ii)
    {
        //TODO: assign value and suit
        GameObject faceDownCardGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, faceDownCards_data[ii], cardsBelow: ii);
        CardView faceDownCardView = faceDownCardGO.GetComponent<CardView>();

        faceDownCardGO.name = faceDownCardView.cardData.ToString();

        return faceDownCardGO;
    }

    private GameObject InstantiateCardGameObject(Vector2 suggestedCardSize, Vector3 pos, bool faceUp, Card card, int cardsBelow = 0){
        GameObject result = InstantiateCardGameObject(suggestedCardSize, pos, faceUp, card.suit, card.value, cardsBelow);
        CardView cardView = result.GetComponent<CardView>();
        
        this.cardData_to_cardView[card.ToString()] = cardView;
        cardView.cardData = card;

        return result;
    }

    private GameObject InstantiateCardGameObject(Vector2 suggestedCardSize, Vector3 pos, bool faceUp, Suit suit = Suit.None, int value = -1, int cardsBelow = 0){
        if( faceUp && (suit==Suit.None || value==-1) ){
            Debug.LogError("value and suit parameters must be provided for faceup cards");
        }

        GameObject cardGO = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos);
        CardView cardView = cardGO.GetComponent<CardView>();
        cardView.isFaceUp = faceUp;
        
        cardGO.transform.parent = cardsContainer;
        
        Sprite suitSprite = SpritesProvider.LoadSuitSprite(suit);
        cardView.bigSuit.sprite = suitSprite;
        cardView.smallSuit.sprite = suitSprite;
        cardView.value.sprite = SpritesProvider.LoadValueSprite(value);
        
        cardView.SetSortingOrderAndZDepth(cardsBelow, false);

        cardView.front.SetActive( faceUp );
        cardView.back.SetActive( !faceUp );

        cardView.bigSuit.sprite = suitSprite;

        return cardGO;
    }

    private Vector3 ComputeTopBarOffset()
    {
        float topBarSize = topBar.sizeDelta.y * topBar.parent.GetComponent<Canvas>().scaleFactor;
        Vector3 topBarOffset = new Vector3(0, ScreenSpace_To_WorldSpace((int)topBarSize), 0);
        return topBarOffset;
    }

    public bool IsPointAboveTableu(Vector2 point){
        Vector2 cardSize_worldSpace = this.ComputeCardSize_WorldSpace(GameManager.Instance.columns_count, x_padding, y_padding);
        float aboveTableu_YCoord = this.tableuPositions[0].y;// - (cardSize_worldSpace.y);

        return point.y > aboveTableu_YCoord;
    }

    
    public TablePosition GetTablePosition(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int closestColumnToPoint = -1;

        for (int i = 0; i < this.tableuPositions.Length; i++)
        {
            var pos = tableuPositions[i];
            float distance = Mathf.Abs(position.x - pos.x);
            if(distance < minDistance){
                minDistance = distance;
                closestColumnToPoint = i;
            }
        }

        if(IsPointAboveTableu(position)){
            //We are trying to place on foundation pile
            if(closestColumnToPoint < 4){
                return new TablePosition(Zone.Foundation, (closestColumnToPoint));
            } else if(closestColumnToPoint == this.tableuPositions.Length-1){
                //This is stock pile
                return new TablePosition(Zone.NotAZone, -1);
            } else {
                //Place between foundation and right edge. We'll generalize and say it's always Waste pile
                return new TablePosition(Zone.Waste, 1);
            }
        } else{ 
            // We are trying to place on tableu
            return new TablePosition(Zone.Tableu, closestColumnToPoint); 
        }
    }

    public void MoveCard(Move move){
        Sequence undoSequence = DOTween.Sequence();

        Card selectedCard = move.movedCards.First();
        int targetColumn = move.to.index;
        
        CardView selectedCardView = this.cardData_to_cardView[selectedCard.ToString()];
        Vector3 selectedCardPos = selectedCardView.positionBeforeDrag;

        Vector3 targetPos;

        CardView destinationCardView = null;

        if(move.from.zone == Zone.Tableu){
            //Flip Card
            Card cardToFlip = move.GetCardToFlip();
            if(cardToFlip != null){
                CardView cardViewToFlip = this.cardData_to_cardView[cardToFlip.ToString()];
                cardViewToFlip.TurnFaceUp(flipSpeed);
                
                undoSequence.AppendCallback(()=>cardViewToFlip.TurnFaceDown(flipSpeed));
            }
        }

        if(move.from.zone == Zone.Waste){
            //Update Waste Pile (Remove/Add card)

            //Move top two cards left, so we can show a third again.
            if(move.gameSnapshot.wastePile.Count > 3){
                Card topCard_wastePile = move.gameSnapshot.wastePile.Peek();
                CardView topCardView_wastePile = this.cardData_to_cardView[topCard_wastePile.ToString()];
                
                CardView[] cardsToScoopRight = new CardView[2];
                cardsToScoopRight[0] = topCardView_wastePile.CardBelow;
                cardsToScoopRight[1] = topCardView_wastePile.CardBelow.CardBelow;
                foreach(CardView card in cardsToScoopRight){
                    Vector3 cardPosition = card.transform.position;
                    card.transform.DOMove(cardPosition + new Vector3(stockPile_padding_x, 0, 0), flipSpeed);
                    
                    undoSequence.Join(card.transform.DOMove(cardPosition, flipSpeed));
                }
            }
        }

        if(move.to.zone == Zone.Tableu){
            //Move selected cards to new column
            bool isTargetColumnEmpty = move.gameSnapshot.tableu[targetColumn].faceUpCards.IsNullOrEmpty();
            if(isTargetColumnEmpty == false){
                CardColumn targetCardColumn = move.gameSnapshot.tableu[targetColumn];
                Card destinationCard = targetCardColumn.faceUpCards.Last();
                destinationCardView = this.cardData_to_cardView[destinationCard.ToString()];
                
                int currZDepth = Mathf.RoundToInt(selectedCardView.transform.position.z * -1);

                targetPos = destinationCardView.transform.position - new Vector3(0,faceUp_padding_y,0);


                selectedCardView.SetSortingOrderAndZDepth(targetCardColumn.TotalCardsCount());
                targetPos.z = selectedCardView.transform.position.z; //Update Z depth

                undoSequence.AppendCallback(()=>selectedCardView.SetSortingOrderAndZDepth(currZDepth));

            }else{
                targetPos = this.tableuPositions[targetColumn];

                int currZDepth = Mathf.RoundToInt(selectedCardView.transform.position.z * -1);

                selectedCardView.SetSortingOrderAndZDepth(0);

                undoSequence.AppendCallback(()=>selectedCardView.SetSortingOrderAndZDepth(currZDepth));
            }

        }else if(move.to.zone == Zone.Foundation){
            //Move selected card to foundation
            targetPos = this.foundationPilesPositions[targetColumn];
            int cardsBelowSelection = move.gameSnapshot.foundationPiles[targetColumn].cards.Count;

            int currZDepth = Mathf.RoundToInt(selectedCardView.transform.position.z * -1);

            selectedCardView.SetSortingOrderAndZDepth(cardsBelowSelection);
            targetPos.z = selectedCardView.transform.position.z;

            undoSequence.AppendCallback(()=>selectedCardView.SetSortingOrderAndZDepth(currZDepth));

        }else{
            throw new Exception("Move is invalid");
        }


        selectedCardView.MoveToPoint(targetPos);

        undoSequence.Join(selectedCardView.transform.DOMove(selectedCardPos, flipSpeed));
        undoSequence.Pause();
        movesUndo_sequences.Push(undoSequence);
    }

    public void NotifyLegalMove(Move move)
    {
        MoveCard(move);
    }

    public void NotifyIllegalMove(IllegalMove move)
    {
        this.cardData_to_cardView[move.card.ToString()].UndoDrag();
    }

    public void NotifyFlipStockCardMove(Move move){
        Sequence undoSequence = DOTween.Sequence();

        int wastePileCount = move.gameSnapshot.wastePile.Count;

        //Get the card on top of the stock. Move and flip it.
        CardView revealedStockCardView = this.cardData_to_cardView[move.SelectedCard.ToString()];
        Vector3 cardSize = ComputeCardSize_WorldSpace(GameManager.Instance.columns_count, x_padding, y_padding);

        //For undo sequence
        Vector3 currPosition = revealedStockCardView.transform.position;
        int currZDepth = Mathf.RoundToInt(revealedStockCardView.transform.position.z * -1);

        revealedStockCardView.SetSortingOrderAndZDepth(wastePileCount-1, false);
        
        undoSequence.AppendCallback(()=>revealedStockCardView.SetSortingOrderAndZDepth(currZDepth, false));
        

        Vector3 targetMovePoint = revealedStockCardView.transform.position;
        float mult = Mathf.Max(3 - wastePileCount, 1);
        targetMovePoint.x = targetMovePoint.x - cardSize.x * (2/3f);
        targetMovePoint = targetMovePoint - (new Vector3(stockPile_padding_x,0,0) * mult);

        //Flip and Move
        revealedStockCardView.transform.DOMove(targetMovePoint, flipSpeed);
        revealedStockCardView.TurnFaceUp(flipSpeed);

        undoSequence.AppendCallback(()=>revealedStockCardView.TurnFaceDown(flipSpeed));
        undoSequence.Append(revealedStockCardView.transform.DOMove(currPosition, flipSpeed));


        //Scoop left the upmost two cards in the pile, if we are now exceeding 3 cards
        if(wastePileCount >= 3){
            List<Card> cardsToMove = move.gameSnapshot.wastePile.Take(3).ToList();
            for (int i = 0; i < 2; i++)
            {
                Card cardToMoveLeft = cardsToMove[i];
                CardView cardView = this.cardData_to_cardView[cardToMoveLeft.ToString()];
                Vector3 startPoint = cardView.transform.position;
                
                Vector3 movePoint = cardView.transform.position - new Vector3(stockPile_padding_x,0,0);

                cardView.transform.DOMove(movePoint, flipSpeed);

                undoSequence.Join(cardView.transform.DOMove(startPoint, flipSpeed));
            }
        }

        undoSequence.Pause();
        movesUndo_sequences.Push(undoSequence);
    }


    public void NotifyRestoreStockpileFromWastePile(List<Card> restoredStockPile){
        Sequence undoSequence = DOTween.Sequence();

        //Flip and move those cards back to where they belong!
        foreach(Card card in restoredStockPile){
            CardView cardView = this.cardData_to_cardView[card.ToString()];
            Vector3 cardStartPos = cardView.transform.position;

            Vector3 moveLocation = stockPile_pos;
            moveLocation.z = cardView.transform.position.z;

            cardView.transform.DOMove(moveLocation, flipSpeed);
            cardView.TurnFaceDown(flipSpeed);

            undoSequence.PrependCallback(()=>cardView.TurnFaceUp(flipSpeed));
            undoSequence.Join(cardView.transform.DOMove(cardStartPos, flipSpeed));
        }

        undoSequence.Pause();
        movesUndo_sequences.Push(undoSequence);
    }

    public void NotifyUndoMove(Move moveToUndo)
    {
        var sequence = movesUndo_sequences.Pop();
        sequence.Play();
    }
}
