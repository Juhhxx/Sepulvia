using System;
using NaughtyAttributes;
using UnityEngine;

public class EncounterEntity : MonoBehaviour
{
    [SerializeField, Expandable] private PartyInfo _party;
    public Party EnemyParty { get; private set; }

    private bool _didEncounter = false;
    private EncounterManager _encounterManager;

    public event Action<Party, EncounterEntity> OnEncounterPlayer;

    private void Start()
    {
        _encounterManager = FindAnyObjectByType<EncounterManager>();

        _encounterManager.RegisterEncounterable(this);
        EnemyParty = _party.Instantiate();
    }

    private void OnDestroy()
    {
        _encounterManager.UnregisterEncounterable(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();

        if (p != null && !_didEncounter)
        {
            OnEncounterPlayer?.Invoke(EnemyParty, this);

            _didEncounter = true;
        }
    }
}
