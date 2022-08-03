namespace PKHeX.Core;

/// <summary>
/// Generation 7 Static Encounter (<see cref="GameVersion.GG"/>
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic7b(GameVersion Version) : EncounterStatic(Version)
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        pk.SetRandomEC();
        var pb = (PB7)pk;
        pb.ResetHeight();
        pb.ResetWeight();
        pb.ResetCP();
    }
}
