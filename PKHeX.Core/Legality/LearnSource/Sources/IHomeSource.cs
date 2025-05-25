namespace PKHeX.Core;

/// <summary>
/// Exposes logic for determining if a move can be learned in a HOME-compatible game context.
/// Used for validating move legality when transferring Pokémon through Pokémon HOME.
/// </summary>
public interface IHomeSource
{
    /// <summary>
    /// Gets the game environment this source represents (e.g., SW/SH, BD/SP, S/V).
    /// </summary>
    LearnEnvironment Environment { get; }

    /// <summary>
    /// Determines if the specified <paramref name="move"/> can be learned by the given <paramref name="pk"/> (with evolution criteria <paramref name="evo"/>)
    /// in the context of Pokémon HOME's move relearner, considering the allowed <paramref name="types"/> of move sources.
    /// </summary>
    /// <param name="pk">The Pokémon entity to check.</param>
    /// <param name="evo">The evolution criteria representing the species/form context.</param>
    /// <param name="move">The move ID to check for learnability.</param>
    /// <param name="types">The types of move sources to consider (default is all).</param>
    /// <returns>Information about how the move can be learned, or a default value if it cannot be learned.</returns>
    MoveLearnInfo GetCanLearnHOME(PKM pk, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All);
}
