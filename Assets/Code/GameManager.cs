using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
//---------------------------
    public int x_padding = 5;
    public int y_padding = 50;
    public int columns_count = 7;
    public GameObject cardPrefab;
//---------------------------

    public CardColumn[] tableu;
    public List<Card> stockPile;
    public List<Card> wastePile;
    public Dictionary<Suit, List<Card>> suit_to_foundationPile;
    //public something stock;
    public DeckShuffler shuffler;

    public float faceDown_padding_y, faceUp_padding_y;

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

    private GameObject InstantiateCardAndScale(Vector2 suggestedCardSize, Vector2 pos){
        var newGo = GameObject.Instantiate(cardPrefab, pos, Quaternion.identity);
        var cardGo = newGo.GetComponent<CardGO>();
        var multiplier = (suggestedCardSize.x) / (cardGo.mainSpriteRenderer.size.x);
        newGo.transform.localScale = (newGo.transform.localScale) * (multiplier);
        return newGo;
    }

    private void SetUpGraphics(int noOfColumns, int x_padding, int y_padding)
    {
        SolitaireGraphics graphics = new SolitaireGraphics();
        Vector2 suggestedCardSize = graphics.ComputeCardSize_WorldSpace(noOfColumns, x_padding, y_padding);
        Vector2[] positions = graphics.ComputePortraitPositions(noOfColumns, x_padding, y_padding);
        
        for (int i = 0; i < this.tableu.Length; i++)
        {
            var pos = positions[i];

            for(int ii = 0; ii<i; ii++){
                var faceDownCard = this.InstantiateCardAndScale(suggestedCardSize, pos);
                CardGO faceDownCardGo = faceDownCard.GetComponent<CardGO>();
                
                faceDownCardGo.front.SetActive(false);
                faceDownCardGo.back.SetActive(true);
                pos.y = pos.y - faceDown_padding_y;
            }

            Card card = this.tableu[i].GetTopCard();

            var newGo = this.InstantiateCardAndScale(suggestedCardSize, pos);
            CardGO cardGo = newGo.GetComponent<CardGO>();

            cardGo.front.SetActive(true);
            cardGo.back.SetActive(false);

            Sprite suitSprite = GraphicsProvider.LoadSuitSprite(card.suit);
            cardGo.bigSuit.sprite = suitSprite;
            cardGo.smallSuit.sprite = suitSprite;
            cardGo.value.sprite = GraphicsProvider.LoadValueSprite(card.value);
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
