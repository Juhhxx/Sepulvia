using System;
using UnityEngine;

public class EncounterEntity : MonoBehaviour
{
    [SerializeField] private PartyInfo _party;

    private bool _didEncounter = false;

    public event Action<PartyInfo> OnEncounterPlayer;

    private void Start()
    {
        EncounterManager.Instance.RegsiterEncounterable(this);
    }

    private void OnDestroy()
    {
        if (EncounterManager.Instance != null)
            EncounterManager.Instance.UnregsiterEncounterable(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();

        if (p != null && !_didEncounter)
        {
            OnEncounterPlayer?.Invoke(_party);
            gameObject.SetActive(false);

            _didEncounter = true;
        }
    }
}
