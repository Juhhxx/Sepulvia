using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField, Scene] private string _overworldScene;
    [SerializeField, Scene] private string _battleScene;

    PartyInfo _playerParty;
    BattleManager _battleManager;

    private void Awake()
    {
        base.SingletonCheck(this, true);

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

        _ = GameSceneManager.Instance.LoadNewSceneAsync(_battleScene, true,
        () =>
        {
            _battleManager = FindAnyObjectByType<BattleManager>();

            _battleManager.StartBattle(_playerParty, party);

            _battleManager.OnBattleEnd += GoBackToOverworld;
        });
    }

    private void GoBackToOverworld()
    {
        GameSceneManager.Instance.ActivateScene(_overworldScene);
        GameSceneManager.Instance.ActivateScene(_battleScene);

        _battleManager.OnBattleEnd -= GoBackToOverworld;
    }

}
