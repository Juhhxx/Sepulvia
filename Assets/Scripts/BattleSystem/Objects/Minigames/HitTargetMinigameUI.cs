using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HitTargetMinigameUI : MonoBehaviour
{
    [SerializeField] private Transform _sliderParent;
    [SerializeField] private HitTargetSliderUI _sliderPrefab;
    private List<HitTargetSliderUI> _sliders = new List<HitTargetSliderUI>();

    [SerializeField] private Image _roundIndicatorImagePrefab;
    [SerializeField] private Transform _roundIndicatorParent;
    private List<Image> _roundIndicatorList = new List<Image>();

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();
        
        _minigameController.OnMinigameStart += SpawnRoundIndicators;
        _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;

        _minigameController.OnMinigameStart += SetUpSliders;
    }

    private void SetUpSliders(int rounds)
    {
        var takenPositions = new float[rounds];
        var takenRotations = new float[rounds];

        for (int i = 0; i < rounds; i++)
        {
            HitTargetSliderUI slider = Instantiate(_sliderPrefab, _sliderParent);

            _sliders.Add(slider);
            slider.SetUpHitZoneIndicator(_minigameController.SuccessRange[i],
                                        _minigameController.GraceRange[i]);

            float posOffset = PositionSlider(takenPositions);
            float rot = RotateSlider(takenRotations);

            takenPositions[i] = posOffset;
            takenRotations[i] = rot;
            
            slider.UpdatePositionRotation(posOffset, rot);
        }
    }

    private float PositionSlider(float[] taken)
    {
        float posOffset = Random.Range(-200, 201);

        if (taken.Contains(posOffset) || !taken.Any(p => Mathf.Abs(p - posOffset) < 20))
        {
            return PositionSlider(taken);
        }
        else return posOffset;
    }

    private float RotateSlider(float[] taken)
    {
        float rot = Random.Range(0, 180);
        
        if (taken.Contains(rot) || taken.Any(r => Mathf.Abs(Mathf.DeltaAngle(r, rot)) < 25))
        {
            return RotateSlider(taken);
        }
        else return rot;
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
        for (int i = 0; i < _sliders.Count; i++)
        {
            _sliders[i].UpdateSlider(_minigameController.CurrentPoint[i]);
        }
    }

}
