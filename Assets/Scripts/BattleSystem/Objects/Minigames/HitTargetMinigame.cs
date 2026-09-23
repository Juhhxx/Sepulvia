using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitTargetMinigame : MonoBehaviour, IMoveMinigame
{
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private float[] _movingSpeedPerRound = new float[1] { 1 };
    [SerializeField] private bool _intermitentMovement;

    [SerializeField, MinMaxSlider(0,1)] private Vector2 _successRange;
    public Vector2 SuccessRange => _successRange;

    [SerializeField, ReadOnly] private float _currentPoint;
    public float CurrentPoint => _currentPoint;

    public event Action<int> OnMinigameStart;
    public event Action<int, bool> OnMinigameRoundEnd;
    public event Action<float> OnMinigameEnd;
    public void StartMinigame(int rounds)
    {
        OnMinigameStart?.Invoke(rounds);

        StopAllCoroutines();
        StartCoroutine(MinigameCR(rounds));
    }
    public void MakeEasier()
    {

    }

    [Button]
    private void Test()
    {
        StartMinigame(3);
    }

    private IEnumerator MinigameCR(int rounds)
    {
        int points = 0;

        for (int round = 0; round < rounds; round++)
        {
            yield return new WaitForSeconds(1);
            
            if (_intermitentMovement)
            {
                yield return MinigameBackAndForth(round);
            }
            else
            {
                yield return MinigameOneShot(round);
            }

            bool result = _currentPoint >= _successRange.x && _currentPoint <= _successRange.y;

            OnMinigameRoundEnd?.Invoke(round, result);

            if (result)
            {
                points++;
                Debug.Log($"WON ROUND  {round}", this);
            }
            else
            {
                Debug.Log($"LOST ROUND  {round}", this);
            }

            yield return new WaitForSeconds(0.1f);

        }

        Debug.Log($"SUCCESS RATE : {points/(float)rounds}", this);

        yield return new WaitForSeconds(1);

        OnMinigameEnd?.Invoke(points / (float)rounds);
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

            if (Input.GetKeyDown(KeyCode.Space)) yield break;

            yield return null;
        }
    }

    private void MovePoint(float time)
    {
        _currentPoint = _movementCurve.Evaluate(time);
        Debug.Log($"TIME: {time} POINT: {_currentPoint}", this);
    }

}
