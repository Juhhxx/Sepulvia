using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolver : RandomBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private PullingManager _pullManager;
    private int _playerSelectedBar = -1;
    private void SetPlayerSelectedBar(int bar)
    {
        Debug.Log($"Player Selected Bar Section {bar}");
        _playerSelectedBar = bar;
    }

    [SerializeField] private InventoryResolver _inventoryResolver;

    private void Start()
    {
        TryInitializeRandom();

        _battleManager.OnActionExecuted += ExecuteAction;
        _pullManager.OnSelectBar += SetPlayerSelectedBar;
        _battleManager.OnSoulBurn += DoChainBurn;
    }

    private void ExecuteAction(BattleAction action)
    {
        switch (action.Type)
        {
            case ActionType.Move:

                DoMove(action.Move, action.User, action.Targets);
                break;

            case ActionType.Item:

                UseItem(action.Item, action.User, action.Targets);
                break;

            case ActionType.Run:

                _battleManager.Run();
                break;
        }
    }

    public void DoMove(Move move, BattlerController user, BattlerController[] targets)
    {
        if (CheckInterrupt(user, targets))
        {
            DialogueManager.Instance.AddDialogue(
                $"{user.Character.Name} was interrupted.");
            
            return;
        }
        
        if (CheckBlock(user, targets))
        {
            DialogueManager.Instance.AddDialogue(
                $"{user.Character.Name} was blocked.");
            
            return;
        }

        Debug.Log($"{user.Character.Name} USED {move.Name} AGAINST {targets[0].Character.Name}");

        move.UsedMove();

        user.Character.RecoveryTime += move.RecoveryCost;
        user.Character.CurrentStance += GetStanceBoost(move);

        DialogueManager.Instance.AddDialogue(
            $"{user.Character.Name} used {move.Name} against {string.Join(", ", targets.Select(t => t.Character.Name))}.");

        move.MoveLogic.OnDoMove(user, targets, this);
    }

    private float GetStanceBoost(Move move)
    {
        float rnd = (float)_random.NextDouble();

        return Mathf.Lerp(move.StanceRewardRange.x, move.StanceRewardRange.y, rnd);
    }

    private bool CheckBlock(BattlerController affected, BattlerController[] targets)
    {
        bool result = false;

        if (affected.IsInterrupting)
        {
            var se = affected.StatusEffectManager.GetStatusEffect<StatusEffectInterrupt>();

            se.StatusEffectLogic.OnTriggerEffect(targets);

            return false;
        }

        foreach (BattlerController bc in targets)
        {
            if (bc.IsBlocking)
            {
                var se = bc.StatusEffectManager.GetStatusEffect<StatusEffectBlock>();

                se.StatusEffectLogic.OnTriggerEffect(affected);

                result = true;
            }
        }

        return result;
    }

    private bool CheckInterrupt(BattlerController affected, BattlerController[] targets)
    {
        bool result = false;

        foreach (BattlerController bc in targets)
        {
            if (bc.IsInterrupting)
            {
                var se = bc.StatusEffectManager.GetStatusEffect<StatusEffectInterrupt>();

                se.StatusEffectLogic.OnTriggerEffect(affected);

                result = true;
            }
        }

        return result;
    }

    public void DoPull(int strenght, BattlerController user)
    {
        int pullStrenght = strenght + user.Character.PullStrenghtBonus;

        if (pullStrenght < 0) pullStrenght = 0;

        if (pullStrenght > 0)
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Character.Name} pulled the Soul to their side by {pullStrenght}.");
        }
        else
        {
            DialogueManager.Instance.AddDialogue(
            $"{user.Character.Name} failed to pull the Soul to their side.");
        }
       
        user.Character.Animator?.SetTrigger("Attack");
        
        Debug.Log($"{_pullManager}");
        if (user.IsPlayer())
        {
            _pullManager.MoveHeart(-pullStrenght, user);
        } 
        else
        {
            _pullManager.MoveHeart(pullStrenght, user);
        }
    }

    public void DoStatusEffect(StatusEffect statusEffect, BattlerController[] targets)
    {
        foreach (BattlerController target in targets)
        {
            target.StatusEffectManager.AddStatusEffect(statusEffect, target);
        }
    }

    private bool _applyingBarModifier = false;
    public bool ApplyingBarModifier => _applyingBarModifier;
    public void DoBarModifier(BarModifierInfo modifier)
    {
        _applyingBarModifier = true;
        StartCoroutine(DoBarModifierCR(modifier));
    }

    private IEnumerator DoBarModifierCR(BarModifierInfo modifier)
    {
        _battleManager.CurrentState = BattleManager.BattleState.PlayerChooseBar;

        yield return new WaitUntil(() => _playerSelectedBar >= 0);

        ApplyBarModifier(_playerSelectedBar, modifier);

        _playerSelectedBar = -1;
        _applyingBarModifier = false;
    }

    public void ApplyBarModifier(int section, BarModifierInfo modifier)
    {
        Debug.Log($"Applying Bar Modifier {modifier.Name} to Section {section}");
        _pullManager.BarSections[section].AddBarModifier(modifier);
    }

    private bool _playingMinigame = false;
    public bool PlayingMinigame => _playingMinigame;
    private GameObject _activeMinigame;
    public IMoveMinigame DoMinigame(BattlerController player, GameObject minigamePrefab)
    {
        _playingMinigame = true;
        _activeMinigame = Instantiate(minigamePrefab);

        IMoveMinigame minigame = _activeMinigame.GetComponent<IMoveMinigame>();
        
        if (player.Character.HasPassiveEffect<DollCharm>())
        {
            minigame.MakeEasier();
        }

        return minigame;
    }
    public void FinishedMinigame()
    {
        _playingMinigame = false;

        Destroy(_activeMinigame);
        _activeMinigame = null;
    }

    public void DoChainBurn(int left, int right)
    {
        DoChainShatter(left, false, true);
        DoChainShatter(right, true, true);
    }
    public void DoChainShatter(int number, bool right, bool burn = false)
    {
        _pullManager.BreakBarSections(number, !right, burn);
    }

    public void UseItem(ItemStack item, BattlerController user, BattlerController[] targets)
    {
        _inventoryResolver.UseItem(item, user, targets);
    }

    public (List<ItemInfo>, int) GiveRewards(EnemyParty enemyParty, bool spared)
    {
        // Return Values
        List<ItemInfo> items = new List<ItemInfo>();
        int essence = 0;

        // Party Values
        List<ItemInfo> possibleRewards = new List<ItemInfo>();
        int totalDifficulty = 0;

        foreach (Enemy e in enemyParty.PartyMembers)
        {
            totalDifficulty += e.DifficultyLevel;

            possibleRewards.AddRange(e.PossibleRewards);
        }

        // Reward Calculation
        essence = _random.Next(1, (totalDifficulty * 2) + 1) * 5;

        if (spared) essence *= 2;

        if (!spared && possibleRewards.Count > 0)
        {
            int num = _random.Next(1, totalDifficulty + 1);

            for (int i = 0; i < num; i++)
            {
                int rnd = _random.Next(0, possibleRewards.Count);

                items.Add(possibleRewards[rnd]);
            }
        }
        
        return (items, essence);
    }

    public bool CanRun(BattlerController user, EnemyParty enemyParty)
    {
        float rnd = (float)_random.NextDouble();

        float difficultyAverage = 0;

        foreach (Enemy e in enemyParty.PartyMembers)
        {
            difficultyAverage += e.DifficultyLevel;
        }

        difficultyAverage /= enemyParty.PartySize;

        float chance = (1 / difficultyAverage) + (user.Character as Player).RunChance;

        bool result = rnd <= chance;

        if (enemyParty.PartyMembers.Any(e => !(e as Enemy).CanRun)) result = false;

        if (result) DialogueManager.Instance.AddDialogue($"{user.Character.Name} ran away!");
        else DialogueManager.Instance.AddDialogue($"{user.Character.Name} couldn't run from battle.");

        return result;
    }
}
