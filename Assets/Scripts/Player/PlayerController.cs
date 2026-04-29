using NaughtyAttributes;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour, IPausable
{
    [SerializeField, Expandable] public PartyInfo _playerParty;
    public Party PlayerParty { get; private set; }

    // Player Party never has more than 1 character
    public Character PlayerCharacter => PlayerParty.PartyMembers[0];

    public void ChangeEssence(int amount) => (PlayerCharacter as Player).ChangeEssence(amount);

    [SerializeField] private float _immunityTime = 1f;
    [SerializeField] private LayerMask _excludeWhenImmune;
    private Timer _immunityTimer;
    private bool _isImmune = false;
    public bool IsImmune => _isImmune;

    [SerializeField, ReadOnly] private bool _inBattle = false;
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
            else
            {
                ToogleImmunity(true);
            }

            _inBattle = value;
        }
    }

    public event Action<bool> OnBattleEnterExit;

    private PlayerMovement _playerMovement;
    public bool CanDash => _playerMovement.CanDash;
    public float DashCooldownTime => _playerMovement.DashCooldownTime;

    private Collider _collider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        PlayerParty = _playerParty.Instantiate();

        Debug.Log($"[Player Controller] Player Character : {PlayerCharacter.Name}", this);

        _playerMovement = GetComponent<PlayerMovement>();

        PlayerCharacter.OnStanceLost += () =>
        {
            if (!InBattle)
            {
                Debug.Log("[Player Controller] Player has fallen unconscious!", this);
                EncounterManager.Instance.DoRandomEncounter();   
            }
        };

        _immunityTimer = new Timer(_immunityTime);

        _immunityTimer.OnTimerDone += () => ToogleImmunity(false);

        _collider = GetComponent<Collider>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        if (Paused) return;

        if (!_inBattle)
        {
            _playerMovement.DoMovement();
        }

        if (_isImmune)
        {
            _immunityTimer.CountTimer();
        }
    }

    private void ToogleImmunity(bool onOff)
    {
        _immunityTimer.ResetTimer();

        _collider.excludeLayers = onOff ? _excludeWhenImmune : LayerMask.GetMask();

        Color c = _spriteRenderer.color;
        c.a = onOff ? 0.5f : 1f;

        _spriteRenderer.color = c;

        _isImmune = onOff;
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
        Paused = onOff;
    }
}
