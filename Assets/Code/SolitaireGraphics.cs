using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class SolitaireGraphics : Singleton<SolitaireGraphics>, ISolitaireGraphics
{
//---------------------------
    public GameObject cardPrefab, foundationPilePrefab;
    public RectTransform topBar;
    public int x_padding = 5;
    public int y_padding = 50;
    public float faceDown_padding_y, faceUp_padding_y;

    public AnimationCurve easeFunction;
    public float cardToTableu_animDuration = 0.6f;
    public float flipSpeed = 0.6f;

//---------------------------
    
    protected override void InitTon(){ }


    private Vector3 stockPile_pos;
    private Vector3[] tableuPositions;
    private Dictionary<Card, CardView> cardData_to_cardView = new Dictionary<Card, CardView>();
    private Transform cardsContainer;

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

    public Vector3[] ComputePortraitPositions(int noOfColumns, int x_padding, int y_padding){
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

    public void SetupGraphics(CardColumn[] tableu, List<Card> stockPileCards)
    {
        int columns_count = tableu.Length;
        
        //Create cards container empty gameobject - Stored as class variable for easy access in insantiation methods
        cardsContainer = new GameObject("Cards Container").transform;
        
        Vector2 suggestedCardSize = ComputeCardSize_WorldSpace(columns_count, x_padding, y_padding);
        Sequence animSequence = DOTween.Sequence();

        //Compute tableu positions
        var y_padding_worldSpace = ScreenSpace_To_WorldSpace(y_padding);

        tableuPositions = ComputePortraitPositions(columns_count, x_padding, y_padding);

        //Compute TopBarOffset
        Vector3 topBarOffset = ComputeTopBarOffset();

        //Init deck pile object
        InstantiateStockPile(stockPileCards, suggestedCardSize, y_padding_worldSpace, topBarOffset);

        //Place cards on tableu with animations
        float anim_delay = 0f;

        // Loop tableu columns 1 by 1
        for (int i = 0; i < tableu.Length; i++)
        {
            var tableuColumn_pos = tableuPositions[i] - topBarOffset;

            //Foundation Pile
            if (i < 4)
            {
                Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];
                InstantiateFoundationPile(suggestedCardSize, y_padding_worldSpace, suits[i], tableuColumn_pos);
            }

            CardView[] cardPile = new CardView[i];
            //FaceDown Card Pile
            List<Card> faceDownCards_data = new List<Card>(tableu[i].faceDownCards);
            for (int ii = 0; ii < i; ii++)
            {
                GameObject faceDownCardGO = InstantiateFaceDownCard(suggestedCardSize, cardPile, faceDownCards_data, ii);

                //Prepare Animation
                tableuColumn_pos.y = tableuColumn_pos.y - faceDown_padding_y;
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
            tableuColumn_pos.y = tableuColumn_pos.y - faceDown_padding_y;
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

    private void InstantiateStockPile(List<Card> stockPileCards, Vector2 suggestedCardSize, float y_padding_worldSpace, Vector3 topBarOffset)
    {
        this.stockPile_pos = tableuPositions.Last() - topBarOffset + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y, 0);
        
        CardView previousCardView = null;
        for (int i = 0; i < stockPileCards.Count; i++)
        {
            Card stockPileCard = stockPileCards[i];
            GameObject stockPileGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, stockPileCard, i);
            CardView stockPileCardView = stockPileGO.GetComponent<CardView>();
            
            if(previousCardView != null){
                stockPileCardView.cardBelow = previousCardView;
                previousCardView.cardAbove = stockPileCardView;
            }

            previousCardView = stockPileCardView;
            stockPileGO.name = stockPileCard.ToString()+"(Stock)";
        }
    }

    private void InstantiateFoundationPile(Vector2 suggestedCardSize, float y_padding_worldSpace, Suit suit, Vector3 tableuColumn_pos)
    {
        Vector3 targetPos = tableuColumn_pos + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y, 0);
        GameObject foundationPileGO = this.InstantiateAndScale(foundationPilePrefab, suggestedCardSize, targetPos);
        foundationPileGO.transform.parent = cardsContainer;
        foundationPileGO.GetComponent<CardView>().bigSuit.sprite = SpritesProvider.LoadSuitSprite(suit);
        foundationPileGO.name = "Foundation_" + suit.ToString();
    }

    private GameObject InstantiateFaceUpCard(CardColumn[] tableu, Vector2 suggestedCardSize, int column, CardView[] facedownCardPileBelow)
    {
        Card card = tableu[column].GetTopCard();

        GameObject cardGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, card.suit, card.value, facedownCardPileBelow.Length);
        CardView cardView = cardGO.GetComponent<CardView>();
        cardView.cardData = tableu[column].faceUpCards[0];
        this.cardData_to_cardView[cardView.cardData] = cardView;
        
        cardGO.name = card.ToString();

        //Reference card below and above
        if (facedownCardPileBelow.Length > 0)
        {
            CardView topmostFaceDownCard = facedownCardPileBelow.Last();
            cardView.cardBelow = topmostFaceDownCard;
            topmostFaceDownCard.cardAbove = cardView;
        }

        return cardGO;
    }

    private GameObject InstantiateFaceDownCard(Vector2 suggestedCardSize, CardView[] cardPile, List<Card> faceDownCards_data, int ii)
    {
        //TODO: assign value and suit
        GameObject faceDownCardGO = this.InstantiateCardGameObject(suggestedCardSize, stockPile_pos, false, faceDownCards_data[ii], cardsBelow: ii);
        CardView faceDownCardView = faceDownCardGO.GetComponent<CardView>();
        faceDownCardView.cardData = faceDownCards_data[ii];
        this.cardData_to_cardView[faceDownCardView.cardData] = faceDownCardView;
        
        faceDownCardGO.name = faceDownCardView.cardData.ToString();

        //Reference card below and above
        cardPile[ii] = faceDownCardView;
        if (ii > 0)
        {
            cardPile[ii].cardBelow = cardPile[ii - 1];
            cardPile[ii - 1].cardAbove = cardPile[ii];
        }

        return faceDownCardGO;
    }

    private GameObject InstantiateCardGameObject(Vector2 suggestedCardSize, Vector3 pos, bool faceUp, Card card, int cardsBelow = 0){
        return InstantiateCardGameObject(suggestedCardSize, pos, faceUp, card.suit, card.value, cardsBelow);
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
        
        cardView.SetSortingOrderAndZDepth(cardsBelow);

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

    public static int INVALID_COLUMN = -6;

    public int GetClosestColumn(Vector2 releasedCardPosition){
        float minDistance = float.MaxValue;
        int closestColumnToPoint = -1;

        for (int i = 0; i < this.tableuPositions.Length; i++)
        {
            var pos = tableuPositions[i];
            float distance = Mathf.Abs(releasedCardPosition.x - pos.x);
            if(distance < minDistance){
                minDistance = distance;
                closestColumnToPoint = i;
            }
        }

        Vector2 cardSize_worldSpace = this.ComputeCardSize_WorldSpace(GameManager.Instance.columns_count, x_padding, y_padding);
        float aboveTableu_YCoord = this.tableuPositions[0].y - (cardSize_worldSpace.y);

        if(releasedCardPosition.y > aboveTableu_YCoord){
            //We are trying to place on foundation pile
            if(closestColumnToPoint < 4){
                return -(closestColumnToPoint+1);
            } else {
                return INVALID_COLUMN;  //Means invalid column
            }
        } else{ 
            // We are trying to place on tableu
            return closestColumnToPoint;
        }

    }

    // public void PlaceCard(Card selectedCard, int targetColumn){
    //     // CardView destinationCardView = this.cardData_to_cardView[destinationCard];
    //     CardView cardView = this.cardData_to_cardView[selectedCard];

    //     Vector3 targetPos = this.tableuPositions[targetColumn];
    //     bool isTargetColumnEmpty = GameManager.Instance.tableu[targetColumn].faceUpCards.IsNullOrEmpty();
    //     if(isTargetColumnEmpty == false){
    //         CardColumn targetCardColumn = GameManager.Instance.tableu[targetColumn];
    //         Card destinationCard = taporgetCardColumn.faceUpCards.Last();
    //         CardView destinationCardView = this.cardData_to_cardView[destinationCard];

    //         targetPos = destinationCardView.transform.position + new Vector3(0,faceUp_padding_y,0);
    //     }


    //     // foreach()
    // }

    public void MoveCard(Move move){
        Card selectedCard = move.movedCards.First();
        int targetColumn = move.destinationCard.column;
        
        CardView selectedCardView = this.cardData_to_cardView[selectedCard];

        Vector3 targetPos;
        bool isTargetColumnEmpty = GameManager.Instance.tableu[targetColumn].faceUpCards.IsNullOrEmpty();

        //Update cardViews references
        if(selectedCardView.cardBelow != null){
            if(selectedCardView.cardBelow.isFaceUp==false){
                selectedCardView.cardBelow.TurnFaceUp(flipSpeed);
            }
            selectedCardView.cardBelow.cardAbove = null;
        }

        if(isTargetColumnEmpty == false){
            CardColumn targetCardColumn = GameManager.Instance.tableu[targetColumn];
            Card destinationCard = targetCardColumn.faceUpCards.Last();
            CardView destinationCardView = this.cardData_to_cardView[destinationCard];

            // float padding = ScreenSpace_To_WorldSpace((int)destinationCardView.smallSuit.size.y);
            targetPos = destinationCardView.transform.position - new Vector3(0,faceUp_padding_y,0);
            // targetPos = destinationCardView.transform.position + new Vector3(0,ScreenSpace_To_WorldSpace((int)faceUp_padding_y),0);
            
            //Update cardview's cardabove/cardbelow for selected and destination card
            selectedCardView.cardBelow = destinationCardView;
            destinationCardView.cardAbove = selectedCardView;

            selectedCardView.SetSortingOrderAndZDepth((int)(-destinationCardView.transform.position.z));
            targetPos.z = selectedCardView.transform.position.z; //Update Z depth
            
            //Update cardViews references
            destinationCardView.cardAbove = selectedCardView;
            selectedCardView.cardBelow = destinationCardView;
        }else{
            targetPos = this.tableuPositions[targetColumn];
            selectedCardView.SetSortingOrderAndZDepth(0);
        }

        

        //Reuse recursive logic to move the card to the target spot.
        selectedCardView.MoveToPoint(targetPos);
        

        //Fix sorting order
        // selectedCard.cardsBelow.Count = 3
    }

    public void NotifyLegalMove(Move move)
    {
        MoveCard(move);
        
    }

    public void NotifyIllegalMove(IllegalMove move)
    {
        this.cardData_to_cardView[move.card].UndoDrag();
    }
}
