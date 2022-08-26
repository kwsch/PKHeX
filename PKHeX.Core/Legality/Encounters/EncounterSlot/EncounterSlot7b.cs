namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.GG"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot7b : EncounterSlot
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;

    public EncounterSlot7b(EncounterArea7b area, ushort species, byte min, byte max) : base(area, species, 0, min, max)
    {
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        pk.SetRandomEC();
        var pb = (PB7)pk;
        pb.ResetHeight();
        pb.ResetWeight();
        pb.ResetCP();
    }
}
