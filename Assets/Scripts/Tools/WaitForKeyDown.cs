using UnityEngine;

public class WaitForKeyDown : CustomYieldInstruction
{
    private string _key;
    private bool _forceKeyPress;

    public override bool keepWaiting
    {
        get
        {
            bool tmp = !Input.GetButtonDown(_key);

            if (_forceKeyPress)
            {
                _forceKeyPress = false;
                tmp = false;
            }

            return tmp;
        }
    }

    public void ForceKeyPress()
    {
        _forceKeyPress = true;
    }

    public WaitForKeyDown(string key)
    {
        _key = key;
        _forceKeyPress = false;
    }
}