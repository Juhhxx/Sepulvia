using System.Collections;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 2f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();

            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnObject()
{
    GameObject obj = Instantiate(
        prefab,
        spawnPoint.position,
        spawnPoint.rotation
    );

    MovingTrain train = obj.GetComponent<MovingTrain>();
    if (train != null)
        train.speed = speed;

    Destroy(obj, lifetime);
}
}