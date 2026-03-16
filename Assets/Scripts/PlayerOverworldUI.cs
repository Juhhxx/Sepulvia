using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverworldUI : MonoBehaviour
{
    [SerializeField] private GameObject _overworldUICanvas;
    [SerializeField] private TextMeshProUGUI _stanceTMP;
    [SerializeField] private TextMeshProUGUI _essenceTMP;
    [SerializeField] private TextMeshProUGUI _soulFragmentsTMP;
    [SerializeField] private GameObject _equipmentSlots;
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

        _scrollingText.text = "";
        StartCoroutine(ScrollTextCR());
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
        _stanceTMP.text = $"{_player.PlayerCharacter.CurrentStance} / {_player.PlayerCharacter.MaxStance}";

        _essenceTMP.text = $"{(_player.PlayerCharacter as PlayerInfo).Essence:000}";

        _soulFragmentsTMP.text = "0";

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

    private IEnumerator ScrollTextCR()
    {
        while (true)
        {
            if (_textToAdd == "")
            {
                _scrollingText.text += INVISIBLECHAR;
            }
            else
            {
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
