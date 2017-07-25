using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_1 = 151;
        internal const int MaxMoveID_1 = 165;
        internal const int MaxItemID_1 = 255; 
        internal const int MaxAbilityID_1 = 0;
        
        internal static readonly ushort[] Pouch_Items_RBY = Enumerable.Range(0, 7)     // 0-6
           .Concat(Enumerable.Range(10, 11))  // 10-20
           .Concat(Enumerable.Range(29, 15))  // 29-43
           .Concat(Enumerable.Range(45, 5))   // 45-49
           .Concat(Enumerable.Range(51, 8))   // 51-58
           .Concat(Enumerable.Range(60, 24))  // 60-83
           .Concat(Enumerable.Range(196, 55)) // 196-250
           .Select(i => (ushort)i).ToArray();

        internal static readonly int[] MovePP_RBY =
        {
            0,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 10, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
            00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
            00, 00, 00, 00, 00, 00
        };

        internal static readonly int[] TransferSpeciesDefaultAbility_1 = {92, 93, 94, 109, 110, 151};

        internal static readonly int[] TMHM_RBY =
        {
            005, 013, 014, 018, 025, 092, 032, 034, 036, 038,
            061, 055, 058, 059, 063, 006, 066, 068, 069, 099,
            072, 076, 082, 085, 087, 089, 090, 091, 094, 100,
            102, 104, 115, 117, 118, 120, 121, 126, 129, 130,
            135, 138, 143, 156, 086, 149, 153, 157, 161, 164,

            015, 019, 057, 070, 148
        };

        internal static readonly int[] G1CaterpieMoves = { 33, 81 };
        internal static readonly int[] G1WeedleMoves = { 40, 81 };
        internal static readonly int[] G1MetapodMoves = G1CaterpieMoves.Concat(new[] { 106 }).ToArray();
        internal static readonly int[] G1KakunaMoves = G1WeedleMoves.Concat(new[] { 106 }).ToArray();
        internal static readonly int[] G1Exeggcute_IncompatibleMoves = { 78, 77, 79 };

        internal static readonly int[] WildPokeBalls1 = {4};

        internal static readonly int[] FutureEvolutionsGen1 =
        {
            169,182,186,196,197,199,208,212,230,233,242,462,463,464,465,466,467,470,471,474,700
        };

        internal static readonly int[] FutureEvolutionsGen1_Gen2LevelUp =
        {
            // Crobat Espeon Umbreon Blissey
            169,196,197,242
        };
        internal static readonly int[] SpecialMinMoveSlots =
        {
            25, 26, 29, 30, 31, 32, 33, 34, 36, 38, 40, 59, 91, 103, 114, 121,
        };
        internal static readonly int[] Types_Gen1 =
        {
            0, 1, 2, 3, 4, 5, 7, 8, 20, 21, 22, 23, 24, 25, 26
        };
        internal static readonly int[] Species_NotAvailable_CatchRate =
        {
            12, 18, 31, 34, 36, 38, 45, 53, 59, 62, 65, 68, 71, 78, 91, 103, 121
        };
        internal static readonly int[] Stadium_CatchRate =
        {
            167, // Normal Box
            168, // Gorgeous Box
        };
        internal static readonly int[] Stadium_GiftSpecies =
        {
            001, // Bulbasaur
            004, // Charmander
            007, // Squirtle
            054, // Psyduck (Amnesia)
            106, // Hitmonlee
            107, // Hitmonchan
            133, // Eevee
            138, // Omanyte
            140, // Kabuto
        };
        internal static readonly int[] Trade_Evolution1 =
        {
            064,
            067,
            075,
            093
        };
    }
}
