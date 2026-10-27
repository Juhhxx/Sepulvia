using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class HitTargetMinigameSpiralUI : MonoBehaviour
{
    [SerializeField] private Transform _centerPivot;
    [SerializeField] private Transform _circleIndicator;
    [SerializeField] private float _spiralRadius;
    [SerializeField] private float _turns; // nubmer of turns in the spiral

    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();
    }

    private void DoSpiralMovement(float position)
    {
        float maxAngle = _turns * 360f;
        
        float currentAngle  = Mathf.Lerp(0, maxAngle, position); // position is between 0-1
        float currentRadius = Mathf.Lerp(_spiralRadius, 0, position); // position is between 0-1

        float posX = Mathf.Cos(currentAngle) * currentRadius;
        float posY = Mathf.Sin(currentAngle) * currentRadius;

        Vector3 finalPos = _centerPivot.position + new Vector3(posX, posY, 0f);

        Debug.Log($"FINAL INDICATOR POSITION: {finalPos}, {posX}, {posY}, {position}");

        _circleIndicator.position = finalPos;
    }

    private void Update()
    {
        DoSpiralMovement(_minigameController.CurrentPoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(_centerPivot.position, _spiralRadius);
    }
}
