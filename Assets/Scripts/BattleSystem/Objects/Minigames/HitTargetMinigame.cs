using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitTargetMinigame : MonoBehaviour, IMoveMinigame
{
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float[] _movingSpeedPerRound = new float[1] { 1 };
    [SerializeField] private bool _intermitentMovement;

    [SerializeField, MinMaxSlider(0,1)] private Vector2 _successRange;
    [SerializeField, MinMaxSlider(0,1)] private Vector2 _easierSuccessRange;
    public Vector2 SuccessRange => _successRange;

    [SerializeField] private float _graceRadius;
    [SerializeField] private float _easierGraceRadius;
    private Vector2 _graceRange;
    public Vector2 GraceRange
    {
        get
        {
            Debug.Log($"{_graceRange}");
            return _graceRange;
        }
    }

    [SerializeField, ReadOnly] private float _currentPoint;
    public float CurrentPoint => _currentPoint;

    private void Awake()
    {
        _graceRange = new Vector2(_successRange.x - _graceRadius,
                                            _successRange.y + _graceRadius);
    }

    public event Action<int> OnMinigameStart;
    public event Action<int, float> OnMinigameRoundEnd;
    public event Action<float> OnMinigameEnd;
    public void StartMinigame(int rounds)
    {
        OnMinigameStart?.Invoke(rounds);

        StopAllCoroutines();
        StartCoroutine(MinigameCR(rounds));
    }

    public event Action OnMakeEasier;
    [Button]
    public void MakeEasier()
    {
        _graceRange = new Vector2(_easierSuccessRange.x - _easierGraceRadius,
                                            _easierSuccessRange.y + _easierGraceRadius);

        _successRange = _easierSuccessRange;

        OnMakeEasier?.Invoke();
    }

    [Button]
    private void Test()
    {
        StartMinigame(3);
    }

    private bool _hitKey = false;

    private IEnumerator MinigameCR(int rounds)
    {
        float points = 0;

        for (int round = 0; round < rounds; round++)
        {
            yield return new WaitForSeconds(1);

            _hitKey = false;
            
            if (_intermitentMovement)
            {
                yield return MinigameBackAndForth(round);
            }
            else
            {
                yield return MinigameOneShot(round);
            }

            float result = CheckIfInRange();

            if (!_hitKey) result = 0;

            OnMinigameRoundEnd?.Invoke(round, result);

            if (result > 0)
            {
                points += result;
                Debug.Log($"WON ROUND  {round}", this);
            }
            else
            {
                Debug.Log($"LOST ROUND  {round}", this);
            }

            yield return new WaitForSeconds(0.1f);

        }

        Debug.Log($"SUCCESS RATE : {points / rounds}", this);

        yield return new WaitForSeconds(1);

        OnMinigameEnd?.Invoke(points / rounds);
    }

    private IEnumerator MinigameBackAndForth(int round)
    {
        float time = 0;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            int speedIdx = round;

            if (round >= _movingSpeedPerRound.Length) speedIdx = _movingSpeedPerRound.Length - 1;
            
            time += _movingSpeedPerRound[speedIdx] * Time.deltaTime;

            float t = Mathf.Abs(Mathf.Sin(time));

            MovePoint(t);

            yield return null;
        }

        _hitKey = true;
    }

    private IEnumerator MinigameOneShot(int round)
    {
        float time = 0;

        while (time < 1)
        {
            int speedIdx = round;

            if (round >= _movingSpeedPerRound.Length) speedIdx = _movingSpeedPerRound.Length - 1;
            
            time += _movingSpeedPerRound[speedIdx] * Time.deltaTime;;

            MovePoint(time);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _hitKey = true;
                yield break;
            }

            yield return null;
        }
    }

    private float CheckIfInRange()
    {
        if (SuccessRange.x <= _currentPoint && _currentPoint <= SuccessRange.y)
        {
            return 1; // full point
        }
        else if (GraceRange.x <= _currentPoint && _currentPoint <= SuccessRange.y)
        {
            return 0.5f; // half point
        }
        else return 0; // zero points
    }

    private void MovePoint(float time)
    {
        _currentPoint = _movementCurve.Evaluate(time);
    }

}
