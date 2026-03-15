using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing.PokeSprite.Avalonia;

/// <summary>
/// Utility class for getting the color of a <see cref="StatusCondition"/>.
/// </summary>
public static class StatusColor
{
    public static SKColor Sleep => new(200, 200, 200);
    public static SKColor Freeze => new(0, 255, 255);
    public static SKColor Paralysis => new(255, 255, 0);
    public static SKColor Burn => new(255, 0, 0);
    public static SKColor Poison => new(128, 0, 255);
    public static SKColor PoisonBad => new(200, 0, 255);
    public static SKColor None => new(255, 255, 255, 255);

    /// <summary>
    /// Gets the color of a <see cref="StatusCondition"/>.
    /// </summary>
    /// <param name="value">Status to get the color of.</param>
    /// <returns>Color of the status.</returns>
    public static SKColor GetStatusColor(int value) => ((StatusCondition)value).GetStatusColor();

    /// <inheritdoc cref="GetStatusColor(int)"/>
    public static SKColor GetStatusColor(this StatusType value) => value switch
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
    public static SKColor GetStatusColor(this PKM pk) => ((StatusCondition)pk.Status_Condition).GetStatusColor();

    /// <inheritdoc cref="GetStatusColor(int)"/>
    public static SKColor GetStatusColor(this StatusCondition value)
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
