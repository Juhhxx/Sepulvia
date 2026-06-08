using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PCG Rooms/Room Data Base")]
public class RoomDataBase : DataBase<RoomData>
{
    public RoomData GetRoom(System.Random random, bool weighted = false)
    {
        if (weighted)
        {
            return GetRoomWeighted(_entries, random);
        }
        return _entries[random.Next(0, _entries.Count)];
    }
    
    public RoomData GetRoomByType(RoomType type, System.Random random, bool weighted = false)
    {
        var rooms = _entries.FindAll(room => room.RoomType == type);

        if (rooms.Count == 0) return GetRoom(random, weighted);

        if (weighted)
        {
            return GetRoomWeighted(rooms, random);
        }

        return rooms[random.Next(0, rooms.Count)];
    }

    private RoomData GetRoomWeighted(List<RoomData> weightedEntries, System.Random random)
    {
        RoomData selectedRoom = null;

        int totalWeight = 0;
        foreach (RoomData room in weightedEntries)
        {
            totalWeight += room.Weight;
        }

        int randomWeight = random.Next(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (RoomData room in weightedEntries)
        {
            cumulativeWeight += room.Weight;
            
            if (randomWeight <= cumulativeWeight)
            {
                selectedRoom = room;
                break;
            }
        }

        return selectedRoom;
    }
}
