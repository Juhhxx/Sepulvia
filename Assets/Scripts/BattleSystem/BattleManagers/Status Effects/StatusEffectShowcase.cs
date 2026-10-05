using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;

public class StatusEffectShowcase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _durationTMP;

    public event Action<bool,StatusEffect> OnShowcaseSelected;
    private StatusEffect _statusEffect;

    private void Start()
    {
        OnShowcaseSelected += FindAnyObjectByType<StatusEffectInfoPanel>().UpdatePanel;
        //ele nasce aqui
            //puxa a corotina aqui para iniciar o efeito de dissolve
    }

    public void SetUpShowcase(StatusEffect se)
    {
        _iconImage.sprite = se.Icon;
        _durationTMP.text = $"{se.TurnDuration}";

        se.OnTurnPassed += UpdateDuration;
        se.OnCompleted += DestroyShowcase;

        _statusEffect = se;
    }

    private void UpdateDuration(int duration)
    {
        _durationTMP.text = $"{duration}";
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
