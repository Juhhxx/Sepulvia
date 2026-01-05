using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private List<PartyInfo> _possibleEncounters;

    PlayerController _player;

    private void Awake()
    {
        base.SingletonCheck(this, false);

        _player = FindAnyObjectByType<PlayerController>();

        _battleManager.OnBattleEnd += ReturnToOverworld;
    }

    public void RegsiterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer += DoEncounter;
    }

    public void UnregsiterEncounterable(EncounterEntity entity)
    {
        entity.OnEncounterPlayer -= DoEncounter;
    }

    private void DoEncounter(PartyInfo party)
    {
        Debug.Log($"[Encounter Manager] Doing encounter with {party.PartyName}", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Battle;

        _battleManager.StartBattle(_player.PlayerParty, party);

        _player.InBattle = true;
    }
    public void DoRandomEncounter()
    {
        PartyInfo party = _possibleEncounters[Random.Range(0, _possibleEncounters.Count)];

        Debug.Log($"[Encounter Manager] Doing encounter with {party.PartyName}", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Battle;

        _battleManager.StartBattle(_player.PlayerParty, party);

        _player.InBattle = true;
    }

    private void ReturnToOverworld()
    {
        Debug.Log($"[Encounter Manager] Ended Encunter", this);

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Overworld;

        _player.InBattle = false;
    }

}
