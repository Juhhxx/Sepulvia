using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Battler Managers")]
    [Space(5)]
    [SerializeField] private BattleResolver _battleResolver;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private BattleUIManager _uiManager;

    [Space(10)]
    [Header("Battlers Info")]
    [Space(5)]
    [SerializeField] private PartyInfo _playerParty;
    [SerializeField] private PartyInfo _enemyParty;

    private IEnumerator BattleLoopCR()
    {
        yield return null;
    }
}
