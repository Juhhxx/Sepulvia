using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Inventory/New Item")]
public class ItemInfo : DataAsset
{
    [field: Header(" Base Item Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    [field: SerializeField] public int StackMaximum { get; private set; }
    [field: SerializeField] public bool CanBeUsedInBattle { get; private set; }
    [field: SerializeField] public bool CanBeBought{ get; private set; }
    [field: SerializeField] public bool CanBeSold { get; private set; }

    [field: SerializeField] public ItemTypes Type { get; private set; }

    [field: SerializeField] public int Level { get; private set; }

    [field: Space(10)]
    [field: Header("Item Shop Value")]
    [field: Space(5)]
    [field: ShowIf(EConditionOperator.Or, "CanBeBought", "CanBeSold")]
    [field: SerializeField] public int Value { get; private set; }

    [field: Space(10)]
    [field: Header("Immidiate Effect Item Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", ItemTypes.Immediate)]
    [field: SerializeField] public Stats Stat { get; private set; }

    [field: ShowIf("Type", ItemTypes.Immediate)]
    [field: SerializeField] public int Amount { get; private set; }

    [field: Space(10)]
    [field: Header("Long Term Effect Item Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", ItemTypes.LongTerm)]
    [field: SerializeField] public StatModifier Modifier { get; private set; }

    [field: Space(10)]
    [field: Header("Equippable Item Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", ItemTypes.Equippable)]
    [field: SerializeField] public EquipmentType EquipmentType { get; private set; }

    [field: ShowIf("IsStatEquip")]
    [field: SerializeField] public Stats StatEquip { get; private set; }

    [field: ShowIf("IsStatEquip")]
    [field: SerializeField] public int AmountEquip { get; private set; }
    private bool IsStatEquip => Type == ItemTypes.Equippable && EquipmentType == EquipmentType.StatModifier;

    [field: ShowIf("IsMoveEquip")]
    [field: SerializeField, MinValue(0), MaxValue(3)] public int MoveIndex { get; private set; }

    [field: ShowIf("IsMoveEquip")]
    [field: SerializeField] public MoveInfo ChangeTo { get; private set; }
    private bool IsMoveEquip => Type == ItemTypes.Equippable && EquipmentType == EquipmentType.MoveModidier;

    [field: ShowIf("Type", ItemTypes.Equippable)]
    [field: SerializeField] public ItemInfo Upgrade { get; private set; }


    [field: Space(10)]
    [field: Header("Item Description")]
    [field: Space(5)]
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }
}
