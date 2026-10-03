using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectShowcaseManager : MonoBehaviour
{
    [SerializeField] private StatusEffectShowcase _showcasePrefab;
    [SerializeField] private Transform _showcaseParent;

    private BattleManager _battleManager;
    private StatusEffectManager _statusEffectManager;

    private void Awake()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
    }

    private void Start()
    {
        _statusEffectManager = _battleManager.GetBattlerController(_battleManager.Player)
                                            .GetComponent<StatusEffectManager>();

        _statusEffectManager.OnAddStatusEffect += AddStatusEffectShowcase;
    }

    private void AddStatusEffectShowcase(StatusEffect se)
    {
        var showcase = Instantiate(_showcasePrefab, _showcaseParent);
        showcase.SetUpShowcase(se);
    }

}
