using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Scriptable Objects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public int StackMaximum { get; private set; }
    [field: SerializeField] public bool CanBeUsedInBattle { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    public ItemInfo Instantiate()
    {
        return Instantiate(this);
    }
}
