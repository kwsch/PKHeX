using System;
using PKHeX.Core;
using SkiaSharp;
using static PKHeX.Core.MoveType;

namespace PKHeX.Drawing.PokeSprite.Avalonia;

/// <summary>
/// Utility class for getting the color of a <see cref="MoveType"/>.
/// </summary>
public static class TypeColor
{
    /// <summary>
    /// Gets the color of a <see cref="MoveType"/>.
    /// </summary>
    /// <param name="type">Type to get the color of.</param>
    /// <returns>Color of the type.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static SKColor GetTypeSpriteColor(byte type) => ((MoveType)type).GetTypeSpriteColor();

    public static SKColor GetTypeSpriteColor(this MoveType type) => type switch
    {
        Normal   => new SKColor(159, 161, 159),
        Fighting => new SKColor(255, 128, 000),
        Flying   => new SKColor(129, 185, 239),
        Poison   => new SKColor(143, 065, 203),
        Ground   => new SKColor(145, 081, 033),
        Rock     => new SKColor(175, 169, 129),
        Bug      => new SKColor(145, 161, 025),
        Ghost    => new SKColor(112, 065, 112),
        Steel    => new SKColor(096, 161, 184),
        Fire     => new SKColor(230, 040, 041),
        Water    => new SKColor(041, 128, 239),
        Grass    => new SKColor(063, 161, 041),
        Electric => new SKColor(250, 192, 000),
        Psychic  => new SKColor(239, 065, 121),
        Ice      => new SKColor(063, 216, 255),
        Dragon   => new SKColor(080, 097, 225),
        Dark     => new SKColor(080, 065, 063),
        Fairy    => new SKColor(239, 113, 239),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    /// <summary>
    /// Color to show for a <see cref="MoveType"/> of <see cref="TeraTypeUtil.Stellar"/>.
    /// </summary>
    public static SKColor Stellar => new(255, 255, 224); // LightYellow

    /// <summary>
    /// Gets the color of a <see cref="MoveType"/> for a Tera sprite.
    /// </summary>
    public static SKColor GetTeraSpriteColor(byte elementalType)
    {
        if (elementalType == TeraTypeUtil.Stellar)
            return Stellar;
        return GetTypeSpriteColor(elementalType);
    }
}
