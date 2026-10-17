using System.Collections.Generic;
using System.Linq;
using GLTFast.Schema;
using NaughtyAttributes;
using UnityEngine;

public class DungeonGenerator : RandomBehaviour
{
    [SerializeField, BoxGroup("Generation Settings")] private bool _usePreMadeDungeon = false;

    [ShowIf("_usePreMadeDungeon")]
    [SerializeField, BoxGroup("Generation Settings")] private PreMadeDungeon _preMadeDungeon;

    [HideIf("_usePreMadeDungeon")]
    [SerializeField, Expandable,BoxGroup("Generation Settings")] private RoomDataBase _roomDataBase;
    [HideIf("_usePreMadeDungeon")]
    [SerializeField, Expandable, BoxGroup("Generation Settings")] private RoomTypeSettings _roomTypeSettings;
    [HideIf("_usePreMadeDungeon")]
    [SerializeField, BoxGroup("Generation Settings")] private int _maxDungeonSize = 5;

    private void Start()
    {
        TryInitializeRandom();
    }

    // Dungeon Generation
    public RoomNode Generate()
    {
        if (_usePreMadeDungeon) return GeneratePreMade();

        if (_roomDataBase.IsEmpty)
        {
            Debug.Log("[Dungeon Generator] Couldn't find Dungeon Room Data Base! Can't generate Dungeon.");
            return null;
        }

        var start = new RoomNode(_roomDataBase.GetRoomByType(RoomType.StartingRoom, _random, true), 0);
        var createdRooms = new List<RoomNode>() { start };

        for (int i = 0; i < _maxDungeonSize + 1; i++)
        {
            // Create new room

            RoomData roomData = null;

            if (i == _maxDungeonSize) roomData = _roomDataBase.GetRoomByType(RoomType.BossRoom, _random, true);
            else roomData = _roomDataBase.GetRoomByType(GetRoomType(), _random, true);

            RoomNode newRoom = new RoomNode(roomData, i + 1); // account for starting room

            // Create Connections

            createdRooms.Last().Connections.Add(new RoomConnection(RoomSide.North, RoomSide.South, newRoom));
            newRoom.Connections.Add(new RoomConnection(RoomSide.South, RoomSide.North, createdRooms.Last()));

            createdRooms.Add(newRoom);
        }

        var end = new RoomNode(_roomDataBase.GetRoomByType(RoomType.CoreRoom, _random, true), createdRooms.Count);

        createdRooms.Last().Connections.Add(new RoomConnection(RoomSide.North, RoomSide.South, end));
        end.Connections.Add(new RoomConnection(RoomSide.South, RoomSide.North, createdRooms.Last()));

        createdRooms.Add(end);

        return start;
    }

    // Pre Made Dungeons
    private RoomNode GeneratePreMade()
    {
        Debug.Log("[Dungeon Generator] Generating Pre-Made Dungeon.");

        var createdRooms = new List<RoomNode>();

        int roomNumber = 0;

        foreach (PreMadeRoomNode prn in _preMadeDungeon.DungeonRooms)
        {
            createdRooms.Add(new RoomNode(prn.RoomData, roomNumber));
            
            roomNumber++;
        }

        for (int i = 0; i < createdRooms.Count; i++)
        {
            var node = createdRooms[i];
            var preMade = _preMadeDungeon.DungeonRooms[i];

            foreach (PreMadeRoomConnection pmc in preMade.Connections)
            {
                node.Connections.Add(new RoomConnection(
                    pmc.DoorId,
                    pmc.TargetDoorId,
                    GetRoomNodeByData(pmc.TargetRoom.RoomData, createdRooms.ToArray())
                ));
            }
        }

        return createdRooms[0];
    }

    private RoomNode GetRoomNodeByData(RoomData data, RoomNode[] nodes)
    {
        foreach (RoomNode node in nodes)
        {
            if (node.RoomData == data) return node;
        }

        return null;
    }

    // Get Room of Type
    private RoomType GetRoomType()
    {
        int totalWeight = 0;
        foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
        {
            totalWeight += _roomTypeSettings.GetWeightForRoomType(type);
        }

        int randomWeight = _random.Next(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
        {
            cumulativeWeight += _roomTypeSettings.GetWeightForRoomType(type);

            if (randomWeight < cumulativeWeight)
            {
                Debug.Log($"Selected Room Type: {type}");
                return type;
            }
        }

        return RoomType.DungeonRoom;
    }

    // Print Dungeon
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
