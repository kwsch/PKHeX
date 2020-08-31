using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class EncounterStatic6 : EncounterStatic, IContestStats
    {
        public override int Generation => 6;

        internal IReadOnlyList<int> Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (base.IsMatchLocation(pkm))
                return true;

            if (Species != (int) Core.Species.Pikachu)
                return false;

            // Cosplay Pikachu is given from multiple locations
            var loc = pkm.Met_Location;
            return loc == 180 || loc == 186 || loc == 194;
        }
    }
}
