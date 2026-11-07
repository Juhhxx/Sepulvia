using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetMinigameSpiralUI : MonoBehaviour
{
    [SerializeField] private HitTargetSpiralUI _spiralPrefab;
    [SerializeField] private RectTransform _spawningArea;
    private HitTargetMinigame _minigameController;
    private List<HitTargetSpiralUI> _spirals = new List<HitTargetSpiralUI>();
    private List<RectTransform> _spiralTransforms = new List<RectTransform>();

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();

        _minigameController.OnMinigameRoundStart += (round) => SetUpSpiral(round);
    }

    private void SetUpSpiral(int round)
    {
        var spiral = Instantiate(_spiralPrefab, _spawningArea);
        var rectT = spiral.GetComponent<RectTransform>();

        _spirals.Add(spiral);
        _spiralTransforms.Add(rectT);

        PositionSpiral(rectT);

        _spiralPrefab.SetUpHitIndicator(_minigameController.SuccessRange[round]);
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
}
