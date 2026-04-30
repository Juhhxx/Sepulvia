using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerOverworldUI : MonoBehaviour
{
    [SerializeField] private GameObject _overworldUICanvas;
    [SerializeField] private TextMeshProUGUI _stanceTMP;
    [SerializeField] private TextMeshProUGUI _essenceTMP;
    [SerializeField] private TextMeshProUGUI _soulFragmentsTMP;
    [SerializeField] private GameObject _equipmentSlots;

    [SerializeField] private Image _dashTimerImage;
    [SerializeField] private bool _showDashTimer = true;

    [SerializeField] private TextMeshProUGUI _scrollingText;
    [SerializeField] private float _scrollingSpeed = 2f;
    private float _textWait;
    [SerializeField] private int _scrollingTextThreshold = 300;

    [SerializeField] private string _testText;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void Displayext() => AddScrollText(_testText);

    private const string INVISIBLECHAR = "<color=#00000000>A</color>";

    private PlayerController _player;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _player.OnBattleEnterExit += (bool inBattle) => ToggleOverworldUI(!inBattle);

        _player.PlayerCharacter.OnStanceChange += (int current, int max, int previous) => UpdateValue(_stanceTMP, $"{current} / {max}");
        _player.PlayerCharacter.Inventory.OnChangeEqupipment += UpdateEquipmentSlots;
        (_player.PlayerCharacter as Player).OnEssenceChange += (int essence) => UpdateValue(_essenceTMP, essence);

        _essenceTMP.text = $"{(_player.PlayerCharacter as Player).Essence:000}";

        _scrollingText.text = "";
        StartCoroutine(ScrollTextCR());

        if (!_showDashTimer) _dashTimerImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateValues();
    }

    public void ToggleOverworldUI(bool onOff)
    {
        _overworldUICanvas.SetActive(onOff);
    }

    public void UpdateValues()
    {
        _soulFragmentsTMP.text = "0";

        _dashTimerImage.fillAmount = 1 - _player.DashCooldownTime;
        if (_player.CanDash) _dashTimerImage.fillAmount = 1;
    }

    private void UpdateValue(TextMeshProUGUI tmp, string text)
    {
        tmp.text = text;
        tmp.transform.DOScale(Vector3.one * 1.25f, 0.15f).OnComplete(() => tmp.transform.DOScale(Vector3.one, 0.15f));
    }

    private void UpdateValue(TextMeshProUGUI tmp, int value)
    {
        if (_updateValueCR != null) StopCoroutine(_updateValueCR);

        _updateValueCR = StartCoroutine(UpdateValueCR(tmp, value));
    }
    Coroutine _updateValueCR = null;
    private IEnumerator UpdateValueCR(TextMeshProUGUI tmp, int targetValue)
    {
        while (int.Parse(tmp.text) != targetValue)
        {
            int currentValue = int.Parse(tmp.text);

            if (currentValue < targetValue) currentValue++;
            else if (currentValue > targetValue) currentValue--;

            tmp.text = currentValue.ToString();
            tmp.transform.DOScale(Vector3.one * 1.25f, 0.1f).OnComplete(() => tmp.transform.DOScale(Vector3.one, 0.1f));

            _player.OnEssenceChange?.Invoke();

            yield return new WaitForSeconds(0.01f);
        }

        _updateValueCR = null;
    }

    private void UpdateEquipmentSlots()
    {
        for (int i = 0; i < _equipmentSlots.transform.childCount; i++)
        {
            Image img = _equipmentSlots.transform.GetChild(i).GetChild(0).GetComponent<Image>();

            if (i < _player.PlayerCharacter.Inventory.EquipmentSlots.Count)
            {
                img.sprite = _player.PlayerCharacter.Inventory.EquipmentSlots[i].Sprite;
                img.color = Color.white;
            }
            else
            {
                Color c = img.color;
                c.a = 0;
                img.color = c;
            }
        }
    }

    [SerializeField, ReadOnly] private string _textToAdd = "";

    public void AddScrollText(string text)
    {
        string add = "";

        if (_textToAdd == "") add = text;
        else add = " | " + text;

        _textToAdd += add;
    }

    private Timer _moveTextTimer = new Timer(2.5f, Timer.TimerReset.Manual);

    private IEnumerator ScrollTextCR()
    {
        while (true)
        {
            if (_textToAdd == "" && !_moveTextTimer.Done)
            {
                _moveTextTimer.CountTimer();
                _scrollingText.text += INVISIBLECHAR;
            }
            else if (_textToAdd != "")
            {
                _moveTextTimer.ResetTimer();
                _scrollingText.text += _textToAdd[0];
                _textToAdd = _textToAdd.Remove(0, 1);
            }

            if (_scrollingText.text.Length > _scrollingTextThreshold)
            {
                _scrollingText.text.Remove(0);
            }

            yield return new WaitForSeconds(_scrollingSpeed);
        }
    }
}
