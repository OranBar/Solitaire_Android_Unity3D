using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolitaireGraphics : MonoBehaviour
{
    //---------------------------
    public GameObject cardPrefab, foundationPilePrefab;
    // public int topBarHeight = 100;
    public RectTransform topBar;
    public int x_padding = 5;
    public int y_padding = 50;
    public float faceDown_padding_y, faceUp_padding_y;

//---------------------------

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
        var tmp = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        Vector2 suggestedCardSize = Camera.main.ScreenToWorldPoint(tmp) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        return suggestedCardSize;
    }

     public Vector2[] ComputePortraitPositions(int noOfColumns, int x_padding, int y_padding){
        Vector2[] tableuPortraitPositions = new Vector2[noOfColumns];
        Vector2 suggestedCardSize_sceenSpace = ComputeCardSize_ScreenSpace(noOfColumns, x_padding, y_padding);
        float cardWidth = suggestedCardSize_sceenSpace.x;
        float cardHeight = suggestedCardSize_sceenSpace.y;


        for (int i = 0; i < noOfColumns; i++)
        {
            float x = (x_padding*(i+1)) + (cardWidth*i) + (cardWidth/2);
            float y = ((y_padding*2) + cardHeight/2 + cardHeight);
            Vector2 cardPosition_screen = new Vector2(x, Camera.main.pixelHeight - y);
            Debug.Log(x+" "+y);

            Vector2 cardPosition_world = Camera.main.ScreenToWorldPoint(cardPosition_screen);
            tableuPortraitPositions[i] = cardPosition_world;
        }
        return tableuPortraitPositions;
    }

    public float ScreenSpace_To_WorldSpace(int noOfPixels){
        Vector2 result = Camera.main.ScreenToWorldPoint(new Vector2(noOfPixels, 0)) - Camera.main.ScreenToWorldPoint(Vector2.zero);
        return result.x;
    }

    private GameObject InstantiateAndScale(GameObject prefab, Vector2 suggestedCardSize, Vector2 pos){
        var newGo = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        var cardGo = newGo.GetComponent<CardGO>();
        var multiplier = (suggestedCardSize.x) / (cardGo.mainSpriteRenderer.size.x);
        newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
        return newGo;
    }

    public void SetUpGraphics(GameManager gameManager)
    {
        int columns_count = gameManager.columns_count;
        Transform cardsContainer = new GameObject("Cards Container").transform;

        Vector2 suggestedCardSize = ComputeCardSize_WorldSpace(columns_count, x_padding, y_padding);
        var y_padding_worldSpace = ScreenSpace_To_WorldSpace(y_padding);

        Vector2[] positions = ComputePortraitPositions(columns_count, x_padding, y_padding);

        float topBarSize = topBar.sizeDelta.y * topBar.parent.GetComponent<Canvas>().scaleFactor;
        Vector2 topBarOffset = new Vector2(0, ScreenSpace_To_WorldSpace((int)topBarSize));

        Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];

        for (int i = 0; i < gameManager.tableu.Length; i++)
        {
            var pos = positions[i] - topBarOffset;

            //Foundation Piles
            if(i < 4){
                var foundationPileGo = this.InstantiateAndScale(foundationPilePrefab, suggestedCardSize, pos + new Vector2(0, y_padding_worldSpace + suggestedCardSize.y) );
                foundationPileGo.name = "Foundation_"+suits[i].ToString();
                foundationPileGo.transform.parent = cardsContainer;
                foundationPileGo.GetComponent<CardGO>().bigSuit.sprite = SpritesProvider.LoadSuitSprite(suits[i]);
            }
            else if(i == columns_count-1){
                var deckPileGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos + new Vector2(0, y_padding_worldSpace + suggestedCardSize.y));
                deckPileGo.name = "DeckPile";
                deckPileGo.transform.parent = cardsContainer; 
                var deckCardGo = deckPileGo.GetComponent<CardGO>();
                deckCardGo.front.SetActive(false);
                deckCardGo.back.SetActive(true);
            }

            //FaceDown Card Piles
            for(int ii = 0; ii<i; ii++){
                var faceDownCard = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos);
                faceDownCard.transform.parent = cardsContainer;
                faceDownCard.transform.parent = cardsContainer;

                CardGO faceDownCardGo = faceDownCard.GetComponent<CardGO>();
                
                foreach(var cardObj_child in faceDownCard.transform.GetAllChildren(true)){
                    var spriteRenderer = cardObj_child.GetComponent<SpriteRenderer>();
                    if(spriteRenderer != null){
                        spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + ii;
                    }
                }
                faceDownCardGo.front.SetActive(false);
                faceDownCardGo.back.SetActive(true);
                pos.y = pos.y - faceDown_padding_y;
            }
            // //FaceUp Cards
            // Card card = this.tableu[i].GetTopCard();

            // var newGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos);
            // newGo.transform.parent = cardsContainer;

            // CardGO cardGo = newGo.GetComponent<CardGO>();

            // cardGo.front.SetActive(true);
            // cardGo.back.SetActive(false);

            // Sprite suitSprite = SpritesProvider.LoadSuitSprite(card.suit);
            // cardGo.bigSuit.sprite = suitSprite;
            // cardGo.smallSuit.sprite = suitSprite;
            // cardGo.value.sprite = SpritesProvider.LoadValueSprite(card.value);
        }
        
        // foreach(var pos in positions){
        //     var newGo = GameObject.Instantiate(cardPrefab, pos, Quaternion.identity);
            
        //     var multiplier = (suggestedCardSize.x) / (newGo.GetComponent<CardGO>().mainSpriteRenderer.size.x);
        //     newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
            
        // }
    }


}
