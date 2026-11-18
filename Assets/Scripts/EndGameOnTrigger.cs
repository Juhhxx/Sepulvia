using UnityEngine;

public class EndGameOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {   
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        // Only ends the game if its a player triggering the collider
        if(player != null)
        {
            // Ends the game
            GameObject.FindAnyObjectByType<MenuManager>().LoadScene("MainMenu"); 

            // Destroys the object
            Destroy(gameObject);
        }

    }
}
