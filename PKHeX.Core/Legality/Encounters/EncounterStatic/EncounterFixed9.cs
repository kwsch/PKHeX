using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Fixed Spawn Encounter
/// </summary>
public sealed record EncounterFixed9 : EncounterStatic, IGemType
{
    public override int Generation => 9;
    public override int Location => Location0;
    public override EntityContext Context => EntityContext.Gen9;
    public GemType TeraType { get; private init; }
    private byte Location0 { get; init; }
    private byte Location1 { get; init; }
    private byte Location2 { get; init; }

    public static EncounterFixed9[] GetArray(ReadOnlySpan<byte> data)
    {
        const int size = 0x14;
        var count = data.Length / size;
        var result = new EncounterFixed9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * size, size));
        return result;
    }

    private EncounterFixed9() : base(GameVersion.SV) { }

    private static EncounterFixed9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = BinaryPrimitives.ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Level = data[0x03],
        FlawlessIVCount = data[0x04],
        TeraType = (GemType)data[0x05],
        Gender = (sbyte)data[0x06],
        // 1 byte reserved
        Moves = new Moveset(
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x08..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0A..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0C..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0E..])),
        Location0 = data[0x10],
        Location1 = data[0x11],
        Location2 = data[0x12],
        // 1 byte reserved
    };

    protected override bool IsMatchLocation(PKM pk)
    {
        if (!pk.HasOriginalMetLocation)
            return true;
        var loc = pk.Met_Location;
        return loc == Location0 || loc == Location1 || loc == Location2;
    }

    protected override bool IsMatchForm(PKM pk, EvoCriteria evo)
    {
        if (Species is (int)Core.Species.Deerling or (int)Core.Species.Sawsbuck)
            return pk.Form <= 3;
        return base.IsMatchForm(pk, evo);
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (TeraType != GemType.Random && pk is ITeraType t && !Tera9RNG.IsMatchTeraType(TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return base.IsMatchExact(pk, evo);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        var pk9 = (PK9)pk;
        pk9.Obedience_Level = (byte)pk9.Met_Level;
        var type = Tera9RNG.GetTeraType(Util.Rand.Rand64(), TeraType, Species, Form);
        pk9.TeraTypeOriginal = (MoveType)type;
        if (criteria.TeraType != -1 && type != criteria.TeraType)
            pk9.SetTeraType(type); // sets the override type

        pk9.HeightScalar = PokeSizeUtil.GetRandomScalar();
        pk9.WeightScalar = PokeSizeUtil.GetRandomScalar();
        pk9.Scale = PokeSizeUtil.GetRandomScalar();
    }
}
