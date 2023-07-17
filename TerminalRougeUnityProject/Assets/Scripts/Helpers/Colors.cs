using System.Collections.Generic;
using System.Linq;
using Helpers;
using JetBrains.Annotations;

public static class Colors
{
    public static List<SerializableKeyValPair<EColorType, string>> AllColors =
        new()
        {
            new(EColorType.Red, "#cc3e33"),
            new(EColorType.Yellow, "#FFFD84")
        };
    
    public static string GetColorHex(EColorType colorType)
    {
        var color = AllColors.FirstOrDefault(c => c.key == colorType);
        return color == default ? "" : color.value;
    }
}