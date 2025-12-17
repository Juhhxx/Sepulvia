using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField, InputAxis] private string _horizontalAxis;
    [SerializeField, InputAxis] private string _verticalAxis;
    private Rigidbody _rb;
    private Vector3 _velocity;
    private Vector3 _motion;
    private Transform _head;
    public Transform Head { get => _head; }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        UpdateVelocity();
        UpdatePosition();
    }
    private void UpdateVelocity()
    {
        float forwardAxis = Input.GetAxis(_verticalAxis);
        float strafeAxis = Input.GetAxis(_horizontalAxis);

        _velocity.z = forwardAxis;
        _velocity.x = strafeAxis;

        _velocity = Vector3.Normalize(_velocity) * _maxSpeed;
    }
    private void UpdatePosition()
    {
        _motion = transform.TransformVector(_velocity);

        _rb.linearVelocity = _motion;
    }
}