using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "PCG Rooms/Room")]
public class RoomData : DataAsset
{
    public GameObject RoomPrefab;
    public string RoomName;
    public RoomType RoomType;
    public int Weight;
}

public enum RoomSide
{
    North = 1,
    East = 2,
    South = -1,
    West = -2,
    None = 0
}

public enum RoomType
{
    StartingRoom,
    DungeonRoom,
    LootRoom,
    AmbushRoom,
    BossRoom,
    CoreRoom
}

[Serializable]
public class RoomNode
{
    public string RoomName;
    public RoomData RoomData;
    public List<RoomConnection> Connections = new List<RoomConnection>();

    public RoomNode(RoomData data, int num)
    {
        RoomData = data;
        RoomName = data.RoomName + " " + num;
    }
}

[System.Serializable] public class RoomConnection 
{ 
    public RoomSide DoorId;
    [SerializeReference] public RoomNode TargetRoom;
    public RoomSide TargetDoorId;

    public RoomConnection(RoomSide doorId, RoomSide targetId, RoomNode node)
    {
        DoorId = doorId;
        TargetDoorId = targetId;
        TargetRoom = node;
    }
}