namespace PKHeX.Core;

/// <summary>
/// Generation 6 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade6(GameVersion Version, byte OT_Memory, byte OT_Intensity, byte OT_Feeling, ushort OT_TextVar) : EncounterTrade(Version), IMemoryOTReadOnly
{
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    public override int Location => Locations.LinkTrade6NPC;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        if (pk is IMemoryOT o)
        {
            o.OT_Memory = OT_Memory;
            o.OT_Intensity = OT_Intensity;
            o.OT_Feeling = OT_Feeling;
            o.OT_TextVar = OT_TextVar;
        }
    }
}
