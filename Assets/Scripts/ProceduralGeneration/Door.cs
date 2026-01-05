using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private RoomId doorId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null) RoomManager.Instance.EnterDoor(doorId); 
    }
}
