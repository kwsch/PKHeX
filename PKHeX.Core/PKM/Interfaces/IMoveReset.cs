namespace PKHeX.Core;

/// <summary>
/// Allows resetting the moveset back to an initial state.
/// </summary>
public interface IMoveReset
{
    /// <summary>
    /// Resets the current moves to the current level up set.
    /// </summary>
    void ResetMoves();
}
