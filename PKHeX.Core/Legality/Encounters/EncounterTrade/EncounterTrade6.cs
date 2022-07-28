namespace PKHeX.Core;

/// <summary>
/// Generation 6 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade6(GameVersion Version, byte OT_Memory, byte OT_Intensity, byte OT_Feeling, ushort OT_TextVar) : EncounterTrade(Version), IMemoryOT
{
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    public override int Location => Locations.LinkTrade6NPC;
    public byte OT_Memory { get; set; } = OT_Memory;
    public byte OT_Intensity { get; set; } = OT_Intensity;
    public byte OT_Feeling { get; set; } = OT_Feeling;
    public ushort OT_TextVar { get; set; } = OT_TextVar;

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
