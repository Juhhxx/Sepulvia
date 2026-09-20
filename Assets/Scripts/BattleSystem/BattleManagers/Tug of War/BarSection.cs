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

    private void OnDestroy()
    {
        Destroy(_modifierPrefab);
    }
}
