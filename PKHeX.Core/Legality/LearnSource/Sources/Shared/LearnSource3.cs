using System;

namespace PKHeX.Core;

public abstract class LearnSource3
{
    private protected readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"u8)); // same for all Gen3 games

    internal static ReadOnlySpan<ushort> TM_3 =>
    [
        264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
        241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
        218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
        280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
        259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
    ];

    internal static ReadOnlySpan<ushort> HM_3 => [ 15, 19, 57, 70, 148, 249, 127, 291 ];
}
