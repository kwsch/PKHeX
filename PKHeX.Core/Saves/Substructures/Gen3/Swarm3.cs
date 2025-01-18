using System;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Swarm3(Memory<byte> Raw)
{
    public const int SIZE = 0x14;

    public Span<byte> Data => Raw.Span;

    public ushort Gen3Species { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public byte MapNum    { get => Data[2]; set => Data[2] = value; }
    public byte MapGroup  { get => Data[3]; set => Data[3] = value; }
    public byte Level     { get => Data[4]; set => Data[4] = value; }
    public byte Unused1   { get => Data[5]; set => Data[5] = value; }
    public ushort Unused2 { get => ReadUInt16LittleEndian(Data[0x6..]); set => WriteUInt16LittleEndian(Data[0x6..], value); }
    public ushort Move1   { get => ReadUInt16LittleEndian(Data[0x8..]); set => WriteUInt16LittleEndian(Data[0x8..], value); }
    public ushort Move2   { get => ReadUInt16LittleEndian(Data[0xA..]); set => WriteUInt16LittleEndian(Data[0xA..], value); }
    public ushort Move3   { get => ReadUInt16LittleEndian(Data[0xC..]); set => WriteUInt16LittleEndian(Data[0xC..], value); }
    public ushort Move4   { get => ReadUInt16LittleEndian(Data[0xE..]); set => WriteUInt16LittleEndian(Data[0xE..], value); }
    public byte Unused3 { get => Data[0x10]; set => Data[0x10] = value; }
    public byte EncounterProbability { get => Data[0x11]; set => Data[0x11] = value; }
    public ushort DaysLeft { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }

    public Swarm3(Species species, byte level, byte map, Move m1, Move m2 = 0, Move m3 = 0, Move m4 = 0) : this(new byte[SIZE])
    {
        Gen3Species = SpeciesConverter.GetInternal3((ushort)species);
        Level = level;
        MapNum = map;
        Move1 = (ushort)m1;
        Move2 = (ushort)m2;
        Move3 = (ushort)m3;
        Move4 = (ushort)m4;
        EncounterProbability = 50;
        DaysLeft = 1337;
    }
}

public static class Swarm3Details
{
    /// <summary>
    /// Hardcoded templates available to set to the save file.
    /// </summary>
    /// <remarks>Ruby/Sapphire</remarks>
    public static readonly Swarm3[] Swarms_RS =
    [
        new(Surskit, 03, 0x11, Bubble, QuickAttack), // Route 102
        new(Surskit, 15, 0x1D, Bubble, QuickAttack), // Route 114
        new(Surskit, 15, 0x20, Bubble, QuickAttack), // Route 117
        new(Surskit, 28, 0x23, Bubble, QuickAttack), // Route 120
        new(Skitty,  15, 0x1F, Growl,  Tackle),      // Route 116
    ];

    /// <summary>
    /// Hardcoded templates available to set to the save file.
    /// </summary>
    /// <remarks><see cref="GameVersion.E"/></remarks>
    public static readonly Swarm3[] Swarms_E =
    [
        new(Seedot,  03, 0x11, Bide,      Harden,      LeechSeed),              // Route 102
        new(Nuzleaf, 15, 0x1D, Harden,    Growth,      NaturePower, LeechSeed), // Route 114
        new(Seedot,  13, 0x20, Harden,    Growth,      NaturePower, LeechSeed), // Route 117
        new(Seedot,  25, 0x23, GigaDrain, Frustration, SolarBeam,   LeechSeed), // Route 120
        new(Skitty,  08, 0x1F, Growl,     Tackle,      TailWhip,    Attract),   // Route 116
    ];
}
