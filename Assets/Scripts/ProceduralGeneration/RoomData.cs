using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Rooms/Room")]
public class RoomData : ScriptableObject
{
    public GameObject RoomPrefab;
    public string RoomName;

    [Tooltip("Rooms you can go to from this room")]
    public List<RoomConnection> Connections;
}

[System.Serializable]
public class RoomConnection
{
    public RoomId doorId;
    public RoomData targetRoom;
    public RoomId targetDoorId;
}

public enum RoomId
{
    North,
    East,
    South,
    West,
    None
}