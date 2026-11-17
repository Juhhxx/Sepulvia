using UnityEngine;

public class RoomSwipeManager : MonoBehaviour
{
    [SerializeField] private ScreenWipeTrigger _swipeTrigger;
    [SerializeField] private RoomManager _roomManager;

    private void Start()
    {
        _roomManager.OnBeginRoomLoad += () => _swipeTrigger.TriggerWipe(true);
        _roomManager.OnRoomLoaded += () => _swipeTrigger.TriggerWipe(false);
    }
}
