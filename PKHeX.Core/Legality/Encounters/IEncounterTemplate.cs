namespace PKHeX.Core;

/// <summary>
/// Represents all details that an entity may be encountered with.
/// </summary>
public interface IEncounterTemplate : ISpeciesForm, IVersion, IGeneration, IShiny
{
    /// <summary>
    /// Original Context
    /// </summary>
    EntityContext Context { get; }

    /// <summary>
    /// Indicates if the encounter originated as an egg.
    /// </summary>
    bool EggEncounter { get; }

    /// <summary>
    /// Minimum level for the encounter.
    /// </summary>
    byte LevelMin { get; }
    byte LevelMax { get; }
}

public static partial class Extensions
{
    private static bool IsWithinEncounterRange(this IEncounterTemplate encounter, int lvl)
    {
        return encounter.LevelMin <= lvl && lvl <= encounter.LevelMax;
    }

    public static bool IsWithinEncounterRange(this IEncounterTemplate encounter, PKM pk)
    {
        if (!pk.HasOriginalMetLocation)
            return encounter.IsWithinEncounterRange(pk.CurrentLevel);
        if (encounter.EggEncounter)
            return pk.CurrentLevel == encounter.LevelMin;
        if (encounter is MysteryGift g)
            return pk.CurrentLevel == g.Level;
        return pk.CurrentLevel == pk.Met_Level;
    }
}
