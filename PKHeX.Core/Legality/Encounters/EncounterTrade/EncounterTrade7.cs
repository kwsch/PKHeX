namespace PKHeX.Core;

/// <summary>
/// Generation 7 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade7(GameVersion Version) : EncounterTrade(Version), IMemoryOTReadOnly
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7;
    public override int Location => Locations.LinkTrade6NPC;
    public byte OT_Memory => 1;
    public byte OT_Intensity => 3;
    public byte OT_Feeling => 5;
    public ushort OT_TextVar => 40;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pk7 = (PK7)pk;
        pk7.OT_Memory = OT_Memory;
        pk7.OT_Intensity = OT_Intensity;
        pk7.OT_Feeling = OT_Feeling;
        pk7.OT_TextVar = OT_TextVar;
    }
}
