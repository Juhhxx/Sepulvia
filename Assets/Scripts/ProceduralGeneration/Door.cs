using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private RoomId _doorId;
    public RoomId DoorId => _doorId;

    [field: SerializeField] public Transform Spawnposition{ get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            RoomManager.Instance.EnterDoor(_doorId);
        }
    }
}
