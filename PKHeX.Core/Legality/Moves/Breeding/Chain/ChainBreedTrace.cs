using System;

namespace PKHeX.Core;

/// <summary>
/// A successful, caller-owned trace, indicating a chain-breeding proof of breeding links and transfer transitions.
/// </summary>
public ref struct ChainBreedTrace(Span<ChainBreedStep> buffer)
{
    private readonly Span<ChainBreedStep> _buffer = buffer;

    /// <summary>
    /// Count of steps in the trace. This is the number of valid entries in <see cref="Steps"/>.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// The maximum number of steps that can be stored in the trace. This is the length of the underlying buffer.
    /// </summary>
    public readonly int Capacity => _buffer.Length;

    /// <summary>
    /// The steps in the trace, in reverse order (from the egg to the root). The first step is the egg, and the last step is the root of the chain (original ancestor).
    /// </summary>
    public readonly ReadOnlySpan<ChainBreedStep> Steps => _buffer[..Count];

    /// <summary>
    /// Clears the trace, resetting the count to zero. The underlying buffer remains unchanged, but the trace is considered empty after this operation.
    /// </summary>
    public void Reset() => Count = 0;

    /// <summary>
    /// Queries whether the step at the specified index in the trace is equal to the provided <see cref="ChainBreedStep"/> state.
    /// </summary>
    /// <param name="index">The index of the step in the trace to compare.</param>
    /// <param name="state">The <see cref="ChainBreedStep"/> state to compare against the step at the specified index.</param>
    /// <returns>True if the step at the specified index is equal to the provided state; otherwise, false.</returns>
    internal readonly bool QueryEquals(int index, in ChainBreedStep state) => _buffer[index].QueryEquals(state);

    /// <summary>
    /// Sets the step at the specified index in the trace to the provided <see cref="ChainBreedStep"/> state. This operation overwrites any existing step at that index.
    /// </summary>
    /// <param name="index">The index of the step in the trace to set.</param>
    /// <param name="state">The <see cref="ChainBreedStep"/> state to set at the specified index.</param>
    internal void SetPending(int index, in ChainBreedStep state) => _buffer[index] = state;

    /// <summary>
    /// Commits the specified count of steps to the trace, updating the <see cref="Count"/> property. The count is clamped to the maximum capacity of the trace.
    /// </summary>
    /// <param name="count"></param>
    internal void Commit(int count) => Count = Math.Max(Count, count);

    /// <summary>
    /// Resolves the parent species and forms for the step at the specified index in the trace. This operation updates the step with the provided parent information.
    /// </summary>
    /// <param name="index">The index of the step in the trace to update.</param>
    /// <param name="motherSpecies">The species of the mother.</param>
    /// <param name="motherForm">The form of the mother.</param>
    /// <param name="fatherSpecies">The species of the father.</param>
    /// <param name="fatherForm">The form of the father.</param>
    internal void Resolve(int index, ushort motherSpecies, byte motherForm, ushort fatherSpecies, byte fatherForm)
        => _buffer[index] = _buffer[index].WithParents(motherSpecies, motherForm, fatherSpecies, fatherForm);

    /// <summary>
    /// Sets the step at the specified index in the trace to a transfer step, indicating a transfer from one game version to another. This operation overwrites any existing step at that index.
    /// </summary>
    /// <param name="index">The index of the step in the trace to set.</param>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="destination">The destination game version.</param>
    /// <param name="origin">The origin game version where the transfer occurs.</param>
    /// <param name="moves">The moves of the Pokémon at time of transfer.</param>
    internal void SetTransfer(int index, ushort species, byte form, GameVersion destination, GameVersion origin, scoped ReadOnlySpan<ushort> moves)
        => _buffer[index] = ChainBreedStep.CreateTransfer(species, form, destination, origin, moves);

    /// <summary>
    /// Sets a Gen6+ breeding step in the trace; due to the nature of Gen6+ breeding, the rules are considered "relaxed" and thus only 1 step is needed to represent the entire breeding chain.
    /// This method sets the first step in the trace to a breeding step with the provided species, form, version, moves, and parent summary.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="version">The game version where the breeding occurs.</param>
    /// <param name="moves">The moves of the Pokémon.</param>
    /// <param name="summary">The summary of the breeding chain.</param>
    internal void SetRelaxed(ushort species, byte form, GameVersion version, scoped ReadOnlySpan<ushort> moves, ChainBreedSummary summary)
    {
        if (_buffer.IsEmpty)
            return;
        _buffer[0] = ChainBreedStep.CreateEgg(species, form, version, moves).WithParents(summary.MotherSpecies, summary.MotherForm, summary.FatherSpecies, summary.FatherForm);
        Count = 1;
    }

    /// <summary>
    /// Converts the trace into a <see cref="ChainBreedSummary"/>.
    /// If the trace is empty, a default summary is returned.
    /// Otherwise, the summary is constructed from the root step and the depth of breeding steps in the trace.
    /// </summary>
    /// <returns>The summary of the breeding chain.</returns>
    public readonly ChainBreedSummary GetSummary()
    {
        if (Count == 0)
            return default; // idc
        var breedingDepth = GetDepth(ChainBreedStepKind.Breed);
        ref readonly var root = ref _buffer[0];
        return new ChainBreedSummary(root.MotherSpecies, root.MotherForm, root.FatherSpecies, root.FatherForm, breedingDepth);
    }

    /// <summary>
    /// Counts the number of steps in the trace that match the specified <see cref="ChainBreedStepKind"/>.
    /// </summary>
    /// <param name="kind">The kind of user action/step to count.</param>
    /// <returns>The number of steps that match the specified kind.</returns>
    public readonly byte GetDepth(ChainBreedStepKind kind)
    {
        byte depth = 0;
        foreach (ref readonly var step in Steps)
        {
            if (step.Kind == kind)
                depth++;
        }
        return depth;
    }
}
