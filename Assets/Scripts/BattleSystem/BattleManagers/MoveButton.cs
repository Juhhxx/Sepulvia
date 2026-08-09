using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Move Move { get; private set; }

    public void SetMove(Move move)
    {
        Move = move;

        _button.onClick.RemoveAllListeners();
        OnMovePressed = null;

        _button.onClick.AddListener(() => OnMovePressed?.Invoke(Move));

        OnMovePressed += _battleManager.AddActionPlayer;

        OnMoveSetUp?.Invoke(Move);
    }

    private BattleManager _battleManager;

    public Action<Move> OnMoveSetUp;
    public Action<Move> OnMoveSelected;
    public Action<Move> OnMovePressed;

    private Button _button;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _button = GetComponent<Button>();
    }
}
