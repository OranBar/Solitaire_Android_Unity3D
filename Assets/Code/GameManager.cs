using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
//---------------------------
    public GameObject cardPrefab, foundationPilePrefab;
    public int topBarHeight = 100;
    public int x_padding = 5;
    public int y_padding = 50;
    public int columns_count = 7;
    public float faceDown_padding_y, faceUp_padding_y;
//---------------------------

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

    [ContextMenu("InitGame")]
    private void InitGame()
    {
        shuffler = new DeckShuffler();

        SetUpTable(columns_count);
        SetUpGraphics(columns_count, x_padding, y_padding);
        
    }

    private GameObject InstantiateAndScale(GameObject prefab, Vector2 suggestedCardSize, Vector2 pos){
        var newGo = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        var cardGo = newGo.GetComponent<CardGO>();
        var multiplier = (suggestedCardSize.x) / (cardGo.mainSpriteRenderer.size.x);
        newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
        return newGo;
    }

    private void SetUpGraphics(int noOfColumns, int x_padding, int y_padding)
    {
        SolitaireGraphics graphics = new SolitaireGraphics();
        Vector2 suggestedCardSize = graphics.ComputeCardSize_WorldSpace(noOfColumns, x_padding, y_padding);
        var y_padding_worldSpace = graphics.ScreenSpace_To_WorldSpace(y_padding);

        Vector2[] positions = graphics.ComputePortraitPositions(noOfColumns, x_padding, y_padding);

        Vector2 topBarOffset = new Vector2(0, graphics.ScreenSpace_To_WorldSpace(topBarHeight));

        Suit[] suits = Enum.GetValues(typeof(Suit)) as Suit[];

        for (int i = 0; i < this.tableu.Length; i++)
        {
            var pos = positions[i] - topBarOffset;

            //Foundation Piles
            if(i < 4){
                var foundationPileGo = this.InstantiateAndScale(foundationPilePrefab, suggestedCardSize, pos + new Vector2(0, y_padding_worldSpace + suggestedCardSize.y) );
                foundationPileGo.GetComponent<CardGO>().bigSuit.sprite = SpritesProvider.LoadSuitSprite(suits[i]);
            }
            else if(i == noOfColumns){
                var deckPileGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos - new Vector2(0,y_padding_worldSpace));
                var deckCardGo = deckPileGo.GetComponent<CardGO>();
                deckCardGo.front.SetActive(false);
                deckCardGo.back.SetActive(true);
            }

            //FaceDown Card Piles
            for(int ii = 0; ii<i; ii++){
                var faceDownCard = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos);
                CardGO faceDownCardGo = faceDownCard.GetComponent<CardGO>();
                
                faceDownCardGo.front.SetActive(false);
                faceDownCardGo.back.SetActive(true);
                pos.y = pos.y - faceDown_padding_y;
            }
            //FaceUp Card
            Card card = this.tableu[i].GetTopCard();

            var newGo = this.InstantiateAndScale(cardPrefab, suggestedCardSize, pos);
            CardGO cardGo = newGo.GetComponent<CardGO>();

            cardGo.front.SetActive(true);
            cardGo.back.SetActive(false);

            Sprite suitSprite = SpritesProvider.LoadSuitSprite(card.suit);
            cardGo.bigSuit.sprite = suitSprite;
            cardGo.smallSuit.sprite = suitSprite;
            cardGo.value.sprite = SpritesProvider.LoadValueSprite(card.value);
        }
        
        // foreach(var pos in positions){
        //     var newGo = GameObject.Instantiate(cardPrefab, pos, Quaternion.identity);
            
        //     var multiplier = (suggestedCardSize.x) / (newGo.GetComponent<CardGO>().mainSpriteRenderer.size.x);
        //     newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
            
        // }
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
