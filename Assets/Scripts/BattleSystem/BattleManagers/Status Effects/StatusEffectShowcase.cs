using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;
using DG.Tweening;

public class StatusEffectShowcase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _turnIndicator;
    [SerializeField] private TextMeshProUGUI _durationTMP;
    [SerializeField] private GameObject _stackIndicator;
    [SerializeField] private TextMeshProUGUI _stackTMP;

    public event Action<bool,StatusEffect> OnShowcaseSelected;
    private StatusEffect _statusEffect;

    private void Start()
    {
        //ele nasce aqui
            //puxa a corotina aqui para iniciar o efeito de dissolve
    }

    public void SetUpShowcase(StatusEffect se, StatusEffectInfoPanel infoPanel)
    {
        _iconImage.sprite = se.Icon;
        _durationTMP.text = se.TurnDuration.ToString();

        se.OnTurnPassed += UpdateDuration;
        se.OnCompleted += DestroyShowcase;

        if (se.StatusEffectLogic is IStackableEffect stackable)
        {
            _stackTMP.text = "1";
            
            stackable.OnStackChange += UpdateStack;
        }
        else
        {
            _stackIndicator.SetActive(false);
        }

        _statusEffect = se;

        OnShowcaseSelected += infoPanel.UpdatePanel;
    }

    private void UpdateDuration(int duration)
    {
        if (_durationTMP == null) return;

        _durationTMP.text = duration.ToString();
    }

    private void UpdateStack(int stack)
    {
        if (_stackIndicator == null) return;

        _stackTMP.rectTransform.DOPunchScale(Vector3.one * 1.2f, 0.2f);
        _stackTMP.text = stack.ToString();
    }

    private void DestroyShowcase()
    {
        Destroy(gameObject);
        //Add lerp to the dissolve here
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnShowcaseSelected?.Invoke(false, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnShowcaseSelected?.Invoke(true, _statusEffect);
    }

    private IEnumerator StartFade()
    {
        yield return null;
    }
}
