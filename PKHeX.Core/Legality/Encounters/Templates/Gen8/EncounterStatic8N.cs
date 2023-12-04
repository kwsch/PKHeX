using System;
using static PKHeX.Core.Encounters8Nest;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Regular Raid Dens)
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8N : EncounterStatic8Nest<EncounterStatic8N>
{
    private readonly byte MinRank;
    private readonly byte MaxRank;
    private readonly byte NestID;

    private string RankString=> MinRank == MaxRank ? $"{MinRank+1}" : $"{MinRank+1}-{MaxRank+1}";
    public override string Name => $"Stock Raid Den Encounter [{NestID:000}] {RankString}★";

    private byte[] NestLocations => Encounters8Nest.NestLocations[NestID];

    public override byte Level { get => LevelMin; init { } }
    public override byte LevelMin => LevelCaps[MinRank * 2];
    public override byte LevelMax => LevelCaps[(MaxRank * 2) + 1];

    public EncounterStatic8N(byte nestID, byte minRank, byte maxRank, byte val, GameVersion game) : base(game)
    {
        NestID = nestID;
        MinRank = minRank;
        MaxRank = maxRank;
        DynamaxLevel = (byte)(MinRank + 1u);
        FlawlessIVCount = val;
    }

    public static EncounterStatic8N Read(ReadOnlySpan<byte> data, GameVersion game) => new(data[6], data[7], data[8], data[9], game)
    {
        Species = ReadUInt16LittleEndian(data),
        Form = data[2],
        Gender = data[3],
        Ability = (AbilityPermission)data[4],
        CanGigantamax = data[5] != 0,
    };

    private static ReadOnlySpan<byte> LevelCaps =>
    [
        15, 20, // 0
        25, 30, // 1
        35, 40, // 2
        45, 50, // 3
        55, 60, // 4
    ];

    protected override bool IsMatchLevel(PKM pk)
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
            if (IsInaccessibleRank12Nest(NestID, (byte)met))
                return false;
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

    public (bool Possible, bool ForceNoShiny) IsPossibleSeed<T>(T pk, ulong seed, bool checkDmax) where T : PKM
    {
        var rand = new Xoroshiro128Plus(seed);
        var levelDelta = (int)rand.NextInt(6);
        var met = pk.Met_Level;
        for (int i = MaxRank; i >= MinRank; i--)
        {
            // check for dmax level
            // 5 star uses rand(3), otherwise rand(2)
            // some raids can be 5 star and below, so check for both possibilities
            if (checkDmax && pk is IDynamaxLevelReadOnly r)
            {
                var current = r.DynamaxLevel;
                int expectD = GetInitialDynamaxLevel(rand, i);
                if (expectD > current)
                    continue;
            }
            var levelMin = LevelCaps[i * 2];
            var expect = levelMin + levelDelta;
            if (expect == met)
                return (Possible: true, ForceNoShiny: false);

            // Check for down-leveled
            if (met > levelMin)
                break;

            if (IsDownLeveled(pk, met - 15, met))
                return (Possible: true, ForceNoShiny: true);
        }
        return default;
    }

    protected override bool IsMatchSeed(PKM pk, ulong seed)
    {
        var (possible, forceNoShiny) = IsPossibleSeed(pk, seed, true);
        if (!possible)
            return false;
        return Verify(pk, seed, forceNoShiny);
    }

    private static byte GetInitialDynamaxLevel(Xoroshiro128Plus rand, int rank)
    {
        var baseValue = rank == 4 ? 6 : (rank + 1);
        var deltaRange = rank == 4 ? 3u : 2u;
        var boost = (int)rand.NextInt(deltaRange);
        return (byte)(baseValue + boost);
    }

    protected override bool TryApply(PK8 pk, ulong seed, Span<int> iv, GenerateParam8 param, EncounterCriteria criteria)
    {
        var (possible, noShiny) = IsPossibleSeed(pk, seed, false);
        if (!possible)
            return false;
        if (noShiny) // Should never be hit via ctor as we don't generate downleveled.
        {
            if (criteria.Shiny.IsShiny())
                return false;
            param = param with { Shiny = Shiny.Never };
        }
        return base.TryApply(pk, seed, iv, param, criteria);
    }

    protected override void FinishCorrelation(PK8 pk, ulong seed)
    {
        var xoro = new Xoroshiro128Plus(seed);
        var levelDelta = (int)xoro.NextInt(6);

        var levelMin = LevelCaps[MinRank * 2];
        var level = levelMin + levelDelta;
        pk.Met_Level = (byte)level;
        pk.CurrentLevel = (byte)level;
        pk.DynamaxLevel = GetInitialDynamaxLevel(xoro, MinRank);
    }
}
