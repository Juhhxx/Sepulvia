using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections;

public class BarSection : MonoBehaviour
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _heartColor;
    [SerializeField] private Image[] _barImages;


    [field: SerializeField, ReadOnly] public BarModifier BarModifier { get; private set; }
    public bool HasModifier { get; private set; }
    private GameObject _modifierPrefab;

    public void AddBarModifier(BarModifierInfo barModifier)
    {
        BarModifier = barModifier.Instantiate();
        _modifierPrefab = Instantiate(barModifier.BarEffectPrefab, GetComponentInParent<Canvas>().transform);
        _modifierPrefab.GetComponent<RectTransform>().anchoredPosition = HeartPosition;
        HasModifier = true;
        
    }
    public void RemoveBarModifier()
    {
        BarModifier = null;
        HasModifier = false;
        _modifierPrefab.GetComponent<Animator>().SetTrigger("Destroy");

        StartCoroutine(DestroyModifierCR(_modifierPrefab));
    }

    private IEnumerator DestroyModifierCR(GameObject modifier)
    {
        yield return new WaitForSeconds(2);

        Destroy(modifier);
    }

    [field: SerializeField, ReadOnly] public BarSection ConnectRight { get; set; }
    [field: SerializeField, ReadOnly] public BarSection ConnectLeft { get; set; }

    [field: SerializeField, ReadOnly] public bool HasHeart { get; private set; }
    public void SetHasHeart(bool has)
    {
        HasHeart = has;

        if (HasHeart) ChangeImagesColor(_heartColor);
        else ChangeImagesColor(_normalColor);
    }

    [field: SerializeField, ReadOnly] public Vector3 HeartPosition { get; private set; }
    public void SetHeartPosition(Vector3 pos)
    {
        HeartPosition = pos;
    }

    [field: SerializeField, ReadOnly] public RectTransform RectTransform { get; private set; }
    public void SetTransform(RectTransform rectTrans)
    {
        RectTransform = rectTrans;
    }

    private Image _image;
    public Image Image => _image;

    private Button _button;
    public Button Button => _button;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();

        if (_barImages.Length > 0) ChangeImagesColor(_normalColor);
    }

    private void ChangeImagesColor(Color color)
    {
        foreach (Image i in _barImages)
        {
            i.color = color;
        }
    }

    private void OnDestroy()
    {
        Destroy(_modifierPrefab);
    }
}
