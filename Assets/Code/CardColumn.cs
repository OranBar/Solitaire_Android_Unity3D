using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardColumn : ICloneable
{
    public int columnIndex;
    public Stack<Card> faceDownCards = new Stack<Card>();
    public List<Card> faceUpCards = new List<Card>();

    public Card GetTopCard(){
        return faceUpCards.Last();
    }

    public int TotalCardsCount(){
        return faceDownCards.Count + faceUpCards.Count;
    }
    
    public List<Card> SelectCard(int faceUpCards_Index){
        Debug.Assert(faceUpCards_Index < faceUpCards.Count);

        List<Card> selected = new List<Card>();
        return faceUpCards.Skip(faceUpCards_Index-1).ToList();
    }

    public object Clone()
    {
        CardColumn clone = new CardColumn();
        clone.columnIndex = this.columnIndex;
        clone.faceDownCards = new Stack<Card>(this.faceDownCards.ToList().Select(c => c.Clone()).Cast<Card>().Reverse());
        clone.faceUpCards = this.faceUpCards.Select(fp => fp.Clone()).Cast<Card>().ToList();
        
        return clone;
    }
}
