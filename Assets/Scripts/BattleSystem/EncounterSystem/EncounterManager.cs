using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class EncounterManager : RandomBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private float _timeBeforeBattleStart = 0.5f;
    [SerializeField] private EnemyPartyDataBase _enemyPartyDataBase;

    PlayerController _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _battleManager.OnBattleEnd += ReturnToOverworld;
    }

    private void Start()
    {
        TryInitializeRandom();
    }

    public void RegisterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer += DoEncounter;
    }

    public void UnregisterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer -= DoEncounter;
    }

    private void DoEncounter(EnemyParty party, EncounterEntity entity = null)
    {
        StartCoroutine(DoEncounterCR(party, entity));
    }
    private IEnumerator DoEncounterCR(EnemyParty party, EncounterEntity entity)
    {
        yield return new WaitForSeconds(_timeBeforeBattleStart);

        Debug.Log($"[Encounter Manager] Doing encounter with {party.PartyName}", this);

        entity?.gameObject.SetActive(false);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Battle;

        _battleManager.StartBattle(_player.PlayerParty, party);

        _player.InBattle = true;
    }

    public void DoRandomEncounter()
    {
        EnemyParty party = _enemyPartyDataBase.GetRandomParty(_random, (_player.PlayerCharacter as Player).Level).Instantiate() as EnemyParty;

        DoEncounter(party);
    }

    private void ReturnToOverworld()
    {
        Debug.Log($"[Encounter Manager] Ended Encounter", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Overworld;

        _player.InBattle = false;
    }
}
