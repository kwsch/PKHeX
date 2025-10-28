namespace PKHeX.Core;

/// <summary>
/// Is tagged as a "Fateful Encounter".
/// </summary>
public interface IFatefulEncounter : IFatefulEncounterReadOnly
{
    /// <summary>
    /// Is tagged as a "Fateful Encounter".
    /// </summary>
    new bool FatefulEncounter { get; set; }
}
