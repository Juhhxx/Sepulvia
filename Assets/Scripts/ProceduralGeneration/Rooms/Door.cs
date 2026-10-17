using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private RoomSide _doorId;
    public RoomSide DoorId => _doorId;

    private bool _used = false;

    [field: SerializeField] public Transform Spawnposition{ get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (_used) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            RoomManager.Instance.EnterDoor(_doorId);
            _used = true;
        }
    }

    private void OnEnable()
    {
        _used = false;
    }
}
