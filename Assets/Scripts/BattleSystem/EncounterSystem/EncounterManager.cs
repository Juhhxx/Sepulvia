using System.Collections.Generic;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private float _timeBeforeBattleStart = 0.5f;
    [SerializeField] private List<PartyInfo> _possibleEncounters;

    PlayerController _player;

    private void Awake()
    {
        base.SingletonCheck(this, false);

        _player = FindAnyObjectByType<PlayerController>();

        _battleManager.OnBattleEnd += ReturnToOverworld;
    }

    public void RegisterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer += DoEncounter;
    }

    public void UnregisterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer -= DoEncounter;
    }

    private void DoEncounter(Party party)
    {
        StartCoroutine(DoEncounterCR(party));
    }
    private IEnumerator DoEncounterCR(Party party)
    {
        yield return new WaitForSeconds(_timeBeforeBattleStart);

        Debug.Log($"[Encounter Manager] Doing encounter with {party.PartyName}", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Battle;

        _battleManager.StartBattle(_player.PlayerParty, party);

        _player.InBattle = true;
    }

    public void DoRandomEncounter()
    {
        Party party = _possibleEncounters[Random.Range(0, _possibleEncounters.Count)].Instantiate();

        DoEncounter(party);
    }

    private void ReturnToOverworld()
    {
        Debug.Log($"[Encounter Manager] Ended Encounter", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Overworld;

        _player.InBattle = false;
    }

}
