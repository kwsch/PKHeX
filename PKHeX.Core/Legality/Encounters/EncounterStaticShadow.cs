using System;

namespace PKHeX.Core
{
    public class EncounterStaticShadow : EncounterStatic
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

    public class TeamLock
    {
        public int Species;
        public string Comment;
        public NPCLock[] Locks;

        internal TeamLock Clone()
        {
            var c = new TeamLock { Comment = Comment, Locks = (NPCLock[])Locks.Clone() };
            for (int i = 0; i < Locks.Length; i++)
                Locks[i] = Locks[i].Clone();
            return c;
        }
    }
}