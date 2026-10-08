using UnityEngine;

public class BattlerVFX : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Timer _destroyTimer;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystem != null)
        {
            _destroyTimer = new Timer(_particleSystem.main.duration + 1f);
            _destroyTimer.OnTimerDone += DestroyVFX;
        }
    }

    private void Start()
    {
        _particleSystem.Play();
    }

    private void Update()
    {
        _destroyTimer?.CountTimer();
    }

    private void DestroyVFX()
    {
        Destroy(gameObject);
    }
}
