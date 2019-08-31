using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockClickDetector : MonoBehaviour
{
    void OnMouseUp()
    {
        GameManager.Instance.NotifyStockMove();
    }
        
}
