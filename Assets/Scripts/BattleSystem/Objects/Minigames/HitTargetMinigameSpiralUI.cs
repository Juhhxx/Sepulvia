using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitTargetMinigameSpiralUI : MonoBehaviour
{
    [SerializeField] private HitTargetSpiralUI _spiralPrefab;
    [SerializeField] private RectTransform _spawningArea;
    [SerializeField] private RawImage _vignetteImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] float _vignetteTransitionDuration = 1f;
    [SerializeField] private float _backgroundTargetOpacity = .8f;
    private Material _vignetteMaterial;
    private HitTargetMinigame _minigameController;
    private List<HitTargetSpiralUI> _spirals = new List<HitTargetSpiralUI>();
    private List<RectTransform> _spiralTransforms = new List<RectTransform>();

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();

        _minigameController.OnMinigameRoundStart += (round) => SetUpSpiral(round);

        _vignetteMaterial = new Material(_vignetteImage.material);
    }

    private void SetUpSpiral(int round)
    {
        var spiral = Instantiate(_spiralPrefab, _spawningArea);
        var rectT = spiral.GetComponent<RectTransform>();

        _spirals.Add(spiral);
        _spiralTransforms.Add(rectT);

        PositionSpiral(rectT);

        _spiralPrefab.SetUpHitIndicator(_minigameController.SuccessRange[round]);
        _spiralPrefab.DoSpawnSpiral();
    }

    private int maxPosInterations = 20;
    private void PositionSpiral(RectTransform spiral, int iteration = 0)
    {
        float posX = Random.Range(_spawningArea.rect.xMin, _spawningArea.rect.xMax);
        float posY = Random.Range(_spawningArea.rect.yMin, _spawningArea.rect.yMax);

        Vector2 newPos = new Vector2(posX, posY);
        spiral.anchoredPosition = newPos;

        if (CheckOverlap(spiral) && iteration < maxPosInterations)
        {
            PositionSpiral(spiral, iteration + 1);
        }
    }

    private bool CheckOverlap(RectTransform spiral)
    {
        foreach (var s in _spiralTransforms)
        {
            if (spiral != s && Vector2.Distance(spiral.anchoredPosition, s.anchoredPosition) < spiral.rect.size.x)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        for (int i = 0; i < _spirals.Count; i++)
        {
            _spirals[i].UpdateSpiral(_minigameController.CurrentPoint[i]);
        }
    }

    [Button]
    public void DoInitiateMaelStorm1Effect()
    {
        _vignetteMaterial.DOFloat(0.65f, "_Vignette_Size", _vignetteTransitionDuration).SetEase(Ease.Linear);
        _vignetteImage.material = _vignetteMaterial;

        _backgroundImage.DOFade(_backgroundTargetOpacity, _vignetteTransitionDuration).SetEase(Ease.Linear);
    }

    [Button]
    public void DoKillMaelstorm1Effect()
    {
        _vignetteMaterial.DOFloat(1.5f, "_Vignette_Size", _vignetteTransitionDuration).SetEase(Ease.Linear);
        _vignetteImage.material = _vignetteMaterial;

        _backgroundImage.DOFade(0f, _vignetteTransitionDuration).SetEase(Ease.Linear);

    }
}
