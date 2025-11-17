using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class StatModifierDisplay : MonoBehaviour
{
    [SerializeField] private Transform _statModifierDisplay;
    [SerializeField] private GameObject _statModifierPrefab;

    public void UpdateDisplay(List<StatModifier> statModifiers)
    {
        var old = _statModifierDisplay.GetComponentsInChildren<Image>();

        foreach (Image i in old) Destroy(i.gameObject);

        foreach(StatModifier sm in statModifiers)
        {
            Image image = Instantiate(_statModifierPrefab, _statModifierDisplay).GetComponent<Image>();

            image.sprite = sm.Sprite;
        }
    }
}
