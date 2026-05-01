using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _outline;
    [SerializeField] private MeshRenderer _renderer;
    private Material[] _defaultMaterials;
    private Material[] _outlineMaterials;

    private ShopManager _shopManager;
    private ShopUIManager _shopUIManager;

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        _shopManager.ChooseShopItems();
        _shopManager.SetUpShop();
        _shopUIManager.ToggleShop(true);
    }

    public void ToggleSelected(bool onOff)
    {
        if (onOff)
        {
            _renderer.materials = _outlineMaterials;
        }
        else
        {
            _renderer.materials = _defaultMaterials;
        }
    }

    private void Start()
    {
        _shopManager = FindAnyObjectByType<ShopManager>();
        _shopUIManager = FindAnyObjectByType<ShopUIManager>();

        CanInteract = true;

        _defaultMaterials = _renderer.materials;
        _outlineMaterials = new Material[_defaultMaterials.Length + 1];

        for (int i = 0; i < _defaultMaterials.Length; i++)
        {
            _outlineMaterials[i] = _defaultMaterials[i];
        }

        _outlineMaterials[_outlineMaterials.Length - 1] = _outline;
    }
}
