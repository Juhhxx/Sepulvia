using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Move _move;
    private BattleUIManager _uiManager;
    private TimelineUIManager _timelineUIManager;

    private void Awake()
    {
        GetComponent<MoveButton>().OnMoveSetUp += SetUp;
    }
    public void SetUp(Move move)
    {
        _move = move;
        _uiManager = FindAnyObjectByType<BattleUIManager>();
        _timelineUIManager = FindAnyObjectByType<TimelineUIManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _uiManager.ToggleMoveInfo(true, _move);

        // If the move is not on cooldown and isn't instant
        if(!_move.CheckIfCooldown() && _move.RecoveryCost > 0)
        {
            //Remove any previous preview indicators and add a new one
            _timelineUIManager.RemoveAllPlayerPreviewIndicators();
            _timelineUIManager.AddPlayerMovePreviewIndicator(_move.RecoveryCost);            
        }

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Remove preview timeline indicators
        _timelineUIManager.RemoveAllPlayerPreviewIndicators();

        _uiManager.ToggleMoveInfo(false);
    }
}
