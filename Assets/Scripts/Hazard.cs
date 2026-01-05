using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Hazard : MonoBehaviour
{

    [SerializeField] private int _damageAmount = 5;
    [SerializeField, Layer] private int _hazardLayer;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        gameObject.layer = _hazardLayer;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.PlayerCharacter.CurrentStance -= _damageAmount;
        }
    }
}
