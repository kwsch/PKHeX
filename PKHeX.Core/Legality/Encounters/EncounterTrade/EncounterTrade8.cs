using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class EncounterTrade8 : EncounterTrade, IDynamaxLevel, IRelearn, IMemoryOT
    {
        public byte DynamaxLevel { get; set; }
        public IReadOnlyList<int> Relearn { get; set; } = Array.Empty<int>();

        public int OT_Memory { get; set; }
        public int OT_TextVar { get; set; }
        public int OT_Feeling { get; set; }
        public int OT_Intensity { get; set; }

        public EncounterTrade8(int species, int level, int m, int a, int f, int i)
        {
            Species = species;
            Level = level;

            OT_Memory = m;
            OT_TextVar = a;
            OT_Feeling = f;
            OT_Intensity = i;
        }

        public override bool IsMatch(PKM pkm, DexLevel evo, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;
            return base.IsMatch(pkm, evo, lvl);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            pk.SetRelearnMoves(Relearn);

            var pk8 = (PK8)pk;
            pk8.DynamaxLevel = DynamaxLevel;
            pk8.HT_Language = sav.Language;
            pk8.OT_Memory = OT_Memory;
            pk8.OT_TextVar = OT_TextVar;
            pk8.OT_Feeling = OT_Feeling;
            pk8.OT_Intensity = OT_Intensity;
        }
    }
}