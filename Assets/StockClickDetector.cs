using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockClickDetector : MonoBehaviour
{

    public float cooldown;
    public float lastClickTime = 0;

    void OnMouseUp()
    {
        if(Time.time - lastClickTime > cooldown){
            lastClickTime = Time.time;
            GameManager.Instance.NotifyStockMove();
        }
    }
        
}
