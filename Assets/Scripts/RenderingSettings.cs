using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RenderingSettings : MonoBehaviour
{
    Volume[] _renderingVolumes;
    List<LiftGammaGain> _lggs = new List<LiftGammaGain>();
    private const string GAMMALEVEL = "GammeLevel";

    [SerializeField] private Button _resetButton;
    [SerializeField] private Slider _gammaSlider;
    [SerializeField, ReadOnly] private float _gamma;
    public float Gamma
    {
        get => _gamma;

        set
        {
            if (value != _gamma)
            {
                UpdateGammas(value);
                PlayerPrefs.SetFloat(GAMMALEVEL, value);
                PlayerPrefs.Save();
            }

            _gamma = value;
        }
    }

    private void Start()
    {
        FindVolume();

        SceneManager.activeSceneChanged += (a, b) => FindVolume();
        _gammaSlider.onValueChanged.AddListener(ChangeGamma);
        _resetButton.onClick.AddListener(ResetValues);
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= (a, b) => FindVolume();
    }

    public void FindVolume()
    {
        _renderingVolumes = FindObjectsByType<Volume>(0);

        GetOverrides();
        
        if (PlayerPrefs.HasKey(GAMMALEVEL))
        {
            Gamma = PlayerPrefs.GetFloat(GAMMALEVEL);
        }
        else
        {
            Gamma = 0;
        }

        _gammaSlider.value = Gamma;
    }

    private void GetOverrides()
    {
        _lggs = new List<LiftGammaGain>();

        foreach (Volume v in _renderingVolumes)
        {
            LiftGammaGain lgg;

            v.profile.TryGet(out lgg);
            _lggs.Add(lgg);
        }
    }

    private void UpdateGammas(float gamma)
    {
        foreach (LiftGammaGain lgg in _lggs)
        {
            UpdateGamma(lgg, gamma);
        }
    }
    private void UpdateGamma(LiftGammaGain lgg, float gamma)
    {
        Vector4 gammaVec = lgg.gamma.value;
        gammaVec.w = gamma;

        lgg.gamma.Override(gammaVec);
    }

    public void ChangeGamma(float gamma)
    {
        Gamma = gamma;
    }

    [Button(enabledMode: EButtonEnableMode.Always)]
    public void ResetValues()
    {
        Gamma = 0;
        if (_gammaSlider != null) _gammaSlider.value = Gamma;
    }
}
