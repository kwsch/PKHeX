namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot9 : EncounterSlot
{
    public override int Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public sbyte Gender { get; }

    public EncounterSlot9(EncounterArea9 area, ushort species, byte form, byte min, byte max, sbyte gender) : base(area, species, form, min, max)
    {
        Gender = gender;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pk9 = (PK9)pk;
        pk9.Obedience_Level = (byte)pk.Met_Level;
        var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, Util.Rand.Rand64());
        pk9.TeraTypeOriginal = (MoveType)type;
        if (criteria.TeraType != -1 && type != criteria.TeraType)
            pk9.SetTeraType(type); // sets the override type
        if (Gender != -1)
            pk.Gender = (byte)Gender;
        pk9.Scale = PokeSizeUtil.GetRandomScalar();
        pk9.EncryptionConstant = Util.Rand32();
    }
}
