using System;
using System.Drawing;

namespace PKHeX.Drawing;

/// <summary>
/// Utility class for manipulating <see cref="Color"/> values.
/// </summary>
public static class ColorUtil
{
    private const byte MaxStat = 180; // shift the green cap down
    private const byte MinStat = 0;

    /// <summary>
    /// Gets the <see cref="Color"/> value for the specified <see cref="stat"/> value.
    /// </summary>
    public static Color ColorBaseStat(int stat)
    {
        float x = (100f * stat) / MaxStat;
        if (x > 100)
            x = 100;
        double red = 255f * (x > 50 ? 1 - (2 * (x - 50) / 100.0) : 1.0);
        double green = 255f * (x > 50 ? 1.0 : 2 * x / 100.0);

        return Blend(Color.FromArgb((int)red, (int)green, 0), Color.White, 0.4);
    }

    public static Color ColorBaseStatTotal(int tot)
    {
        const byte sumShiftDown = 175;
        const float sumDivisor = 3f;
        var sumToSingle = Math.Max(MinStat, tot - sumShiftDown) / sumDivisor;
        return ColorBaseStat((int)sumToSingle);
    }

    public static Color Blend(Color color, Color backColor, double amount)
    {
        byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
        byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
        byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
        return Color.FromArgb(r, g, b);
    }
}
