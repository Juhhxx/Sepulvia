using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class BarSection : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public BarModifier BarModifier { get; private set; }

    [field: SerializeField, ReadOnly] public BarSection ConnectRight { get; private set; }
    [field: SerializeField, ReadOnly] public BarSection ConnectLeft { get; private set; }

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

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (HasHeart) _image.color = Color.yellow;
        else _image.color = Color.white;
    }
}
