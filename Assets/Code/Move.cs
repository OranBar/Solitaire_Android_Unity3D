using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move 
{
    public List<Card> movedCards;
    
    public TablePosition from, to;
    public Card cardToFlip;
    public SerializedGameState gameSnapshot;

    public Move(List<Card> movedCards, TablePosition from, TablePosition to, SerializedGameState gameSnapshot)
    {
        this.movedCards = movedCards;
        this.from = from;
        this.to = to;
        this.gameSnapshot = gameSnapshot;
    }
    
    public Card SelectedCard{
        get{
            return movedCards.First();
        }
    }   

    //If null, no card needs to be flipped
    public Card GetCardToFlip(){
        if(this.from.zone == Zone.Tableu){
            //Update start tableu pile - unreference moved cards
            CardColumn startCardColum = gameSnapshot.tableu[SelectedCard.column];
            
            //Flip card below if needed
            List<Card> faceUpCards_afterMove = startCardColum.faceUpCards.TakeUntil(c => c == SelectedCard).ToList();
            if(faceUpCards_afterMove.Count == 0 && startCardColum.faceDownCards.Count > 0){
                Card faceDownCardToFlip = gameSnapshot.tableu[SelectedCard.column].faceDownCards.Peek();
                return faceDownCardToFlip;
            }
        } 
        return null;
    }
}
