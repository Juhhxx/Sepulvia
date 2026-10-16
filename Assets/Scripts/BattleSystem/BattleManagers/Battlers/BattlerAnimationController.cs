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

    private BattlerVFX _currentVFX;

    public void DoBattlerVFX(BattlerVFX vfx)
    {
        if (_currentVFX != null && _currentVFX.IsPlaying) return; // Don't play 2 vfxs at once

        if (vfx == null) return;
        
        _currentVFX = Instantiate(vfx, _battlerVFXPivot);
    }
}
