using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockClickDetector : MonoBehaviour
{

    public float cooldown;
    public float lastClickTime = 0;

    void Start()
    {
        //Diasllow using the stock pile until the intial animation is complete
        lastClickTime = SolitaireGraphics.Instance.cardToTableu_animDuration * 29;    
    }

    void OnMouseUp()
    {
        if(Time.time - lastClickTime > cooldown){
            lastClickTime = Time.time;
            GameManager.Instance.NotifyStockMove();
        }
    }
        
}
