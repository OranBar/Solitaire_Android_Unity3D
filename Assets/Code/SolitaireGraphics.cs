using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class SolitaireGraphics : Singleton<SolitaireGraphics>
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


    private Transform deckPile;
    private Vector3[] tableuPositions;

    public Vector2 ComputeCardSize_ScreenSpace(int noOfColumns, int x_padding, int y_padding){
        int screenWidth = Camera.main.pixelWidth;
        int screenHeight = Camera.main.pixelHeight;

        int spaceNeededForMargins = (noOfColumns+1)*x_padding;

        float cardWidth = (screenWidth - spaceNeededForMargins) / noOfColumns;
        // float cardRatio = (2.5f / 3.5f); 
        // float cardHeight = cardWidth * cardRatio;
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
        var newGo = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        var cardView = newGo.GetComponent<CardView>();
        var multiplier = (suggestedCardSize.x) / (cardView.mainSpriteRenderer.size.x);
        newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
        return newGo;
    }

    public void SetUpGraphics(GameManager gameManager)
    {
        Sequence animSequence = DOTween.Sequence();
        //Create cards container
        int columns_count = gameManager.columns_count;
        Transform cardsContainer = new GameObject("Cards Container").transform;
        
        //Compute tableu positions
        Vector2 suggestedCardSize = ComputeCardSize_WorldSpace(columns_count, x_padding, y_padding);
        var y_padding_worldSpace = ScreenSpace_To_WorldSpace(y_padding);

        tableuPositions = ComputePortraitPositions(columns_count, x_padding, y_padding);

        //Compute TopBarOffset
        float topBarSize = topBar.sizeDelta.y * topBar.parent.GetComponent<Canvas>().scaleFactor;
        Vector3 topBarOffset = new Vector3(0, ScreenSpace_To_WorldSpace((int)topBarSize), 0);

        //Init deck pile object
        Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];

        var deckPile_pos = tableuPositions.Last() - topBarOffset;
        GameObject deckPileGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, deckPile_pos + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y,0));
        this.deckPile = deckPileGo.transform;
        this.deckPile.name = "DeckPile";
        this.deckPile.transform.parent = cardsContainer; 
        
        var deckcardView = deckPileGo.GetComponent<CardView>();
        deckcardView.front.SetActive(false);
        deckcardView.back.SetActive(true);

        //Place cards on tableu with animations
        float anim_delay = 0f;

        for (int i = 0; i < gameManager.tableu.Length; i++)
        {
            var target_pos = tableuPositions[i] - topBarOffset;

            //Foundation Piles
            if(i < 4){
                var foundationPileGo = this.InstantiateAndScale(foundationPilePrefab, suggestedCardSize, target_pos + new Vector3(0, y_padding_worldSpace + suggestedCardSize.y,0) );
                foundationPileGo.name = "Foundation_"+suits[i].ToString();
                foundationPileGo.transform.parent = cardsContainer;
                foundationPileGo.GetComponent<CardView>().bigSuit.sprite = SpritesProvider.LoadSuitSprite(suits[i]);
            }
            
            CardView[] cardPile = new CardView[i];
            //FaceDown Card Piles
            for(int ii = 0; ii<i; ii++){
                var faceDownCard = this.InstantiateAndScale(cardPrefab, suggestedCardSize, deckPile.position);
                faceDownCard.transform.parent = cardsContainer;
                faceDownCard.transform.parent = cardsContainer;

                CardView faceDowncardView = faceDownCard.GetComponent<CardView>();
                faceDowncardView.isFaceUp = false;

                //Reference card below and above
                cardPile[ii] = faceDowncardView;
                if(ii > 0){
                    cardPile[ii].cardBelow = cardPile[ii-1];
                    cardPile[ii-1].cardAbove = cardPile[ii];
                }

                faceDowncardView.IncreaseSortingOrder(ii);
                // FixCardSortingLayers(faceDownCard, ii);

                faceDowncardView.front.SetActive(false);
                faceDowncardView.back.SetActive(true);
                
                //PrepareAnimation
                target_pos.y = target_pos.y - faceDown_padding_y;
                target_pos.z = -ii; //If a card is above another, the Z has to reflect that, or the colliders will overlap and steal each others' calls
                faceDownCard.transform.DOMove(target_pos, cardToTableu_animDuration).SetDelay(anim_delay);
                
                anim_delay += 0.15f;
            }
            //FaceUp Cards
            Card card = gameManager.tableu[i].GetTopCard();

            var newGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, deckPile.position);
            newGo.transform.parent = cardsContainer;

            CardView cardView = newGo.GetComponent<CardView>();
            cardView.isFaceUp = true;

            //Reference card below and above
            if(cardPile.Length > 0){
                CardView topmostFaceDownCard = cardPile.Last();
                cardView.cardBelow = topmostFaceDownCard;
                topmostFaceDownCard.cardAbove = cardView;
            }
        
            cardView.IncreaseSortingOrder(i);

            cardView.front.SetActive(false);
            cardView.back.SetActive(true);

            Sprite suitSprite = SpritesProvider.LoadSuitSprite(card.suit);
            cardView.bigSuit.sprite = suitSprite;
            cardView.smallSuit.sprite = suitSprite;
            cardView.value.sprite = SpritesProvider.LoadValueSprite(card.value);
            
            target_pos.y = target_pos.y - faceDown_padding_y;   
            target_pos.z = -i; //If a card is above another, the Z has to reflect that, or the colliders will overlap and steal each others' calls

            //PrepareAnimation
            Sequence moveAndTurnSequence = DOTween.Sequence();
            moveAndTurnSequence
                .PrependInterval(anim_delay)
                .Append(newGo.transform.DOMove(target_pos, cardToTableu_animDuration))
                .Append(newGo.transform.DORotate(new Vector3(0,-90,0), flipSpeed/2f)
                    .OnComplete(()=> {cardView.front.SetActive(true); cardView.back.SetActive(false);}))
                .Append(newGo.transform.DORotate(new Vector3(0,0,0), flipSpeed/2f));

            anim_delay +=0.15f;
        }
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
        // float aboveTableu_YCoord = this.deckPile.position.y - (cardSize_worldSpace.y/2f);
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

}
