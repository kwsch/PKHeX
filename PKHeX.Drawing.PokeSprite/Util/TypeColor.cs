using System;
using System.Drawing;
using PKHeX.Core;
using static PKHeX.Core.MoveType;

namespace PKHeX.Drawing.PokeSprite
{
    public static class TypeColor
    {
        public static Color GetTypeSpriteColor(byte type) => (MoveType)type switch
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
    }
}
