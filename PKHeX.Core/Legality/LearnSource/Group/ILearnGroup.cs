using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in the games that it represents.
/// </summary>
public interface ILearnGroup
{
    /// <summary>
    /// Gets the maximum move ID that this group can learn.
    /// </summary>
    ushort MaxMoveID { get; }

    /// <summary>
    /// Gets the next group to traverse to continue checking moves.
    /// </summary>
    ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option);

    /// <summary>
    /// Checks if it is plausible that the <see cref="pk"/> has visited this game group.
    /// </summary>
    bool HasVisited(PKM pk, EvolutionHistory history);

    bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current);

    void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current);
}
