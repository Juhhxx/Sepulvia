using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class ShopSoulUpgradeUIManager : MonoBehaviour
{
    [SerializeField] private InventorySlotManager _soulUpgradeSlotOne;
    [SerializeField] private InventorySlotManager _soulUpgradeSlotTwo;
    [SerializeField] private InventorySlotManager _soulUpgradeSlotResult;

    public Button SoulOneButton { get; private set; }
    public Button SoulTwoButton { get; private set; }
    public Button SoulUpgradeResultButton { get; private set; }

    [SerializeField] private Transform _availableSoulsParent;

    [SerializeField] private GameObject _availableSoulDisplayPrefab;
    [SerializeField] private GameObject _itemAnimationPrefab;
    [SerializeField] private TextMeshProUGUI _warningTextTMP;

    private List<InventorySlotManager> _availableSoulDisplays = new List<InventorySlotManager>();
    private List<Button> _buttons = new List<Button>();
    public List<Button> GetButtonsSouls() => _buttons;

    private void Start()
    {
        SoulOneButton = _soulUpgradeSlotOne.GetComponent<Button>();
        SoulTwoButton = _soulUpgradeSlotTwo.GetComponent<Button>();
        SoulUpgradeResultButton = _soulUpgradeSlotResult.GetComponent<Button>();
    }

    public void CreateAvailableSouls(int size)
    {
        for (int i = 0; i < _availableSoulDisplays.Count; i++) Destroy(_availableSoulDisplays[i].gameObject);
        _availableSoulDisplays.Clear();

        for (int i = 0; i < size; i++)
        {
            GameObject display = Instantiate(_availableSoulDisplayPrefab, _availableSoulsParent);
            _availableSoulDisplays.Add(display.GetComponent<InventorySlotManager>());
        }
    }
    public void UpdateShopAvailableSouls(ItemStack[] souls)
    {
        _buttons.Clear();

        for (int i = 0; i < souls.Length; i++)
        {
            ItemStack stack = (i < souls.Length) ? souls[i] : null;
            InventorySlotManager display = _availableSoulDisplays[i];

            if (stack != null) display.UpdateSlot(stack.Item.Sprite, stack.Amount);
            else display.UpdateSlot();

            _buttons.Add(display.GetComponent<Button>());
        }
    }

    public void UpdateSoulUpgradeSlots(ItemInfo[] souls)
    {
        if (souls[0] != null) _soulUpgradeSlotOne.UpdateSlot(souls[0].Sprite);
        else _soulUpgradeSlotOne.UpdateSlot();

        if (souls[1] != null) _soulUpgradeSlotTwo.UpdateSlot(souls[1].Sprite);
        else _soulUpgradeSlotTwo.UpdateSlot();
    }

    public void UpdateSoulUpgradeResultSlot(ItemInfo result = null)
    {
        if (result != null) _soulUpgradeSlotResult.UpdateSlot(result.Sprite);
        else _soulUpgradeSlotResult.UpdateSlot();
    }

    public void UpdateWarningText(string warning)
    {
        Debug.Log($"Warning Text : {warning}");
        
        _warningTextTMP.text = warning;
        _warningTextTMP.rectTransform.localScale = Vector3.zero;

        _warningTextTMP.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
    }
}
