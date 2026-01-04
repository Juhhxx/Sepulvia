using System;
using System.Linq;
using System.Text.RegularExpressions;

public static class EnumExtensions
{
    public static string ToTitle<T>(this T value) where T : Enum
    {
        return Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");
    }
}
