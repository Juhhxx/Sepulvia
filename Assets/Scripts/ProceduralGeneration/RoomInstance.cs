using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    [field: SerializeField] public RoomData RoomData { get; set; }
    [field: SerializeField] public Transform Spawnposition{ get; set; }
}
