using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit{
    Hearts=0,
    Diamonds=1,
    Clubs=2,
    Spades=3
}

public enum SuitColor{
    Red = 0,
    Black = 1
}

static class SuitMethods
{
    public static SuitColor GetSuitColor(this Suit s1)
    {
        if(s1 == Suit.Hearts || s1 == Suit.Diamonds){
            return SuitColor.Red;
        }
        else if(s1 == Suit.Clubs || s1 == Suit.Spades){
            return SuitColor.Black;
        }

        throw new Exception("Can't identify suit color. Did you add Jollys?");
        
    }
}

public class Card 
{
    public readonly int value;
    public readonly Suit suit;
    public readonly SuitColor suitColor;
    public int column = -1;

    public Card(int value, Suit suit)
    {
        this.value = value;
        this.suit = suit;
        this.suitColor = suit.GetSuitColor();
    }

    public override string ToString(){
        
        return CardValueToString(value)+""+suit.ToString();
    }


    public static string CardValueToString(int value){
        switch(value){
            case 1: return "A"; 
            case 11: return "J"; 
            case 12: return "Q"; 
            case 13: return "K"; 
            default: return value.ToString();
        }
    }
}


/*
public class Card 
{
    
    public readonly int value;
    public readonly int suit;
    public SuitColor suitColor;
    public List<Card> cardsBelow;
    public List<Card> cardsAbove;
    public bool isFaceDown;

    public void AddCard(Card newCard){
        foreach(Card card in this.cardsBelow){
            card.cardsAbove.Add(newCard);
        }
        foreach(Card card in this.cardsAbove){
            card.cardsAbove.Add(newCard);
        }
        
    }
}

 */