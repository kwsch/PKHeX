using System;

namespace PKHeX.Core
{
    public sealed record EncounterTrade7 : EncounterTrade, IMemoryOT
    {
        public override int Generation => 7;
        // immutable setters
        public int OT_Memory { get => 1; set => throw new InvalidOperationException(); }
        public int OT_Intensity { get => 3; set => throw new InvalidOperationException(); }
        public int OT_Feeling { get => 5; set => throw new InvalidOperationException(); }
        public int OT_TextVar { get => 40; set => throw new InvalidOperationException(); }

        public EncounterTrade7(GameVersion game) : base(game) { }

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
}
