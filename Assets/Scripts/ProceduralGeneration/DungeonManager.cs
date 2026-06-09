using UnityEngine;
using NaughtyAttributes;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private DungeonGenerator _dungeonGenerator;

    private bool _coreTaken = false;
    public bool CoreTaken => _coreTaken;

    private int _currentDungeon = 0;

    public void SetCoreTaken()
    {
        _coreTaken = true;
    }

    private void Start()
    {
        CreateDungeon();
    }

    [Button]
    private void CreateDungeon()
    {
        _currentDungeon++;
        
        RoomNode startRoom = _dungeonGenerator.Generate();
        _dungeonGenerator.ShowGeneratedDungeon(startRoom);

        _coreTaken = false;

        RoomManager.Instance.LoadFirstRoom(startRoom);
    }
}
