using UnityEngine;
using NaughtyAttributes;
using TNRD;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Inventory/New Item")]
public class ItemInfo : DataAsset
{
    [field: Header(" Base Item Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }

    [field: SerializeField] public int StackMaximum { get; private set; }
    [field: SerializeField] public bool CanBeUsedInBattle { get; private set; }
    [field: SerializeField] public bool CanBeBought{ get; private set; }
    [field: SerializeField] public bool CanBeSold { get; private set; }

    [field: SerializeField] public ItemTypes Type { get; private set; }

    [ShowIf("Type", ItemTypes.Consumable)]
    [SerializeField] private SerializableInterface<IConsumable> _consumable;
    public IConsumable ConsumableLogic => _consumable.Value;

    [ShowIf("Type", ItemTypes.Equippable)]
    [SerializeField] private SerializableInterface<IEquippable> _equippable;
    public IEquippable EquippableLogic => _equippable.Value;

    [field: SerializeField] public int Level { get; private set; }

    [field: Space(10)]
    [field: Header("Item Shop Value")]
    [field: Space(5)]
    [field: ShowIf(EConditionOperator.Or, "CanBeBought", "CanBeSold")]
    [field: SerializeField] public int Value { get; private set; }
}

public interface IConsumable
{
    public void OnConsumed(BattlerController user, BattlerController[] targets);
    public void OnConsumed(Character user);
}
public interface IEquippable
{
    public void OnEquip(Character user);
    public void OnUnequip();
}
