using UnityEngine;
using UnityEngine.UI;

public class HitTargetSliderUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
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

    public void UpdatePositionRotation(float pos, float rotation)
    {
        _chainUI.UpdateRotation(rotation);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, 0);
    }

    public void UpdateSlider(float value)
    {
        _slider.value = value;
    }
}
