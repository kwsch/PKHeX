using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

public static class RibbonSpriteUtil
{
    public static Bitmap? GetRibbonSprite(RibbonIndex ribbon)
    {
        var name = $"Ribbon{ribbon}";
        return GetRibbonSprite(name);
    }

    public static Bitmap? GetRibbonSprite(string name)
    {
        var resource = name.Replace("CountG3", "G3").ToLowerInvariant();
        return (Bitmap?)Resources.ResourceManager.GetObject(resource);
    }

    public static Bitmap? GetRibbonSprite(string name, int max, int value)
    {
        var resource = GetRibbonSpriteName(name, max, value);
        return (Bitmap?)Resources.ResourceManager.GetObject(resource);
    }

    private static string GetRibbonSpriteName(string name, int max, int value)
    {
        if (max != 4) // Memory
        {
            var sprite = name.ToLowerInvariant();
            if (max == value)
                return sprite + "2";
            return sprite;
        }

        // Count ribbons
        string n = name.Replace("Count", string.Empty).ToLowerInvariant();
        return value switch
        {
            2 => n + "super",
            3 => n + "hyper",
            4 => n + "master",
            _ => n,
        };
    }
}
