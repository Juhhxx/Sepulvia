using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;

    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private GameObject _inventorySlots;

    private List<GameObject> _createdObjects;

    private List<Button> _buttons;
    public List<Button> GetInventoryButtons() => _buttons;


    public void ShowInventory(InventoryInfo inventory)
    {
        if (_createdObjects != null) ClearSlots();
        else _createdObjects = new List<GameObject>();

        _buttons = new List<Button>();

        for (int i = 0; i < inventory.MaxInventorySpaces; i++)
        {
            ItemStack stack = (i < inventory.ItemSlots.Count) ? inventory.ItemSlots[i] : null;

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
            _buttons.Add(slot.GetComponent<Button>());
        }

        _inventoryCanvas.SetActive(true);
    }

    public void HideInventory() => _inventoryCanvas.SetActive(false);

    private void ClearSlots()
    {
        foreach(GameObject go in _createdObjects) DestroyImmediate(go);
        _createdObjects.Clear();
    }
}
