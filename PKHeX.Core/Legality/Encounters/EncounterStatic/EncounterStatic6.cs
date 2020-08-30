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
    }
}
