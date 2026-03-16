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
    private Animator _anim;
    private SpriteRenderer _spr;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _spr = GetComponentInChildren<SpriteRenderer>();

        _dashCooldownTimer = new Timer(2f);
        _dashCooldownTimer.OnTimerDone += () => { _canDash = true; };
    }

    public void DoMovement()
    {
        CheckDash();

        if (!_canDash)
        {
            _dashCooldownTimer.CountTimer();
        }
        
        UpdateVelocity();
        if (!_isDashing) UpdatePosition();

        _anim.SetFloat("XSpeed", Mathf.Min(Mathf.Abs(_velocity.x), 1));
        _anim.SetFloat("YSpeed", Mathf.Clamp(_velocity.z, -1, 1));
        _anim.SetBool("IsDashing", _isDashing);

        if (_velocity.magnitude > 0.5)
        {
            _anim.SetFloat("XDir", Mathf.Min(Mathf.Abs(_velocity.x), 1));
            _anim.SetFloat("YDir", Mathf.Clamp(_velocity.z, -1, 1));
            _spr.flipX = _velocity.x > 0;
        }
    }

    public void SetVelocity(Vector3 velocity) => _rb.linearVelocity = velocity;
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

        // _velocity = dashVelocity;
        _rb.linearVelocity = dashVelocity;

        _rb.excludeLayers = _excludeWhenDashing;

        yield return new WaitForSeconds(_dashTime);

        _rb.excludeLayers = LayerMask.GetMask();

        _isDashing = false;
    }
}