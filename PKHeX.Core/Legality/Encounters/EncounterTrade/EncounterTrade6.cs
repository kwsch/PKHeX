namespace PKHeX.Core
{
    public sealed class EncounterTrade6 : EncounterTrade, IMemoryOT
    {
        public int OT_Memory { get; }
        public int OT_Intensity { get; }
        public int OT_Feeling { get; }
        public int OT_TextVar { get; }

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
            pk.OT_Memory = OT_Memory;
            pk.OT_Intensity = OT_Intensity;
            pk.OT_Feeling = OT_Feeling;
            pk.OT_TextVar = OT_TextVar;
        }
    }
}
