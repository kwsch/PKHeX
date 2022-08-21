namespace PKHeX.Core;

/// <summary>
/// Represents a Team of Pokémon that is generated before a shadow <see cref="Species"/>.
/// </summary>
public sealed class TeamLock
{
    /// <summary>
    /// Team generated before the Species.
    /// </summary>
    public readonly NPCLock[] Locks;
    /// <summary>
    /// For trainers that have different teams, this indicates what conditions (when/where) the trainer must be battled.
    /// </summary>
    public readonly string Comment;
    /// <summary>
    /// Species of shadow Pokémon that is generated after the <see cref="Locks"/>.
    /// </summary>
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
