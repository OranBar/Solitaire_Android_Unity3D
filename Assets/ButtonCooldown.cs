using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldown : MonoBehaviour
{
    public float beginCooldown=0, clickCooldown=0;
    private Button myButton;

    public void Start(){
        myButton = GetComponent<Button>();
        beginCooldown = SolitaireGraphics.Instance.cardToTableu_animDuration * 29;
        
        if(beginCooldown > 0){
            myButton.interactable = false;
            StartCoroutine(SetButtonInteractable_Coro(true, beginCooldown));
        }
        if(clickCooldown > 0){
            myButton.onClick.AddListener(()=>{
                myButton.interactable = false;
                StartCoroutine(SetButtonInteractable_Coro(true, clickCooldown));
            });
        }
    }

    private IEnumerator SetButtonInteractable_Coro(bool interactable, float delay){
        yield return new WaitForSeconds(delay);
        myButton.interactable = interactable;
    }
}
