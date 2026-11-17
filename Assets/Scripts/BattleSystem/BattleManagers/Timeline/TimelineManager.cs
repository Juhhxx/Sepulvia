using System;
using NaughtyAttributes;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    [SerializeField, Range(0.5f,10)] private float _turnDuration = 1f; // Duration of a turn in seconds
    private Timer _timer;

    public Action OnTurnBegin;
    public Action OnTurnEnd;

    private bool _countTurn = false;
    [SerializeField, ReadOnly] private int _currentTurn = 0;
    public int CurrentTurn => _currentTurn;
    public void SetTurn(int turn)
    {
        _currentTurn = turn;
        _timer.ResetTimer();
        Debug.Log($"RESET TIMELINE TIMER: {_timer.CurrentTime} {_timer.Done}", this);
    }

    public void ToggleTurnCounting(bool onOff)
    {
        Debug.Log($"CHANGE TIMELINE COUNTING: {_countTurn}", this);
        _countTurn = onOff;
    }

    private void Start()
    {
        _timer = new Timer(_turnDuration);

        _timer.OnTimerBegin += () => {
            OnTurnBegin?.Invoke();
            Debug.Log("TURN BEGAN", this);
        };

        _timer.OnTimerDone += () => {
            _currentTurn++;
            OnTurnEnd?.Invoke();
            Debug.Log("TURN ENDED", this);
        };
    }

    private void Update()
    {
        // Debug.Log($"TIMELINE COUNTING: {_countTurn}", this);

        if (_countTurn)
        {
            Debug.Log($"TIMELINE TIMER: {_timer.CurrentTime} {_timer.Done}", this);
            _timer.CountTimer();
        }
    }
}
