using NaughtyAttributes;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IPausable
{
    [field: SerializeField, Expandable] public PartyInfo PlayerParty { get; private set; }

    // Player Party never has more than 1 character
    public CharacterInfo PlayerCharacter => PlayerParty.PartyMembers[0];

    private bool _inBattle = false;
    public bool InBattle 
    {
        get => _inBattle;

        set
        {
            if (value != _inBattle)
            {
                OnBattleEnterExit?.Invoke(value);
            }
            
            if (value)
            {
                _playerMovement?.SetVelocity(Vector3.zero);
            }

            _inBattle = value;
        }
    }

    public event Action<bool> OnBattleEnterExit;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        PlayerParty = PlayerParty.Instantiate();
        _playerMovement = GetComponent<PlayerMovement>();

        PlayerCharacter.OnStanceLost += () =>
        {
            if (!InBattle)
            {
                Debug.Log("[Player Controller] Player has fallen unconscious!", this);
                EncounterManager.Instance.DoRandomEncounter();   
            }
        };
    }

    private void Start()
    {
        PauseManager.Instance?.RegisterPausable(this);
    }

    private void OnEnable()
    {
        PauseManager.Instance?.RegisterPausable(this);
    }

    private void OnDisable()
    {
        PauseManager.Instance?.UnregisterPausable(this);
    }

    private void Update()
    {
        if (!_inBattle && !Paused)
        {
            _playerMovement.DoMovement();
        }
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
        Paused = onOff;
    }
}
