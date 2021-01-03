using System;
using System.Collections.Generic;
using static PKHeX.Core.Ball;

namespace PKHeX.Core
{
    internal static class BallUseLegality
    {
        internal static ICollection<int> GetWildBalls(int generation, GameVersion game) => generation switch
        {
            1 => WildPokeBalls1,
            2 => WildPokeBalls2,
            3 => WildPokeBalls3,
            4 => GameVersion.HGSS.Contains(game) ? WildPokeBalls4_HGSS : WildPokeBalls4_DPPt,
            5 => WildPokeBalls5,
            6 => WildPokeballs6,
            7 => GameVersion.Gen7b.Contains(game) ? WildPokeballs7b : WildPokeballs7,
            8 => GameVersion.GO == game ? WildPokeballs8g : WildPokeballs8,
            _ => Array.Empty<int>()
        };

        private static readonly int[] WildPokeBalls1 = { 4 };
        private static readonly int[] WildPokeBalls2 = WildPokeBalls1;

        private static readonly HashSet<int> WildPokeBalls3 = new()
        {
            (int)Master, (int)Ultra, (int)Great, (int)Poke, (int)Net, (int)Dive,
            (int)Nest, (int)Repeat, (int)Timer, (int)Luxury, (int)Premier,
        };

        private static readonly HashSet<int> WildPokeBalls4_DPPt = new(WildPokeBalls3)
        {
            (int)Dusk, (int)Heal, (int)Quick,
        };

        private static readonly HashSet<int> WildPokeBalls4_HGSS = new(WildPokeBalls4_DPPt)
        {
            (int)Fast, (int)Level, (int)Lure, (int)Heavy, (int)Love, (int)Friend, (int)Moon,
        };

        private static readonly HashSet<int> WildPokeBalls5 = WildPokeBalls4_DPPt;

        internal static readonly HashSet<int> DreamWorldBalls = new(WildPokeBalls5) {(int)Dream};

        internal static readonly HashSet<int> WildPokeballs6 = WildPokeBalls5; // Same as Gen5

        internal static readonly HashSet<int> WildPokeballs7 = new(WildPokeBalls4_HGSS) {(int)Beast}; // Same as HGSS + Beast

        private static readonly HashSet<int> WildPokeballs7b = new()
        {
            (int)Master, (int)Ultra, (int)Great, (int)Poke,
            (int)Premier,
        };

        private static readonly HashSet<int> WildPokeballs8g = new()
        {
                         (int)Ultra, (int)Great, (int)Poke,
            (int)Premier,
        };

        internal static readonly HashSet<int> WildPokeballs8 = new(WildPokeballs7) // All Except Cherish
        {
            (int)Dream,
            (int)Safari,
            (int)Sport,
            // no cherish ball
        };
    }
}
