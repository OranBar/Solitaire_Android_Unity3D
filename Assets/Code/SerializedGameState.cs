using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerializedGameState 
{
    public CardColumn[] tableu;
    public Stack<Card> stockPile;
    public Stack<Card> wastePile;
    public FoundationPile[] foundationPiles;

    public SerializedGameState(GameManager gameState){
        this.tableu = gameState.tableu.Select(cc => cc.Clone()).Cast<CardColumn>().ToArray();
        this.stockPile = new Stack<Card>(gameState.stockPile.ToList().Select(c => c.Clone()).Cast<Card>().Reverse());
        this.wastePile = new Stack<Card>(gameState.wastePile.ToList().Select(c => c.Clone()).Cast<Card>().Reverse());
        this.foundationPiles = gameState.foundationPiles.Select(fp => fp.Clone()).Cast<FoundationPile>().ToArray();
    }
    
}
