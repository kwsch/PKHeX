using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Tile type the <see cref="PKM"/> was encountered from.
    /// </summary>
    /// <remarks>
    /// Used in Generation 4 games, this value is set depending on what type of overworld tile the player is standing on when the <see cref="PKM"/> is obtained.
    /// Some locations have multiple tile types, requiring multiple values possible.
    /// </remarks>
    [Flags]
    public enum EncounterType
    {
        Undefined = 0,
        None                          = 1 << 00,
        RockSmash                     = 1 << 01,
        TallGrass                     = 1 << 02,
        DialgaPalkia                  = 1 << 04,
        Cave_HallOfOrigin             = 1 << 05,
        Surfing_Fishing               = 1 << 07,
        Building_EnigmaStone          = 1 << 09,
        MarshSafari                   = 1 << 10,
        Starter_Fossil_Gift_DP        = 1 << 12,
        DistortionWorld_Pt            = 1 << 23,
        Starter_Fossil_Gift_Pt_DPTrio = 1 << 24,
    }

    public static class EncounterTypeExtension
    {
        public static bool Contains(this EncounterType g1, int g2) => (g1 & (EncounterType)(1 << g2)) != 0;

        public static int GetIndex(this EncounterType g)
        {
            int val = (int) g;
            for (int i = 0; i < 8 * sizeof(EncounterType); i++)
            {
                val >>= 1;
                if (val == 0)
                    return i;
            }
            return 0;
        }
    }
}
