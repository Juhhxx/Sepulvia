using System;
using UnityEngine;

public class EncounterEntity : MonoBehaviour
{
    [SerializeField] private PartyInfo _party;
    public Party EnemyParty { get; private set; }

    private bool _didEncounter = false;

    public event Action<Party> OnEncounterPlayer;

    private void Start()
    {
        EncounterManager.Instance.RegisterEncounterable(this);
        EnemyParty = _party.Instantiate();
    }

    private void OnDestroy()
    {
        if (EncounterManager.Instance != null)
            EncounterManager.Instance.UnregisterEncounterable(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();

        if (p != null && !_didEncounter)
        {
            OnEncounterPlayer?.Invoke(EnemyParty);
            gameObject.SetActive(false);

            _didEncounter = true;
        }
    }
}
