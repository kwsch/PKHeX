using System;
using static PKHeX.Core.Encounters8Nest;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Max Raid) Underground
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>, ILocation
{
    int ILocation.Location => MaxLair;
    private const ushort Location = MaxLair;
    public override string Name => "Max Lair Encounter";

    public EncounterStatic8U(ushort species, byte form, byte level) : base(GameVersion.SWSH) // no difference in met location for hosted raids
    {
        Species = species;
        Form = form;
        Level = level;
        DynamaxLevel = 8;
        FlawlessIVCount = 4;
    }

    public static EncounterStatic8U Read(ReadOnlySpan<byte> data)
    {
        var spec = ReadUInt16LittleEndian(data);
        var move1 = ReadUInt16LittleEndian(data[4..]);
        var move2 = ReadUInt16LittleEndian(data[6..]);
        var move3 = ReadUInt16LittleEndian(data[8..]);
        var move4 = ReadUInt16LittleEndian(data[10..]);
        var moves = new Moveset(move1, move2, move3, move4);

        return new EncounterStatic8U(spec, data[2], data[3])
        {
            Ability = (AbilityPermission)data[12],
            CanGigantamax = data[13] != 0,
            Moves = moves,
        };
    }
    protected override ushort GetLocation() => Location;

    // no downleveling, unlike all other raids
    protected override bool IsMatchLevel(PKM pk) => pk.Met_Level == Level;
    protected override bool IsMatchLocation(PKM pk) => Location == pk.Met_Location;

    public bool IsShinyXorValid(ushort pkShinyXor) => pkShinyXor is > 15 or 1;
}
