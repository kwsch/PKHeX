namespace PKHeX.Core
{
    public sealed class TeamLock
    {
        public readonly int Species;
        public readonly string Comment;
        public readonly NPCLock[] Locks;

        internal TeamLock Clone() => new TeamLock(Species, Comment, (NPCLock[])Locks.Clone());

        public TeamLock(int species, NPCLock[] locks)
        {
            Species = species;
            Locks = locks;
            Comment = string.Empty;
        }

        public TeamLock(int species, string comment, NPCLock[] locks)
        {
            Species = species;
            Locks = locks;
            Comment = comment;
        }
    }
}