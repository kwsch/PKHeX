namespace PKHeX.Core;

/// <summary>
/// Interface for <see cref="IEncounterTemplate"/> objects that can be one of many forms.
/// </summary>
public interface IEncounterFormRandom
{
    /// <summary>
    /// Indicates if the form is random and unspecified.
    /// </summary>
    bool IsRandomUnspecificForm { get; }
}
