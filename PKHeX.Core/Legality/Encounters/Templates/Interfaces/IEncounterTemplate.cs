namespace PKHeX.Core;

/// <summary>
/// Represents all details that an entity may be encountered with.
/// </summary>
public interface IEncounterTemplate : ISpeciesForm, IVersion, IGeneration, IShiny, ILevelRange
{
    /// <summary>
    /// Original Context
    /// </summary>
    EntityContext Context { get; }

    /// <summary>
    /// Indicates if the encounter originated as an egg.
    /// </summary>
    bool EggEncounter { get; }
}

public static partial class Extensions
{
    public static bool IsWithinEncounterRange(this IEncounterTemplate encounter, PKM pk)
    {
        var level = pk.CurrentLevel;
        if (!pk.HasOriginalMetLocation)
            return encounter.IsLevelWithinRange(level);
        if (encounter.EggEncounter)
            return level == encounter.LevelMin;
        if (encounter is MysteryGift g)
            return level == g.Level;
        return level == pk.Met_Level;
    }
}
