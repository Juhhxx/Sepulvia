using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Hazard : MonoBehaviour
{

    [SerializeField] private int damageAmount = 5;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.PlayerCharacter.CurrentStance -= damageAmount;
        }
    }
}
