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
    [SerializeField] private Image _circleBackgroundImage;
    [SerializeField] private Image _hitIndicatorImage;
    [SerializeField] private Image _clickPromptImage;
    [SerializeField] private float _circleIndicatorTransitionDuration = .3f;
    [SerializeField] private float _hitIndicatorTransitionDuration = .3f;
    [SerializeField] private Material _hitIndicatorMaterialOG;
    private Material _circleBackgroundMaterial;
    private Material _hitIndicatorMaterial;
    private bool _enteredSuccess;

    private Animator _indicatorAnim;

    private void Awake()
    {
        _hitIndicator.rectTransform.anchoredPosition = Vector2.zero;

        _indicatorAnim = _circleIndicator.GetComponent<Animator>();

        _circleBackgroundMaterial = new Material(_circleBackgroundImage.material);
        _hitIndicatorMaterial = new Material(_hitIndicatorImage.material);
    }

    private void Start()
    {
        DoSpawnSpiral();
    }

    [Button]
    public void DoSpawnSpiral()
    {
        _circleBackgroundImage.material = _circleBackgroundMaterial;
        _circleBackgroundMaterial.SetFloat("_Effect_Intensity", 0f);
        
        _circleBackgroundMaterial.DOFloat(1f, "_Effect_Intensity", _circleIndicatorTransitionDuration).SetEase(Ease.Linear);

        _hitIndicatorImage.material = _hitIndicatorMaterial;
        _hitIndicatorMaterial.SetFloat("_Radius", 10f);

        _hitIndicatorMaterial.DOFloat(0.55f, "_Radius", _hitIndicatorTransitionDuration).SetEase(Ease.InOutBack);
    }

    [Button]
    public void DoKillSpiral()
    {
        _clickPromptImage.DOFade(0f, _circleIndicatorTransitionDuration).SetEase(Ease.Linear).OnComplete(() => _clickPromptImage.gameObject.SetActive(false));

        _circleBackgroundImage.material = _circleBackgroundMaterial;
        _circleBackgroundMaterial.DOFloat(0f, "_Effect_Intensity", _circleIndicatorTransitionDuration).SetEase(Ease.Linear).OnComplete(() => _circleBackgroundImage.gameObject.SetActive(false));

        _hitIndicatorImage.material = _hitIndicatorMaterial;
        _hitIndicatorMaterial.DOFloat(10f, "_Radius", _hitIndicatorTransitionDuration).SetEase(Ease.InOutExpo).OnComplete(() => _hitIndicatorImage.gameObject.SetActive(false));
    }

    public void DoIndicatorAnimTrigger(string trigger)
    {
        _indicatorAnim.SetTrigger(trigger);
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

        if (value >= _successRange.x && !_enteredSuccess)
        {
            _hitIndicatorMaterial.DOFloat(.08f, "_Thickness", _hitIndicatorTransitionDuration).SetEase(Ease.OutBack);
            _hitIndicatorMaterial.DOColor(new Color(191f, 36f,0f,1f) * 1f, "_Main_Color", _hitIndicatorTransitionDuration).SetEase(Ease.OutBack);
            _hitIndicatorMaterial.DOColor(new Color(191f, 65f,0f,1f) * 2f, "_Outline_Color", _hitIndicatorTransitionDuration).SetEase(Ease.OutBack);
        
            _enteredSuccess = true;
        }
        else
        {
            //_hitIndicatorMaterial = _hitIndicatorMaterialOG;
        }
    }

}
