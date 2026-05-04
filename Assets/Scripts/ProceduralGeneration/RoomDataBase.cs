using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PCG Rooms/Room Data Base")]
public class RoomDataBase : ScriptableObject
{
    [SerializeField, Expandable] private List<RoomData> _rooms = new List<RoomData>();

    public bool IsEmpty => _rooms.Count == 0;

    public RoomData GetRoom(System.Random random)
    {
        return _rooms[random.Next(0, _rooms.Count)];
    }
    public RoomData GetRoomByType(RoomType type, System.Random random)
    {
        var rooms = _rooms.FindAll(room => room.RoomType == type);

        return rooms[random.Next(0, rooms.Count)];
    }
}
