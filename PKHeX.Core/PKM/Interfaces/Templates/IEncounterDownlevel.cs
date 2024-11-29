namespace PKHeX.Core;

/// <summary>
/// Interface for encounters that can be down-leveled by an additional game situation.
/// </summary>
public interface IEncounterDownlevel
{
    /// <summary>
    /// Get the minimum level when forcibly down-leveled by an additional game situation.
    /// </summary>
    byte GetDownleveledMin();
}
