using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyPatrolMovement))]
[RequireComponent(typeof(EnemyFollowMovement))]
public class EnemyBrain : MonoBehaviour, IPausable
{
    private EnemyPatrolMovement _patrolMovement;
    private EnemyFollowMovement _followMovement;

    private IMovementType _activeMovement;
    private IMovementType ActiveMovement
    {
        get => _activeMovement;

        set
        {
            if (value != _activeMovement)
            {
                OnMovementChange?.Invoke();
                value.ResetMovement();
            }

            _activeMovement = value;
        }
    }

    public Vector3 Direction => ActiveMovement.Direction;
    public float Speed => ActiveMovement.Speed;

    public event Action OnMovementChange;

    private Transform _target;
    private Timer _memoryTimer;

    public enum MovementType { Patrolling, Following }

    [SerializeField, ReadOnly] private MovementType _movementeState = MovementType.Patrolling;

    private bool _setUp = false;
    public void SetPath(Path path)
    {
        _patrolMovement.SetPath(path);

        _movementeState = MovementType.Patrolling;

        gameObject.SetActive(true);

        UpdateMovement();
        _activeMovement.ResetMovement();

        _setUp = true;
    }

    [SerializeField] private bool _doFollow = true;
    [SerializeField, ShowIf("_doFollow")] private float _playerMemoryTimer = 2f;
    [SerializeField, ShowIf("_doFollow")] private float _detectionRadius = 2;

    public void Awake()
    {
        _patrolMovement = GetComponent<EnemyPatrolMovement>();
        _followMovement = GetComponent<EnemyFollowMovement>();

        _memoryTimer = new Timer(_playerMemoryTimer);
        _memoryTimer.OnTimerDone += CheckPlayer;

        _target = FindAnyObjectByType<PlayerMovement>().transform;
    }
    private void OnEnable()
    {
        PauseManager.Instance.RegisterPausable(this);
    }
    private void OnDisable()
    {
        PauseManager.Instance.UnregisterPausable(this);
    }

    private void UpdateMovement()
    {
        ActiveMovement = _movementeState == MovementType.Patrolling ? 
                                    _patrolMovement : _followMovement;
    }
    private void BrainLogic()
    {
        if (!_setUp) return;
        
        if (DetectPlayer())
        {
            _movementeState = MovementType.Following;
        }

        UpdateMovement();
    }

    private bool DetectPlayer() => 
    Vector3.Distance(_target.position, transform.position) <= _detectionRadius;
    private void CheckPlayer()
    {
        Debug.LogWarning($"{name}: Checking if Player still visible", this);
        if (!DetectPlayer())
        {
            _movementeState = MovementType.Patrolling;
        }

        UpdateMovement();
    }

    private void Update()
    {
        if (_doFollow && !Paused)
        {
            BrainLogic();

            if (_movementeState == MovementType.Following) _memoryTimer.CountTimer();
        }

        // Debug.Log($"{name} - Direction: {Direction} Speed: {Speed}", this);
    }
    private void LateUpdate()
    {
        if (!Paused) ActiveMovement.Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_doFollow)
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
       Paused = onOff;
       ActiveMovement.ResetMovement();
    }
}
