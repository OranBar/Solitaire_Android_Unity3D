using System;
using System.Collections.Generic;
using System.Linq;

public class FoundationPile : ICloneable
{
    public Stack<Card> cards = new Stack<Card>();
    public Suit suit;

    public FoundationPile(Suit suit)
    {
        this.suit = suit;
    }

    public object Clone(){
        FoundationPile clonePile = new FoundationPile(this.suit);
        clonePile.cards = new Stack<Card>(this.cards.Select(c => c.Clone()).Cast<Card>().Reverse());
        
        return clonePile;
    }
}