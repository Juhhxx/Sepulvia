using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class HitTargetMinigame : MonoBehaviour, IMoveMinigame
{
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float[] _movingSpeedPerRound = new float[1] { 1 };
    [SerializeField] private bool _intermitentMovement;

    [SerializeField] private bool _doRandomHitRange = false;

    // For random hit point
    [ShowIf(nameof(_doRandomHitRange))]
    [SerializeField, MinMaxSlider(0,1)] private Vector2 _successHitRange;

    [ShowIf(nameof(_doRandomHitRange))]
    [SerializeField] private float _successRadius;

    // For predefined hitpoint
    [HideIf(nameof(_doRandomHitRange))]
    [SerializeField, MinMaxSlider(0,1)] private Vector2[] _successRange;
    
    public Vector2[] SuccessRange => _successRange;

    // Grace
    [SerializeField] private float _graceRadius;
    private Vector2[] _graceRange;
    public Vector2[] GraceRange
    {
        get
        {
            Debug.Log($"{_graceRange}");
            return _graceRange;
        }
    }

    // Make easy
    [SerializeField] private float _easySpeedReduction = 0.5f;

    [SerializeField, ReadOnly] private float[] _currentPoint;
    public float[] CurrentPoint => _currentPoint;

    [SerializeField] private bool _waitForRounds = true;

    [HideIf(nameof(_waitForRounds))]
    [SerializeField] private float _roundDelay = 1f;

    public event Action<int> OnMinigameStart;
    public event Action<int, float> OnMinigameRoundEnd;
    public event Action<float> OnMinigameEnd;
    public void StartMinigame(int rounds)
    {
        _graceRange = new Vector2[rounds];
        _currentPoint = new float[rounds];

        SetRanges(rounds);

        OnMinigameStart?.Invoke(rounds);

        StopAllCoroutines();
        StartCoroutine(MinigameCR(rounds));
    }

    private void SetRanges(int rounds)
    {
        SetSuccessRange(rounds);

        for (int i = 0; i < rounds; i++)
        {
            _graceRange[i] = new Vector2(_successRange[i].x - _graceRadius, 
                                        _successRange[i].y + _graceRadius);

            Debug.Log($"ROUND {i}: SUCCESS {_successRange[i]} GRACE {_graceRange[i]}");
        }
    }

    private void SetSuccessRange(int rounds)
    {
        if (_doRandomHitRange)
        {
            _successRange = new Vector2[rounds];

            for (int i = 0; i < rounds; i++)
            {
                var point = UnityEngine.Random.Range(_successHitRange.x, _successHitRange.y);

                _successRange[i] = new Vector2(point - _successRadius, point + _successRadius);
            }
        }
        else
        {
            if (_successRange.Length < rounds)
            {
                var tmp = _successRange[0];

                _successRange = new Vector2[rounds];

                for (int i = 0; i < rounds; i++)
                {
                    _successRange[i] = tmp;
                }
            }
        }
    }

    public event Action OnMakeEasier;
    [Button]
    public void MakeEasier()
    {
        for (int i = 0; i < _movingSpeedPerRound.Length; i++)
        {
            _movingSpeedPerRound[i] *= _easySpeedReduction;
        }

        OnMakeEasier?.Invoke();
    }

    [SerializeField] private int _testRounds = 3;
    [Button]
    private void Test()
    {
        StartMinigame(_testRounds);
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

            float result = CheckIfInRange(round);

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

        while (!Input.GetMouseButtonDown(0))
        {
            int speedIdx = round;

            if (round >= _movingSpeedPerRound.Length) speedIdx = _movingSpeedPerRound.Length - 1;
            
            time += _movingSpeedPerRound[speedIdx] * Time.deltaTime;

            float t = Mathf.Abs(Mathf.Sin(time));

            MovePoint(t, round);

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

            MovePoint(time, round);

            if (Input.GetMouseButtonDown(0))
            {
                _hitKey = true;
                yield break;
            }

            yield return null;
        }
    }

    private float CheckIfInRange(int round)
    {
        if (SuccessRange[round].x <= _currentPoint[round] && _currentPoint[round] <= SuccessRange[round].y)
        {
            return 1; // full point
        }
        else if (GraceRange[round].x <= _currentPoint[round] && _currentPoint[round] <= GraceRange[round].y)
        {
            return 0.5f; // half point
        }
        else return 0; // zero points
    }

    private void MovePoint(float time, int round)
    {
        _currentPoint[round] = _movementCurve.Evaluate(time);
    }

}
