using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public List<Card> movedCards;
    public Card destinationCard;
    // public int startColumn, destinationColumn;

    public Move(List<Card> movedCards, Card destinationCard)
    {
        this.movedCards = movedCards;
        this.destinationCard = destinationCard;
        // this.startColumn = startColumn;
        // this.destinationColumn = destinationColumn;
    }
}
