using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyPatrolMovement : MonoBehaviour
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

    private void SetInPath()
    {
        if (_path == null) return;

        transform.position = _path.GetCurrentWaypoint();
    }

    private void Start()
    {
    }
}
