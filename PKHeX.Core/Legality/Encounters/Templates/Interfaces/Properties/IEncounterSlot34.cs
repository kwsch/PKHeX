namespace PKHeX.Core;

/// <summary>
/// Common RNG-related properties for encounter slots in Gen 3-4.
/// </summary>
public interface IEncounterSlot34 : ILevelRange, ISlotRNGType, IMagnetStatic, INumberedSlot
{
    byte PressureLevel { get; }
    byte AreaRate { get; }
}

public interface IEncounterSlot3 : IEncounterSlot34, ISpeciesForm
{
    bool IsSafariHoenn { get; }
}
