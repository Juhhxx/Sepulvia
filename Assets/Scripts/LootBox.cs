using System.Collections.Generic;
using UnityEngine;

public class LootBox : RandomBehaviour, IInteractable
{
    [SerializeField] private int _uses = 1;
    private int _currentUses = 0;
    [SerializeField] private ItemDataBase _itemDB;
    private PlayerOverworldUI _overworldUI;
    private OutlineManager _outline;
    private PlayerController _player;

    public bool CanInteract { get; private set; }
    public void Interact()
    {
        if (_currentUses >= _uses)
        {
            _overworldUI.AddScrollText($"The trash can is empty...");
            return;
        }

        float rndGet = (float)_random.NextDouble();

        if (rndGet < 0.5f)
        {
            ItemInfo item= _itemDB.GetRandomBuyableItem(_random);
            _player.PlayerCharacter.Inventory.AddItem(item);

            _overworldUI.AddScrollText($"Wow! You got {item.Name}!!!");
        }
        else
        {
            _overworldUI.AddScrollText($"Ew!!! Stop messing with the trash");
        }
        _currentUses++;
    }

    public void ToggleSelected(bool onOff)
    {
        _outline.ToggleOutline(onOff);
    }

    private void Start()
    {
        CanInteract = true;
        _player = FindAnyObjectByType<PlayerController>();
        _overworldUI = FindAnyObjectByType<PlayerOverworldUI>();
        _outline = GetComponent<OutlineManager>();

        TryInitializeRandom();
    }
}
