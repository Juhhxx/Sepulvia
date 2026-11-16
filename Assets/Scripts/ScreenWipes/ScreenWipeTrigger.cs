using UnityEngine;
using NaughtyAttributes;
using System;

public class ScreenWipeTrigger : MonoBehaviour
{
    [SerializeField, ScreenWipeDropdown] private string _wipe;

    private ScreenWipe _currentWipe;
    public ScreenWipe CurrentWipe => _currentWipe;
    
    private bool _testWipe = false;

    [Button("Test Wipe", EButtonEnableMode.Playmode)]
    public void TestWipe()
    {
        _testWipe = !_testWipe;
        TriggerWipe(_testWipe);
    }

    public void TriggerWipe(bool fadeIn, Action onComplete = null)
    {
        _currentWipe = ScreenWipeManager.Instance?.GetWipe(_wipe);

        if (_currentWipe == null)
        {
            Debug.LogWarning($"[ScreenWipeTrigger] Screen wipe '{_wipe}' wasn't found!");
            return;
        }

        if (fadeIn)
        {
            _currentWipe.WipeIn(onComplete);
        }
        else
        {
            _currentWipe.WipeOut(onComplete);
        }
    }
}
