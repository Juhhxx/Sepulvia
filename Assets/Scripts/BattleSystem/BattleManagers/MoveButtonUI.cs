using TMPro;
using UnityEngine;

public class MoveButtonUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _iconSpr;
    [SerializeField] private TextMeshProUGUI _nameTMP;

    public void UpdateButton(Move move)
    {
        _iconSpr.sprite = move.Icon;
        _nameTMP.text = move.Name;
    }
}
