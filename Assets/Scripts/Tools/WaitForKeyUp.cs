using UnityEngine;

public class WaitForKeyUp : CustomYieldInstruction
{
    private string _key;
    public override bool keepWaiting
    {
        get
        {
            return !Input.GetButtonUp(_key);
        }
    }

    public WaitForKeyUp(string key)
    {
        _key = key;
    }
}