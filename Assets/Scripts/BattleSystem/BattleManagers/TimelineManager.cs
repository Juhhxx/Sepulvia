using System;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private float _turnDuration = 1f; // Duration of a turn in seconds
    private Timer _timer;

    public Action OnTurnBegin;
    public Action OnTurnEnd;

    private bool _countTurn = false;

    public void ToggleTurnCounting(bool onOff)
    {
        _countTurn = onOff;
    }

    private void Start()
    {
        _timer = new Timer(_turnDuration);

        _timer.OnTimerBegin += () => OnTurnBegin?.Invoke();
        _timer.OnTimerDone += () => OnTurnEnd?.Invoke();
    }

    private void Update()
    {
        if (_countTurn) _timer.CountTimer();
    }
}
