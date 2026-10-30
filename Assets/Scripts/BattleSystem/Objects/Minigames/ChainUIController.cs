using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ChainUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _chainRotationPivot;
    [SerializeField] private RectTransform _chainMovePivot;
    [SerializeField] private RectTransform _chainLeft;
    [SerializeField] private RectTransform _chainRight;

    [OnValueChanged(nameof(UpdateEditor))]
    [SerializeField, Range(0,360)] private float _rotation;

    [OnValueChanged(nameof(UpdateEditor))]
    [SerializeField, Range(0,1)] private float _position;


    private void UpdateEditor()
    {
        UpdateChainPosition(_rotation, _position);
    }

    private void UpdateChainPosition(float rotation, float posHit)
    {
        UpdateRotation(rotation);
        UpdateHitPosition(posHit);
    }

    public void UpdateRotation(float rotation)
    {
        _chainRotationPivot.rotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void UpdateHitPosition(float posHit)
    {
        // Width of the chain part that appears on screen divided by 2
        float moveSpace = _chainRotationPivot.sizeDelta.x/2;

        float posH = Mathf.Lerp(-moveSpace, moveSpace, posHit);

        _chainMovePivot.anchoredPosition = new Vector2(posH, 0f);
    }
}
