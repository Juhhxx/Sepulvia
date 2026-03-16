using System.Linq;
using UnityEngine;

public class MetroManager : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isFueld = false;
    [SerializeField] private ItemInfo _fuelitem;
    private PlayerOverworldUI _overworldUI;
    
    [SerializeField] private Material _outline;
    [SerializeField] private MeshRenderer _renderer;
    private Material[] _defaultMaterials;
    private Material[] _outlineMaterials;

    private PlayerController _player;

    private void Start()
    {
        CanInteract = true;
        _player = FindAnyObjectByType<PlayerController>();
        _overworldUI = FindAnyObjectByType<PlayerOverworldUI>();

        _defaultMaterials = _renderer.materials;
        _outlineMaterials = new Material[_defaultMaterials.Length + 1];

        for (int i = 0; i < _defaultMaterials.Length; i++)
        {
            _outlineMaterials[i] = _defaultMaterials[i];
        }

        _outlineMaterials[_outlineMaterials.Length - 1] = _outline;
    }

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        Debug.Log("HELLO");
        if (_isFueld) _overworldUI.AddScrollText("Yay! You fueld up your train and can now travel to other stations! Shame that's not implemented yet :(");
        else
        {
            if (_player.PlayerCharacter.Inventory.Contains(_fuelitem))
            {
                _player.PlayerCharacter.Inventory.RemoveItem(_fuelitem);
                _isFueld = true;
                _overworldUI.AddScrollText("Yay! You fueld up your train and can now travel to other stations! Shame that's not implemented yet :(");
                return;
            }

            _overworldUI.AddScrollText("Your train is out of fuel! Find something to fuel it up");
        }
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
}
