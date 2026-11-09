using System.Collections.Generic;
using UnityEngine;

public class BarModifierController : MonoBehaviour, ITooltipable
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
    private BarModifier _modifier;

    private void Awake()
    {
        _rectTrans = GetComponent<RectTransform>();
        _anim = GetComponent<Animator>();
    }

    public void SetModifier(BarModifier modifier)
    {
        _modifier = modifier;
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

    public TooltipInfo GetTooltipInfo()
    {
        string header = _modifier.Name;
        string content = _modifier.Description;

        List<TooltipInfo> additional = new List<TooltipInfo>();

        foreach (IDescriptable d in _modifier.AdditionalDescriptions)
        {
            additional.Add(new TooltipInfo(d.GetName(),
                                        d.GetDescription()));
        }

        return new TooltipInfo(header, content, additional.ToArray());
    }
}
