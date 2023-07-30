using System;

namespace PKHeX.Core;

public interface IFixedTrainer
{
    bool IsFixedTrainer { get; }

    bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language);
}

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
