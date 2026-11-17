using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScreenWipe : MonoBehaviour
{
    [SerializeField] private string _wipeName;
    public string WipeName => _wipeName;
    
    [SerializeField] private Graphic _wipeGraphic;

    [SerializeField] private float _wipeDuration = 1f;
    [SerializeField] private float _wipeDelay = 1f;
    [SerializeField] private Ease _wipeEase = Ease.Linear;

    private Material _wipeMaterial;
    private Tween _wipeTween;
    private bool _isPlaying = false;
    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        // Copy wipe material to avoid modifying the original material
        _wipeMaterial = new Material(_wipeGraphic.material);
        _wipeGraphic.material = _wipeMaterial;

        // Set Wipe to 0
        _wipeMaterial.SetFloat("_WipeProgress", 0f);
        _wipeGraphic.raycastTarget = false;
    }

    private void PlayWipe(float targetProgress, Action onComplete = null)
    {
        // Stop any previous animation.
        _wipeTween?.Kill();

        _isPlaying = true;
        _wipeGraphic.raycastTarget = true;

        _wipeTween = _wipeMaterial
            .DOFloat(targetProgress, "_WipeProgress", _wipeDuration)
            .SetDelay(targetProgress == 0 ? _wipeDelay : 0f) // if wiping out apply delay to tween
            .SetEase(_wipeEase)
            .OnComplete(() =>
            {
                _isPlaying = false;
                _wipeGraphic.raycastTarget = false;
                _wipeTween = null;

                onComplete?.Invoke();
            })
            .OnKill(() =>
            {
                // A killed tween should no longer be considered active.
                _isPlaying = false;
            });
    }

    public void WipeIn(Action onComplete = null)
    {
        Debug.Log($"[ScreenWipe] Wiping in '{_wipeName}'");
        PlayWipe(1f, onComplete);
    }

    public void WipeOut(Action onComplete = null)
    {
        Debug.Log($"[ScreenWipe] Wiping out '{_wipeName}'");
        PlayWipe(0f, onComplete);
    }
}
