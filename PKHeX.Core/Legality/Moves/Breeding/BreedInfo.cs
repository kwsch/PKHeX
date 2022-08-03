using System;

namespace PKHeX.Core;

/// <summary>
/// Value passing object to simplify some initialization.
/// </summary>
/// <typeparam name="T">Egg Move source type enumeration.</typeparam>
internal readonly ref struct BreedInfo<T> where T : unmanaged
{
    /// <summary> Indicates the analyzed source of each move. </summary>
    public readonly Span<T> Actual;

    /// <summary> Indicates all possible sources of each move. </summary>
    public readonly Span<byte> Possible;

    /// <summary> Level Up entry for the egg. </summary>
    public readonly Learnset Learnset;

    /// <summary> Moves the egg knows after it is finalized. </summary>
    public readonly ReadOnlySpan<int> Moves;

    /// <summary> Level the egg originated at. </summary>
    public readonly int Level;

    public BreedInfo(Span<T> actual, Span<byte> possible, Learnset learnset, ReadOnlySpan<int> moves, int level)
    {
        Actual = actual;
        Possible = possible;
        Learnset = learnset;
        Moves = moves;
        Level = level;
    }
}
