using UnityEngine;
using NaughtyAttributes;

public class PillarMovement : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 _shiftingRange = new Vector2(2f, 5f);
    [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 _shiftingtVelocity = new Vector2(2f, 5f);

    [SerializeField] private bool _isMoving = true;
    private Vector3 _startingPosition;
    private float _amplitude;
    private float _speed;
    private float _phaseOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float t;
    void Start()
    {
        _startingPosition = transform.position;
        _speed = Random.Range(_shiftingtVelocity.x, _shiftingtVelocity.y);
        _amplitude = Random.Range(_shiftingRange.x, _shiftingRange.y);

        _phaseOffset = Random.Range(0f, Mathf.PI * 2f);

    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving)
        {
            float yOffset = Mathf.Sin(Time.time * _speed + _phaseOffset) * _amplitude;

            transform.position = _startingPosition + Vector3.up * yOffset;
        }
        
    }
}
