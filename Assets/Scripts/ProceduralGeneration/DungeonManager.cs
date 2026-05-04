using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private DungeonGenerator _dungeonGenerator;

    private void Start()
    {
        RoomNode startRoom = _dungeonGenerator.Generate();
        _dungeonGenerator.ShowGeneratedDungeon(startRoom);

        RoomManager.Instance.LoadFirstRoom(startRoom);
    }
}
