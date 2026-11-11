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
        
        _minigameController.OnMinigameStart         += SetUpSliders;
        _minigameController.OnMinigameRoundStart    += UpdateSliderActivated;
        _minigameController.OnMinigameRoundEnd      += SpawnResultParticles;

        // _minigameController.OnMinigameStart += SpawnRoundIndicators;
        // _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;

    }

    // Slider set up
    private void SetUpSliders(int rounds)
    {
        var takenPositions = new float[rounds];
        var takenRotations = new float[rounds];

        for (int i = 0; i < rounds; i++)
        {
            HitTargetSliderUI slider = Instantiate(_sliderPrefab, _sliderParent);

            slider.transform.SetAsFirstSibling();

            _sliders.Add(slider);
            slider.SetUpHitZoneIndicator(_minigameController.SuccessRange[i],
                                        _minigameController.GraceRange[i]);

            float posOffset = PositionSlider(takenPositions);
            float rot = RotateSlider(takenRotations, rounds);

            takenPositions[i] = posOffset;
            takenRotations[i] = rot;
            
            slider.UpdatePositionRotation(posOffset, rot);

            slider.GetComponent<ChainUIController>().DoSpawnComeIn();
        }
        CameraEffectsUtility.DoCameraShake(.7f, .2f, 1f); //Shake at the end of all 3 chains instanced
    }

    private float PositionSlider(float[] taken, int iteration = 0)
    {
        float directionOffset = Random.Range(0f, 1f);
        float posOffset = Random.Range(20, 301);
        
        if (directionOffset < 0.5f) posOffset = -posOffset;

        if (taken.Contains(posOffset) || taken.Any(p => Mathf.Abs(p - posOffset) < 35) && iteration < 20)
        {
            return PositionSlider(taken, iteration + 1);
        }
        else return posOffset;
    }

    private float RotateSlider(float[] taken, int rounds, int iteration = 0)
    {
        float directionOffset = Random.Range(0f, 1f);
        float rot = Random.Range(0, 15 * rounds);
        
        if (directionOffset < 0.5f) rot = -rot;

        if (taken.Contains(rot) || taken.Any(r => Mathf.Abs(Mathf.DeltaAngle(r, rot)) < 10) && iteration < 20)
        {
            return RotateSlider(taken, rounds,iteration + 1);

        }
        else return rot;
    }

    // Slider methods
    private void UpdateSliderActivated(int round)
    {
        for (int i = 0; i < _sliders.Count; i++)
        {
            _sliders[i].ToggleActivated(i == round);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _sliders.Count; i++)
        {
            _sliders[i].UpdateSlider(_minigameController.CurrentPoint[i]);
            
            _sliders[i].UpdateSliderSprite(_minigameController.CurrentPoint[i],
                                            _minigameController.SuccessRange[i],
                                            _minigameController.GraceRange[i]);
        }
    }

    // Particle Effects
    private void SpawnResultParticles(int round, float successRate)
    {
        _sliders[round].DoSpawnParticles(successRate);
    }

    // Round Indicators
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
        
        if (result == 1f)
        {
           color = Color.green; 
        }
        else if (result == 0.5f) 
        {
            color = Color.yellow;
        }
        else
        {
            color = Color.red; 
        } 

        _roundIndicatorList[round].color = color;
    }

}
