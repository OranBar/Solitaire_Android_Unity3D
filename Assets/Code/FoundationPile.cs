using System.Collections.Generic;

public class FoundationPile
{
    public Stack<Card> cards = new Stack<Card>();
    public Suit suit;

    public FoundationPile(Suit suit)
    {
        this.suit = suit;
    }
}