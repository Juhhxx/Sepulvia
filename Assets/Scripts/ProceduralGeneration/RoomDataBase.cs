using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PCG Rooms/Room Data Base")]
public class RoomDataBase : DataBase<RoomData>
{
    public RoomData GetRoom(System.Random random)
    {
        return _entries[random.Next(0, _entries.Count)];
    }
    public RoomData GetRoomByType(RoomType type, System.Random random)
    {
        var rooms = _entries.FindAll(room => room.RoomType == type);

        return rooms[random.Next(0, rooms.Count)];
    }
}
