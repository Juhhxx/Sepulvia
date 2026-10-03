using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButtonUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _iconImageBW;
    [SerializeField] private GameObject _cooldownIndicator;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private TextMeshProUGUI _cooldownTMP;

    private MoveButton _moveButton;

    private void UpdateButton(Move move)
    {
        _iconImage.sprite = move.Icon;
        _iconImageBW.sprite = move.Icon;
    }

    private void UpdateCooldown(Move move)
    {
        _cooldownIndicator.SetActive(move.CheckIfCooldown());
        _iconImageBW.gameObject.SetActive(move.CheckIfCooldown());

        if (move.CheckIfCooldown())
        {
            int turns = move.Cooldown - move.TurnsPassed;

            _cooldownTMP.text = turns.ToString();
            _cooldownImage.fillAmount = turns / (float)move.Cooldown;
            _iconImageBW.fillAmount = turns / (float)move.Cooldown;
        }
    }

    private void Awake()
    {
        _moveButton = GetComponent<MoveButton>();

        if (_moveButton != null)
        {
            _moveButton.OnMoveSetUp += UpdateButton;
            _moveButton.OnMoveCooldown += UpdateCooldown;
        }
    }
}
