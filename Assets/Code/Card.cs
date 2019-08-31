using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card: ICloneable 
{
    public readonly int value;
    public readonly Suit suit;
    public readonly SuitColor suitColor;
    public Zone zone;
    public int column = -1; //Only when zone == tableu

    public Card(int value, Suit suit)
    {
        this.value = value;
        this.suit = suit;
        this.suitColor = suit.GetSuitColor();
    }

    public object Clone(){
        Card result = new Card(value, suit);
        result.zone = this.zone;
        return result;
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