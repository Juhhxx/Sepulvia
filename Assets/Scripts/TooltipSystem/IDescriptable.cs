using System;
using System.Collections.Generic;

public interface IDescriptable
{
    public string GetName();
    public string GetDescription();
    public IDescriptable[] GetAdditionalDescriptions();
}