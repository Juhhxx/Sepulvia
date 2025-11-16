using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Scriptable Objects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [field: Header(" Base Item Parameters")]
    [field: Space(5)]
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    [field: SerializeField] public int StackMaximum { get; private set; }
    [field: SerializeField] public bool CanBeUsedInBattle { get; private set; }

    [field: SerializeField] public ItemTypes Type { get; private set; }

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
    [field: Header("Item Description")]
    [field: Space(5)]
    [field: SerializeField, ResizableTextArea] public string Description { get; private set; }

    public ItemInfo Instantiate()
    {
        return Instantiate(this);
    }
}
