using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class EncounterTrade8 : EncounterTrade, IDynamaxLevel, IRelearn, IMemoryOT
    {
        public byte DynamaxLevel { get; set; }
        public IReadOnlyList<int> Relearn { get; set; } = Array.Empty<int>();

        public int OT_Memory { get; }
        public int OT_TextVar { get; }
        public int OT_Feeling { get; }
        public int OT_Intensity { get; }

        public EncounterTrade8(int species, int level, int m, int a, int f, int i)
        {
            Species = species;
            Level = level;

            OT_Memory = m;
            OT_TextVar = a;
            OT_Feeling = f;
            OT_Intensity = i;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;
            return base.IsMatch(pkm, lvl);
        }

        protected override void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(SAV, criteria, pk);
            pk.SetRelearnMoves(Relearn);

            var pk8 = (PK8)pk;
            pk8.DynamaxLevel = DynamaxLevel;
            pk8.HT_Language = SAV.Language;
            pk8.OT_Memory = OT_Memory;
            pk8.OT_TextVar = OT_TextVar;
            pk8.OT_Feeling = OT_Feeling;
            pk8.OT_Intensity = OT_Intensity;
        }
    }
}