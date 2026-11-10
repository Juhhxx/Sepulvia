using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class HitTargetMinigame : MonoBehaviour, IMoveMinigame
{
    [Header("Minigame Settings")]
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float[] _movingSpeedPerRound = new float[1] { 1 };
    [SerializeField] private bool _intermitentMovement;

    [HideIf(nameof(_intermitentMovement))]
    [SerializeField] private float _additionalTime = 0.3f;


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
            return _graceRange;
        }
    }

    // Make easy
    [Header("Easy Mode Settings")]
    [SerializeField] private float _easySpeedReduction = 0.5f;

    [Header("Current Position of Minigame")]
    [SerializeField, ReadOnly] private float[] _currentPoint;
    public float[] CurrentPoint => _currentPoint;

    [SerializeField] private bool _waitForRounds = true;

    [HideIf(nameof(_waitForRounds))]
    [Header("Delay Between Rounds")]
    [SerializeField] private float _roundDelay = 1f;

    // Wait times
    [Header("Wait Times")]
    [SerializeField] private float _waitBeforeRound = 1f;
    [SerializeField] private float _waitAfterRound = 0.1f;
    [SerializeField] private float _waitBeforeMinigame = 1f;
    [SerializeField] private float _waitAfterMinigame = 1f;

    public event Action<int> OnMinigameStart; // On minigame start, theres a delay for the rounds to begin so animations can play
    public event Action<int> OnMinigameRoundStart; // when a round starts
    public event Action<int, float> OnMinigameRoundEnd; // when a round ends
    public event Action OnMinigameEndRounds; // Used to play animations after all rounds are finished, but before the minigame ends
    public event Action<float> OnMinigameEnd;

    public void StartMinigame(int rounds)
    {
        _graceRange = new Vector2[rounds];
        _currentPoint = new float[rounds];

        SetRanges(rounds);

        OnMinigameStart?.Invoke(rounds);

        StopAllCoroutines();
        
        if (_waitForRounds)
        {
            StartCoroutine(MinigameCR(rounds));
        }
        else
        {
            StartCoroutine(MinigameDelayCR(rounds));
        }
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

    [Header("Testing Settings")]
    [SerializeField] private int _testRounds = 3;
    [Button]
    private void Test()
    {
        StartMinigame(_testRounds);
    }

    private bool[] _hitKey;
    private bool[] _finishedRound;
    private bool GetFinishedLastRound(int round)
    {
        if (round == 0) return true;
        else return _finishedRound[round - 1];
    }

    private IEnumerator MinigameCR(int rounds)
    {
        float points = 0;
        _hitKey = new bool[rounds];
        _finishedRound = new bool[rounds];

        yield return new WaitForSeconds(_waitBeforeMinigame); // delay before starting minigame

        for (int round = 0; round < rounds; round++)
        {
            yield return MinigameRoundCR(round, (result) => { points += result; });
        }

        Debug.Log($"SUCCESS RATE : {points / rounds}", this);

        OnMinigameEndRounds?.Invoke(); // Used to play animations after all rounds are finished, but before the minigame ends

        yield return new WaitForSeconds(_waitAfterMinigame); // delay after ending minigame

        OnMinigameEnd?.Invoke(points / rounds);
    }

    private Coroutine[] _roundsCR;
    private IEnumerator MinigameDelayCR(int rounds)
    {
        float points = 0;
        _hitKey = new bool[rounds];
        _finishedRound = new bool[rounds];
        _roundsCR = new Coroutine[rounds];

        yield return new WaitForSeconds(_waitBeforeMinigame); // delay before starting minigame

        for (int round = 0; round < rounds; round++)
        {
            int roundIdx = round;

            _roundsCR[round] = StartCoroutine(MinigameRoundCR(round, (result) =>
            {
                points += result;
                _roundsCR[roundIdx] = null;
            }));

            yield return new WaitForSeconds(_roundDelay);
        }

        yield return new WaitUntil(() => _roundsCR.All(r => r == null));

        Debug.Log($"SUCCESS RATE : {points / rounds}", this);

        OnMinigameEndRounds?.Invoke(); // Used to play animations after all rounds are finished, but before the minigame ends

        yield return new WaitForSeconds(_waitAfterMinigame); // delay after ending minigame

        OnMinigameEnd?.Invoke(points / rounds);
    }

    private IEnumerator MinigameRoundCR(int round, Action<float> onRoundEnd)
    {
        OnMinigameRoundStart?.Invoke(round);

        yield return new WaitForSeconds(_waitBeforeRound); // delay before starting round

        _hitKey[round] = false;
        _finishedRound[round] = false;
        
        if (_intermitentMovement)
        {
            yield return MinigameBackAndForth(round);
        }
        else
        {
            yield return MinigameOneShot(round);
        }

        float result = CheckIfInRange(round);

        if (!_hitKey[round]) result = 0;

        onRoundEnd?.Invoke(result);
        OnMinigameRoundEnd?.Invoke(round, result);

        if (result > 0)
        {
            Debug.Log($"WON ROUND  {round}", this);
        }
        else
        {
            Debug.Log($"LOST ROUND  {round}", this);
        }

        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        
        _finishedRound[round] = true;

        yield return new WaitForSeconds(_waitAfterRound); // delay after ending round
    }

    private IEnumerator MinigameBackAndForth(int round)
    {
        float time = 0;

        while (!Input.GetMouseButtonDown(0) || !GetFinishedLastRound(round))
        {
            int speedIdx = round;

            if (round >= _movingSpeedPerRound.Length) speedIdx = _movingSpeedPerRound.Length - 1;
            
            time += _movingSpeedPerRound[speedIdx] * Time.deltaTime;

            float t = Mathf.PingPong(time, 1);

            MovePoint(t, round);

            yield return null;
        }

        _hitKey[round] = true;
    }

    private IEnumerator MinigameOneShot(int round)
    {
        float time = 0;

        while (time < 1 + _additionalTime)
        {
            int speedIdx = round;

            if (round >= _movingSpeedPerRound.Length) speedIdx = _movingSpeedPerRound.Length - 1;
            
            time += _movingSpeedPerRound[speedIdx] * Time.deltaTime;;

            MovePoint(time, round);

            if (Input.GetMouseButtonDown(0) && GetFinishedLastRound(round))
            {
                _hitKey[round] = true;
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
