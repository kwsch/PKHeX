using System;

namespace PKHeX.Core;

/// <summary>
/// Value passing object to simplify some initialization.
/// </summary>
/// <typeparam name="T">Egg Move source type enumeration.</typeparam>
internal readonly ref struct BreedInfo<T>(Span<T> Actual, Span<byte> Possible, Learnset Learnset, ReadOnlySpan<ushort> Moves, byte Level)
    where T : unmanaged
{
    /// <summary> Indicates the analyzed source of each move. </summary>
    public readonly Span<T> Actual = Actual;

    /// <summary> Indicates all possible sources of each move. </summary>
    public readonly Span<byte> Possible = Possible;

    /// <summary> Level Up entry for the egg. </summary>
    public readonly Learnset Learnset = Learnset;

    /// <summary> Moves the egg knows after it is finalized. </summary>
    public readonly ReadOnlySpan<ushort> Moves = Moves;

    /// <summary> Level the egg originated at. </summary>
    public readonly byte Level = Level;
}
