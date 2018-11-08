namespace PKHeX.Core
{
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