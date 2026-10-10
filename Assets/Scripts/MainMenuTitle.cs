using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MainMenuTitle : MonoBehaviour
{
    [SerializeField] private Vector3 _scaleChange = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private float _scaleDuration = 1f;
    [SerializeField] private Ease _scaleEase;

    [SerializeField] private Vector3 _rotationChange = new Vector3(0f, 0f, 20f);
    [SerializeField] private float _rotatioDuration = 1f;
    [SerializeField] private Ease _rotationEase;

    private Vector3 _initialScale;
    private Vector3 _initialRot;
    private RectTransform _rectTrans;

    private void Start()
    {
        _rectTrans = GetComponent<RectTransform>();

        _initialScale = _rectTrans.localScale;
        _initialRot = _rectTrans.localEulerAngles;

        DoAnimIdle();
    }

    [Button]
    private void DoAnimIdle()
    {

        _rectTrans.localScale = _initialScale;
        _rectTrans.localRotation = Quaternion.Euler(_initialRot);

        _rectTrans.DOKill();

        _rectTrans.DOScale(_scaleChange, _scaleDuration).SetEase(_scaleEase).SetLoops(-1, LoopType.Yoyo);
        _rectTrans.DORotate(_rotationChange, _rotatioDuration).SetEase(_rotationEase).SetLoops(-1, LoopType.Yoyo);
    }

}
