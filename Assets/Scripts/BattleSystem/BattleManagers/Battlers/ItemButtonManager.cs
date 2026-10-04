using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    [SerializeField] private Image _itemButtonImage;

    [SerializeField] private GameObject _timer;
    [SerializeField] private Image _timerImage;
    [SerializeField] private TextMeshProUGUI _timerTMP;

    private BattlerController _playerController;
    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();

        ToggleItemButton(true);
    }

    private void ToggleItemButton(bool on)
    {
        _button.interactable = on;

        _timer.SetActive(!on);
    }

    private void Update()
    {
        if (_playerController == null)
        {
            var battleManager = FindAnyObjectByType<BattleManager>();
            _playerController = battleManager.GetBattlerController(battleManager.Player);
            return;
        }

        ToggleItemButton(_playerController.ItemColldownTurns == 0);

        float fillAmount = _playerController.ItemColldownTurns == 0 ?
                            0 :
                            1 - (_playerController.ItemColldownTurns / (float)_playerController.ItemCooldown);

        _timerImage.DOFillAmount(fillAmount, 0.2f);

        _timerTMP.text = $"{_playerController.ItemColldownTurns}";
    }
}
