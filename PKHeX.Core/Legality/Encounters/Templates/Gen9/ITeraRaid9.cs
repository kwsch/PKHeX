namespace PKHeX.Core;

/// <summary>
/// Properties shared for all Tera Crystal raids.
/// </summary>
public interface ITeraRaid9 : IGemType
{
    /// <summary>
    /// Is a BCAT raid.
    /// </summary>
    bool IsDistribution { get; }

    /// <summary>
    /// BCAT distribution index.
    /// </summary>
    byte Index { get; }

    /// <summary>
    /// Star count difficulty.
    /// </summary>
    byte Stars { get; }

    /// <summary>
    /// Raw random chance value the encounter will be chosen.
    /// </summary>
    byte RandRate { get; }

    /// <summary>
    /// Checks if the provided <see cref="seed"/> will pick this object by random choice.
    /// </summary>
    bool CanBeEncountered(uint seed);
}
