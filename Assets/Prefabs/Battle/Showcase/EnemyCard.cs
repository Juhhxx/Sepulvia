using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCard : MonoBehaviour
{
    [SerializeField] private GameObject _enemyCard;
    [SerializeField] private Image _enemyIconImage;
    [SerializeField] private TextMeshProUGUI _enemyNameText;
    [SerializeField] private Transform _enemyMoveCardsParent;
    [SerializeField] private MoveCard _moveCardPrefab;

    public void Setup(bool on, Enemy enemy = null)
    {
        _enemyCard.SetActive(on);

        if (on && enemy != null)
        {
            SetupCard(enemy);
        }
    }

    private void SetupCard(Enemy enemy)
    {
        _enemyIconImage.sprite = enemy.TimelineIndicator;
        _enemyNameText.text = enemy.Name;

        foreach (Transform child in _enemyMoveCardsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var move in enemy.MoveSet)
        {
            var moveCard = Instantiate(_moveCardPrefab, _enemyMoveCardsParent);
            moveCard.Setup(move);
        }
    }
}
