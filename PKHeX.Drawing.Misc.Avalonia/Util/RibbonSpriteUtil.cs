using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class RibbonSpriteUtil
{
    public static SKBitmap? GetRibbonSprite(RibbonIndex ribbon)
    {
        var name = $"Ribbon{ribbon}";
        return GetRibbonSprite(name);
    }

    public static SKBitmap? GetRibbonSprite(string name)
    {
        var resource = name.Replace("CountG3", "G3").ToLowerInvariant();
        return ResourceLoader.GetObject(resource);
    }

    public static SKBitmap? GetRibbonSprite(string name, int max, int value)
    {
        var resource = GetRibbonSpriteName(name, max, value);
        return ResourceLoader.GetObject(resource);
    }

    private static string GetRibbonSpriteName(string name, int max, int value)
    {
        if (max != 4)
        {
            var sprite = name.ToLowerInvariant();
            if (value >= max)
                return sprite + "2";
            return sprite;
        }

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
