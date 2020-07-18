namespace PKHeX.Core
{
    public sealed class EncounterTrade7 : EncounterTrade, IMemoryOT
    {
        public int OT_Memory => 1;
        public int OT_Intensity => 3;
        public int OT_Feeling => 5;
        public int OT_TextVar => 40;

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
