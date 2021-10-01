namespace PKHeX.Core
{
    public sealed class TeamLock
    {
        public readonly int Species;
        public readonly string Comment;
        public readonly NPCLock[] Locks;

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

        public override string ToString()
        {
            if (Comment.Length == 0)
                return $"{(Species)Species} [{Locks.Length}]";
            return $"{(Species)Species} [{Locks.Length}] - {Comment}";
        }
    }
}
