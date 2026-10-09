using UnityEngine;

[RequireComponent(typeof(PointerButtonEvents))]
public class EnemyHover : MonoBehaviour
{
    [SerializeField] private BattlerController _battlerController;
    private PointerButtonEvents _pointerButtonEvents;
    private EnemyCard _enemyCard;

    private void Start()
    {
        _enemyCard = FindAnyObjectByType<EnemyCard>(findObjectsInactive: FindObjectsInactive.Include);

        _pointerButtonEvents = GetComponent<PointerButtonEvents>();
        _pointerButtonEvents.OnPointerEnterEvent.AddListener(ShowEnemyCard);
        _pointerButtonEvents.OnPointerExitEvent.AddListener(HideEnemyCard);
    }

    private void ShowEnemyCard()
    {
        _enemyCard.Setup(true, _battlerController.Character as Enemy);
    }

    private void HideEnemyCard()
    {
        _enemyCard.Setup(false);
    }

}
