using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

public class HitTargetSpiralUI : MonoBehaviour
{
    [SerializeField] private RectTransform _centerPivot;
    [SerializeField] private RectTransform _circleIndicator;
    [SerializeField] private Image _hitIndicator;
    [SerializeField] private float _spiralRadius;
    [SerializeField] private float _turns; // nubmer of turns in the spiral
    [SerializeField] private bool _showPossibleHits;

    [SerializeField] private Vector2 _successRange;
    [SerializeField] private Image _circleIndicatorImage;
    [SerializeField] private Image _hitIndicatorImage;
    [SerializeField] private float _circleIndicatorTransitionDuration = .3f;
    [SerializeField] private float _hitIndicatorTransitionDuration = .3f;
    private Material _circleBackgroundMaterial;
    private Material _hitIndicatorMaterial;



    private void Start()
    {
        _hitIndicator.rectTransform.anchoredPosition = Vector2.zero;

        _circleBackgroundMaterial = new Material(_circleIndicatorImage.material);

        _hitIndicatorMaterial = new Material(_hitIndicatorImage.material);
    }

    [Button]
    private void DoSpawnCircleIndicator()
    {
        _circleIndicatorImage.material = _circleBackgroundMaterial;
        _circleBackgroundMaterial.DOFloat(1f, "_Effect_Intensity", _circleIndicatorTransitionDuration).SetEase(Ease.Linear);

        _hitIndicatorImage.material = _hitIndicatorMaterial;
        _hitIndicatorMaterial.DOFloat(1f, "_Radius", _hitIndicatorTransitionDuration).SetEase(Ease.InOutBack);
    }
    [Button]
    private void DoKillCircleIndicator()
    {
        _circleIndicatorImage.material = _circleBackgroundMaterial;
        _circleBackgroundMaterial.DOFloat(0f, "_Effect_Intensity", _circleIndicatorTransitionDuration).SetEase(Ease.Linear);

        _hitIndicatorImage.material = _hitIndicatorMaterial;
        _hitIndicatorMaterial.DOFloat(10f, "_Radius", _hitIndicatorTransitionDuration).SetEase(Ease.InOutExpo);
    }

    private void DoSpiralMovement(float position, RectTransform indicator)
    {
        float maxAngle = _turns * (2 * Mathf.PI); // 360 in radians
        
        float currentAngle  = Mathf.Lerp(0, maxAngle, position); // position is between 0-1
        float currentRadius = Mathf.Lerp(_spiralRadius, 0, position); // position is between 0-1

        float posX = Mathf.Cos(currentAngle) * currentRadius;
        float posY = Mathf.Sin(currentAngle) * currentRadius;

        Vector2 finalPos = _centerPivot.anchoredPosition + new Vector2(posX, posY);

        indicator.anchoredPosition = finalPos;

        if (_successRange.x <= position && position <= _successRange.y && _showPossibleHits)
        {
            Image i = Instantiate(_circleIndicator, _centerPivot).GetComponent<Image>();
            i.color = Color.green;
        }
    }

    public void SetUpHitIndicator(Vector2 successRange)
    {
        _successRange = new Vector2(successRange.x, successRange.y);

        Debug.LogWarning($"Success Range: {successRange}, {_successRange}");

        float range = Mathf.Abs(successRange.x - successRange.y);

        float diameter = (range * _centerPivot.sizeDelta.x) + _circleIndicator.sizeDelta.x;

       _hitIndicator.rectTransform.sizeDelta = new Vector2(diameter, diameter);
    }

    public void UpdateSpiral(float value)
    {
        DoSpiralMovement(value, _circleIndicator);
    }

}
