using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISolitaireGraphics 
{
    void SetupGraphics(CardColumn[] tableu, List<Card> stockPile);
    void NotifyLegalMove(Move move);
    void NotifyIllegalMove(IllegalMove move);
}
