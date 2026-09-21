using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetMinigameUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _roundIndicatorImagePrefab;
    [SerializeField] private Transform _roundIndicatorParent;
    private List<Image> _roundIndicatorList = new List<Image>();

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();

        _minigameController.OnMinigameStart += SpawnRoundIndicators;
        _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;
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
