using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
public class WinScreenBeadController : MonoBehaviour
{
    [SerializeField] private Transform _beadTransform;
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _spawnSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Button]
    public void DoBeadSpawnIn()
    {
        _beadTransform.localScale = Vector3.zero;
        _beadTransform.DOScale(Vector3.one, _spawnSpeed).SetEase(Ease.OutBack);
    }
    [Button]
    public void DoBeadAwakening()
    {
        _beadTransform.rotation = Quaternion.Euler(0, 0, 0);
        Quaternion targetRotation = Quaternion.Euler(180,0,0);
        //_beadTransform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutBack);
        _beadTransform.DORotate(new Vector3(180, 0, 0), _rotationSpeed).SetEase(Ease.InOutBack);
    }
    [Button]
    public void DoNormalRotationRight()
    {
        Quaternion targetRotation = Quaternion.Euler(180,0,0);
        //_beadTransform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutBack);
        _beadTransform.DORotate(new Vector3(540, 0, 0), _rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
    }
    [Button]
    public void DoNormalRotationLeft()
    {
        Quaternion targetRotation = Quaternion.Euler(180,0,0);
        //_beadTransform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutBack);
        _beadTransform.DORotate(new Vector3(-540, 0, 0), _rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
    }

    [Button]
    public void DoBeadSpawnOut()
    {
        _beadTransform.localScale = Vector3.one;
        _beadTransform.DOScale(Vector3.zero, _spawnSpeed).SetEase(Ease.InBack);
    }
}
