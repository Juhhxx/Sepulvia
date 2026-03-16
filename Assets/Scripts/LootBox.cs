using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ItemInfo> _itemPool;
    private PlayerOverworldUI _overworldUI;
    
    [SerializeField] private Material _outline;
    [SerializeField] private MeshRenderer _renderer;
    private Material[] _defaultMaterials;
    private Material[] _outlineMaterials;

    private PlayerController _player;

    public bool CanInteract { get; private set; }
    public void Interact()
    {
        float rndGet = Random.Range(0f, 1f);

        if (rndGet < 0.5f)
        {
            int rnd = Random.Range(0, _itemPool.Count);
            _player.PlayerCharacter.Inventory.AddItem(_itemPool[rnd]);

            _overworldUI.AddScrollText($"Wow! You got {_itemPool[rnd].Name}!!!");
        }
        else
        {
            _overworldUI.AddScrollText($"Ew!!! Stop messing with the trash");
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
}
