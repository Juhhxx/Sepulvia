using System;
using NaughtyAttributes;
using UnityEngine;

public class EncounterEntity : MonoBehaviour
{
    [SerializeField, Expandable] private PartyInfo _party;
    public EnemyParty EnemyParty { get; private set; }

    private bool _didEncounter = false;
    private EncounterManager _encounterManager;

    public event Action<EnemyParty, EncounterEntity> OnEncounterPlayer;

    private void Start()
    {
        _encounterManager = FindAnyObjectByType<EncounterManager>();

        _encounterManager.RegisterEncounterable(this);
        EnemyParty = _party.Instantiate() as EnemyParty;
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
