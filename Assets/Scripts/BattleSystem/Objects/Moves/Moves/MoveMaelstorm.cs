using System;
using System.Collections;
using TNRD;
using UnityEngine;

public class MoveMaelstorm : IMove
{
    [SerializeField] private int _maxCurseStacks;
    [SerializeField] private StatusEffectInfo _curse;
    [SerializeField] private SerializableInterface<IMoveMinigame> _minigamePrefab;
    private IMoveMinigame _minigame;

    public void OnDoMove(BattlerController user, BattlerController[] targets, BattleResolver resolver)
    {
        _minigame = resolver.DoMinigame(targets[0], (_minigamePrefab.Value as HitTargetMinigame).gameObject);

        _minigame.StartMinigame(_maxCurseStacks);
        _minigame.OnMinigameEnd += (float success) =>
        {
            for (int i = 0; i < (_maxCurseStacks * (1 - success)); i++)
            {
                resolver.DoStatusEffect(_curse.Instantiate(), targets);
            }

            resolver.FinishedMinigame();
        };

    }
}