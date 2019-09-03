using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card : ICloneable, IEquatable<Card>
{
    public readonly int value;
    public readonly Suit suit;
    public readonly SuitColor suitColor;
    // public Zone zone;
    // public int column = -1; //Only when zone == tableu

    public Zone GetZone(GameState snapshot){
        return snapshot.GetCardZone(this);
    }

    public int GetColumn(GameState snapshot){
        return snapshot.GetCardColumn(this);
    }

    public Card(int value, Suit suit)
    {
        this.value = value;
        this.suit = suit;
        this.suitColor = suit.GetSuitColor();
    }

    public object Clone(){
        Card result = new Card(value, suit);
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
#region EqualsOverride
    public override bool Equals(object obj)
        {
            return this.Equals(obj as Card);
        }

        public bool Equals(Card p)
        {
            // If parameter is null, return false.
            if (object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return this.ToString() == p.ToString();
        }

        public override int GetHashCode()
        {
            return this.value * 0x00010000 + (int)suit;
        }

        public static bool operator ==(Card lhs, Card rhs)
        {
            // Check for null on left side.
            if (object.ReferenceEquals(lhs, null))
            {
                if (object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Card lhs, Card rhs)
        {
            return !(lhs == rhs);
        }
#endregion
}