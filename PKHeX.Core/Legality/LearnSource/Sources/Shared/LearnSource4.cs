using System;

namespace PKHeX.Core;

public abstract class LearnSource4
{
    private protected static readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"u8));

    /// <summary>
    /// Gets the preferred list of HM moves to disallow on transfer from <see cref="PK4"/> to <see cref="PK5"/>.
    /// </summary>
    /// <remarks>
    /// If Defog is in the moveset, then we prefer HG/SS (remove Whirlpool) over D/P/Pt.
    /// Defog is a competitively viable move, while Whirlpool is not really useful.
    /// </remarks>
    /// <param name="hasDefog">True if the current moveset has <see cref="Move.Defog"/>.</param>
    public static ReadOnlySpan<ushort> GetPreferredTransferHMs(bool hasDefog) => hasDefog ? HM_HGSS : HM_DPPt;

    internal static ReadOnlySpan<ushort> SpecialTutors_Compatibility_4_BlastBurn   => [ 006, 157, 257, 392 ];
    internal static ReadOnlySpan<ushort> SpecialTutors_Compatibility_4_HydroCannon => [ 009, 160, 260, 395 ];
    internal static ReadOnlySpan<ushort> SpecialTutors_Compatibility_4_FrenzyPlant => [ 003, 154, 254, 389 ];
    internal static ReadOnlySpan<ushort> SpecialTutors_Compatibility_4_DracoMeteor => [ 147, 148, 149, 230, 329, 330, 334, 371, 372, 373, 380, 381, 384, 443, 444, 445, 483, 484, 487 ];

    internal static ReadOnlySpan<ushort> Tutors_4 =>
    [
        291, 189, 210, 196, 205, 009, 007, 276, 008, 442, 401, 466, 380, 173, 180, 314,
        270, 283, 200, 246, 235, 324, 428, 410, 414, 441, 239, 402, 334, 393, 387, 340,
        271, 257, 282, 389, 129, 253, 162, 220, 081, 366, 356, 388, 277, 272, 215, 067,
        143, 335, 450, 029,
    ];

    internal static ReadOnlySpan<ushort> TM_4 =>
    [
        264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
        241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
        218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
        280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
        259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
        355, 411, 412, 206, 362, 374, 451, 203, 406, 409,
        261, 318, 373, 153, 421, 371, 278, 416, 397, 148,
        444, 419, 086, 360, 014, 446, 244, 445, 399, 157,
        404, 214, 363, 398, 138, 447, 207, 365, 369, 164,
        430, 433,
    ];

    internal static ReadOnlySpan<ushort> HM_DPPt =>
    [
        (int)Move.Cut,
        (int)Move.Fly,
        (int)Move.Surf,
        (int)Move.Strength,
        (int)Move.Defog,
        (int)Move.RockSmash,
        (int)Move.Waterfall,
        (int)Move.RockClimb,
    ];

    internal static ReadOnlySpan<ushort> HM_HGSS =>
    [
        (int)Move.Cut,
        (int)Move.Fly,
        (int)Move.Surf,
        (int)Move.Strength,
        (int)Move.Whirlpool,
        (int)Move.RockSmash,
        (int)Move.Waterfall,
        (int)Move.RockClimb,
    ];
}
