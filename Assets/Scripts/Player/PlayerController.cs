using NaughtyAttributes;
using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IPausable
{
    [SerializeField, Expandable] public PartyInfo _playerParty;
    public Party PlayerParty { get; private set; }

    // Player Party never has more than 1 character
    public Character PlayerCharacter => PlayerParty.PartyMembers[0];

    public void ChangeEssence(int amount)
    {
        (PlayerCharacter as Player).ChangeEssence(amount);
        OnEssenceChange?.Invoke();
    }

    // Immunity
    [SerializeField] private float _immunityTime = 1f;
    [SerializeField] private LayerMask _excludeWhenImmune;
    private Timer _immunityTimer;
    private bool _isImmune = false;
    public bool IsImmune => _isImmune;

    // Hurt
    [SerializeField] private float _hurtTime = 1f;
    [SerializeField] private float _hurtForce = 3f;
    private Timer _hurtTimer;
    private bool _isHurt = false;
    public bool IsHurt => _isHurt;

    // Battle State
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
                ToggleImmunity(true);
            }

            _inBattle = value;
        }
    }
    public event Action<bool> OnBattleEnterExit;

    // Movement and Dash
    private PlayerMovement _playerMovement;
    public bool CanDash => _playerMovement.CanDash;
    public float DashCooldownTime => _playerMovement.DashCooldownTime;

    // Player Components
    private Collider _collider;
    private SpriteRenderer _spriteRenderer;
    private Animator _anim;

    // Player States
    public UnityEvent OnPlayerDamaged;
    public UnityEvent OnPlayerHealed;
    public UnityEvent OnEssenceChange;

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

        PlayerCharacter.OnStanceChange += (int current, int max, int previous) =>
        {
            if (!InBattle)
            {
                if (previous > current) OnPlayerDamaged?.Invoke();
                else if (previous < current) OnPlayerHealed?.Invoke();
            }
        };

        // Immunity Timer
        _immunityTimer = new Timer(_immunityTime);

        _immunityTimer.OnTimerDone += () => ToggleImmunity(false);

        // Hurt Timer
        _hurtTimer = new Timer(_hurtTime);

        _hurtTimer.OnTimerDone += () => ToggleHurt(false);

        _collider = GetComponent<Collider>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponent<Animator>();
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

        _anim.SetBool("Hurt", _isHurt);

        if (!_inBattle && !_isHurt)
        {
            _playerMovement.DoMovement();
        }

        if (_isImmune)
        {
            _immunityTimer.CountTimer();
        }

        if (_isHurt)
        {
            _hurtTimer.CountTimer();
        }
    }

    // Immunity and Hurt Logic
    private void ToggleImmunity(bool onOff)
    {
        _immunityTimer.ResetTimer();

        _collider.excludeLayers = onOff ? _excludeWhenImmune : LayerMask.GetMask();

        Color c = _spriteRenderer.color;
        c.a = onOff ? 0.5f : 1f;

        _spriteRenderer.color = c;

        _isImmune = onOff;
    }

    public void HurtPlayer(int damage, Transform damageSource)
    {
        PlayerCharacter.CurrentStance -= damage;

        ToggleHurt(true, damageSource);
    }
    private void ToggleHurt(bool onOff, Transform damageSource = null)
    {
        _hurtTimer.ResetTimer();

        _collider.excludeLayers = onOff ? _excludeWhenImmune : LayerMask.GetMask();

        if (damageSource != null)
        {
            Vector3 knockbackDir = (-_playerMovement.Direction).normalized;
            knockbackDir.y = 0;

            Debug.DrawRay(transform.position, knockbackDir, Color.red, 5f);
            Debug.DrawRay(transform.position, -_playerMovement.Direction, Color.blue, 5f);

            _playerMovement.SetVelocity(knockbackDir * _hurtForce);   
        }

        _isHurt = onOff;
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
        Paused = onOff;
    }
}
