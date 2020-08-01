namespace PKHeX.Core
{
    public sealed class EncounterTrade6 : EncounterTrade, IMemoryOT
    {
        public int OT_Memory { get; set; }
        public int OT_Intensity { get; set; }
        public int OT_Feeling { get; set; }
        public int OT_TextVar { get; set; }

        public EncounterTrade6(int m, int i, int f, int v)
        {
            OT_Memory = m;
            OT_Intensity = i;
            OT_Feeling = f;
            OT_TextVar = v;
        }

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
