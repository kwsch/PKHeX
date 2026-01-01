using System;
using static PKHeX.Core.Distribution3NY;

namespace PKHeX.Core;

public enum Distribution3NY : byte
{
    Evolution,
    Dragon,
    Monster,
    Halloween,
    EXDragon,
    UnknownSpring,
    Colosseum,
    Box,
    BabyTrade,
    SlitherSwim,
    AncientAliens,
    Sixth,
}

public static class Gen3PCNY
{
    // PCNYa never existed -- the memory card labelled "A" in produced OT: PCNYc
    private const string B = "PCNYb";
    private const string C = "PCNYc";
    private const string D = "PCNYd";

    extension(Distribution3NY dist)
    {
        public bool IsValidTrainerID(ushort tid) => tid is (not 0) and < 3000;
        public ushort GetTrainerID() => (ushort)Util.Rand.Next(1, 3000);

        public bool IsValidTrainerName(ReadOnlySpan<char> name) => dist switch
        {
            Evolution => name is B or C,

            Dragon => name is B or C or D, // only C and D, but B was used temporarily by staff to acquire some in the event of machine downtime

            Monster => name is B or C,
            Halloween => name is B or C,
            EXDragon => name is B or C,

            UnknownSpring => name is C or D,
            Colosseum => name is C or D,
            Box => name is C or D,

            BabyTrade => name is D,
            SlitherSwim => name is D,
            AncientAliens => name is D,
            Sixth => name is D,
            _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
        };

        public string GetTrainerName(bool pivot) => dist switch
        {
            Evolution => pivot ? C : B,

            Dragon => pivot ? D : C, // B was used temporarily by staff to acquire some in the event of machine downtime; don't use to generate.

            Monster => pivot ? C : B,
            Halloween => pivot ? C : B,
            EXDragon => pivot ? C : B,

            UnknownSpring => pivot ? D : C,
            Colosseum => pivot ? D : C,
            Box => pivot ? D : C,

            BabyTrade => D,
            SlitherSwim => D,
            AncientAliens => D,
            Sixth => D,
            _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
        };
    }
}
