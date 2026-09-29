using UnityEngine;

public class BattlerAnimationController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void  DoTrigger(string name)
    {
        _animator.SetTrigger(name);
    }
}
