using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomBlock", menuName = "PCG/RoomBlock")]
public class RoomBlock : ScriptableObject
{
    [field: SerializeField] public GameObject RoomPrefab { get; private set; }
    [field: SerializeField] public List<Transform> ConnectionPoints { get; private set; }
    
}
