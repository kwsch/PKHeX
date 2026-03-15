using PKHeX.Core;
using SkiaSharp;
using System;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class DonutSpriteUtil
{
    public static SKBitmap? Sprite(this Donut9a donut) => GetDonutImage(donut);
    public static SKBitmap? StarSprite => ResourceLoader.GetObject("star");
    public static SKBitmap? GetDonutFlavorImage(string donut) => ResourceLoader.GetObject(donut);
    public static SKBitmap? GetFlavorProfileImage() => ResourceLoader.GetObject("flavorprofile");

    private static SKBitmap? GetDonutImage(Donut9a donut)
    {
        if (donut.Donut is >= 198 and <= 202)
            return GetSpecialDonutImage(donut);

        ReadOnlySpan<string> flavors = ["sweet", "spicy", "sour", "bitter", "fresh", "mix"];
        var variant = donut.Donut % 6;
        var stars = donut.Stars;
        var flavor = flavors[variant];
        var resource = $"donut_{flavor}{stars:00}";
        return ResourceLoader.GetObject(resource);
    }

    private static SKBitmap? GetSpecialDonutImage(Donut9a donut) => donut.Donut switch
    {
        198 => ResourceLoader.GetObject("donut_uni491"),
        199 => ResourceLoader.GetObject("donut_uni383"),
        200 => ResourceLoader.GetObject("donut_uni382"),
        201 => ResourceLoader.GetObject("donut_uni384"),
        202 => ResourceLoader.GetObject("donut_uni807"),
        _ => null,
    };
}
