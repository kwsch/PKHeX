using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade8 : EncounterTrade, IDynamaxLevel, IRelearn, IMemoryOT
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;
    public override int Location => Locations.LinkTrade6NPC;
    public IReadOnlyList<int> Relearn { get; init; } = Array.Empty<int>();

    public ushort OT_TextVar { get; set; }
    public byte OT_Memory { get; set; }
    public byte OT_Feeling { get; set; }
    public byte OT_Intensity { get; set; }
    public byte DynamaxLevel { get; set; }
    public byte FlawlessIVCount { get; init; }
    public override Shiny Shiny { get; }

    public EncounterTrade8(GameVersion game, int species, byte level, byte memory, ushort arg, byte feel, byte intensity, Shiny shiny = Shiny.Never) : base(game)
    {
        Species = species;
        Level = level;
        Shiny = shiny;

        OT_Memory = memory;
        OT_TextVar = arg;
        OT_Feeling = feel;
        OT_Intensity = intensity;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;
        if (pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return base.IsMatchExact(pk, evo);
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        pk.SetRelearnMoves(Relearn);

        var pk8 = (PK8)pk;
        pk8.DynamaxLevel = DynamaxLevel;
        pk8.HT_Language = (byte)sav.Language;
        pk8.OT_Memory = OT_Memory;
        pk8.OT_TextVar = OT_TextVar;
        pk8.OT_Feeling = OT_Feeling;
        pk8.OT_Intensity = OT_Intensity;
    }
}
