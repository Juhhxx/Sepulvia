using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Path : MonoBehaviour
{
    public enum PathType { Loop, Reverse }
    [SerializeField] private PathType _pathType = PathType.Loop;

    [SerializeField] private bool _showPath = true;

    [SerializeField, ReorderableList, ReadOnly] private List<Transform> _waypoints = new List<Transform>();

    public int Size => _waypoints.Count;

    private int _direction = 1;
    public void SwitchDirection() => _direction *= -1; 

    private int _index = 0;

    public Vector3 GetCurrentWaypoint() => _waypoints[_index].position;

    public Vector3 GetNextWaypoint()
    {
        if (Size == 0) return transform.position;

        GoToNextIndex();

        return GetCurrentWaypoint();
    }

    private void GoToNextIndex()
    {
        _index += _direction;

        switch (_pathType)
        {
            case PathType.Loop:

                _index %= Size;
                break;
            
            case PathType.Reverse:

                if (_index < 0 || _index >= Size)
                {
                    SwitchDirection();
                    _index += _direction * 2;
                }
                break;
        }
    }

    // Waypoint Creation
    public void CreateWaypoint(int id, Vector3 pos)
    {
        GameObject waypoint = new GameObject($"Waypoint_{id}");

        waypoint.transform.parent = gameObject.transform;

        waypoint.transform.position = pos;

        _waypoints.Add(waypoint.transform);
    }

    public void DestroyWaypoints()
    {
        if (Size == 0) return;

        foreach (Transform w in _waypoints)
        {
#if UNITY_EDITOR
            DestroyImmediate(w.gameObject);
#else
            Destroy(w.gameObject);
#endif
        }

        _waypoints.Clear();
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    private void AddWaypoint()
    {
        if (Size == 0)
        {
            CreateWaypoint(0, Vector3.zero);
            return;
        }
        else if (Size == 1)
        {
            Vector3 dir = Vector3.right;

            CreateWaypoint(1, _waypoints.Last().position + (dir * 2.5f));

            return;                                   
        }
        else
        {
            Vector3 dir =   _waypoints[Size - 1].position -
                            _waypoints[Size - 2].position;

            dir = Vector3.Normalize(dir);

            Debug.Log(dir);

            CreateWaypoint(Size, _waypoints.Last().position + (dir * 2.5f));

            return; 
        }
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    public void RemoveWaypoint()
    {
        Transform waypoint = _waypoints.Last();

        _waypoints.Remove(waypoint);
        
#if UNITY_EDITOR
        DestroyImmediate(waypoint.gameObject);
#else
        Destroy(waypoint.gameObject);
#endif
    }


    // Waypoint Draw
    private void OnDrawGizmos()
    {
        if (_showPath)
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_waypoints[i].position, 0.05f);

                if (i != _waypoints.Count - 1)
                {
                    Gizmos.DrawLine(_waypoints[i].position,
                                    _waypoints[i + 1].position);

                    DrawArrowGizmo(_waypoints[i].position,
                                    _waypoints[i + 1].position);
                }
            }

            if (_pathType == PathType.Loop && _waypoints.Count > 1)
            {
                Gizmos.DrawLine(_waypoints.Last().position,
                                _waypoints[0].position);

                DrawArrowGizmo(_waypoints.Last().position,
                                _waypoints[0].position);
            }
        }
    }

    private void DrawArrowGizmo(Vector3 from, Vector3 to, float arrowHeadLength = 0.35f, float arrowHeadAngle = 25f)
    {
        // Main direction
        Vector3 dir = to - from;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        // Midpoint of the line
        Vector3 mid = Vector3.Lerp(from, to, 0.5f);

        // Arrow head directions
        Quaternion rot = Quaternion.LookRotation(dir);

        Vector3 right = rot * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left  = rot * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        // Draw arrow head
        Gizmos.DrawLine(mid, mid + right * arrowHeadLength);
        Gizmos.DrawLine(mid, mid + left * arrowHeadLength);
    }

}
