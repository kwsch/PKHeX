namespace PKHeX.Core;

/// <summary>
/// Contains information about initial Hatch Cycles
/// </summary>
public interface IHatchCycle
{
    /// <summary>
    /// Number of Hatch Cycles required to hatch an egg.
    /// </summary>
    /// <remarks>
    /// When non-zero, this value is used instead of the default value from Personal data.
    /// </remarks>
    byte EggCycles { get; }
}
