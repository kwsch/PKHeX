using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving ribbon sprite images.
/// </summary>
public static class RibbonSpriteUtil
{
    /// <summary>
    /// Gets the ribbon sprite image for the specified <see cref="RibbonIndex"/>.
    /// </summary>
    /// <param name="ribbon">The ribbon index to get the sprite for.</param>
    /// <returns>A <see cref="Bitmap"/> representing the ribbon sprite, or null if not available.</returns>
    public static Bitmap? GetRibbonSprite(RibbonIndex ribbon)
    {
        var name = $"Ribbon{ribbon}";
        return GetRibbonSprite(name);
    }

    /// <summary>
    /// Gets the ribbon sprite image for the specified ribbon name.
    /// </summary>
    /// <param name="name">The name of the ribbon.</param>
    /// <returns>A <see cref="Bitmap"/> representing the ribbon sprite, or null if not available.</returns>
    public static Bitmap? GetRibbonSprite(string name)
    {
        var resource = name.Replace("CountG3", "G3").ToLowerInvariant();
        return (Bitmap?)Resources.ResourceManager.GetObject(resource);
    }

    /// <summary>
    /// Gets the ribbon sprite image for the specified ribbon name, maximum value, and current value.
    /// </summary>
    /// <param name="name">The name of the ribbon.</param>
    /// <param name="max">The maximum value for the ribbon.</param>
    /// <param name="value">The current value for the ribbon.</param>
    /// <returns>A <see cref="Bitmap"/> representing the ribbon sprite, or null if not available.</returns>
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
            if (value >= max)
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
