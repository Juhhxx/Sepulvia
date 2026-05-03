using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class EncounterManager : RandomBehaviour
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private float _timeBeforeBattleStart = 0.5f;
    [SerializeField] private List<PartyInfo> _possibleEncounters;

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

    private void DoEncounter(Party party, EncounterEntity entity = null)
    {
        StartCoroutine(DoEncounterCR(party, entity));
    }
    private IEnumerator DoEncounterCR(Party party, EncounterEntity entity)
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
        Party party = _possibleEncounters[_random.Next(0, _possibleEncounters.Count)].Instantiate();

        DoEncounter(party);
    }

    private void ReturnToOverworld()
    {
        Debug.Log($"[Encounter Manager] Ended Encounter", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Overworld;

        _player.InBattle = false;
    }
}
