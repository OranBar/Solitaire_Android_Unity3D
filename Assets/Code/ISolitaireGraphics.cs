using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISolitaireGraphics 
{
    void SetupGraphics(CardColumn[] tableu, Stack<Card> stockPile);
    void NotifyLegalMove(Move move);
    void NotifyIllegalMove(IllegalMove move);
    void NotifyRestoreStockpileFromWastePile(List<Card> restoredStockPile);
    void NotifyFlipStockCardMove(Card nextCard, List<Card> cardsInWastePile);
}
