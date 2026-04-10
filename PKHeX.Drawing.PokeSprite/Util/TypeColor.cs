using System;
using System.Drawing;
using PKHeX.Core;
using static PKHeX.Core.MoveType;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Utility class for getting the color of a <see cref="MoveType"/>.
/// </summary>
public static class TypeColor
{
    /// <summary>
    /// Gets the color of a <see cref="MoveType"/>, based on mainline sprite colors.
    /// </summary>
    /// <param name="type">Type to get the color of.</param>
    /// <returns>Color of the type.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Color GetTypeSpriteColor(byte type)
        => ((MoveType)type).GetTypeSpriteColor();

    /// <summary>
    /// Gets the color of a <see cref="MoveType"/>, based on mainline sprite colors.
    /// </summary>
    /// <param name="type">Type to get the color of.</param>
    /// <returns>Color of the type.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Color GetTypeSpriteColorChampions(byte type)
        => ((MoveType)type).GetTypeSpriteColorChampions();

    extension(MoveType type)
    {
        /// <inheritdoc cref="GetTypeSpriteColor(byte)"/>
        public Color GetTypeSpriteColor() => type switch
        {
            Normal   => Color.FromArgb(159, 161, 159),
            Fighting => Color.FromArgb(255, 128, 000),
            Flying   => Color.FromArgb(129, 185, 239),
            Poison   => Color.FromArgb(143, 065, 203),
            Ground   => Color.FromArgb(145, 081, 033),
            Rock     => Color.FromArgb(175, 169, 129),
            Bug      => Color.FromArgb(145, 161, 025),
            Ghost    => Color.FromArgb(112, 065, 112),
            Steel    => Color.FromArgb(096, 161, 184),
            Fire     => Color.FromArgb(230, 040, 041),
            Water    => Color.FromArgb(041, 128, 239),
            Grass    => Color.FromArgb(063, 161, 041),
            Electric => Color.FromArgb(250, 192, 000),
            Psychic  => Color.FromArgb(239, 065, 121),
            Ice      => Color.FromArgb(063, 216, 255),
            Dragon   => Color.FromArgb(080, 097, 225),
            Dark     => Color.FromArgb(080, 065, 063),
            Fairy    => Color.FromArgb(239, 113, 239),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        /// <inheritdoc cref="GetTypeSpriteColorChampions(byte)"/>
        public Color GetTypeSpriteColorChampions() => type switch
        {
            Normal =>   Color.FromArgb(0x99, 0x99, 0x99),
            Fighting => Color.FromArgb(0xff, 0xa2, 0x02),
            Flying =>   Color.FromArgb(0x95, 0xc9, 0xff),
            Poison =>   Color.FromArgb(0x99, 0x4d, 0xcf),
            Ground =>   Color.FromArgb(0xab, 0x79, 0x39),
            Rock =>     Color.FromArgb(0xbc, 0xb8, 0x89),
            Bug =>      Color.FromArgb(0x9f, 0xa4, 0x24),
            Ghost =>    Color.FromArgb(0x6e, 0x45, 0x70),
            Steel =>    Color.FromArgb(0x6a, 0xae, 0xd3),
            Fire =>     Color.FromArgb(0xff, 0x61, 0x2c),
            Water =>    Color.FromArgb(0x29, 0x92, 0xff),
            Grass =>    Color.FromArgb(0x42, 0xbf, 0x24),
            Electric => Color.FromArgb(0xff, 0xdb, 0x00),
            Psychic =>  Color.FromArgb(0xff, 0x63, 0x7f),
            Ice =>      Color.FromArgb(0x42, 0xd8, 0xff),
            Dragon =>   Color.FromArgb(0x54, 0x62, 0xd6),
            Dark =>     Color.FromArgb(0x4f, 0x47, 0x47),
            Fairy =>    Color.FromArgb(0xff, 0xb1, 0xff),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    /// <summary>
    /// Color to show for a <see cref="MoveType"/> of <see cref="TeraTypeUtil.Stellar"/>.
    /// </summary>
    public static Color Stellar => Color.LightYellow;

    /// <summary>
    /// Gets the color of a <see cref="MoveType"/> for a Tera sprite.
    /// </summary>
    public static Color GetTeraSpriteColor(byte elementalType)
    {
        if (elementalType == TeraTypeUtil.Stellar)
            return Stellar;
        return GetTypeSpriteColor(elementalType);
    }
}
