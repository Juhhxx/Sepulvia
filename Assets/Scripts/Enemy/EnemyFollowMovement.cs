using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowMovement : MonoBehaviour, IMovementType
{
    [SerializeField] private float _followSpeed;

    private NavMeshAgent _agent;
    private Transform _target;

    public Vector3 Direction => Vector3.zero;
    public float Speed => _agent.speed;

    private void Start()
    {
        _target = FindAnyObjectByType<PlayerMovement>().transform;

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.speed = _followSpeed;
        _agent.stoppingDistance = 0.5f;
    }

    public void ResetMovement()
    {
        _agent.destination = transform.position;
    }

    public void Move()
    {
        _agent.destination = _target.position;
    }
}
