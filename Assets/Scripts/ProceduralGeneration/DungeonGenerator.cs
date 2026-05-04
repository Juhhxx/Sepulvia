using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : RandomBehaviour
{
    [SerializeField, BoxGroup("Generation Settings")] private RoomDataBase _roomDataBase;
    [SerializeField, BoxGroup("Generation Settings")] private int _maxDungeonSize = 5;

    private void Start()
    {
        TryInitializeRandom();
    }

    public RoomNode Generate()
    {
        if (_roomDataBase.IsEmpty) return null;

        var start = new RoomNode(_roomDataBase.GetRoomByType(RoomType.StartingRoom, _random), 0);
        var createdRooms = new List<RoomNode>() { start };

        for (int i = 1; i < _maxDungeonSize; i++)
        {
            // Create new room

            RoomData roomData = null;

            if (i + 1 == _maxDungeonSize) roomData = _roomDataBase.GetRoomByType(RoomType.CoreRoom, _random);
            else roomData = _roomDataBase.GetRoomByType(RoomType.DungeonRoom, _random);

            RoomNode newRoom = new RoomNode(roomData, i);

            // Create Connections

            createdRooms[i - 1].Connections.Add(new RoomConnection(RoomSide.North, RoomSide.South, newRoom));
            newRoom.Connections.Add(new RoomConnection(RoomSide.South, RoomSide.North, createdRooms[i - 1]));

            createdRooms.Add(newRoom);
        }

        return start;
    }

    public void ShowGeneratedDungeon(RoomNode firstNode)
    {
        Debug.Log($"Dungeon Created :");
        Debug.Log($"|    {firstNode.RoomData.RoomName} - Start");

        RoomNode node = firstNode.Connections.Find(c => c.DoorId == RoomSide.North).TargetRoom;
        while (node != null)
        {
            Debug.Log("|");
            Debug.Log($"|    {node.RoomData.RoomName}");

            node = node.Connections.Find(c => c.DoorId == RoomSide.North)?.TargetRoom;
        }

    }
}
