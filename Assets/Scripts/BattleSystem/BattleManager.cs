using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUIManager _uiManager;
    [SerializeField] private PullingManager _pullManager;
    [SerializeField] private PartyInfo _playerParty;
    [SerializeField] private PartyInfo _enemyParty;
}
