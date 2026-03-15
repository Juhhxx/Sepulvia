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

    public void RegisterPausable(IPausable pausable)
    {
        Debug.Log($"[Pause Manager] Registered {pausable}", this);
        OnTogglePause += pausable.TogglePause;
    }
    public void UnregisterPausable(IPausable pausable)
    {
        Debug.Log($"[Pause Manager] Unregistered {pausable}", this);
        OnTogglePause -= pausable.TogglePause;
    }
    
    public void Pause()
    {
        Debug.Log("[Pause Manager] Paused Game", this);
        OnTogglePause?.Invoke(true);
    }
    public void UnPause()
    {
        Debug.Log("[Pause Manager] Unpaused Game", this);
        OnTogglePause?.Invoke(false);
    }
}