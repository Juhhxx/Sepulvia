using UnityEngine;
using UnityEngine.UI;

public class HitTargetSpiralUI : MonoBehaviour
{
    [SerializeField] private Transform _centerPivot;
    [SerializeField] private Transform _circleIndicator;
    [SerializeField] private Image _hitIndicator;
    [SerializeField] private float _spiralRadius;
    [SerializeField] private float _turns; // nubmer of turns in the spiral


    private void Start()
    {
        _hitIndicator.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void DoSpiralMovement(float position, Transform indicator)
    {
        float maxAngle = _turns * (2 * Mathf.PI); // 360 in radians
        
        float currentAngle  = Mathf.Lerp(0, maxAngle, position); // position is between 0-1
        float currentRadius = Mathf.Lerp(_spiralRadius, 0, position); // position is between 0-1

        float posX = Mathf.Cos(currentAngle) * currentRadius;
        float posY = Mathf.Sin(currentAngle) * currentRadius;

        Vector3 finalPos = _centerPivot.position + new Vector3(posX, posY, 0f);

        indicator.position = finalPos;
    }

    public void SetUpHitIndicator(Vector2 successRange)
    {
        float range = successRange.magnitude;

       _hitIndicator.rectTransform.sizeDelta = new Vector2(range * _spiralRadius, range * _spiralRadius);
    }

    public void UpdateSpiral(float value)
    {
        DoSpiralMovement(value, _circleIndicator);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(_centerPivot.position, _spiralRadius);
    }
}
