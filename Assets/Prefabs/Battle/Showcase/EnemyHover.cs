using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PointerButtonEvents))]
public class EnemyHover : MonoBehaviour
{
    [SerializeField] private BattlerController _battlerController;
    [SerializeField] private float _delay = 0.5f;
    private PointerButtonEvents _pointerButtonEvents;
    private EnemyCard _enemyCard;

    private void Start()
    {
        _enemyCard = FindAnyObjectByType<EnemyCard>(findObjectsInactive: FindObjectsInactive.Include);

        _pointerButtonEvents = GetComponent<PointerButtonEvents>();

        _pointerButtonEvents.OnPointerEnterEvent.AddListener(ShowEnemyCard);
        _pointerButtonEvents.OnPointerExitEvent.AddListener(HideEnemyCard);
    }

    private IEnumerator ShowEnemyCardCR()
    {
        yield return new WaitForSeconds(_delay);

        _enemyCard.Setup(true, _battlerController.Character as Enemy);
    }

    private Coroutine _showCard;
    private void ShowEnemyCard()
    {
        if (_showCard != null)
        {
            StopCoroutine(_showCard);
        }

        _showCard = StartCoroutine(ShowEnemyCardCR());
    }

    private void HideEnemyCard()
    {
        if (_showCard != null)
        {
            StopCoroutine(_showCard);
        }
        
        _showCard = null;

        _enemyCard.Setup(false);
    }

}
