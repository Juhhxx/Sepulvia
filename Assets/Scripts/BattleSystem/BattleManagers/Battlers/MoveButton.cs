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
    private Character _player;

    public Action<Move> OnMoveSetUp;
    public Action<Move> OnMoveCooldown;
    public Action<Move> OnMoveSelected;
    public Action<Move> OnMovePressed;

    private Button _button;

    private TimelineUIManager _timelineUIManager;

    private void Start()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
        _timelineUIManager = FindAnyObjectByType<TimelineUIManager>();
        _button = GetComponent<Button>();

        SetUp();
    }

    private void SetUp()
    {
        if (_battleManager != null)
        {
            _player = _battleManager.Player;

            var moveList = _buttonType == MoveTypes.Normal ? _player.MoveSet : _player.StanceMoveSet;

            if (moveList.Count >= _buttonNumber)
            {
                _move = moveList[_buttonNumber - 1];
                gameObject.SetActive(true);
            }
            else gameObject.SetActive(false);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => SendMoveInput());

            if (_move != null) OnMoveSetUp?.Invoke(_move);
        }
    }

    private void OnEnable()
    {
        SetUp();
        
        if(_battleManager != null)
        {
            _button.interactable = _move.CheckIfCanUseMove(_player);
            OnMoveCooldown?.Invoke(_move);
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
