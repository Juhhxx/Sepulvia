using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverworldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stanceTMP;
    [SerializeField] private TextMeshProUGUI _essenceTMP;
    [SerializeField] private TextMeshProUGUI _soulFragmentsTMP;
    [SerializeField] private GameObject _equipmentSlots;

    private PlayerController _player;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

    }

    private void Update()
    {
        UpdateValues();
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
}
