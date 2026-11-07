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
        _canvasGroup.alpha = on ? 1f : 0.5f;
        _sliderHandle.enabled = on;
        if (on) transform.SetAsLastSibling();
    }

    public void UpdateSlider(float value)
    {
        _slider.value = value;
    }
}
