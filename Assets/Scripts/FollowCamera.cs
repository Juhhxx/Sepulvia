using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private bool _followX;
    [SerializeField] private bool _followY;
    [SerializeField] private bool _followZ;

    private Transform _cameraTrans;

    private void Start()
    {
        _cameraTrans = GameSceneManager.Instance.CurrentCamera.transform;
    }

    private void Follow()
    {
        Vector3 dir = _cameraTrans.position - transform.position;

        if (!_followX) dir.x = 0;
        if (!_followY) dir.y = 0;
        if (!_followZ) dir.z = 0;

        dir = Vector3.Normalize(dir);

        dir = transform.position + dir;

        transform.LookAt(dir);
    }

    private void LateUpdate()
    {
        Follow();
    }
}
