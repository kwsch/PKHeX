namespace PKHeX.Core
{
    internal class EncounterStaticShadow : EncounterStatic
    {
        public EncounterLock[][] Locks { get; internal set; } = new EncounterLock[0][];
        public int Gauge { get; internal set; }
        public bool EReader { get; set; }
        internal override EncounterStatic Clone()
        {
            var result = (EncounterStaticShadow)base.Clone();
            result.CloneLocks();
            return result;
        }

        private void CloneLocks()
        {
            Locks = new EncounterLock[Locks.Length][];
            for (var i = 0; i < Locks.Length; i++)
            {
                Locks[i] = (EncounterLock[])Locks[i].Clone();
                for (int j = 0; j < Locks[i].Length; j++)
                    Locks[i][j] = Locks[i][j].Clone();
            }
        }
    }
}