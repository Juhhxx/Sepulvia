using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MoveInfo _move;
    private BattleUIManager _uiManager;

    public void SetUpHover(MoveInfo move, BattleUIManager uiManager)
    {
        _move = move;
        _uiManager = uiManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _uiManager.ToggleMoveInfo(true, _move);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _uiManager.ToggleMoveInfo(false);
    }
}
