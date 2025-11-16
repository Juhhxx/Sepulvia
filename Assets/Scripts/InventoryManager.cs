using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private InventoryInfo _inventory;

    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _inventorySlots;

    private List<GameObject> _createdObjects;

    [Button(enabledMode: EButtonEnableMode.Always)]
    public void ShowInventory()
    {
        if (_createdObjects != null) ClearSlots();
        else _createdObjects = new List<GameObject>();

        for (int i = 0; i < _inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < _inventory.ItemSlots.Count) ? _inventory.ItemSlots[i] : null;

            GameObject slot = Instantiate(_inventorySlotPrefab, _inventorySlots.transform);
            Image image = slot.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI tmp = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (stack != null)
            {
                image.sprite = stack.Item.Sprite;
                image.color = Color.white;
                
                tmp.text = $"{stack.Amount}";
            }
            else
            {
                image.color = new Color(1f , 1f, 1f, 0f);
                tmp.text = "";
            }
            
            _createdObjects.Add(slot);
        }

        _inventoryCanvas.SetActive(true);
    }

    public void HideInventory() => _inventoryCanvas.SetActive(false);

    private void ClearSlots()
    {
        foreach(GameObject go in _createdObjects) DestroyImmediate(go);
        _createdObjects.Clear();
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    public void AddOnePoition() => _inventory.AddItem(_inventory.ItemSlots[0].Item);
    [Button(enabledMode: EButtonEnableMode.Always)]
    public void AddOneSoul() => _inventory.AddItem(_inventory.ItemSlots[2].Item);
    [Button(enabledMode: EButtonEnableMode.Always)]
    public void AddOneRespawn() => _inventory.AddItem(_inventory.ItemSlots[1].Item);
}
