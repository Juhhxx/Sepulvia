using UnityEngine;
using System;

[Serializable]
public class SoulBurnProfile : ICloneable
{
    [SerializeField] public Sprite Icon;
    [SerializeField] private int _waitTimeTurns;
    public int WaitTimeTurns => _waitTimeTurns;

    [SerializeField] private int _waitTimeMinimum;
    public int WaitTimeMinimum => _waitTimeMinimum;

    [SerializeField] private float _waitTimeChangeRate;
    public float WaitTimeChangeRate => _waitTimeChangeRate;

    [SerializeField] private bool _doLeft;
    [SerializeField] private bool _doRight;
    public (bool, bool) DoLeftRight => (_doLeft, _doRight);

    [SerializeField] private int _amount;
    public int Amount => _amount;

    private int _turnsUntil;
    public int TurnsUntil => _turnsUntil;

    private int _turnsPassed = 0;

    public event Action<int,int> OnSoulBurn; // ints to indicate how much burn on each side
    public event Action<int,int> OnTurnPassed; // int to indicate how many turns until soul burn

    public void OnStartBattle()
    {
        _turnsUntil = _waitTimeTurns;
        _turnsPassed = 0;
    }

    public void PassTurn()
    {
        var tmp = _turnsUntil - _turnsPassed;

        _turnsPassed++;

        OnTurnPassed?.Invoke(_turnsUntil - _turnsPassed, tmp);
    }

    public void CheckSoulBurn()
    {
        if (_turnsPassed == _turnsUntil) OnTurnReached();
    }

    public void OnTurnReached()
    {
        _turnsUntil = Mathf.FloorToInt(_waitTimeTurns * _waitTimeChangeRate);
        _turnsUntil = Mathf.Max(_turnsUntil, _waitTimeMinimum);
        _turnsPassed = 0;

        OnSoulBurn?.Invoke(_doLeft ? _amount : 0,
                            _doRight ? _amount : 0);

        OnTurnPassed?.Invoke(_turnsUntil - _turnsPassed, 0);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
