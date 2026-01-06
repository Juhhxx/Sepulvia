using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;

    [SerializeField, InputAxis] private string _horizontalAxis;
    [SerializeField, InputAxis] private string _verticalAxis;

    [SerializeField] private float _dashCooldownTime = 3f;
    [SerializeField] private float _dashTime = 1f;
    [SerializeField] private float _dashSpeedMultiplier = 3f;
    [SerializeField, InputAxis] private string _dashButton;
    [SerializeField] private LayerMask _excludeWhenDashing;

    private Rigidbody _rb;
    private Vector3 _velocity;
    private Vector3 _motion;

    [SerializeField, ReadOnly] private bool _canDash = true;
    [SerializeField, ReadOnly] private bool _isDashing = false;
    private Timer _dashCooldownTimer;

    private PlayerController _playerController;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();

        _dashCooldownTimer = new Timer(2f);
        _dashCooldownTimer.OnTimerDone += () => { _canDash = true; };
    }

    private void Update()
    {
        if (_playerController.InBattle) return;
        
        CheckDash();

        if (!_canDash)
        {
            _dashCooldownTimer.CountTimer();
        }
    }
    private void FixedUpdate()
    {
        if (_playerController.InBattle) return;

        UpdateVelocity();
        if (!_isDashing) UpdatePosition();
    }

    private void UpdateVelocity()
    {
        float forwardAxis = Input.GetAxis(_verticalAxis);
        float strafeAxis = Input.GetAxis(_horizontalAxis);

        _velocity.z = forwardAxis;
        _velocity.x = strafeAxis;

        _velocity = Vector3.Normalize(_velocity);
    }
    private void UpdatePosition()
    {
        _motion = transform.TransformVector(_velocity * _maxSpeed);

        _rb.linearVelocity = _motion;
    }

    private void CheckDash()
    {
        if (Input.GetButtonDown(_dashButton) && _canDash)
        {
            StartCoroutine(DashCR());
        }
    }

    private IEnumerator DashCR()
    {
        _isDashing = true;
        _canDash = false;

        _dashCooldownTimer.ResetTimer();

        Vector3 dashVelocity = _motion * _dashSpeedMultiplier;

        _rb.linearVelocity = dashVelocity;

        _rb.excludeLayers = _excludeWhenDashing;

        yield return new WaitForSeconds(_dashTime);

        _rb.excludeLayers = LayerMask.GetMask();

        _isDashing = false;
    }
}