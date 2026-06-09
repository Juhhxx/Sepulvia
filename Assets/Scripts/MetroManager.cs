using System.Collections;
using System.Linq;
using UnityEngine;

public class MetroManager : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isFueld = false;
    [SerializeField] private ItemInfo _fuelitem;
    private PlayerOverworldUI _overworldUI;
    
    private PlayerController _player;
    private OutlineManager _outline;
    private DungeonManager _dungeonManager;

    private void Start()
    {
        CanInteract = true;

        _player = FindAnyObjectByType<PlayerController>();
        _dungeonManager = FindAnyObjectByType<DungeonManager>();

        _outline = GetComponent<OutlineManager>();

        _overworldUI = FindAnyObjectByType<PlayerOverworldUI>();
    }

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        if (_isFueld)
        {
            _overworldUI.AddScrollText("Going to other Station...");
            StopAllCoroutines();
            StartCoroutine(StationTravelCR());
        }
        else
        {
            if (_player.PlayerCharacter.Inventory.Contains(_fuelitem))
            {
                _player.PlayerCharacter.Inventory.RemoveItem(_fuelitem);
                _isFueld = true;
                _overworldUI.AddScrollText("Yay! You fueld up your train and can now travel to other stations!");
                return;
            }

            _overworldUI.AddScrollText("Your train is out of fuel! Find something to fuel it up");
        }
    }

    private IEnumerator StationTravelCR()
    {
        PauseManager.Instance.Pause();

        yield return new WaitForSeconds(1.5f);

        _dungeonManager.CreateDungeon();

        PauseManager.Instance.UnPause();
    }

    public void ToggleSelected(bool onOff)
    {
        _outline.ToggleOutline(onOff);
    }
}
