namespace PKHeX.Core;

/// <summary>
/// Interface exposing a method to adapt the entity to the Handling Trainer.
/// </summary>
public interface IHandlerUpdate
{
    /// <summary>
    /// Indicates if the entity belongs to the <see cref="ITrainerInfo"/>.
    /// </summary>
    /// <param name="tr">Trainer to check if it originally possessed the entity.</param>
    bool BelongsTo(ITrainerInfo tr);

    /// <summary>
    /// Updates the entity to match the <see cref="ITrainerInfo"/>.
    /// </summary>
    /// <param name="tr">Trainer that is now in possession of the entity.</param>
    void UpdateHandler(ITrainerInfo tr);
}
