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
    [SerializeField] private StatusEffectInfo _curse;

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
        var player = FindAnyObjectByType<PlayerController>().PlayerCharacter;
        player.CurrentStance = player.MaxStance;
    }

    public void GiveCurse()
    {
        BattleManager bm = FindAnyObjectByType<BattleManager>();

        if (bm.Player != null && _curse != null)
        {
            bm.GetBattlerController(bm.Player).StatusEffectManager.AddStatusEffect(_curse.Instantiate());
        }
    }
}
