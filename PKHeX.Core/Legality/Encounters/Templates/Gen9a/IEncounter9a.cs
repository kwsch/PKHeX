namespace PKHeX.Core;

/// <summary>
/// Common interface for encounters originating from <see cref="EntityContext.Gen9a"/>
/// </summary>
public interface IEncounter9a : IEncounterable, IEncounterMatch, IAlphaReadOnly, IFlawlessIVCount, ISeedCorrelation64<PKM>
{
    /// <summary>
    /// RNG Correlation pattern that this encounter follows when generating its random values.
    /// </summary>
    LumioseCorrelation Correlation { get; }

    /// <summary>
    /// Obtains the generation parameters for this encounter given the provided personal info (for Gender ratio).
    /// </summary>
    GenerateParam9a GetParams(PersonalInfo9ZA pi);
}
