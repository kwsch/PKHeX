namespace PKHeX.Core;

/// <summary>
/// Breeding Egg Encounter Properties.
/// </summary>
public interface IEncounterEgg : IEncounterable
{
    bool CanHaveVoltTackle { get; }
}
