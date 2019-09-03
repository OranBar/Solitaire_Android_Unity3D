using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : ICloneable
{
    public CardColumn[] tableu;
    public Stack<Card> stockPile;
    public Stack<Card> wastePile;
    public FoundationPile[] foundationPiles;

    public GameState(){
        this.stockPile = new Stack<Card>();
        this.wastePile = new Stack<Card>();
        
        Suit[] suits = new Suit[]{Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades};
        foundationPiles = new FoundationPile[suits.Length];
        for (int i = 0; i < suits.Length; i++)
        {
            FoundationPile pile = new FoundationPile(suits[i]);
            foundationPiles[i] = pile;
        }
    }

    public object Clone(){
        GameState result = new GameState();
        result.tableu = tableu.Select(cc => cc.Clone()).Cast<CardColumn>().ToArray();
        result.stockPile = new Stack<Card>(stockPile.ToList().Select(c => c.Clone()).Cast<Card>().Reverse());
        result.wastePile = new Stack<Card>(wastePile.ToList().Select(c => c.Clone()).Cast<Card>().Reverse());
        result.foundationPiles = foundationPiles.Select(fp => fp.Clone()).Cast<FoundationPile>().ToArray();
        return result; 
    }
    
    public Zone GetCardZone(Card card){
        foreach(CardColumn tableuColumn in tableu){
            if(tableuColumn.faceDownCards.Contains(card) || tableuColumn.faceUpCards.Contains(card)){
                return Zone.Tableu;
            }
        }    
        if(stockPile.Contains(card)){
            return Zone.Stock;
        }
        if(wastePile.Contains(card)){
            return Zone.Waste;
        }
        foreach(FoundationPile foundationPile in foundationPiles){
            if(foundationPile.cards.Contains(card)){
                return Zone.Foundation;
            }
        }
        return Zone.NotAZone;
    }

    public int GetCardColumn(Card card){
        for (int i = 0; i < tableu.Length; i++)
        {
            CardColumn tableuColumn = tableu[i];
            if(tableuColumn.faceDownCards.Contains(card) || tableuColumn.faceUpCards.Contains(card)){
                return i;
            }
        }
        for (int i = 0; i < foundationPiles.Length; i++)
        {
            FoundationPile foundationPile = foundationPiles[i];
            if(foundationPile.cards.Contains(card)){
                return i;
            }
        }

        return -1;
    }
}
