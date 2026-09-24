using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] private int _buttonNumber;
    [SerializeField] private MoveTypes _buttonType;
    [SerializeField, ReadOnly] private Move _move;
    public Move Move => _move;

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

            var moveList = _buttonType == MoveTypes.Normal ? player.MoveSet : player.StanceMoveSet;

            if (moveList.Count >= _buttonNumber)
            {
                _move = moveList[_buttonNumber - 1];
                gameObject.SetActive(true);
            }
            else gameObject.SetActive(false);
        }

        _button.onClick.AddListener(() => SendMoveInput());

        if (_move != null) OnMoveSetUp?.Invoke(_move);
    }

    private void OnEnable()
    {
        if(_playerController != null)
        {
            _button.interactable = _move.CheckIfCanUseMove(_playerController.Character);
        }
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
