using System.Collections.Generic;
using UnityEngine;

public class BattleTest : MonoBehaviourSingleton<BattleTest>
{
    [SerializeField] private PartyInfo _playerParty;
    [SerializeField] private List<PartyInfo> _enemyPartys;
    [SerializeField] private BattleManager _battleManager;

    private void Start()
    {
        base.SingletonCheck(this, false);
        StartBattle();
    }
    public void StartBattle()
    {
        _battleManager.StartBattle(_playerParty, _enemyPartys[Random.Range(0, _enemyPartys.Count)]);
    }

    public void Quit() => Application.Quit();
}
