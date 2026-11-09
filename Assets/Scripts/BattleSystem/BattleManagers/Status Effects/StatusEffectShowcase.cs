using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class StatusEffectShowcase : MonoBehaviour, ITooltipable
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _turnIndicator;
    [SerializeField] private TextMeshProUGUI _durationTMP;
    [SerializeField] private GameObject _stackIndicator;
    [SerializeField] private TextMeshProUGUI _stackTMP;

    private StatusEffect _statusEffect;

    private void Start()
    {
        //ele nasce aqui
            //puxa a corotina aqui para iniciar o efeito de dissolve
    }

    public void SetUpShowcase(StatusEffect se)
    {
        _iconImage.sprite = se.Icon;
        _durationTMP.text = se.TurnDuration.ToString();

        se.OnTurnPassed += UpdateDuration;
        se.OnCompleted += DestroyShowcase;

        if (se.StatusEffectLogic is IStackableEffect stackable && stackable.ShowStack)
        {
            _stackTMP.text = "1";
            
            stackable.OnStackChange += UpdateStack;
        }
        else
        {
            _stackIndicator.SetActive(false);
        }

        _statusEffect = se;
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
        // Clone the image's material and apply it, so it doesn't interact with any other icon's material
        Material dissolveMaterial = new Material (_iconImage.material);
        _iconImage.material = dissolveMaterial;

        // Disable object text indicators
        _turnIndicator.gameObject.SetActive(false);
        _stackIndicator.gameObject.SetActive(false);

        dissolveMaterial.SetFloat("_Dissolve_Amount", 1f);
        float dissolveDuration = 0.75f;

        // Dissolve the status effect before destroying it
        DOTween.To(
            () => dissolveMaterial.GetFloat("_Dissolve_Amount"),
            value => dissolveMaterial.SetFloat("_Dissolve_Amount", value),
            -0.5f,
            dissolveDuration
        )
        .OnComplete(() =>
        {   
            Destroy(gameObject);
        });
        
    }

    private IEnumerator StartFade()
    {
        yield return null;
    }

    public TooltipInfo GetTooltipInfo()
    {
        string header = _statusEffect.Name;
        string content = _statusEffect.Description;

        List<TooltipInfo> additional = new List<TooltipInfo>();

        foreach (IDescriptable d in _statusEffect.AdditionalDescriptions)
        {
            additional.Add(new TooltipInfo(d.GetName(),
                                        d.GetDescription()));
        }

        return new TooltipInfo(header, content, additional.ToArray());
    }
}
