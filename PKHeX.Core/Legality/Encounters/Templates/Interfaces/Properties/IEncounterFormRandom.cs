namespace PKHeX.Core;

/// <summary>
/// Interface for an <see cref="IEncounterTemplate"/> that can be one of many forms.
/// </summary>
public interface IEncounterFormRandom
{
    /// <summary>
    /// Indicates if the form is random and unspecified.
    /// </summary>
    bool IsRandomUnspecificForm { get; }
}
