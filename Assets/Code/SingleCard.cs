using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCard : ICard
{
    public override void AddCard(ICard card)
    {
        if(this.cardBelow.isFaceDown == false){
            CardStack newCardStack = new CardStack();
            newCardStack.AddCard(this);
            newCardStack.AddCard(card);
            // cardBelow.cardAbove = newCardStack;
        }
        throw new System.NotImplementedException();
    }
}
