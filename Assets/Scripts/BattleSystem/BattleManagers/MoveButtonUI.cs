using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _nameTMP;

    private MoveButton _moveButton;

    private void UpdateButton(Move move)
    {
        _iconImage.sprite = move.Icon;
        _nameTMP.text = move.Name;
    }

    private void Awake()
    {
        _moveButton = GetComponent<MoveButton>();

        if (_moveButton != null)
        {
            _moveButton.OnMoveSetUp += UpdateButton;
        }
    }
}
