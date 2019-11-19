using System;

namespace PKHeX.Core
{
    public class EncounterTrade8 : EncounterTrade, IDynamaxLevel, IRelearn
    {
        public byte DynamaxLevel { get; set; }
        public int[] Relearn { get; set; } = Array.Empty<int>();

        public EncounterTrade8(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;
            return base.IsMatch(pkm, lvl);
        }

        protected override void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            if (pk is IDynamaxLevel d)
                d.DynamaxLevel = DynamaxLevel;
            pk.SetRelearnMoves(Relearn);
            ((PK8)pk).HT_Language = SAV.Language;
            base.ApplyDetails(SAV, criteria, pk);
        }
    }
}