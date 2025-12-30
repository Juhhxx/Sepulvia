using System;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(EnemyPatrolMovement))]
[RequireComponent(typeof(EnemyFollowMovement))]
public class EnemyBrain : MonoBehaviour
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
                _activeMovement?.ResetMovement();
                OnMovementChange?.Invoke();
            }

            _activeMovement = value;
        }
    }

    public Vector3 Direction => ActiveMovement.Direction;
    public float Speed => ActiveMovement.Speed;

    public event Action OnMovementChange;

    private Transform _target;

    public enum MovementType { Patrolling, Following }

    private MovementType _movementeState = MovementType.Patrolling;

    [SerializeField] private bool _doFollow = true;
    [SerializeField, ShowIf("_doFollow")] private float _detectionRadius = 2;

    public void Start()
    {
        _patrolMovement = GetComponent<EnemyPatrolMovement>();
        _followMovement = GetComponent<EnemyFollowMovement>();

        _target = FindAnyObjectByType<PlayerMovement>().transform;

        UpdateMovement();
    }

    private bool DetectPlayer() => 
    Vector3.Distance(_target.position, transform.position) <= _detectionRadius;

    private void UpdateMovement()
    {
        ActiveMovement = _movementeState == MovementType.Patrolling ? 
                                    _patrolMovement : _followMovement;
    }

    private void BrainLogic()
    {
        if (DetectPlayer())
        {
            _movementeState = MovementType.Following;
        }
        UpdateMovement();
    }

    private void Update()
    {
        if (_doFollow) BrainLogic();
    }

    private void LateUpdate()
    {
        ActiveMovement.Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_doFollow)
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
