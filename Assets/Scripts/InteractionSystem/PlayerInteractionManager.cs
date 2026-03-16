using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Space(5)]
    [SerializeField] private float _interactionRadius = 5f;
    [SerializeField, InputAxis] private string _interactionButton;

    [Space(10)]
    [Header("Interaction Debug")]
    [Space(5)]
    [SerializeField, ReadOnly] private IInteractable _currentInteractable;
    public IInteractable CurrentInteractable
    {
        get => _currentInteractable;
        set
        {
            _currentInteractable?.ToggleSelected(false);
            value?.ToggleSelected(true);
            
            Debug.Log($"[Interaction Manager] Set Current Interactable as {value}");
            _currentInteractable = value;
        }
    }

    private void Update()
    {
        CheckInteractable();

        if (_currentInteractable != null) CheckInteraction();
    }

    private void CheckInteractable()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, _interactionRadius, transform.up, out hit))
        {
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                CurrentInteractable = interactable;
                return;
            }
            else
            {
                CurrentInteractable = null;
            }
        }
        else CurrentInteractable = null;
    }
    private void CheckInteraction()
    {
        if (Input.GetButtonDown(_interactionButton))
        {
            if (CurrentInteractable.CanInteract)
            {
                Debug.Log($"[Interaction Manager] Interacted with {CurrentInteractable}");
                CurrentInteractable.Interact();
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = _currentInteractable == null ? Color.blue : Color.green;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
