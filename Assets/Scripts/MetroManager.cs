using System.Linq;
using UnityEngine;

public class MetroManager : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isFueld = false;
    [SerializeField] private ItemInfo _fuelitem;
    private PlayerOverworldUI _overworldUI;
    
    private PlayerController _player;
    private OutlineManager _outline;

    private void Start()
    {
        CanInteract = true;
        _player = FindAnyObjectByType<PlayerController>();
        _outline = GetComponent<OutlineManager>();

        _overworldUI = FindAnyObjectByType<PlayerOverworldUI>();
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
        _outline.ToggleOutline(onOff);
    }
}
