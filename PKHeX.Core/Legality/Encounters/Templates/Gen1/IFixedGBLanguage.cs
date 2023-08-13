namespace PKHeX.Core;

/// <summary>
/// Exposes info on language restriction for Gen1/2.
/// </summary>
public interface IFixedGBLanguage
{
    /// <summary>
    /// Language restriction for the encounter template.
    /// </summary>
    EncounterGBLanguage Language { get; }
}
