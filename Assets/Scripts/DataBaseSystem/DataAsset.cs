using UnityEngine;

public class DataAsset : ScriptableObject
{
    [SerializeField] private int _id = -1;
    public int ID => _id;

#if UNITY_EDITOR
    public void SetID(int id)
    {
        _id = id;
    }
#endif
}