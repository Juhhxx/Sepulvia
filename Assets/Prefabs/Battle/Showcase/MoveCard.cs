using UnityEngine;
using TMPro;

public class MoveCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moveNameText;
    [SerializeField] private TextMeshProUGUI _moveDescriptionText;

    public void Setup(Move move)
    {
        int recoveryCost = move.RecoveryCost;

        if (move.MoveLogic is MoveCombo mc)
        {
            recoveryCost = mc.GetTotalRecoveryCost();
        }
        
        _moveNameText.text = move.Name + $" (RC: {recoveryCost} C: {move.Cooldown})";
        _moveDescriptionText.text = move.Description;

        if (move.CheckIfCooldown())
        {
            _moveDescriptionText.text += $"\nIn Cooldown: {move.Cooldown - move.TurnsPassed}";
        }
    }
}
