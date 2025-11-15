using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Scriptable Objects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    
}
