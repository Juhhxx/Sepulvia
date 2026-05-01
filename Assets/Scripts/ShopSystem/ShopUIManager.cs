using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private Transform _shopDisplayParent;
    [SerializeField] private GameObject _shopDisplayPrefab;

    private List<ShopDisplayManager> _shopDisplays = new List<ShopDisplayManager>();

    private List<Button> _buttons = new List<Button>();
    public List<Button> GetButtons() => _buttons;

    public void CreateShopDisplays(int shopSize)
    {
        for (int i = 0; i < shopSize; i++)
        {
            GameObject display = Instantiate(_shopDisplayPrefab, _shopDisplayParent);
            _shopDisplays.Add(display.GetComponent<ShopDisplayManager>());
        }
    }

    public void UpdateShopDisplays(List<ItemInfo> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ItemInfo item = items[i];
            
            ShopDisplayManager display = _shopDisplays[i];

            display.UpdateDisplay(item.Sprite, item.Value);

            _buttons.Add(display.GetComponent<Button>());
        }
    }

}
