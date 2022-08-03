using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade7(GameVersion Version) : EncounterTrade(Version), IMemoryOT
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7;
    public override int Location => Locations.LinkTrade6NPC;
    // immutable setters
    public byte OT_Memory { get => 1; set => throw new InvalidOperationException(); }
    public byte OT_Intensity { get => 3; set => throw new InvalidOperationException(); }
    public byte OT_Feeling { get => 5; set => throw new InvalidOperationException(); }
    public ushort OT_TextVar { get => 40; set => throw new InvalidOperationException(); }

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
