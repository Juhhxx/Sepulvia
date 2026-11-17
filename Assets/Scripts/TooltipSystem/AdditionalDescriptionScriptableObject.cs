using UnityEngine;

[CreateAssetMenu(fileName = "AdditionalTooltip", menuName = "Tooltip System/Create Additional Tooltip")]
public class AdditionalDescriptionScriptableObject : ScriptableObject, IDescriptable
{
    public string Header;
    public string Content;
    public IDescriptable[] GetAdditionalDescriptions()
    {
        return null;
    }

    public string GetDescription()
    {
        return Content;
    }

    public string GetName()
    {
        return Header;
    }
}
