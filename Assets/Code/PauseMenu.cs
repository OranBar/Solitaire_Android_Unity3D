using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Singleton<PauseMenu>
{
    public bool isPaused;
    void OnEnable()
    {
        isPaused=true;
    }

    void OnDisable()
    {
        isPaused=false;
    }

    protected override void InitTon() { }
}
