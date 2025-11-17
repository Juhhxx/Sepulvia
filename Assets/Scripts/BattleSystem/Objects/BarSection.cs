using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class BarSection : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public BarModifier BarModifier { get; private set; }
    public bool HasModifier { get; private set; }
    public void AddBarModifier(BarModifier barModifier)
    {
        BarModifier = barModifier;
        _image.color = barModifier.Color;
        HasModifier = true;
        
    }
    public void RemoveBarModifier()
    {
        BarModifier = null;
        HasModifier = false;
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

    private void Start()
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
}
