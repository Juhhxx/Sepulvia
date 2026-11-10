using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HitTargetMinigameUI : MonoBehaviour
{
    [SerializeField] private Transform _sliderParent;
    [SerializeField] private HitTargetSliderUI _sliderPrefab;
    [SerializeField] private Canvas _minigameCanvas;
    private List<HitTargetSliderUI> _sliders = new List<HitTargetSliderUI>();

    [SerializeField] private Image _roundIndicatorImagePrefab;
    [SerializeField] private Transform _roundIndicatorParent;
    [SerializeField] private ParticleSystem _hitPerfectVFX;
    [SerializeField] private ParticleSystem _hitOkVFX;
    [SerializeField] private ParticleSystem _hitMissVFX;
    private List<Image> _roundIndicatorList = new List<Image>();

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();
        
        _minigameController.OnMinigameStart += SpawnRoundIndicators;
        _minigameController.OnMinigameRoundStart += UpdateSliderActivated;
        _minigameController.OnMinigameRoundEnd += UpdateRoundIndicator;

        _minigameController.OnMinigameStart += SetUpSliders;
    }

    // Slider set up
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
        ParticleSystem ps;
        Vector3 sliderPosition = _sliderParent.position;
        //Add VFX play for miss, whiff and success here
        if (result == 1f)
        {
           color = Color.green; 
           ps = Instantiate(_hitPerfectVFX, _minigameCanvas.transform.position, Quaternion.identity); 
           ps.transform.parent = _minigameCanvas.transform;
        }
        else if (result == 0.5f) 
        {
            color = Color.yellow;
            ps = Instantiate(_hitOkVFX, _sliderParent.position, Quaternion.identity); 
            ps.transform.parent = _minigameCanvas.transform;
        }
        else
        {
            color = Color.red; 
            ps = Instantiate(_hitMissVFX, _sliderParent.position, Quaternion.identity);
            ps.transform.parent = _minigameCanvas.transform;
        } 

        _roundIndicatorList[round].color = color;
    }

}
