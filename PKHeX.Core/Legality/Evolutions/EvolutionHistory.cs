using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the possible evolution bounds for a parsed entity with respect to its origins and game traversal.
/// </summary>
public class EvolutionHistory
{
    public readonly EvoCriteria[] FullChain;

    public EvolutionHistory(EvoCriteria[] fullChain, int count)
    {
        FullChain = fullChain;
        Length = count;
    }

    public readonly int Length;

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

    public EvoCriteria[] Gen1  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen2  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen3  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen4  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen5  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen6  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen7  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen7b = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen8  = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen8a = Array.Empty<EvoCriteria>();
    public EvoCriteria[] Gen8b = Array.Empty<EvoCriteria>();
}
