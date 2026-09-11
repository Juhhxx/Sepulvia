using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] private int _buttonNumber;
    [SerializeField, ReadOnly] private Move _move;

    private BattleManager _battleManager;
    private BattlerController _playerController;

    public Action<Move> OnMoveSetUp;
    public Action<Move> OnMoveSelected;
    public Action<Move> OnMovePressed;

    private Button _button;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _button = GetComponent<Button>();

        if (_battleManager != null)
        {
            Character player = _battleManager.Player;
            _playerController = _battleManager.GetBattlerController(player);

            _move = player.MoveSet[_buttonNumber - 1];
        }

        _button.onClick.AddListener(() => SendMoveInput());

        OnMoveSetUp?.Invoke(_move);
    }

    private void SendMoveInput()
    {
        if (_move != null)
        {
            if (_move.CheckIfCanUseMove(_playerController.Character))
            {
                _playerController.SetMove(_move);
            }
        }
    }
}
