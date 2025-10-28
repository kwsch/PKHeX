using System;
using System.Drawing;

namespace PKHeX.Drawing;

/// <summary>
/// Utility class for manipulating <see cref="Color"/> values.
/// </summary>
public static class ColorUtil
{
    /// <summary> Clamps absurdly high stats to a reasonable cap. </summary>
    private const byte MaxStat = 180;
    /// <summary> Ensures the color does not go below this value. </summary>
    private const byte MinStat = 0;

    /// <summary> Shift the color total down a little as the BST floor isn't 0. </summary>
    private const byte ShiftDownBST = 175;
    /// <summary> Divide by a fudge factor so that we're roughly within the range of a single stat. </summary>
    private const float ShiftDivBST = 3;

    /// <summary>
    /// Gets the <see cref="Color"/> value for the specified <see cref="stat"/> value.
    /// </summary>
    /// <param name="stat">Single stat value</param>
    public static Color ColorBaseStat(int stat)
    {
        // Red to Yellow to Green, no Blue component.
        var x = (uint)stat >= MaxStat ? 1f : ((float)stat) / MaxStat;
        return GetPastelRYG(x);
    }

    /// <summary>
    /// Gets the <see cref="Color"/> value for the specified <see cref="bst"/> value.
    /// </summary>
    /// <param name="bst">Base Stat Total</param>
    public static Color ColorBaseStatTotal(int bst)
    {
        var sumToSingle = Math.Max(MinStat, bst - ShiftDownBST) / ShiftDivBST;
        return ColorBaseStat((int)sumToSingle);
    }

    /// <summary>
    /// Gets a pastel color from Red to Yellow to Green with a 40%/60% blend with White.
    /// </summary>
    /// <param name="x">Percentage of the way from Red to Green [0,1]</param>
    public static Color GetPastelRYG(float x)
    {
        var r = x > .5f ? 510 * (1 - x) : 255;
        var g = x > .5f ? 255 : 510 * x;

        // Blend with white to make it lighter rather than a darker shade.
        const float white = 0.4f;
        const byte b = (byte)(0xFF * (1 - white));
        var br = (byte)((r * white) + b);
        var bg = (byte)((g * white) + b);

        return Color.FromArgb(br, bg, b);
    }

    /// <summary>
    /// Combines two colors together with the specified amount.
    /// </summary>
    /// <param name="color">New color to layer atop</param>
    /// <param name="backColor">Original color to layer beneath</param>
    /// <param name="amount">Amount to prefer <see cref="color"/> over <see cref="backColor"/></param>
    public static Color Blend(Color color, Color backColor, double amount)
    {
        byte r = MixComponent(color.R, backColor.R, amount);
        byte g = MixComponent(color.G, backColor.G, amount);
        byte b = MixComponent(color.B, backColor.B, amount);
        return Color.FromArgb(r, g, b);
    }

    /// <summary>
    /// Combines two color components with the specified first color percent preference [0,1].
    /// </summary>
    /// <param name="a">First color's component</param>
    /// <param name="b">Second color's component</param>
    /// <param name="x">Percent preference of <paramref name="a"/> over <paramref name="b"/></param>
    public static byte MixComponent(byte a, byte b, double x) => (byte)((a * x) + (b * (1 - x)));
}
