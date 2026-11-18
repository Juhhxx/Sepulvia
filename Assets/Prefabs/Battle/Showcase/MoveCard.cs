using UnityEngine;
using TMPro;

public class MoveCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moveNameText;
    [SerializeField] private TextMeshProUGUI _moveDescriptionText;

    public void Setup(Move move, Character character)
    {
        int recoveryCost = move.RecoveryCost;

        if (move.MoveLogic is MoveComboEnemy mc)
        {
            recoveryCost = mc.GetTotalRecoveryCost();
        }
        
        string header = move.Name + $" (RC: {recoveryCost} C: {move.Cooldown})";
        string content = "";

        string description = move.Description;

        // Get the values for the pull bonus
        string playerPullStr = "";
        int strBonus = character.PullStrenghtBonus;

        if (strBonus != 0)
        {
            if (strBonus > 0) playerPullStr = "<color=green>(+" + strBonus + ")</color>";
            else playerPullStr =  "<color=red>(" + strBonus + ")</color>";
        }

        description = description.Replace("(pull_str)", $"{playerPullStr}");

        content += $"\n\n{description}";

        if (move.CheckIfCooldown())
        {
            description += $"\nIn Cooldown: {move.Cooldown - move.TurnsPassed}";
        }

        _moveNameText.text = header;
        _moveDescriptionText.text = content;
    }
}
