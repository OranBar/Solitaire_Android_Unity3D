using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class DeckShuffler : ICloneable
{
    
    private Stack<Card> deck;


    public DeckShuffler(){
    }

    public DeckShuffler(Stack<Card> deckSeed){
        this.deck = deckSeed;
    }

    public void ShuffleDeck(){
        Random rnd = new Random();
        Card[] tmpDeck = new Card[52];
        List<int> availableIndexes = Enumerable.Range(0, 52).ToList();

        Suit[] suits = new Suit[]{Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades};
        foreach(Suit suit in suits){
            for(int cardValue=1; cardValue<=13; cardValue++){
                Card card = new Card(cardValue, suit);
                
                int cardIndex = availableIndexes[rnd.Next(availableIndexes.Count)];
                tmpDeck[cardIndex] = card;
                availableIndexes.Remove(cardIndex);
            }
        }

        this.deck = new Stack<Card>(tmpDeck);
    }

    public List<Card> DrawCards(int noOfCardsToDraw)
    {
        List<Card> result = new List<Card>();
        for(int i=0; i < noOfCardsToDraw; i++){
            result.Add(DrawCard());
        }
        return result;
    }

    public Card DrawCard()
    {
        if(this.deck.Count == 0){
            Debug.LogError("Card drwan from empty deck");
            return null;
        }
        return this.deck.Pop();
    }

    public int GetRemainigCardsCount(){
        return this.deck.Count;
    }

    public object Clone()
    {
        var deckSeed = deck.ToList().Select(c => c.Clone()).Cast<Card>().Reverse();
        return new DeckShuffler(new Stack<Card>(deckSeed));
    }
}
