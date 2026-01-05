using UnityEngine;

public class GiveItem : MonoBehaviour
{
    [SerializeField] private ItemInfo _itemToGive;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.PlayerCharacter.Inventory.AddItem(_itemToGive);
            Destroy(gameObject);
        }
    }
}
