using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyPatrolMovement : MonoBehaviour, IMovementType
{
    [OnValueChanged("SetInPath")]
    [SerializeField] private Path _path;

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void CreatePath()
    {
        if (_path != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(_path.gameObject);
#else
            Destroy(_path.gameObject);
#endif
            _path = null;
        }

        GameObject go = new GameObject($"{name}_Path");

        go.transform.position = transform.position;
        go.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

        Path path = go.AddComponent<Path>();

        Vector3 pos = transform.position;

        // Create Default Square Path
        path.CreateWaypoint(0, pos);
        path.CreateWaypoint(1, pos += (Vector3.right * 2.5f));
        path.CreateWaypoint(2, pos += (Vector3.forward * 2.5f));
        path.CreateWaypoint(3, pos += (Vector3.left * 2.5f));

        _path = path;
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void SetInPath()
    {
        if (_path == null) return;

        transform.position = _path.GetCurrentWaypoint();
    }

    [SerializeField] private bool _stopAtWaypoints = false;
    [SerializeField, ShowIf("_stopAtWaypoints")] private float _stopTime;

    [SerializeField] private float _speed = 1f;

    private Vector3 _direction = Vector3.zero;
    private Vector3 _motion = Vector3.zero;
    private Rigidbody _rb;
    private bool _doCheck = true;

    public Vector3 Direction => _direction;
    public float Speed => _rb.linearVelocity.magnitude;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        SetInPath();
    }

    private void Update()
    {
        if (_doCheck) CheckIfReached();
        UpdateMovement();
    }

    private void CheckIfReached()
    {
        if (Vector3.Distance(transform.position, _path.GetCurrentWaypoint()) <= 0.15f)
        {
            if (_stopAtWaypoints) StartCoroutine(UpdateDirectionCR());
            else UpdateDirection();

            _motion = Vector3.zero;
        }
    }
    private void UpdateDirection()
    {
        Vector3 dir = _path.GetNextWaypoint() - transform.position;
        
        dir = dir.normalized;

        _direction = dir;

        Debug.Log($"Enemy {name} : Changed Direction to {_direction}");
    }

    private IEnumerator UpdateDirectionCR()
    {
        _doCheck = false;

        _direction = Vector3.zero;

        Debug.Log($"Enemy {name} : Stopping at waypoint for {_stopTime} seconds.");

        yield return new WaitForSeconds(_stopTime);

        _doCheck = true;

        Vector3 dir = _path.GetNextWaypoint() - transform.position;
        
        dir = dir.normalized;

        _direction = dir;

        Debug.Log($"Enemy {name} : Changed Direction to {_direction}");
    }

    private void UpdateMovement()
    {
        _motion = _direction * _speed;
    }
    public void Move()
    {
        _rb.linearVelocity = _motion;
    }

    public void ResetMovement()
    {
        _rb.linearVelocity = Vector3.zero;
    }
}
