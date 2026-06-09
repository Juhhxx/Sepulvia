using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class HazardController : RandomBehaviour, IPausable
{
    [SerializeField] private ParticleSystem _hazardEffect;
    [SerializeField] private Collider _hazardCollider;
    [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 _effectIntervalRange = new Vector2(2f, 5f);
    [SerializeField] private bool _alwaysActive = true;
    [SerializeField, HideIf("_alwaysActive")] private bool _hideInactive = false;

    private DungeonManager _dungeonManager;
    private RoomManager _roomManager;

    private void Awake()
    {
        _dungeonManager = FindAnyObjectByType<DungeonManager>();
        _roomManager = FindAnyObjectByType<RoomManager>();
    }

    private void Start()
    {
        _roomManager.OnRoomLoaded += () => {
            if (!_alwaysActive && _hideInactive && !_dungeonManager.CoreTaken) gameObject.SetActive(false);
            else gameObject.SetActive(true);

            Debug.Log("Room Loaded, Hazard Active: " + gameObject.activeSelf);
        };
    }

    private void OnEnable()
    {
        PauseManager.Instance.RegisterPausable(this);
        TryInitializeRandom();

        _hazardCollider.enabled = false;

        if (!_alwaysActive && _dungeonManager.CoreTaken) StartCoroutine(PlayHazardCR());
    }

    private void OnDisable()
    {
        PauseManager.Instance.UnregisterPausable(this);
    }

    private IEnumerator PlayHazardCR()
    {
        yield return new WaitUntil(() => _random != null);
        while (true)
        {
            float t = (float)_random.NextDouble();
            float waitTime = Mathf.Lerp(_effectIntervalRange.x, _effectIntervalRange.y, t);

            yield return new WaitForSeconds(waitTime);

            _hazardEffect.Play();
            _hazardCollider.enabled = true;

            yield return new WaitForSeconds(_hazardEffect.main.duration);

            _hazardCollider.enabled = false;
        }
    }

    // Pausing Logic
    public bool Paused { get; private set; }

    public void TogglePause(bool onOff)
    {
        Paused = onOff;

        if (!onOff)
        {
            StartCoroutine(PlayHazardCR());

            if (_hazardEffect.isPaused)
            {
                _hazardEffect.Play();
            }
        }
        else
        {
            StopAllCoroutines();
            if (_hazardEffect.isPlaying)
            {
                _hazardEffect.Pause();
            }
        }
    }
}
