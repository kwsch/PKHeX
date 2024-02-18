using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.Encounters8Nest;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Distributed Data)
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8ND : EncounterStatic8Nest<EncounterStatic8ND>
{
    /// <summary>
    /// Distribution raid index for <see cref="GameVersion.SWSH"/>
    /// </summary>
    public byte Index { get; }
    public override string Name => $"Distribution Raid Den Encounter - {Index:000}";

    public EncounterStatic8ND(byte lvl, byte dyna, byte flawless, byte index, [ConstantExpected] GameVersion game) : base(game)
    {
        Level = lvl;
        DynamaxLevel = dyna;
        FlawlessIVCount = flawless;
        Index = index;
    }

    public static EncounterStatic8ND Read(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        var d = data[13];
        var dlvl = (byte)(d & 0x7F);
        var gmax = d >= 0x80;
        var f = data[14];
        var flawless = (byte)(f & 0xF);
        var shiny = (f >> 4) switch
        {
            1 => Shiny.Never,
            2 => Shiny.Always,
            _ => Shiny.Random,
        };

        var move1 = ReadUInt16LittleEndian(data[4..]);
        var move2 = ReadUInt16LittleEndian(data[6..]);
        var move3 = ReadUInt16LittleEndian(data[8..]);
        var move4 = ReadUInt16LittleEndian(data[10..]);
        var moves = new Moveset(move1, move2, move3, move4);

        return new EncounterStatic8ND(data[12], dlvl, flawless, data[15], game)
        {
            Species = ReadUInt16LittleEndian(data),
            Form = data[2],
            Ability = (AbilityPermission)data[3],
            CanGigantamax = gmax,
            Moves = moves,
            Shiny = shiny,
        };
    }

    protected override bool IsMatchLevel(PKM pk)
    {
        var lvl = pk.MetLevel;

        if (lvl <= 25) // 1 or 2 stars
        {
            var met = pk.MetLocation;
            if (met <= byte.MaxValue && InaccessibleRank12DistributionLocations.Contains((byte)met))
                return false;
        }

        if (lvl == Level)
            return true;

        // Check downleveled (20-55)
        if (lvl > Level)
            return false;
        if (lvl is < 20 or > 55)
            return false;

        if (lvl % 5 != 0)
            return false;

        // shared nests can be down-leveled to any
        if (pk.MetLocation == SharedNest)
            return true;

        // native down-levels: only allow 1 rank down (1 badge 2star -> 25), (3badge 3star -> 35)
        var badges = (lvl - 20) / 5;
        return badges is 1 or 3 && !pk.IsShiny;
    }

    private const byte IndexMinDLC2 = 40;
    private const byte IndexMinDLC1 = 25;

    protected override bool IsMatchLocation(PKM pk)
    {
        var loc = pk.MetLocation;
        return loc is SharedNest || Index switch
        {
            >= IndexMinDLC2 => EncounterArea8.IsWildArea(loc),
            >= IndexMinDLC1 => EncounterArea8.IsWildArea8(loc) || EncounterArea8.IsWildArea8Armor(loc),
            _ => EncounterArea8.IsWildArea8(loc),
        };
    }

    protected override bool IsMatchSeed(PKM pk, ulong seed)
    {
        bool IsDownleveled = pk.MetLevel < Level;
        return Verify(pk, seed, IsDownleveled);
    }
}
