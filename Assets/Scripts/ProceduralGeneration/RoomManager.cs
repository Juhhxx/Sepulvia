using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomManager : MonoBehaviourSingleton<RoomManager>
{
    [SerializeField] private EnemySpawnManager _enemySpawner;
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private RoomInstance _currentRoom;
    private Animator _fadeAnimator;
    private PlayerController _playerController;

    private List<RoomNode> _loadedRooms = new List<RoomNode>();
    private List<RoomInstance> _instantiatedRooms = new List<RoomInstance>();

    public event Action OnRoomLoaded;

    private void Awake()
    {
        base.SingletonCheck(this, false);
        _fadeAnimator = GetComponentInChildren<Animator>();
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    public void EnterDoor(RoomSide doorId)
    {
        var connection = _currentRoom.RoomNode.Connections
            .Find(c => c.DoorId == doorId);

        if (connection == null)
        {
            Debug.LogWarning("No connection for door: " + doorId);
            return;
        }

        StartCoroutine(LoadRoomCR(connection.TargetRoom, connection.TargetDoorId));
    }

    public void LoadFirstRoom(RoomNode room)
    {
        LoadRoom(room);

        _navMeshSurface.BuildNavMesh();
    }

    private bool _loadingRoom = false;
    public void LoadRoom(RoomNode roomNode, RoomSide targetId = RoomSide.None)
    {
        _loadingRoom = true;

        if (_currentRoom != null)
            _currentRoom.gameObject.SetActive(false);
        
        if (_loadedRooms.Contains(roomNode))
        {
            foreach (RoomInstance ri in _instantiatedRooms)
            {
                if (ri.RoomNode == roomNode)
                {
                    ri.gameObject.SetActive(true);
                    _currentRoom = ri;
                    break;
                }
            }
        }
        else
        {
            GameObject roomGO = Instantiate(roomNode.RoomData.RoomPrefab, transform);
            _currentRoom = roomGO.GetComponent<RoomInstance>();
            _currentRoom.SetUp(roomNode);

            _loadedRooms.Add(roomNode);
            _instantiatedRooms.Add(_currentRoom);
        }

        _currentRoom.RoomNode = roomNode;

        if (targetId == RoomSide.None)
            _playerController.transform.position = _currentRoom.Spawnposition.position;
        else
        {
            Transform spawn = _currentRoom.GetDoor(targetId).Spawnposition;
            _playerController.transform.position = spawn.position;
        }

        _loadingRoom = false;
    }

    private IEnumerator LoadRoomCR(RoomNode roomNode, RoomSide targetId)
    {
        _fadeAnimator.SetTrigger("Fade");

        yield return new WaitForSeconds(1f);

        LoadRoom(roomNode, targetId);

        yield return new WaitUntil(() => !_loadingRoom);
        
        SpawnEnemies(_currentRoom);

        OnRoomLoaded?.Invoke();

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
                int minEnemies = roomInstance.RoomNode.RoomData.RoomType == RoomType.AmbushRoom ? 1 : 0;
                
                enemyDatas = _enemySpawner.GenerateEnemies(paths, paths.Count, roomInstance.Random, minEnemies);

                roomInstance.SetRoomEnemyDatas(enemyDatas);
            }

            _enemySpawner.SpawnEnemies(enemyDatas, roomInstance.EnemyParent);
        }
    }
}
