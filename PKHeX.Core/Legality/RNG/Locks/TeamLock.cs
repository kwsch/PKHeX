namespace PKHeX.Core
{
    public sealed class TeamLock
    {
        public readonly NPCLock[] Locks;
        public readonly string Comment;
        public readonly int Species;

        public TeamLock(int species, NPCLock[] locks) : this(species, string.Empty, locks) { }

        public TeamLock(int species, string comment, NPCLock[] locks)
        {
            Species = species;
            Locks = locks;
            Comment = comment;
        }

        public override string ToString()
        {
            if (Comment.Length == 0)
                return $"{(Species)Species} [{Locks.Length}]";
            return $"{(Species)Species} [{Locks.Length}] - {Comment}";
        }
    }
}
