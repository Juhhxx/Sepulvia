using UnityEngine;

public class SetFPS : MonoBehaviour
{
    [SerializeField] private int _targetFps = 60;
    private void Awake()
    {
        Application.targetFrameRate = _targetFps;
    }
}
