using System;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_7b = 809; // Melmetal
    internal const int MaxMoveID_7b = 742; // Double Iron Bash
    internal const int MaxItemID_7b = 1057; // Magmar Candy
    internal const int MaxBallID_7b = (int)Ball.Beast;
    internal const int MaxGameID_7b = (int)GameVersion.GE;
    internal const int MaxAbilityID_7b = MaxAbilityID_7_USUM;
    internal static readonly ushort[] HeldItems_GG = Array.Empty<ushort>();
    public const byte AwakeningMax = 200;
    
    #region Moves

    public static bool IsAllowedMoveGG(ushort move) => AllowedMovesGG.BinarySearch(move) >= 0;

    private static ReadOnlySpan<ushort> AllowedMovesGG => new ushort[]
    {
        000, 001, 002, 003, 004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061, 062, 063, 064, 065, 066, 067, 068, 069,
        070, 071, 072, 073, 074, 075, 076, 077, 078, 079,
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
        090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
        120, 121, 122, 123, 124, 125, 126, 127, 128, 129,
        130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
        160, 161, 162, 163, 164,

        182, 188, 200, 224, 227, 231, 242, 243, 247, 252,
        257, 261, 263, 269, 270, 276, 280, 281, 339, 347,
        355, 364, 369, 389, 394, 398, 399, 403, 404, 405,
        406, 417, 420, 430, 438, 446, 453, 483, 492, 499,
        503, 504, 525, 529, 583, 585, 603, 605, 606, 607,
        729, 730, 731, 733, 734, 735, 736, 737, 738, 739,
        740, 742,
    };

    #endregion
}
