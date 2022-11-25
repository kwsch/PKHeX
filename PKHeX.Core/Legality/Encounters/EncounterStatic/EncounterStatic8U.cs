using System;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Max Raid) Underground
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>
{
    public override int Location => MaxLair;

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
        var spec = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data);
        var move1 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[4..]);
        var move2 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[6..]);
        var move3 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[8..]);
        var move4 = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(data[10..]);
        var moves = new Moveset(move1, move2, move3, move4);

        return new EncounterStatic8U(spec, data[2], data[3])
        {
            Ability = (AbilityPermission)data[12],
            CanGigantamax = data[13] != 0,
            Moves = moves,
        };
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        return base.IsMatchExact(pk, evo);
    }

    // no downleveling, unlike all other raids
    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo) => pk.Met_Level == Level;
}
