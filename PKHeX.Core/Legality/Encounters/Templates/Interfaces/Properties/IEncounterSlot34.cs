namespace PKHeX.Core;

/// <summary>
/// Common RNG-related properties for encounter slots in Gen 3-4.
/// </summary>
public interface IEncounterSlot34 : ILevelRange, IMagnetStatic, INumberedSlot, ISpeciesForm
{
    byte PressureLevel { get; }
    byte AreaRate { get; }
}

public interface IEncounterSlot4 : IEncounterSlot34
{
    SlotType4 Type { get; }
}

public interface IEncounterSlot3 : IEncounterSlot34
{
    SlotType3 Type { get; }
    bool IsSafariHoenn { get; }
}
