using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomManager : MonoBehaviourSingleton<RoomManager>
{
    [SerializeField] private RoomData _startingRoom;
    [SerializeField] private EnemySpawnManager _enemySpawner;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private RoomInstance _currentRoom;
    private Animator _fadeAnimator;
    private PlayerController _playerController;

    public struct RoomTicket
    {
        public RoomData RoomData;
        public GameObject RoomGO;
    }
    private List<RoomData> _loadedRooms = new List<RoomData>();
    private List<RoomInstance> _instantiatedRooms = new List<RoomInstance>();

    private void Awake()
    {
        base.SingletonCheck(this, false);
    }

    private void Start()
    {
        _fadeAnimator = GetComponentInChildren<Animator>();
        _playerController = FindAnyObjectByType<PlayerController>();

        LoadRoom(_startingRoom);

        _navMeshSurface.BuildNavMesh();
    }

    public void EnterDoor(RoomId doorId)
    {
        var connection = _currentRoom.RoomData.Connections
            .Find(c => c.doorId == doorId);

        if (connection == null)
        {
            Debug.LogWarning("No connection for door: " + doorId);
            return;
        }

        StartCoroutine(LoadRoomCR(connection.targetRoom, connection.targetDoorId));
    }

    private void LoadRoom(RoomData roomData, RoomId targetId = RoomId.None)
    {
        if (_currentRoom != null)
            _currentRoom.gameObject.SetActive(false);
        
        if (_loadedRooms.Contains(roomData))
        {
            foreach (RoomInstance ri in _instantiatedRooms)
            {
                if (ri.RoomData == roomData)
                {
                    ri.gameObject.SetActive(true);
                    _currentRoom = ri;
                    break;
                }
            }
        }
        else
        {
            GameObject roomGO = Instantiate(roomData.RoomPrefab, transform);
            _currentRoom = roomGO.GetComponent<RoomInstance>();
            _currentRoom.SetUp(roomData);

            _loadedRooms.Add(roomData);
            _instantiatedRooms.Add(_currentRoom);
        }

        _currentRoom.RoomData = roomData;

        SpawnEnemies(_currentRoom);

        if (targetId == RoomId.None)
            _playerController.transform.position = _currentRoom.Spawnposition.position;
        else
        {
            Transform spawn = _currentRoom.GetDoor(targetId).Spawnposition;
            _playerController.transform.position = spawn.position;
        }
    }

    private IEnumerator LoadRoomCR(RoomData roomData, RoomId targetId)
    {
        _fadeAnimator.SetTrigger("Fade");

        yield return new WaitForSeconds(1f);

        LoadRoom(roomData, targetId);

        yield return new WaitForEndOfFrame();

        _navMeshSurface.BuildNavMesh();
    }

    private void SpawnEnemies(RoomInstance roomInstance)
    {
        var paths = roomInstance.GetPaths();

        if (paths.Count > 0)
        {
            Debug.Log($"SPAWNING ENEMIES FOR ROOM {roomInstance}");

            var enemyDatas = roomInstance.GetRoomEnemyDatas();

            if (enemyDatas == null)
            {
                enemyDatas = _enemySpawner.GenerateEnemies(paths, paths.Count, roomInstance.Random);

                roomInstance.SetRoomEnemyDatas(enemyDatas);
            }

            _enemySpawner.SpawnEnemies(enemyDatas, roomInstance.EnemyParent);
        }
    }
}
