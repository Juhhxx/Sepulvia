using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BattleMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip _battleClip;           // The battle music clip
    [SerializeField] private float _fadeDuration = 1.0f;      // Fade in/out duration in seconds
    [SerializeField] private float _maxVolume;     
    private AudioSource _audioSource;
    private bool _isFading = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;           // Keep looping the music
        _audioSource.volume = 0f;           // Start silent
    }

    void Start()
    {
        if (_battleClip != null)
        {
            _audioSource.clip = _battleClip;
            _audioSource.Play();
            StartCoroutine(FadeInAudio(_fadeDuration));
        }
        else
        {
            Debug.LogWarning("No _battleClip assigned to AutoBattleMusic on " + gameObject.name);
        }
    }
    // Call this to start the battle music
    public void StartBattleMusic()
    {
        if (_battleClip == null) return;
        _audioSource.clip = _battleClip;
        _audioSource.Play();
        StopAllCoroutines();
        StartCoroutine(FadeInAudio(_fadeDuration));
    }

    // Call this to stop the battle music
    public void StopBattleMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAudio(_fadeDuration));
    }

    private System.Collections.IEnumerator FadeInAudio(float duration)
    {
        _isFading = true;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _audioSource.volume = Mathf.Clamp01(currentTime / duration) * _maxVolume;
            yield return null;
        }
        _audioSource.volume = _maxVolume;
        _isFading = false;
    }

    private System.Collections.IEnumerator FadeOutAudio(float duration)
    {
        _isFading = true;
        float startVolume = _audioSource.volume;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _audioSource.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, 0f, currentTime / duration));
            yield return null;
        }
        _audioSource.volume = 0f;
        _audioSource.Stop();
        _isFading = false;
    }
}

