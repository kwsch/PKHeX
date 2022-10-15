using System;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Regular Raid Dens)
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8N : EncounterStatic8Nest<EncounterStatic8N>
{
    private readonly uint MinRank;
    private readonly uint MaxRank;
    private readonly byte NestID;

    private byte[] NestLocations => Encounters8Nest.NestLocations[NestID];

    public override byte Level { get => LevelMin; init { } }
    public override byte LevelMin => LevelCaps[MinRank * 2];
    public override byte LevelMax => LevelCaps[(MaxRank * 2) + 1];

    public EncounterStatic8N(byte nestID, uint minRank, uint maxRank, byte val, GameVersion game) : base(game)
    {
        NestID = nestID;
        MinRank = minRank;
        MaxRank = maxRank;
        DynamaxLevel = (byte)(MinRank + 1u);
        FlawlessIVCount = val;
    }

    private static readonly byte[] LevelCaps =
    {
        15, 20, // 0
        25, 30, // 1
        35, 40, // 2
        45, 50, // 3
        55, 60, // 4
    };

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        var met = pk.Met_Level;
        var metLevel = met - 15;
        var rank = ((uint)metLevel) / 10;
        if (rank > 4)
            return false;
        if (rank > MaxRank)
            return false;

        if (rank <= 1 && met <= byte.MaxValue)
        {
            if (InaccessibleRank12Nests.TryGetValue((byte)met, out var nests))
            {
                var nest = Array.IndexOf(nests, NestID);
                if (nest >= 0)
                    return false;
            }
        }

        if (rank < MinRank) // down-leveled
            return IsDownLeveled(pk, metLevel, met);

        return metLevel % 10 <= 5;
    }

    public bool IsDownLeveled(PKM pk)
    {
        var met = pk.Met_Level;
        var metLevel = met - 15;
        return met != LevelMax && IsDownLeveled(pk, metLevel, met);
    }

    private bool IsDownLeveled(PKM pk, int metLevel, int met)
    {
        if (metLevel % 5 != 0)
            return false;

        // shared nests can be down-leveled to any
        if (pk.Met_Location == SharedNest)
            return met >= 20;

        // native down-levels: only allow 1 rank down (1 badge 2star -> 25), (3badge 3star -> 35)
        return ((MinRank <= 1 && 1 <= MaxRank && met == 25)
                || (MinRank <= 2 && 2 <= MaxRank && met == 35)) && !pk.IsShiny;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        var loc = pk.Met_Location;
        if (loc == SharedNest)
            return true;
        if (loc > byte.MaxValue)
            return false;
        return Array.IndexOf(NestLocations, (byte)loc) >= 0;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        return base.IsMatchExact(pk, evo);
    }
}
