using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Space(5)]
    [SerializeField] private float _interactionRadius = 5f;
    [SerializeField, InputAxis] private string _interactionButton;
    [SerializeField] private LayerMask _interactableLayer;

    [Space(10)]
    [Header("Interaction Debug")]
    [Space(5)]
    [SerializeField, ReadOnly] private IInteractable _currentInteractable;
    public IInteractable CurrentInteractable
    {
        get => _currentInteractable;
        set
        {
            if (value != _currentInteractable)
            {
                _currentInteractable?.ToggleSelected(false);
                value?.ToggleSelected(true);
                Debug.Log($"[Interaction Manager] Set Current Interactable as {value}");
            }

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
        Collider[] hits = Physics.OverlapSphere(transform.position, _interactionRadius, _interactableLayer);

        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                CurrentInteractable = interactable;
                return;
            }
        }

        CurrentInteractable = null;
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
