using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in game.
/// </summary>
public interface ILearnSource
{
    /// <summary>
    /// Checks if the <see cref="pk"/> can learn the requested <see cref="move"/> while existing as <see cref="evo"/>.
    /// </summary>
    /// <param name="pk">Entity reference</param>
    /// <param name="pi">Entity game stats</param>
    /// <param name="evo">Details about the state of the entity</param>
    /// <param name="move">Move ID to check</param>
    /// <param name="types">Types of move sources to iterate</param>
    /// <param name="option">Option to check if it can be currently known, or previously known.</param>
    /// <returns>Details about how the move can be learned. Will be equivalent to default if cannot learn.</returns>
    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current);

    /// <summary>
    /// Yields an iterable list of all potential moves that an <see cref="evo"/> can learn from this <see cref="ILearnSource"/>.
    /// </summary>
    /// <param name="result">Result storage for flags</param>
    /// <param name="pk">Entity reference</param>
    /// <param name="evo">Details about the state of the entity</param>
    /// <param name="types">Types of move sources to iterate</param>
    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All);

    /// <summary>
    /// Gets the learnset for the given <see cref="species"/> and <see cref="form"/>.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    public Learnset GetLearnset(ushort species, byte form);

    /// <summary>
    /// Gets the <see cref="PersonalInfo"/> for the given <see cref="species"/> and <see cref="form"/>.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="pi">Result value</param>
    /// <returns>True if the <see cref="PersonalInfo"/> reference is a valid entity reference.</returns>
    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo? pi);
}
