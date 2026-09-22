using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetMinigameUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _hitZoneIndicator;
    [SerializeField] private Image _roundIndicatorImagePrefab;
    [SerializeField] private Transform _roundIndicatorParent;
    private List<Image> _roundIndicatorList = new List<Image>();

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();

        _minigameController.OnMinigameStart += SpawnRoundIndicators;
        _minigameController.OnMinigameStart += (a) => SetUpHitZoneIndicator(_minigameController.SuccessRange);
        _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;
    }

    private void SetUpHitZoneIndicator(Vector2 successRange)
    {
        RectTransform sliderRT = _slider.GetComponent<RectTransform>();
        RectTransform indicatorRT = _hitZoneIndicator.GetComponent<RectTransform>();

        float sliderWidth = sliderRT.rect.width;
        float indicatorWidth = sliderWidth * (successRange.y - successRange.x);
        float indicatorPosX = sliderWidth * (successRange.x + ((successRange.y - successRange.x) / 2));

        indicatorRT.sizeDelta = new Vector2(indicatorWidth, indicatorRT.sizeDelta.y);
        indicatorRT.anchoredPosition = new Vector2(indicatorPosX, indicatorRT.anchoredPosition.y);
    }

    private void SpawnRoundIndicators(int rounds)
    {
        for (int i = 0; i < rounds; i++)
        {
            Image indicator = Instantiate(_roundIndicatorImagePrefab, _roundIndicatorParent);

            _roundIndicatorList.Add(indicator);
        }
    }

    private void UpdateRoundIndicator(int round, bool win)
    {
        _roundIndicatorList[round].color = win ? Color.green : Color.red;
    }

    private void Update()
    {
        _slider.value = _minigameController.CurrentPoint;
    }

}
