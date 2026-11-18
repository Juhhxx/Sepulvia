using System;
using UnityEngine;

public class TimelineIndicatorUI : MonoBehaviour, ITooltipable
{
    private string _header = "";
    private string _content = "";
    public void SetInfo(IDescriptable descriptable)
    {
        if (descriptable == null) return;

        _header = descriptable.GetName();
        _content = descriptable.GetDescription();
    }

    public TooltipInfo GetTooltipInfo()
    {
        return new TooltipInfo(_header, _content);
    }
}
