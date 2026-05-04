using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomInstance : RandomBehaviour
{
    [field: SerializeField] public RoomNode RoomNode { get; set; }
    [field: SerializeField] public Transform Spawnposition { get; private set; }
    [field: SerializeField] public Transform EnemyParent { get; private set; }

    private List<Door> _doorList;
    public Door GetDoor(RoomSide id)
    {
        foreach (Door d in _doorList)
        {
           if (d.DoorId == id) return d; 
        }
        return null;
    }
    private List<Path> _pathList;
    public List<Path> GetPaths() => _pathList;

    private List<EnemyData> _roomEnemyDataList;
    public List<EnemyData> GetRoomEnemyDatas() => _roomEnemyDataList;
    public void SetRoomEnemyDatas(List<EnemyData> enemyDataList) => _roomEnemyDataList = enemyDataList;

    public System.Random Random => _random;

    private void Awake()
    {
        _doorList = GetComponentsInChildren<Door>().ToList();
        _pathList = GetComponentsInChildren<Path>().ToList();
    }

    public void SetUp(RoomNode data)
    {
        gameObject.name = data.RoomName;

        TryInitializeRandom(gameObject.name);
    }
}

public struct EnemyData
{
    public GameObject Enemy;
    public Path Path;

    public EnemyData(GameObject enemy, Path path)
    {
        Enemy = enemy;
        Path = path;
    }
}
