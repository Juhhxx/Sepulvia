using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    private ShopManager _shopManager;
    private OutlineManager _outline;

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        _shopManager.ToggleShop(true);
    }

    public void ToggleSelected(bool onOff)
    {
        _outline.ToggleOutline(onOff);
    }

    private void Start()
    {
        _shopManager = FindAnyObjectByType<ShopManager>();
        _outline = GetComponent<OutlineManager>();

        _shopManager.ChooseShopItems();
        CanInteract = true;
    }
}
