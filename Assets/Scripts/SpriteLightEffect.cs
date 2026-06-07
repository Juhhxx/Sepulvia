using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class SpriteLightEffect : MonoBehaviour
{
    [SerializeField, Range(0, 2)] private float _intensity = 1f;
    [SerializeField] private float _maxRange = 15f;
    [SerializeField] private float _colorRange = 20f;

    private SpriteRenderer _spriteRenderer;
    [SerializeField, ReadOnly] private Color _originalColor;
    [SerializeField, ReadOnly] private Color _medianColor;

    private List<Light> _nearbyLights = new List<Light>();

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    private void FindLights()
    {
        var lightsTemp = FindObjectsByType<Light>(0);

        Debug.Log($"Found {lightsTemp.Length} lights in the scene.");

        foreach (Light light in lightsTemp)
        {
            if (Vector3.Distance(transform.position, light.transform.position) <= _maxRange)
            {
                if (!_nearbyLights.Contains(light))
                {
                    AddLight(light);
                }
            }
            else if (_nearbyLights.Contains(light))
            {
                RemoveLight(light);
            }
        }

        Debug.Log($"There are {_nearbyLights.Count} nearby lights.");
    }

    private void AddLight(Light light)
    {
        _nearbyLights.Add(light);
    }

    private void RemoveLight(Light light)
    {
        _nearbyLights.Remove(light);
    }

    private Color CalculateMedianColor()
    {
        int length = _nearbyLights.Count + 1;

        float r = 1, g = 1, b = 1;

        foreach (Light light in _nearbyLights)
        {
            r += light.color.r * (1 - (Vector3.Distance(transform.position, light.transform.position) / _colorRange));
            g += light.color.g * (1 - (Vector3.Distance(transform.position, light.transform.position) / _colorRange));
            b += light.color.b * (1 - (Vector3.Distance(transform.position, light.transform.position) / _colorRange));
        }

        _medianColor = new Color(r / length, g / length, b / length, 1);

        return _medianColor;
    }

    private void ApplyLightEffect()
    {
        _spriteRenderer.DOKill();

        if (_nearbyLights.Count == 0)
        {
            _spriteRenderer.DOColor(_originalColor, 0.5f);
            return;
        }

        Debug.Log($"Applying light effect with median color: {CalculateMedianColor()} and intensity: {_intensity}");

        Color targetColor = Color.Lerp(_originalColor, CalculateMedianColor(), _intensity);

        _spriteRenderer.DOColor(targetColor, 0.5f);
    }

    private void LateUpdate()
    {
        FindLights();
        ApplyLightEffect();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxRange);
    }
}
