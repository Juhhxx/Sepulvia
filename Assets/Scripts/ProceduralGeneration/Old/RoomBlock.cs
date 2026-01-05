using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomBlock", menuName = "PCG/RoomBlock")]
public class RoomBlock : ScriptableObject
{
    [field: SerializeField] public GameObject RoomPrefab { get; private set; }

    [System.Serializable]
    public struct ConnectionPoint
    {
        public RoomBlockConnectionPoints Point;
        public RoomBlock ConnectedBlock;
    }

    [field: SerializeField] public List<ConnectionPoint> ConnectionPoints { get; private set; }

}

public enum RoomBlockConnectionPoints
{
    North,
    East,
    South,
    West
}
