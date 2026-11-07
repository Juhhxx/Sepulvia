using UnityEngine;

public class BarModifierController : MonoBehaviour
{
    [SerializeField] private PlaySound _placeSound;
    [SerializeField] private PlaySound _loopSound;
    [SerializeField] private PlaySound _breakSound;

    public void PlayPlaceSound()
    {
        _placeSound.SoundPlay();
    }

    public void PlayLoopSound()
    {
        _loopSound.SoundPlay();
    }
    public void StopLoopSound()
    {
        _loopSound.Stop();
    }

    public void PlayBreakSound()
    {
        _breakSound.SoundPlay();
    }

    private RectTransform _rectTrans;
    public RectTransform RectTransform => _rectTrans;

    private Animator _anim;

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _anim = GetComponent<Animator>();
    }

    public void AdjustSizePosition(int activeChains, Vector2 defaultPos)
    {
        var pos = defaultPos;
        pos.y = Mathf.Lerp(pos.y - 25f, pos.y, (activeChains / 3f));

        _rectTrans.anchoredPosition = pos;
        _rectTrans.localScale = Vector3.one * Mathf.Lerp(0.6f, 1f, (activeChains / 3f));
    }

    public void DoAnimationTriger(string trigger)
    {
        _anim.SetTrigger(trigger);
    }
}
