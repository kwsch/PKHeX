namespace PKHeX.Core;

/// <summary>
/// Represents a Team of Pokémon that is generated before a shadow <see cref="Species"/>.
/// </summary>
public sealed class TeamLock(ushort Species, NPCLockDetail Comment, NPCLock[] Locks)
{
    /// <summary>
    /// Team generated before the Species.
    /// </summary>
    public readonly NPCLock[] Locks = Locks;
    /// <summary>
    /// For trainers that have different teams, this indicates what conditions (when/where) the trainer must be battled.
    /// </summary>
    public readonly NPCLockDetail Comment = Comment;
    /// <summary>
    /// Species of shadow Pokémon that is generated after the <see cref="Locks"/>.
    /// </summary>
    public readonly ushort Species = Species;

    public TeamLock(ushort Species, NPCLock[] Locks) : this(Species, 0, Locks) { }

    public override string ToString()
    {
        if (Comment == 0)
            return $"{(Species)Species} [{Locks.Length}]";
        return $"{(Species)Species} [{Locks.Length}] - {Comment}";
    }
}

public enum NPCLockDetail : byte
{
    CipherLab,
    Post,
    PhenacCity,
    PhenacCityAndPost,

    SeenParas,
    SeenBeedrill,
    SeenTangela,
    SeenVenomoth,
    SeenPrimeape,
    SeenGolduck,
    SeenDodrio,
    SeenFarfetchd,
    SeenKangaskhan,
    SeenMagmar,
    SeenRapidash,
    SeenScyther,
    SeenSolrock,
    SeenSwellow,
    SeenSwellowElectabuzz,
    SeenPoliwrath,
    SeenManectric,
    SeenManectricSalamence,
    SeenManectricMarowak,
    SeenManectricMarowakSalamence,
    SeenRhydonMoltres,
    SeenRhydonMoltresExeggutor,
    SeenRhydonMoltresTauros,
    SeenRhydonMoltresArticuno,
    SeenRhydonMoltresTaurosArticuno,
    SeenRhydonMoltresExeggutorTauros,
    SeenRhydonMoltresExeggutorArticuno,
    SeenRhydonMoltresExeggutorTaurosArticuno,
}
