using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PreMadeDungeon", menuName = "PreMade PCG/PreMadeDungeon")]
public class PreMadeDungeon : ScriptableObject
{
    [SerializeField] public List<PreMadeRoomNode> DungeonRooms;
}
