using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField, Scene] private string _overworldScene;
    [SerializeField, Scene] private string _battleScene;

    PartyInfo _playerParty;

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

        _ = DoEncounterAsync(party);
    }

    private async Task DoEncounterAsync(PartyInfo party)
    {
        await SceneManager.LoadSceneAsync(_battleScene, LoadSceneMode.Additive);

        Scene s = SceneManager.GetSceneByName(_battleScene);

        SceneManager.SetActiveScene(s);

        BattleManager bm = FindAnyObjectByType<BattleManager>();

        bm.StartBattle(_playerParty, party);

        bm.OnBattleEnd += async () =>
        {
            Scene s = SceneManager.GetSceneByName(_overworldScene);

            SceneManager.SetActiveScene(s);

            await SceneManager.UnloadSceneAsync(_battleScene);
        };
    }
}
