using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ICard 
{
    public readonly int value;
    public readonly int suit;
    public SuitColor suitColor;
    public ICard cardBelow;
    public ICard cardAbove;
    public bool isFaceDown;

    public abstract void AddCard(ICard card);
}
