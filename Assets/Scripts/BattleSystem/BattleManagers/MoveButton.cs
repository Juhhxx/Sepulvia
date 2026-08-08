using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Move Move { get; private set; }

    public void SetMove(Move move)
    {
        Move = move;

        _button.onClick.AddListener(() => OnMovePressed?.Invoke(Move));

        OnMovePressed += move => _battleManager.Player.QueueAction(new BattleAction(_battleManager.Player, move));
    }

    private BattleManager _battleManager;

    public Action<Move> OnMoveSelected;
    public Action<Move> OnMovePressed;

    private Button _button;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _button = GetComponent<Button>();
    }
}
