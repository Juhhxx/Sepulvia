using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetMinigameUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _hitZoneIndicator;
    [SerializeField] private Image _graceZoneIndicator;
    [SerializeField] private Image _roundIndicatorImagePrefab;
    [SerializeField] private Transform _roundIndicatorParent;
    private List<Image> _roundIndicatorList = new List<Image>();

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();
        
        _minigameController.OnMinigameStart += SpawnRoundIndicators;
        _minigameController.OnMinigameStart += (a) => SetUpHitZoneIndicator(_minigameController.SuccessRange, _minigameController.GraceRange);
        _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;
    }

    private void SetUpHitZoneIndicator(Vector2 successRange, Vector2 graceRange)
    {
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
        indicatorRT.anchoredPosition = new Vector2(indicatorPosX, graceIndicatorRT.anchoredPosition.y);
    }

    private void SpawnRoundIndicators(int rounds)
    {
        for (int i = 0; i < rounds; i++)
        {
            Image indicator = Instantiate(_roundIndicatorImagePrefab, _roundIndicatorParent);

            _roundIndicatorList.Add(indicator);
        }
    }

    private void UpdateRoundIndicator(int round, float result)
    {
        Color color;

        if (result == 1f) color = Color.green;
        else if (result == 0.5f) color = Color.yellow;
        else color = Color.red;

        _roundIndicatorList[round].color = color;
    }

    private void Update()
    {
        _slider.value = _minigameController.CurrentPoint;
    }

}
