using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move 
{
    public List<Card> movedCards;
    
    public TablePosition from, to;

    public Move(List<Card> movedCards, TablePosition from, TablePosition to)
    {
        this.movedCards = movedCards;
        this.from = from;
        this.to = to;
    }
    
    public Card SelectedCard{
        get{
            return movedCards.First();
        }
    }   

    public Card GetCardToFlip(){
        if(this.from.zone == Zone.Tableu){
            //Update start tableu pile - unreference moved cards
            CardColumn startCardColum = GameManager.Instance.tableu[SelectedCard.column];
            // startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == SelectedCard).ToList();
       
            //Flip card below if needed
            if(startCardColum.faceUpCards.Count == 0 && startCardColum.faceDownCards.Count > 0){
                Card faceDownCardToFlip = GameManager.Instance.tableu[SelectedCard.column].faceDownCards.Peek();
                return faceDownCardToFlip;
            }
        } 
        return null;
    }

    public bool MoveResultsInCardFlipped(){
        if(this.from.zone == Zone.Tableu){
            //Update start tableu pile - unreference moved cards
            CardColumn startCardColum = GameManager.Instance.tableu[SelectedCard.column];
            // startCardColum.faceUpCards = startCardColum.faceUpCards.TakeUntil(c => c == SelectedCard).ToList();
       
            //Flip card below if needed
            return (startCardColum.faceUpCards.Count == 0 && startCardColum.faceDownCards.Count > 0);
        } 
        return false;
    }
}
