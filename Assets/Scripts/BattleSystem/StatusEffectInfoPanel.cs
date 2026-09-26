using System;
using TMPro;
using UnityEngine;

public class StatusEffectInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private GameObject _panel;

    public void UpdatePanel(bool on, StatusEffect effect)
    {
        if (on)
        {
            _titleTMP.text = effect.Name;
            _descriptionTMP.text = effect.Description;
        }

        _panel.SetActive(on);
    }
}
