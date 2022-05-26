using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the possible evolution bounds for a parsed entity with respect to its origins and game traversal.
/// </summary>
public class EvolutionHistory
{
    private static readonly EvoCriteria[] NONE = Array.Empty<EvoCriteria>();

    public EvoCriteria[] Gen1  = NONE;
    public EvoCriteria[] Gen2  = NONE;
    public EvoCriteria[] Gen3  = NONE;
    public EvoCriteria[] Gen4  = NONE;
    public EvoCriteria[] Gen5  = NONE;
    public EvoCriteria[] Gen6  = NONE;
    public EvoCriteria[] Gen7  = NONE;
    public EvoCriteria[] Gen7b = NONE;
    public EvoCriteria[] Gen8  = NONE;
    public EvoCriteria[] Gen8a = NONE;
    public EvoCriteria[] Gen8b = NONE;

    public readonly int Length;
    public readonly EvoCriteria[] FullChain;

    public EvolutionHistory(EvoCriteria[] fullChain, int count)
    {
        FullChain = fullChain;
        Length = count;
    }

    public ref EvoCriteria[] this[int index]
    {
        get
        {
            if (index == 1) return ref Gen1;
            if (index == 2) return ref Gen2;
            if (index == 3) return ref Gen3;
            if (index == 4) return ref Gen4;
            if (index == 5) return ref Gen5;
            if (index == 6) return ref Gen6;
            if (index == 7) return ref Gen7;
            if (index == 8) return ref Gen8;
            throw new IndexOutOfRangeException(nameof(index));
        }
    }

    internal void Invalidate() => this[Length - 1] = NONE;

    public bool HasVisitedSWSH => Gen8.Length != 0;
    public bool HasVisitedPLA => Gen8a.Length != 0;
    public bool HasVisitedBDSP => Gen8b.Length != 0;
}
