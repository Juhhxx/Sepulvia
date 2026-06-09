using UnityEngine;

public class MovingTrain : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}