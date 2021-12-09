namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Trade Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterTrade"/>
    public sealed record EncounterTrade6(GameVersion Version, int OT_Memory, int OT_Intensity, int OT_Feeling, int OT_TextVar) : EncounterTrade(Version), IMemoryOT
    {
        public override int Generation => 6;
        public override int Location => Locations.LinkTrade6NPC;
        public int OT_Memory { get; set; } = OT_Memory;
        public int OT_Intensity { get; set; } = OT_Intensity;
        public int OT_Feeling { get; set; } = OT_Feeling;
        public int OT_TextVar { get; set; } = OT_TextVar;

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
}
