using System;

namespace PKHeX.Core;

public abstract class LearnSource5
{
    private protected readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"u8));

    internal static ReadOnlySpan<ushort> TMHM_BW =>
    [
        468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
        241, 269, 058, 059, 063, 113, 182, 240, 477, 219,
        218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
        280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
        259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
        502, 411, 412, 206, 503, 374, 451, 507, 510, 511,
        261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
        444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
        404, 525, 526, 398, 138, 447, 207, 365, 369, 164,
        430, 433, 528, 249, 555,

        015, 019, 057, 070, 127, 291,
    ];

    internal static ReadOnlySpan<ushort> TypeTutor567 =>
    [
        (int)Move.GrassPledge,
        (int)Move.FirePledge,
        (int)Move.WaterPledge,
        (int)Move.FrenzyPlant,
        (int)Move.BlastBurn,
        (int)Move.HydroCannon,
        (int)Move.DracoMeteor,
        (int)Move.DragonAscent,
    ];
}
