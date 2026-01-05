using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomManager : MonoBehaviourSingleton<RoomManager>
{
    [SerializeField] private RoomData startingRoom;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private RoomInstance currentRoom;
    private Animator _fadeAnimator;
    private PlayerController _playerController;

    private void Awake()
    {
        base.SingletonCheck(this, false);
    }

    private void Start()
    {
        _fadeAnimator = GetComponentInChildren<Animator>();
        _playerController = FindAnyObjectByType<PlayerController>();

        LoadRoom(startingRoom);

        _navMeshSurface.BuildNavMesh();
    }

    public void EnterDoor(RoomId doorId)
    {
        var connection = currentRoom.RoomData.connections
            .Find(c => c.doorId == doorId);

        if (connection == null)
        {
            Debug.LogWarning("No connection for door: " + doorId);
            return;
        }

        StartCoroutine(LoadRoomCR(connection.targetRoom));
    }

    private void LoadRoom(RoomData roomData)
    {
        if (currentRoom != null)
            Destroy(currentRoom.gameObject);

        GameObject roomGO = Instantiate(roomData.roomPrefab);
        currentRoom = roomGO.GetComponent<RoomInstance>();
        currentRoom.RoomData = roomData;
    }

    private IEnumerator LoadRoomCR(RoomData roomData)
    {
        _fadeAnimator.SetTrigger("Fade");

        yield return new WaitForSeconds(1f);

        _playerController.transform.position = Vector3.zero;

        LoadRoom(roomData);

        yield return new WaitForEndOfFrame();

        _navMeshSurface.BuildNavMesh();
    }
}
