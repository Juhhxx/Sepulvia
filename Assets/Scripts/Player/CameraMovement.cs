using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed = 0.05f;

    private void Update()
    {
        Vector3 pos = _target.position;

        transform.position = transform.position + (pos - transform.position) * _followSpeed;
    }
}
