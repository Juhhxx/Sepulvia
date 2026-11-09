public class TooltipAdditionalText : IDescriptable
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