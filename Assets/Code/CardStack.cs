using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : ICard
{
    public List<ICard> cardsAbove = new List<ICard>();
    public override void AddCard(ICard newCard)
    {
        cardsAbove.Add(newCard);
        foreach(ICard card in cardsAbove){
            card.AddCard(newCard);
        }        
    }
}
