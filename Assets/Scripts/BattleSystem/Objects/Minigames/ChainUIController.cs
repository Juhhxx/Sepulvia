using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChainUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _chainRotationPivot;
    [SerializeField] private RectTransform _chainMovePivot;
    [SerializeField] private RectTransform _chainLeft;
    [SerializeField] private RectTransform _chainRight;
    [SerializeField] private Image _sliderHandle;


    [OnValueChanged(nameof(UpdateEditor))]
    [SerializeField, Range(0,360)] private float _rotation;

    [OnValueChanged(nameof(UpdateEditor))]
    [SerializeField, Range(0,1)] private float _position;

    [SerializeField] private float _separateDistance = 50f;
    [SerializeField] private float _separateDuration = .5f;
    [SerializeField] private float _comeInDistance = 8000f;
    [SerializeField] private float _comeInDuration = 2f;
    [SerializeField] private float _comeOutDuration = 1.5f;
    [SerializeField] private Ease _comeInEase = Ease.OutExpo;
    [SerializeField] private Ease _comeOutEase = Ease.InOutBack;

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
        _chainRotationPivot.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void UpdateHitPosition(float posHit)
    {
        // Width of the chain part that appears on screen divided by 2
        float moveSpace = _chainRotationPivot.sizeDelta.x/2;

        float posH = Mathf.Lerp(-moveSpace, moveSpace, posHit);

        _chainMovePivot.anchoredPosition = new Vector2(posH, 0f);
    }

    //Animations
    [Button]
    public void DoSeparate()
    {
        _chainLeft.DOAnchorPosX(_chainLeft.anchoredPosition.x - _separateDistance, _separateDuration).SetEase(Ease.OutExpo);
        _chainRight.DOAnchorPosX(_chainRight.anchoredPosition.x + _separateDistance, _separateDuration).SetEase(Ease.OutExpo);
        //CameraEffectsUtility.DoCameraShake(.5f, 2f);
    }

    [Button]
    public void DoSpawnJoinUp()
    {
        _chainLeft.DOAnchorPosX(_chainLeft.anchoredPosition.x + _separateDistance, _separateDuration).SetEase(Ease.OutExpo);
        _chainRight.DOAnchorPosX(_chainRight.anchoredPosition.x - _separateDistance, _separateDuration).SetEase(Ease.OutExpo);
    }
    [Button]
    public void DoSpawnComeIn()
    {
        Color disabledSider = new Color(_sliderHandle.color.r, _sliderHandle.color.g, _sliderHandle.color.b, 0f);
        _sliderHandle.color = disabledSider;

        RectTransform chainTransform = _chainRotationPivot;

        Vector2 rotationVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _chainRotationPivot.rotation.eulerAngles.z), Mathf.Sin(Mathf.Deg2Rad * _chainRotationPivot.rotation.eulerAngles.z));
        Vector2 moveVector = rotationVector.normalized * _comeInDistance;

        _chainRotationPivot.anchoredPosition = _chainRotationPivot.anchoredPosition + - moveVector; //Get it off screen
        //find the rotation angle vector
        
        _chainRotationPivot.DOAnchorPos(_chainRotationPivot.anchoredPosition + moveVector, _comeInDuration).SetEase(_comeInEase).OnComplete(() => {
            Color enabledSider = new Color(_sliderHandle.color.r, _sliderHandle.color.g, _sliderHandle.color.b, 1f);
            _sliderHandle.color = enabledSider;
        });
        CameraEffectsUtility.DoCameraShake(.1f, 2f);

    }

    [Button]
    public void DoDispawn()
    {
        Color disabledSider = new Color(_sliderHandle.color.r, _sliderHandle.color.g, _sliderHandle.color.b, 0f);
        _sliderHandle.color = disabledSider;

        RectTransform chainTransform = _chainRotationPivot;

        Vector2 rotationVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * _chainRotationPivot.rotation.eulerAngles.z), Mathf.Sin(Mathf.Deg2Rad * _chainRotationPivot.rotation.eulerAngles.z));
        Vector2 moveVector = rotationVector.normalized * _comeInDistance;

        _chainRotationPivot.DOAnchorPos(_chainRotationPivot.anchoredPosition - moveVector, _comeOutDuration).SetEase(_comeOutEase);
        CameraEffectsUtility.DoCameraShake(.1f, 2f);
    }
}
