using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetSliderUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderHandle;
    [SerializeField] private Sprite _sliderNormalSpr;
    [SerializeField] private Sprite _sliderGraceSpr;
    [SerializeField] private Sprite _sliderHitSpr;
    [SerializeField] private Image _hitZoneIndicator;
    [SerializeField] private Image _graceZoneIndicator;
    [SerializeField] private ChainUIController _chainUI;

    [SerializeField] private GameObject _leftChain;
    [SerializeField] private GameObject _rightChain;
    [SerializeField] private Image _chain;
    [SerializeField] private Color _backgroundChainColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [SerializeField] private Material chainMaterial;
    [SerializeField] private Material outlinedChainMaterial;

    [SerializeField] private Image leftSegmentToBreak;
    [SerializeField] private Image rightSegmentToBreak;

    [SerializeField] private RectTransform _particleParent;
    [SerializeField] private ParticleSystem _hitPerfectVFX;
    [SerializeField] private ParticleSystem _hitOkVFX;
    [SerializeField] private ParticleSystem _hitMissVFX;
    
    void Start()
    {
        SetupJiggle();
    }

    public void SetUpHitZoneIndicator(Vector2 successRange, Vector2 graceRange)
    {
        // Update color representation (hiddden in game for debug purposes only)
        RectTransform sliderRT = _slider.GetComponent<RectTransform>();
        RectTransform indicatorRT = _hitZoneIndicator.GetComponent<RectTransform>();
        RectTransform graceIndicatorRT = _graceZoneIndicator.GetComponent<RectTransform>();

        float sliderWidth = sliderRT.rect.width;
        float indicatorWidth = sliderWidth * (successRange.y - successRange.x);
        float indicatorPosX = sliderWidth * (successRange.x + ((successRange.y - successRange.x) / 2));
        float graceIndicatorWidth = sliderWidth * (graceRange.y - graceRange.x);

        indicatorRT.sizeDelta = new Vector2(indicatorWidth, indicatorRT.sizeDelta.y);
        indicatorRT.anchoredPosition = new Vector2(indicatorPosX, indicatorRT.anchoredPosition.y);

        graceIndicatorRT.sizeDelta = new Vector2(graceIndicatorWidth, graceIndicatorRT.sizeDelta.y);
        graceIndicatorRT.anchoredPosition = new Vector2(indicatorPosX, graceIndicatorRT.anchoredPosition.y);

        // Update Visual Representation in Chains
        _chainUI.UpdateHitPosition((successRange.x + successRange.y)/2f);
    }

    public void UpdateSliderSprite(float pos, Vector2 successRange, Vector2 graceRange)
    {
        if (pos >= successRange.x && pos <= successRange.y)
        {
            _sliderHandle.sprite = _sliderHitSpr;
        }
        else if (pos >= graceRange.x && pos <= graceRange.y)
        {
            _sliderHandle.sprite = _sliderGraceSpr;
        }
        else
        {
            _sliderHandle.sprite = _sliderNormalSpr;
        }
    }

    public void UpdatePositionRotation(float pos, float rotation)
    {
        _chainUI.UpdateRotation(rotation);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, 0);
    }

    public void ToggleActivated(bool on)
    {   
        // Get the left and right chain sections
        Image[] leftChain = _leftChain.GetComponentsInChildren<Image>();
        Image[] rightChain = _rightChain.GetComponentsInChildren<Image>();

        // Change the chain colors, depending on the active state
        ChangeChainSectionColor(leftChain, on);
        ChangeChainSectionColor(rightChain, on);

        // Change the materials of the chain segment to break, depending on the active state
        ChangeChainMaterial(leftSegmentToBreak, on);
        ChangeChainMaterial(rightSegmentToBreak, on);
        
        //_canvasGroup.alpha = on ? 1f : 0.5f;
        _sliderHandle.enabled = on;
        if (on) transform.SetAsLastSibling();
    }

    private void ChangeChainSectionColor(Image[] chainSection, bool isActive)
    {   
        // For each segment in the chain section
        for(int i = 0; i < chainSection.Length; i++)
        {   
            Image child = chainSection[i];

            // Change color of the current segment depending on isActive
            child.color = isActive ? Color.white : _backgroundChainColor;
        } 
    }

    private void ChangeChainMaterial(Image chainImage, bool isOutline)
    {   
        Material newChainMaterial;

        if(isOutline)
        {   
            // Create a copy of the outline material and assign it to the chain
            newChainMaterial = new Material(outlinedChainMaterial);
            chainImage.material = newChainMaterial;
        }
        else
        {
            // Create a copy of the default chain material, initialize it and assign it to the chain
            newChainMaterial = new Material(chainMaterial);
            newChainMaterial.SetFloat("_Offset", Random.Range(0f,1f));
            chainImage.material = newChainMaterial;            
        }
    }

    private void SetupJiggle()
    {   
        foreach (Image child in _leftChain.GetComponentsInChildren<Image>())
        {   
            // Initialize chain material as a default (false)
            ChangeChainMaterial(child, false);
        }
        foreach (Image child in _rightChain.GetComponentsInChildren<Image>())
        {
            // Initialize chain material as a default (false)
            ChangeChainMaterial(child, false);
        }
    }

    public void UpdateSlider(float value)
    {
        _slider.value = value;
    }

    public void DoSpawnParticles(float successRate)
    {
        if (successRate == 1f)
        {
            if (_hitPerfectVFX != null) Instantiate(_hitPerfectVFX, _particleParent);
        }
        else if (successRate == 0.5f)
        {
            if (_hitOkVFX != null) Instantiate(_hitOkVFX, _particleParent);
        }
        else
        {
            if (_hitMissVFX != null) Instantiate(_hitMissVFX, _particleParent);
        }
    }
}
