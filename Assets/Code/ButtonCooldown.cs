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
        //36 is the number of cards that are put down when playing with 7 columns
        beginCooldown = SolitaireGraphics.Instance.cardToTableu_animDuration * 37;

        if(beginCooldown > 0){
            ActivateBeginCooldown();
        }
        if(clickCooldown > 0){
            myButton.onClick.AddListener(()=>{
                ActivateClickCooldown();
            });
        }
    }

    public void ActivateBeginCooldown(){
        myButton.interactable = false;
        StartCoroutine(SetButtonInteractable_Coro(true, beginCooldown));
    }

    public void ActivateClickCooldown(){
        myButton.interactable = false;
        StartCoroutine(SetButtonInteractable_Coro(true, clickCooldown));
    }

    private IEnumerator SetButtonInteractable_Coro(bool interactable, float delay){
        yield return new WaitForSeconds(delay);
        myButton.interactable = interactable;
    }
}
