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
    private Character _player;

    public Action<Move> OnMoveSetUp;
    public Action<Move> OnMoveCooldown;
    public Action<Move, Character> OnMoveStanceCost;
    public Action<Move> OnMoveSelected;
    public Action<Move> OnMovePressed;

    private Button _button;

    private TimelineUIManager _timelineUIManager;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _timelineUIManager = FindAnyObjectByType<TimelineUIManager>();
        _button = GetComponent<Button>();

        if (_battleManager != null)
        {
            _player = _battleManager.Player;
            _playerController = _battleManager.GetBattlerController(_player);

            var moveList = _buttonType == MoveTypes.Normal ? _player.MoveSet : _player.StanceMoveSet;

            if (moveList.Count >= _buttonNumber)
            {
                _move = moveList[_buttonNumber - 1];
                gameObject.SetActive(true);
            }
            else gameObject.SetActive(false);
        }

        _button.onClick.AddListener(() => SendMoveInput());

        if (_move != null) {
            OnMoveSetUp?.Invoke(_move);

            // Here so buttons for stance moves start greyed out
            if(_move.StanceCost > 0) OnMoveStanceCost?.Invoke(_move, _player);
        }
    }

    private void OnEnable()
    {
        if(_battleManager != null)
        {
            _button.interactable = _move.CheckIfCanUseMove(_player);
            OnMoveCooldown?.Invoke(_move);

            // If the move has a stance cost, trigger a UI event
            if(_move.StanceCost > 0) OnMoveStanceCost?.Invoke(_move, _player);
        }
    }

    private void SendMoveInput()
    {
        if (_move != null)
        {
            if (_move.CheckIfCanUseMove(_player))
            {
                _battleManager.GetBattlerController(_player).SetMove(_move);

                // Clear player move preview
                _timelineUIManager.RemoveAllPlayerPreviewIndicators();
            }
        }
    }
}
