using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomInstance : MonoBehaviour, IRandom
{
    [field: SerializeField] public RoomData RoomData { get; set; }
    [field: SerializeField] public Transform Spawnposition { get; private set; }
    [field: SerializeField] public Transform EnemyParent { get; private set; }

    private List<Door> _doorList;
    public Door GetDoor(RoomId id)
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

    private void Awake()
    {
        _doorList = GetComponentsInChildren<Door>().ToList();
        _pathList = GetComponentsInChildren<Path>().ToList();
    }
    public void SetUp(RoomData data)
    {
        SeedManager.Instance.RegisterRandom(this, transform.GetPath());
        gameObject.name = data.RoomName + "_" + _seed;
    }

    public System.Random Random { get; private set; }
    private int _seed;
    public void InitializeRandom(int seed)
    {
        _seed = seed;
        Random = new System.Random(seed);
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
