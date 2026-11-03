using UnityEngine;
using UnityEngine.UI;

public class HitTargetSpiralUI : MonoBehaviour
{
    [SerializeField] private RectTransform _centerPivot;
    [SerializeField] private RectTransform _circleIndicator;
    [SerializeField] private Image _hitIndicator;
    [SerializeField] private float _spiralRadius;
    [SerializeField] private float _turns; // nubmer of turns in the spiral
    [SerializeField] private bool _showPossibleHits;

    [SerializeField] private Vector2 _successRange;


    private void Start()
    {
        _hitIndicator.rectTransform.anchoredPosition = Vector2.zero;
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
