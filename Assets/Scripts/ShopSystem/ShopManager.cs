using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private int _shopSize = 4;
    [SerializeField] private Canvas _shopCanvas;
    [SerializeField] private ShopUIManager _shopUIManager;
    [SerializeField] private List<ItemInfo> _possibleItems;

    private PlayerController _player;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();

        _shopUIManager.CreateShopDisplays(_shopSize);
        SetUpShop();
    }

    private void ToggleShop(bool onOff)
    {
        if (onOff) SetUpShop();

        _shopCanvas.gameObject.SetActive(onOff);
    }

    private void SetUpShop()
    {
        List<ItemInfo> items = new List<ItemInfo>();

        for (int i = 0; i < _shopSize; i++)
        {
            items.Add(_possibleItems[Random.Range(0, _possibleItems.Count)]);
        }

        _shopUIManager.UpdateShopDisplays(items);

        _shopUIManager.GetButtons().ForEach(button => button.onClick.RemoveAllListeners());

        List<Button> buttons = _shopUIManager.GetButtons();
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => {

                bool hasBought = BuyItem(items[index]);

                if (hasBought) buttons[index].GetComponent<ShopDisplayManager>().DoDisplayPurchaseAnim();
                else buttons[index].GetComponent<ShopDisplayManager>().DoDisplayNotEnoughAnim();

            });
        }
    }

    public bool BuyItem(ItemInfo item)
    {
        if (_player.Essence >= item.Value)
        {
            _player.ChangeEssence(-item.Value);
            _player.PlayerCharacter.Inventory.AddItem(item);

            return true;
        }
        
        return false;
    }
}
