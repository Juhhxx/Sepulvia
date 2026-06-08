using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "RoomTypeSettings", menuName = "Scriptable Objects/RoomTypeSettings")]
public class RoomTypeSettings : ScriptableObject
{
    public List<RoomTypeWeight> RoomTypeWeights;

    public int GetWeightForRoomType(RoomType type)
    {
        RoomTypeWeight weight = RoomTypeWeights.Find(w => w.Type == type);
        return weight.Weight;
    }

    [Serializable]
    public struct RoomTypeWeight
    {
        public RoomType Type;
        public int Weight;
    }

    [Button]
    private void Refresh()
    {
        foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
        {
            if (!RoomTypeWeights.Exists(w => w.Type == type))
            {
                RoomTypeWeights.Add(new RoomTypeWeight() { Type = type, Weight = 1 });
            }
        }
    }
}
