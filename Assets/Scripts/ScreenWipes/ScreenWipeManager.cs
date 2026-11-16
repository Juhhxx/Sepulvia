using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System;

public class ScreenWipeManager : MonoBehaviourSingleton<ScreenWipeManager>
{
    [SerializeField] private List<ScreenWipe> _wipes = new();

    public IReadOnlyList<ScreenWipe> Wipes => _wipes;

    private void Awake()
    {
        base.SingletonCheck(this, true);
        
        RefreshWipes();
    }

    [Button("Refresh Wipes")]
    public void RefreshWipes()
    {
        _wipes.Clear();
        _wipes.AddRange(GetComponentsInChildren<ScreenWipe>(true));
    }

    public ScreenWipe GetWipe(string wipeName)
    {
        ScreenWipe wipe = _wipes.Find(w => w.WipeName == wipeName);

        if (wipe == null)
        {
            Debug.LogWarning($"[ScreenWipeManager] Screen wipe '{wipeName}' wasn't found!");
            return null;
        }

        return wipe;
    }
}
