using System;

namespace PKHeX.Core;

internal static class LearnSource2
{
    internal static ReadOnlySpan<byte> TMHM_GSC =>
    [
        223, 029, 174, 205, 046, 092, 192, 249, 244, 237,
        241, 230, 173, 059, 063, 196, 182, 240, 202, 203,
        218, 076, 231, 225, 087, 089, 216, 091, 094, 247,
        189, 104, 008, 207, 214, 188, 201, 126, 129, 111,
        009, 138, 197, 156, 213, 168, 211, 007, 210, 171,

        015, 019, 057, 070, 148, 250, 127,
    ];

    internal static ReadOnlySpan<byte> Tutors_GSC => [ (int)Move.Flamethrower, (int)Move.Thunderbolt, (int)Move.IceBeam ];
}
