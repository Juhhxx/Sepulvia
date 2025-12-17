using System;
using UnityEngine;

public class EncounterEntity : MonoBehaviour
{
    [SerializeField] private PartyInfo _party;

    public event Action<PartyInfo> OnEncounterPlayer;

    private void OnEnable()
    {
        EncounterManager.Instance.RegsiterEncounterable(this);
    }

    private void OnDisable()
    {
        EncounterManager.Instance.UnregsiterEncounterable(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();

        if (p != null)
        {
            OnEncounterPlayer?.Invoke(_party);
        }
    }
}
