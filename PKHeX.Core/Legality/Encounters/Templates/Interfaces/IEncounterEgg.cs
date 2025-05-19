namespace PKHeX.Core;

/// <summary>
/// Breeding Egg Encounter Properties.
/// </summary>
public interface IEncounterEgg : IEncounterable
{
    ILearnSource Learn { get; }
    bool CanHaveVoltTackle { get; }
}
