using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    [field: SerializeField] public RoomData RoomData { get; set; }
    [field: SerializeField] public Transform Spawnposition{ get; set; }

    private List<Door> _doorList;
    public Door GetDoor(RoomId id)
    {
        foreach (Door d in _doorList)
        {
           if (d.DoorId == id) return d; 
        }
        return null;
    }

    public void Awake()
    {
        _doorList = GetComponentsInChildren<Door>().ToList();
    }

}
