using UnityEngine;
using NaughtyAttributes;

public class StatusEffectShowcaseManager : MonoBehaviour
{
    [SerializeField] private StatusEffectShowcase _showcasePrefab;
    [SerializeField] private Transform _showcaseParent;
    [SerializeField] private StatusEffectInfoPanel _infoPanel;
    [SerializeField] private bool _isEnemyShowcase;
    [SerializeField, ShowIf("_isEnemyShowcase")] private StatusEffectManager _statusEffectManager;

    private BattleManager _battleManager;

    private void Awake()
    {
        if (!_isEnemyShowcase)
        {
            _battleManager = FindAnyObjectByType<BattleManager>();
        }
    }

    private void Start()
    {
        if (!_isEnemyShowcase) return;

        _statusEffectManager.OnAddStatusEffect += AddStatusEffectShowcase;
    }

    private void Update()
    {
        if (_isEnemyShowcase) return;

        if (_statusEffectManager == null)
        {
            _statusEffectManager = _battleManager.GetBattlerController(_battleManager.Player)
                                            .GetComponent<StatusEffectManager>();
                                            
            _statusEffectManager.OnAddStatusEffect += AddStatusEffectShowcase;
        }
    }

    private void AddStatusEffectShowcase(StatusEffect se)
    {
        if (se.Icon == null) return;
        
        var showcase = Instantiate(_showcasePrefab, _showcaseParent);
        showcase.SetUpShowcase(se, _infoPanel);
    }

}
