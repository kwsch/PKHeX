using System;

namespace PKHeX.Core
{
    public sealed class EncounterStaticShadow : EncounterStatic
    {
        public TeamLock[] Locks { get; internal set; } = Array.Empty<TeamLock>();
        public int Gauge { get; internal set; }
        public bool EReader { get; set; }

        internal override EncounterStatic Clone()
        {
            var result = (EncounterStaticShadow)base.Clone();
            if (Locks.Length > 0)
            {
                result.Locks = new TeamLock[Locks.Length];
                for (int i = 0; i < Locks.Length; i++)
                    result.Locks[i] = Locks[i].Clone();
            }
            return result;
        }
    }
}