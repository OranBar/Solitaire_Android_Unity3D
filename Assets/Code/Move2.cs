using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Move2 
{
    public List<Card> movedCards;
    public Zone from_zone;
    public int from_index;
    public Zone to_zone;
    public int to_index;

    public Move2(List<Card> movedCards, Zone from_zone, int from_index, Zone to_zone, int to_index)
    {
        this.movedCards = movedCards;
        this.from_zone = from_zone;
        this.from_index = from_index;
        this.to_zone = to_zone;
        this.to_index = to_index;
    }
}
