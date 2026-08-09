using TMPro;
using UnityEngine;

public class MoveButtonUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _iconSpr;
    [SerializeField] private TextMeshProUGUI _nameTMP;

    private MoveButton _moveButton;

    private void UpdateButton(Move move)
    {
        _iconSpr.sprite = move.Icon;
        _nameTMP.text = move.Name;
    }

    private void Start()
    {
        _moveButton = GetComponent<MoveButton>();

        if (_moveButton != null)
        {
            _moveButton.OnMoveSetUp += UpdateButton;
        }
    }
}
