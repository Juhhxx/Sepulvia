using UnityEngine;
using TMPro;

public class MoveCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moveNameText;
    [SerializeField] private TextMeshProUGUI _moveDescriptionText;

    public void Setup(Move move)
    {
        _moveNameText.text = move.Name + $" (RC: {move.RecoveryCost} C: {move.Cooldown})";
        _moveDescriptionText.text = move.Description;
    }
}
