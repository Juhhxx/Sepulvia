using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MoveInfo _move;
    private BattleUIManager _uiManager;
    private Button _button;

    public void SetUpHover(MoveInfo move, BattleUIManager uiManager)
    {
        _move = move;
        _uiManager = uiManager;
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.enabled) return;
        
        _uiManager.ToggleMoveInfo(true, _move);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _uiManager.ToggleMoveInfo(false);
    }
}
