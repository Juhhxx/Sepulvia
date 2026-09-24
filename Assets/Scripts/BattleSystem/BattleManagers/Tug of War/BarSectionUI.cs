using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;

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

    [SerializeField] private float _burnHeatUpDuration = 2f;
    [SerializeField] private Color _startColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color _targetColor = new Color(191/255f , 30/255f , 0f );
    [SerializeField] private float _targetColorIntensity = 5f;


    private Color _currentColor;

    private BarSection _barSection;
    private PointerButtonEvents _pointerEvents;

    private void Awake()
    {
        _currentColor = _normalColor;

        _barSection = GetComponent<BarSection>();

        _barSection.OnDestroySection += DoExplodeShatter;
        _barSection.OnBurnSection += StartDoBurn;
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

    private Coroutine burn;
    [Button]
    private void StartDoBurn()
    {
        
        if (burn != null)
        {
            StopCoroutine(burn);
        }

        foreach (Image chain in _sectionChains) 
        {
            chain.material = Instantiate(chain.material); //Arranjar o material de cada instancia de link
        }

        burn = StartCoroutine(DoBurn());
        
    }
    
    private IEnumerator DoBurn()
    {
        
        //Ativar o Particle System
        //Lerp each one and the particleSystem Color between the start color and the "incandescent" one
            // start RGB(255,255,255) HDR lv 0
            //target RGB(191, 30, 0) HDR lv 2
                //pa mudar a intenisdade do HDR, é o coeficiente so new Color( r * intensity, g * intensity, b * intensity).        
        Color startColor = _startColor;
        Color targetColor = _targetColor * _targetColorIntensity;

        float t = 0f;
        float duration = _burnHeatUpDuration;

        while(t <1f) //Lerp para uma cor incandescente
        {
            t += Time.deltaTime/duration;
            
            Color newColor = Color.Lerp(startColor, targetColor, t);

            foreach (Image chain in _sectionChains)
            {
                chain.material.SetColor("_Glow_Color", newColor);
            }

            yield return null;
        }
        
        DoShakeAnim(); //little anticipation shake

        yield return new WaitForSeconds(.1f);

        //After that adicionar rigidbody ams com graviutyScales ligeiramente diferentes para cairem a velocidades diferentes
        foreach( Image chain in _sectionChains)
        {
            GameObject chainObj = chain.gameObject;
            chain.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = chain.GetComponent<Rigidbody2D>();

            rb.gravityScale = Random.Range(.08f,1.2f);

            float nx = Random.Range(-.5f,.5f);

            rb.AddForce(new Vector2(nx, 0f), ForceMode2D.Impulse);
        }
        
        
    }

}