using UnityEngine;
using System.Collections.Generic;

public class RoomBlockInfo : MonoBehaviour
{
    [field: SerializeField] public GameObject RoomPrefab { get; private set; }
    [field: SerializeField] public List<Transform> ConnectionPoints { get; private set; }

    private bool _isOverlapping = false;
    public bool IsOverlapping => _isOverlapping;

    private void OnTriggerEnter(Collider other) => _isOverlapping = true;
    private void OnTriggerExit(Collider other) => _isOverlapping = false;

    private void Update()
    {
        Debug.Log($"OVERLAP : {_isOverlapping}");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if (_isOverlapping) Gizmos.DrawWireSphere(transform.position, 10f);
    }

}
