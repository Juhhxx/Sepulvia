using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviourSingleton<PauseManager>
{
    public Action<bool> OnTogglePause;

    private void Awake()
    {
        base.SingletonCheck(this, true);
    }
    
    public void Pause()
    {
        Debug.Log("[Pause Manager] Paused Game", this);
    }
    public void UnPause()
    {
        Debug.Log("[Pause Manager] Unpaused Game", this);
    }
}