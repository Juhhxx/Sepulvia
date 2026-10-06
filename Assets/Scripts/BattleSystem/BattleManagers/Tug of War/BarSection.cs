using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections;

public class BarSection : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public BarModifier BarModifier { get; private set; }
    public bool HasModifier { get; private set; } = false;
    public bool WasDestroyed { get; private set; } = false;

    public event Action<BarModifier> OnAddModifier;
    public event Action OnRemoveModifier;
    public void AddBarModifier(BarModifier barModifier)
    {
        BarModifier = barModifier;
        HasModifier = true;

        OnAddModifier?.Invoke(barModifier);
    }
    public void RemoveBarModifier()
    {
        BarModifier.Completed();
        BarModifier = null;
        HasModifier = false;
        
        OnRemoveModifier?.Invoke();
    }

    public event Action OnChangeConnections;

    [SerializeField, ReadOnly] private BarSection _connectRight;
    public BarSection ConnectRight
    {
        get => _connectRight;
        set
        {
            _connectRight = value;
            OnChangeConnections?.Invoke();
        }
    }

    [SerializeField, ReadOnly] private BarSection _connectLeft;
    public BarSection ConnectLeft
    {
        get => _connectLeft;
        set
        {
            _connectLeft = value;
            OnChangeConnections?.Invoke();
        }
    }

    public event Action<bool> OnHeartChange;
    [field: SerializeField, ReadOnly] public bool HasHeart { get; private set; }
    public void SetHasHeart(bool has)
    {
        HasHeart = has;
        OnHeartChange?.Invoke(has);
    }

    [field: SerializeField, ReadOnly] public Vector3 HeartPosition { get; private set; }
    public void SetHeartPosition(Vector3 pos)
    {
        HeartPosition = pos;
    }

    [field: SerializeField, ReadOnly] public RectTransform RectTransform { get; private set; }

    private Button _button;
    public Button Button => _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        RectTransform = GetComponent<RectTransform>();
    }

    public event Action OnDestroySection;
    public void DestroySection()
    {
        WasDestroyed = true;
        OnDestroySection?.Invoke();
    }

    public event Action OnBurnSection;
    public void BurnSection()
    {
        WasDestroyed = true;
        OnBurnSection?.Invoke();
    }
}
