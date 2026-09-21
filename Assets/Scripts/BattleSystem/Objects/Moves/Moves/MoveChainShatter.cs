using System;
using System.Collections;
using TNRD;
using UnityEngine;
public class MoveChainShatter : IMove
{
    [SerializeField] private int _maxChainShatter;
    [SerializeField] private bool _right;
    [SerializeField] private SerializableInterface<IMoveMinigame> _minigamePrefab;
    private IMoveMinigame _minigame;

    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        _minigame = resolver.DoMinigame((_minigamePrefab.Value as HitTargetMinigame).gameObject);

        _minigame.StartMinigame(_maxChainShatter);
        _minigame.OnMinigameEnd += (float success) =>
        {
            resolver.DoChainShatter((int)(_maxChainShatter * success), _right);
            resolver.FinishedMinigame();
        };

    }
}