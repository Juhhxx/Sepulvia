using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class HazardController : MonoBehaviour, IPausable
{
    [SerializeField] private ParticleSystem _hazardEffect;
    [SerializeField] private Collider _hazardCollider;
    [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 _effectIntervalRange = new Vector2(2f, 5f);

    private void OnEnable()
    {
        _hazardCollider.enabled = false;
        StartCoroutine(PlayHazardCR());

        PauseManager.Instance.RegisterPausable(this);
    }

    private void OnDisable()
    {
        PauseManager.Instance.UnregisterPausable(this);
    }

    private IEnumerator PlayHazardCR()
    {
        while (true)
        {
            float waitTime = Random.Range(_effectIntervalRange.x, _effectIntervalRange.y);

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
