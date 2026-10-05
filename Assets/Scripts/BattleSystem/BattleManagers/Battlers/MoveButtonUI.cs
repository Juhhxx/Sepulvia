using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _iconImageBW;
    [SerializeField] private Image _frameImage;
    [SerializeField] private Image _frameSelectImage;
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

    private void OnEnable()
    {
        _frameSelectImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _frameSelectImage.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _frameSelectImage.enabled = false;
    }
}
