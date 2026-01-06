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
        foreach (CheatCode cheat in _cheatCodes)
        {
            if (Input.GetKey(cheat.FirstKey) && Input.GetKeyDown(cheat.SecondKey))
            {
                cheat.Cheat.Invoke();
            }
        }
    }

    public void InfiniteStance()
    {
        Debug.Log("Infinite Stance Activated");
        FindAnyObjectByType<PlayerController>().PlayerCharacter.SetBaseStance(9999);
        FindAnyObjectByType<PlayerController>().PlayerCharacter.CurrentStance = 9999;
    }
}
