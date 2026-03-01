using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using System;
using System.Drawing;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving and composing sprites for Donuts.
/// </summary>
public static class DonutSpriteUtil
{
    /// <summary>
    /// Gets the sprite image for the specified <see cref="Donut9a"/>.
    /// </summary>
    /// <param name="donut">The donut to get the sprite for.</param>
    /// <returns>A <see cref="Bitmap"/> representing the sprite image.</returns>
    public static Bitmap? Sprite(this Donut9a donut) => GetDonutImage(donut);
    public static Bitmap? StarSprite => (Bitmap?)Resources.ResourceManager.GetObject("star");
    public static Bitmap? GetDonutFlavorImage(string donut) => (Bitmap?)Resources.ResourceManager.GetObject(donut);
    public static Bitmap? GetFlavorProfileImage() => (Bitmap?)Resources.ResourceManager.GetObject("flavorprofile");

    private static Bitmap? GetDonutImage(Donut9a donut)
    {
        if (donut.Donut is >= 198 and <= 202)
            return GetSpecialDonutImage(donut);

        ReadOnlySpan<string> flavors = ["sweet", "spicy", "sour", "bitter", "fresh", "mix"];
        var variant = donut.Donut % 6;
        var stars = donut.Stars;
        var flavor = flavors[variant];
        var resource = $"donut_{flavor}{stars:00}";
        return (Bitmap?)Resources.ResourceManager.GetObject(resource);
    }

    private static Bitmap? GetSpecialDonutImage(Donut9a donut) => donut.Donut switch
    {
        198 => Resources.donut_uni491, // Bad Dreams Cruller
        199 => Resources.donut_uni383, // Omega Old-Fashioned Donut
        200 => Resources.donut_uni382, // Alpha Old-Fashioned Donut
        201 => Resources.donut_uni384, // Delta Old-Fashioned Donut
        202 => Resources.donut_uni807, // Plasma-Glazed Donut
        _ => null,
    };
}
