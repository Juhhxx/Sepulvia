using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Rooms/Room")]
public class RoomData : ScriptableObject
{
    public GameObject roomPrefab;

    [Tooltip("Rooms you can go to from this room")]
    public List<RoomConnection> connections;
}

[System.Serializable]
public class RoomConnection
{
    public RoomId doorId;            // e.g. "NorthDoor"
    public RoomData targetRoom;
}

public enum RoomId
{
    North,
    East,
    South,
    West
}