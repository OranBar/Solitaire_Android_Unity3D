using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardColumn 
{
    public int columnIndex;
    public Stack<Card> faceDownCards = new Stack<Card>();
    public List<Card> faceUpCards = new List<Card>();

    public Card GetTopCard(){
        return faceUpCards.Last();
    }

    public void AddCard(List<Card> newCards){
        // foreach(Card card in newCards){
        //     faceUpCards.Add
        // }
        faceUpCards.AddRange(newCards);
    }

    //I don't think we will ever use this
    public void AddCard(Card newCard){
        Debug.Log("Strange seeing you here");
        
        faceUpCards.Add(newCard);
    }

    public void FlipCard(){
        Debug.Assert(faceUpCards.Count == 0);

        if(faceDownCards.Any()){
            Card cardToFlip = faceDownCards.Pop();
            AddCard(cardToFlip);
        }
    }
    
    public List<Card> SelectCard(int faceUpCards_Index){
        Debug.Assert(faceUpCards_Index < faceUpCards.Count);

        List<Card> selected = new List<Card>();
        return faceUpCards.Skip(faceUpCards_Index-1).ToList();
    }

}
