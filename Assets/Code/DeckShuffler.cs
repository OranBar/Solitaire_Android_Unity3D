using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DeckShuffler 
{
    
    public string seed;
    public Card[] deck;
    
    public void InitCards(){
        Random rnd = new Random();
        deck = new Card[52];
        List<int> availableIndexes = Enumerable.Range(0, 52).ToList();

        foreach(Suit suit in Enum.GetValues(typeof(Suit))){
            for(int cardValue=1; cardValue<=13; cardValue++){
                Card card = new Card(cardValue, suit);
                
                int cardIndex = availableIndexes[rnd.Next(availableIndexes.Count)];
                deck[cardIndex] = card;
                availableIndexes.Remove(cardIndex);
            }
        }
    }

    private void RefreshSeed(){

    }

    public void ShuffleDeck()
    {
        RefreshSeed();
        throw new NotImplementedException();
    }

    public List<Card> DrawCards(int noOfCardsToDraw)
    {
        List<Card> result = new List<Card>();
        for(int i=0; i<noOfCardsToDraw; i++){
            result.Add(DrawCard());
        }
        return result;
    }

    public Card DrawCard()
    {
        throw new NotImplementedException();
    }
}
