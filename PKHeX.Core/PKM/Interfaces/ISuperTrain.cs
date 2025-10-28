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
    /// Gets or sets whether all accessible Super Training regimens are complete for this Pokémon.
    /// </summary>
    bool SuperTrainSupremelyTrained { get; set; }

    /// <summary>
    /// Distribution Event training regimens completed bitflags.
    /// </summary>
    ushort DistTrainBitFlags { get; set; }
}
