using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Move _move;
    private BattleUIManager _uiManager;

    private void Awake()
    {
        GetComponent<MoveButton>().OnMoveSetUp += SetUp;
    }
    public void SetUp(Move move)
    {
        _move = move;
        _uiManager = FindAnyObjectByType<BattleUIManager>();
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
