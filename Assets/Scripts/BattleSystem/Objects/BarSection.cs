using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections;

public class BarSection : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public BarModifier BarModifier { get; private set; }
    public bool HasModifier { get; private set; }
    private GameObject _modifierPrefab;

    public void AddBarModifier(BarModifier barModifier)
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
    }

    private void Update()
    {
        if (!HasModifier)
        {
            if (HasHeart) _image.color = Color.yellow;
            else _image.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        Destroy(_modifierPrefab);
    }
}
