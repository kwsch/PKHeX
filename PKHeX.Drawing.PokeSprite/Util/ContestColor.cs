using System;
using System.Drawing;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Utility class for getting the color of a contest category.
/// </summary>
public static class ContestColor
{
    public static Color Cool   => Color.FromArgb(248, 152, 096);
    public static Color Beauty => Color.FromArgb(128, 152, 248);
    public static Color Cute   => Color.FromArgb(248, 168, 208);
    public static Color Clever => Color.FromArgb(112, 224, 112);
    public static Color Tough  => Color.FromArgb(248, 240, 056);

    /// <summary>
    /// Retrieves a predefined color based on the specified index.
    /// </summary>
    /// <param name="index">The index of the color to retrieve. Valid values are 0 through 4.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than 4.</exception>
    public static Color GetColor(int index) => index switch
    {
        0 => Cool,
        1 => Beauty,
        2 => Cute,
        3 => Clever,
        4 => Tough,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
    };
}
