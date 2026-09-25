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

    private void Start()
    {
        var battleManager = FindAnyObjectByType<BattleManager>();
        _playerController = battleManager.GetBattlerController(battleManager.Player);

        _button = GetComponent<Button>();

        _playerController.ItemTimer.OnTimerBegin += () => ToggleItemButton(false);
        _playerController.ItemTimer.OnTimerDone += () => ToggleItemButton(true);

        ToggleItemButton(true);
    }

    private void ToggleItemButton(bool on)
    {
        _button.interactable = on;

        _timer.SetActive(!on);
    }

    private void Update()
    {
        _timerImage.fillAmount = 1 - _playerController.ItemTimer.CurrentTime / _playerController.ItemTimer.MaxTime;
        _timerTMP.text = $"{(int)((_playerController.ItemTimer.CurrentTime % 60) % 60) + 1}";
    }
}
