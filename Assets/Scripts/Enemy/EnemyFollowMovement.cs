using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowMovement : MonoBehaviour, IMovementType
{
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _stopingDistance = 0.5f;

    private Rigidbody _rb;
    private Transform _target;
    private Vector3 _direction = Vector3.zero;
    private Vector3 _motion = Vector3.zero;

    public Vector3 Direction => _direction;
    public float Speed => _rb.linearVelocity.magnitude;

    private void Start()
    {
        _target = FindAnyObjectByType<PlayerMovement>().transform;
        _rb = GetComponent<Rigidbody>();
    }

    private void UpdateMovement()
    {
        _direction = _target.position - transform.position;

        _direction.y = transform.position.y;
        _direction = Vector3.Normalize(_direction);

        _motion = _direction * _followSpeed;
    }

    public void Move()
    {
        UpdateMovement();

        if (!Mathf.Approximately(Vector3.Distance(transform.position, _target.position), _stopingDistance))
        {
            _rb.linearVelocity = _motion;
        }
    }

    public void ResetMovement()
    {
        if (_rb != null) _rb.linearVelocity = Vector3.zero;
    }
}
