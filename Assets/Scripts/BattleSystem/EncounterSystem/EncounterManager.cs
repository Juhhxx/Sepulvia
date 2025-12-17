using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviourSingleton<EncounterManager>
{
    [SerializeField, Scene] private string _battleScene;

    private void Awake()
    {
        base.SingletonCheck(this);
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
    }
}
