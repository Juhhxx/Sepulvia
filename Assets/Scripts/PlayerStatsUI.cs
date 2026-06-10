using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _statsTMP;
    [SerializeField] private GameObject _moveButtons;

    public void UpdateStats(Character c)
    {
        _nameTMP.text = c.Name;

        string pullBonus = c.PullStrenghtBonus >= 0 ? $"+{c.PullStrenghtBonus}" : $"{c.PullStrenghtBonus}";

        _statsTMP.text = 
        $"Stance : {c.CurrentStance}/{c.MaxStance}\nStance Gain : {c.StanceRecover}\nSpeed : {c.Speed}\nPull Str. : {pullBonus}";

        var tmp = _moveButtons.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].text = c.MoveSet[i].Name;
        }
    }
}
