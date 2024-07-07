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
    private const string B = "PCNYb";
    private const string C = "PCNYc";
    private const string D = "PCNYd";

    public static bool IsValidTrainerID(this Distribution3NY dist, ushort tid) => tid is (not 0) and < 3000;

    public static ushort GetTrainerID(this Distribution3NY dist) => (ushort)Util.Rand.Next(1, 3000);

    public static bool IsValidTrainerName(this Distribution3NY dist, ReadOnlySpan<char> name) => dist switch
    {
        Evolution => name is B or C,

        Dragon => name is C or D,

        Monster => name is B or C,
        Halloween => name is B or C,
        EXDragon => name is B or C,

        UnknownSpring => name is C or D,
        Colosseum => name is C or D,
        Box => name is C or D,
        BabyTrade => name is C or D,
        SlitherSwim => name is C or D,
        AncientAliens => name is C or D,
        Sixth => name is C or D,
        _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
    };

    public static string GetTrainerName(this Distribution3NY dist, bool pivot) => dist switch
    {
        Evolution => pivot ? C : B,
        Dragon => pivot ? D : C,
        Monster => pivot ? C : B,
        Halloween => pivot ? C : B,
        EXDragon => pivot ? C : B,

        UnknownSpring => pivot ? D : C,
        Colosseum => pivot ? D : C,
        Box => pivot ? D : C,
        BabyTrade => pivot ? D : C,
        SlitherSwim => pivot ? D : C,
        AncientAliens => pivot ? D : C,
        Sixth => pivot ? D : C,
        _ => throw new ArgumentOutOfRangeException(nameof(dist), dist, null),
    };
}
