using UnityEngine;

public class BattlerAnimationController : MonoBehaviour
{
    [SerializeField] private Transform _battlerVFXPivot;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void DoTrigger(string name)
    {
        _animator.SetTrigger(name);
    }

    public void DoBattlerVFX(BattlerVFX vfx)
    {
        if (vfx == null) return;
        
        Instantiate(vfx, _battlerVFXPivot);
    }
}
