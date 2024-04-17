using System.Drawing;
using PKHeX.Core;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Utility class for getting the color of a <see cref="StatusCondition"/>.
/// </summary>
public static class StatusColor
{
    public static Color Sleep => Color.FromArgb(200, 200, 200);
    public static Color Freeze => Color.FromArgb(0, 255, 255);
    public static Color Paralysis => Color.FromArgb(255, 255, 0);
    public static Color Burn => Color.FromArgb(255, 0, 0);
    public static Color Poison => Color.FromArgb(128, 0, 255);
    public static Color PoisonBad => Color.FromArgb(200, 0, 255);
    public static Color None => Color.FromArgb(255, 255, 255, 255); // Transparent

    /// <summary>
    /// Gets the color of a <see cref="StatusCondition"/>.
    /// </summary>
    /// <param name="value">Status to get the color of.</param>
    /// <returns>Color of the status.</returns>
    public static Color GetStatusColor(int value) => ((StatusCondition)value).GetStatusColor();

    /// <inheritdoc cref="GetStatusColor(int)"/>
    public static Color GetStatusColor(this StatusType value) => value switch
    {
        StatusType.None => None,
        StatusType.Sleep => Sleep,
        StatusType.Freeze => Freeze,
        StatusType.Paralysis => Paralysis,
        StatusType.Burn => Burn,
        StatusType.Poison => Poison,
        _ => None,
    };

    /// <inheritdoc cref="GetStatusColor(int)"/>
    public static Color GetStatusColor(this PKM pk) => ((StatusCondition)pk.Status_Condition).GetStatusColor();

    /// <inheritdoc cref="GetStatusColor(int)"/>
    public static Color GetStatusColor(this StatusCondition value)
    {
        if (value == StatusCondition.None)
            return None;
        if (value < StatusCondition.Poison)
            return Sleep;
        if (value.HasFlag(StatusCondition.Poison))
            return Poison;
        if (value.HasFlag(StatusCondition.Freeze))
            return Freeze;
        if (value.HasFlag(StatusCondition.Paralysis))
            return Paralysis;
        if (value.HasFlag(StatusCondition.Burn))
            return Burn;
        if (value.HasFlag(StatusCondition.PoisonBad))
            return PoisonBad;
        return default;
    }
}
