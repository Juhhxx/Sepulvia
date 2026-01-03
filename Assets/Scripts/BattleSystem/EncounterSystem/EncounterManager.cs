using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField] private BattleManager _battleManager;

    PartyInfo _playerParty;

    private void Awake()
    {
        base.SingletonCheck(this, false);

        _playerParty = FindAnyObjectByType<PlayerController>().PlayerParty;
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
        Debug.Log($"Doing encounter with {party.PartyName}");

        GameSceneManager.Instance.CurrentGameScene = GameSceneManager.GameSceneTypes.Battle;

        _battleManager.StartBattle(_playerParty, party);
    }

}
