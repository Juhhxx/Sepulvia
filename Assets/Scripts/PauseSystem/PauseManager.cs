using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviourSingleton<PauseManager>
{
    public Action<bool> OnTogglePause;

    private void Awake()
    {
        base.SingletonCheck(this, true);
        
        SceneManager.activeSceneChanged += (_,_) => OnTogglePause = null;
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