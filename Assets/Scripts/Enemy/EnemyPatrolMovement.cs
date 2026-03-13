using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

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
        go.transform.parent = transform.parent;
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
    public void SetInPath()
    {
        if (_path == null) return;

        transform.position = _path.Waypoints[0].position;
    }

    [SerializeField] private bool _stopAtWaypoints = false;
    [SerializeField, ShowIf("_stopAtWaypoints")] private float _stopTime;

    [SerializeField] private float _speed = 1f;

    private NavMeshAgent _agent;
    private bool _doCheck = true;

    public Vector3 Direction => _agent.steeringTarget.normalized;
    public float Speed => _agent.speed;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.speed = _speed;
        _agent.stoppingDistance = 0.5f;

    }

    private void CheckIfReached()
    {
        if (Vector3.Distance(transform.position, _path.GetCurrentWaypoint()) <= 0.5f)
        {
            if (_stopAtWaypoints) StartCoroutine(UpdateDirectionCR());
            else UpdateDirection();
        }
    }
    private void UpdateDirection()
    {
        if (_agent.isOnNavMesh) _agent.destination = _path.GetNextWaypoint();
        
        Debug.Log($"Enemy {name} : Changed Destination to {_agent.destination}");
    }

    private IEnumerator UpdateDirectionCR()
    {
        _doCheck = false;

        Debug.Log($"Enemy {name} : Stopping at waypoint for {_stopTime} seconds.");

        yield return new WaitForSeconds(_stopTime);

        _doCheck = true;

        if (_agent.isOnNavMesh) _agent.destination = _path.GetNextWaypoint();

        Debug.Log($"Enemy {name} : Changed Destination to {_agent.destination}");
    }

    public void Move()
    {
        if (_doCheck) CheckIfReached();
    }

    public void ResetMovement()
    {
        if (_agent == null) return;

        if (_agent.isOnNavMesh) _agent.destination = transform.position;
        _path.Reset();
        SetInPath();
    }
}
