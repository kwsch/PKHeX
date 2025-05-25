namespace PKHeX.Core;

/// <summary>
/// Interface for Super Training data and status for a Pokémon.
/// </summary>
public interface ISuperTrain
{
    /// <summary>
    /// Bit flags representing the completion state of each Super Training regimen.
    /// Each bit typically corresponds to a specific training event or medal.
    /// </summary>
    uint SuperTrainBitFlags { get; set; }

    /// <summary>
    /// Gets or sets whether Secret Super Training is unlocked for this Pokémon.
    /// </summary>
    bool SecretSuperTrainingUnlocked { get; set; }

    /// <summary>
    /// Gets or sets whether all Secret Super Training regimens are complete for this Pokémon.
    /// </summary>
    bool SecretSuperTrainingComplete { get; set; }

    /// <summary>
    /// Gets the number of Super Training medals earned.
    /// </summary>
    /// <param name="lowBitCount">The number of regular regimens to count (default: 30).</param>
    /// <remarks>
    /// Sometimes only the first `n` bitflags are required, such as the SuperTraining ribbon needing only the first 12 bits, indicating sufficient completion.
    /// </remarks>
    /// <returns>The count of completed Super Training regimens (medals).</returns>
    int SuperTrainingMedalCount(int lowBitCount = 30);
}
