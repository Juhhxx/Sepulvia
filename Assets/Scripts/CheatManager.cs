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
}
