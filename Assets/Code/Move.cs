using System.Collections;
using System.Collections.Generic;
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
}
