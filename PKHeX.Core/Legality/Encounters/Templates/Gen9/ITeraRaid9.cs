namespace PKHeX.Core;

/// <summary>
/// Properties shared for all Tera Crystal raids.
/// </summary>
public interface ITeraRaid9 : IEncounterable, IEncounterMatch, IGemType, IGenerateSeed32, IMoveset, IFixedGender, IFlawlessIVCount, IEncounterConvertible<PK9>
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

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    new PK9 ConvertToPKM(ITrainerInfo tr);
    new PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria);
}
