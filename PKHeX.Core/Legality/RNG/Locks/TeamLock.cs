namespace PKHeX.Core
{
    public class TeamLock
    {
        public int Species;
        public string Comment;
        public NPCLock[] Locks;

        internal TeamLock Clone()
        {
            return new TeamLock { Comment = Comment, Locks = (NPCLock[])Locks.Clone() };
        }
    }
}