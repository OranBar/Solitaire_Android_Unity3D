using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public Card movedCard, destinationCard;
    public int startColumn, destinationColumn;

    public Move(Card movedCard, Card destinationCard, int startColumn, int destinationColumn)
    {
        this.movedCard = movedCard;
        this.destinationCard = destinationCard;
        this.startColumn = startColumn;
        this.destinationColumn = destinationColumn;
    }

}
