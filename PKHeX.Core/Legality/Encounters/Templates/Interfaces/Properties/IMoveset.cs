namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a <see cref="Moveset"/> for the object.
/// </summary>
public interface IMoveset
{
    /// <summary>
    /// Moveset for the object.
    /// </summary>
    /// <remarks>Check if <see cref="Moveset.HasMoves"/> before using.</remarks>
    Moveset Moves { get; }
}
