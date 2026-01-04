using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _statsTMP;
    [SerializeField] private GameObject _moveButtons;

    public void UpdateStats(CharacterInfo c)
    {
        _nameTMP.text = c.Name;

        _statsTMP.text = 
        $"Stance : {c.CurrentStance}/{c.MaxStance}\nStance Gain : {c.StanceRecover}\nSpeed : {c.Speed}\nPull Str. : +{c.PullStrenghtBonus}";

        var tmp = _moveButtons.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < tmp.Length; i++)
        {
            Debug.Log($"{c.MoveSet.Count}");
            Debug.Log($"{i}");
            Debug.Log($"{tmp[i]}");
            Debug.Log($"{c.MoveSet[i]}");
            
            tmp[i].text = c.MoveSet[i].Name;
        }
    }
}
