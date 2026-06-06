using UnityEngine;

public class CoreInteractable : RandomBehaviour, IInteractable
{
    [SerializeField] private ItemDataBase _itemDB;
    private DungeonManager _dungeonManager;
    private PlayerController _player;
    private OutlineManager _outline;


    private void Awake()
    {
        CanInteract = true;
        
        _dungeonManager = FindAnyObjectByType<DungeonManager>();
        _player = FindAnyObjectByType<PlayerController>();
        _outline = GetComponent<OutlineManager>();
        TryInitializeRandom();
    }

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        _player.PlayerCharacter.Inventory.AddItem(_itemDB.GetRandomItemOfType(_random, ItemTypes.Fuel));
        _dungeonManager.SetCoreTaken();
        Destroy(gameObject);
    }

    public void ToggleSelected(bool onOff)
    {
        _outline.ToggleOutline(onOff);
    }
}
