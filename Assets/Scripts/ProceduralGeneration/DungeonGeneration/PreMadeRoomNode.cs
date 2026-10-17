using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu (menuName ="PreMade PCG/PreMadeRoomNode")]
public class PreMadeRoomNode : ScriptableObject
{
    public RoomData RoomData;
    public List<PreMadeRoomConnection> Connections = new List<PreMadeRoomConnection>();
}

[Serializable]
public class PreMadeRoomConnection
{
    public RoomSide DoorId;
    public PreMadeRoomNode TargetRoom;
    public RoomSide TargetDoorId;
}