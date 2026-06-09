using NaughtyAttributes;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IPausable, ISaveable
{
    [SerializeField, Expandable] public PartyInfo _playerParty;
    [field: SerializeField] public PlayerParty PlayerParty { get; private set; }

    public Character PlayerCharacter => PlayerParty.Player;

    private Color _originalColor;

    public void ChangeEssence(int amount)
    {
        (PlayerCharacter as Player).ChangeEssence(amount);
        OnEssenceChange?.Invoke();
    }
    public int Essence => (PlayerCharacter as Player).Essence;

    // Immunity
    [SerializeField] private float _immunityTime = 1f;
    [SerializeField] private LayerMask _excludeWhenImmune;
    private Timer _immunityTimer;
    private bool _isImmune = false;
    public bool IsImmune => _isImmune;

    // Hurt
    [SerializeField] private float _hurtTime = 1f;
    [SerializeField] private float _hurtForce = 3f;
    [SerializeField] private Color _hurtColor;
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

    // Shop State
    public bool InShop { get; set; }

    // Movement and Dash
    private PlayerMovement _playerMovement;
    public bool CanDash => _playerMovement.CanDash;
    public float DashCooldownTime => _playerMovement.DashCooldownTime;

    // Player Components
    private Collider _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Animator _anim;
    private SpriteLightEffect _lightEffect;

    // Other Components
    private EncounterManager _encounterManager;

    // Player State Events
    public UnityEvent OnPlayerDamaged;
    public UnityEvent OnPlayerHealed;
    public UnityEvent OnEssenceChange;

    private void Awake()
    {
        PlayerParty = _playerParty.Instantiate() as PlayerParty;

        Debug.Log($"[Player Controller] Player Character : {PlayerCharacter.Name}", this);

        _playerMovement = GetComponent<PlayerMovement>();
        _encounterManager = FindAnyObjectByType<EncounterManager>();

        PlayerCharacter.OnStanceLost += () =>
        {
            if (!InBattle)
            {
                Debug.Log("[Player Controller] Player has fallen unconscious!", this);
                _encounterManager.DoRandomEncounter();   
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
        _anim = GetComponent<Animator>();
        _lightEffect = _spriteRenderer.GetComponent<SpriteLightEffect>();
    }

    private void Start()
    {
        _originalColor = _spriteRenderer.color;
        PauseManager.Instance?.RegisterPausable(this);
        SaveManager.Instance.RegsiterSaveable("PlayerInfo", this);
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
        if (_lightEffect != null) _lightEffect.enabled = !onOff;

        _isImmune = onOff;
    }

    public void HurtPlayer(int damage, Transform damageSource)
    {
        PlayerCharacter.CurrentStance -= damage;

        ToggleHurt(true, damageSource);

        CameraEffectsUtility.DoCameraShake(0.5f, 1.5f);
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

        _spriteRenderer.color = onOff ? _hurtColor : _originalColor;
        if (_lightEffect != null) _lightEffect.enabled = !onOff;

        _isHurt = onOff;
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
        Paused = onOff;

        if (Paused)
        {
            _playerMovement.SetVelocity(Vector3.zero);
        }
    }

    public object GetSaveData()
    {
        SaveData saveData;

        saveData.PlayerStanceLevel = PlayerCharacter.StanceLevel;
        saveData.PlayerSpeedLevel = PlayerCharacter.SpeedLevel;
        saveData.PlayerCurrentStance = PlayerCharacter.CurrentStance;
        saveData.PlayerEssence = (PlayerCharacter as Player).Essence;

        return saveData;
    }

    public void LoadSaveData(object data)
    {
        SaveData saveData = (SaveData)data;

        PlayerCharacter.LevelUpStat(Stats.Stance, saveData.PlayerStanceLevel);
        PlayerCharacter.LevelUpStat(Stats.Speed, saveData.PlayerSpeedLevel);
        PlayerCharacter.CurrentStance = saveData.PlayerCurrentStance;
        (PlayerCharacter as Player).Essence = saveData.PlayerEssence;
    }

    [Serializable]
    private struct SaveData
    {
        public int PlayerCurrentStance;
        public int PlayerStanceLevel;
        public int PlayerSpeedLevel;
        public int PlayerEssence;
    }
}
