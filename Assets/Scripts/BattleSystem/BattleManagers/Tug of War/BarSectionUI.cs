using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Callbacks;
using NaughtyAttributes;

public class BarSectionUI : MonoBehaviour
{
    [SerializeField] private Image _sectionDivisorRight;
    [SerializeField] private Image _sectionDivisorLeft;
    [SerializeField] private Image[] _sectionChains;

    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _hasHeartColor;
    [SerializeField] private Color _isSelectedColor;
    
    [SerializeField] private float _shakeIntensity = 20;
    [SerializeField] private float _torqueIntensity = 10;
    [SerializeField] private float _explodeIntensity = 6;


    private Color _currentColor;

    private BarSection _barSection;
    private PointerButtonEvents _pointerEvents;

    private void Awake()
    {
        _currentColor = _normalColor;

        _barSection = GetComponent<BarSection>();

        _barSection.OnDestroySection += DoExplodeShatter;
        _barSection.OnHeartChange += SetHeartColor;
        _barSection.OnChangeConnections += () => 
            ToggleSectionDivisors(_barSection.ConnectRight != null, 
                _barSection.ConnectLeft != null);

        _pointerEvents = GetComponent<PointerButtonEvents>();

        _pointerEvents.OnPointerEnterEvent.AddListener(() => ToggleSelection(true));
        _pointerEvents.OnPointerExitEvent.AddListener(() => ToggleSelection(false));
    }

    private void SetHeartColor(bool hasHeart)
    {
        _currentColor = hasHeart ? _hasHeartColor : _normalColor;
        ChangeImagesColor(_currentColor);
    }
    private void ToggleSelection(bool isSelected)
    {
        if (isSelected && !_barSection.Button.enabled) return;

        var color = isSelected ? _isSelectedColor : _currentColor;

        ChangeImagesColor(color);
    }

    private void ChangeImagesColor(Color color)
    {
        foreach (Image i in _sectionChains)
        {
            i.color = color;
        }
    }

    private void ToggleSectionDivisors(bool right, bool left)
    {
        _sectionDivisorRight.enabled = right;
        _sectionDivisorLeft.enabled = left;
    }

    [Button]
    private void DoShakeAnim()
    {
        foreach(Image chain in _sectionChains)
        {
            chain.rectTransform.DOShakeAnchorPos(.1f,_shakeIntensity,10,90);
            chain.rectTransform.DOShakeRotation(.1f,_shakeIntensity/2,10,90);
        }
    }

    [Button]
    private void DoExplodeShatter()
    {
        foreach(Image chain in _sectionChains)
        {
            _sectionDivisorLeft.enabled = false;
            _sectionDivisorRight.enabled = false;

            GameObject chainObj = chain.gameObject;
            chain.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = chain.GetComponent<Rigidbody2D>();

            float nx = Random.Range(-.5f,.5f);
            float ny = Random.Range(0.1f, 1f);
            float t = Random.Range(-1,1);

            rb.AddForce(new Vector2(nx, ny) * _explodeIntensity, ForceMode2D.Impulse);
            rb.AddTorque(_torqueIntensity * t);
        }
    }

}