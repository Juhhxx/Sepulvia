using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class CheatManager : MonoBehaviour
{
    [Serializable]
    public class CheatCode
    {
        public KeyCode FirstKey;
        public KeyCode SecondKey;
        public UnityEvent Cheat;
    }

    [SerializeField, ReorderableList] private List<CheatCode> _cheatCodes;

    private void Update()
    {
        DoCheats();
    }
    
    private void DoCheats()
    {
        foreach (CheatCode cheat in _cheatCodes)
        {
            if (Input.GetKey(cheat.FirstKey) && Input.GetKeyDown(cheat.SecondKey))
            {
                cheat.Cheat?.Invoke();
            }
        }
    }
}
