using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the possible evolution bounds for a parsed entity with respect to its origins and game traversal.
/// </summary>
public class EvolutionHistory
{
    private static readonly EvoCriteria[] NONE = Array.Empty<EvoCriteria>();
    public static readonly EvolutionHistory Empty = new(NONE, 0);

    public EvoCriteria[] Gen1  = NONE;
    public EvoCriteria[] Gen2  = NONE;
    public EvoCriteria[] Gen3  = NONE;
    public EvoCriteria[] Gen4  = NONE;
    public EvoCriteria[] Gen5  = NONE;
    public EvoCriteria[] Gen6  = NONE;
    public EvoCriteria[] Gen7  = NONE;
    public EvoCriteria[] Gen8  = NONE;

    public ref EvoCriteria[] Gen7b => ref Gen7; // future: separate field instead of copy
    public ref EvoCriteria[] Gen8a => ref Gen8; // future: separate field instead of copy
    public ref EvoCriteria[] Gen8b => ref Gen8; // future: separate field instead of copy

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
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    internal void Invalidate(EntityContext current) => Get(current) = NONE;

    public bool HasVisitedSWSH => Gen8.Length != 0;
    public bool HasVisitedPLA => Gen8a.Length != 0;
    public bool HasVisitedBDSP => Gen8b.Length != 0;

    public ref EvoCriteria[] Get(EntityContext context)
    {
        if (context == EntityContext.Gen7b)
            return ref Gen7b;
        if (context == EntityContext.Gen8a)
            return ref Gen8a;
        if (context == EntityContext.Gen8b)
            return ref Gen8b;
        return ref this[context.Generation()];
    }

    public EvoCriteria[] Get(int generation, GameVersion version)
    {
        if (generation == 7 && (GameVersion.GG.Contains(version) || version == GameVersion.GO))
            return Gen7b;
        if (generation == 8 && GameVersion.BDSP.Contains(version))
            return Gen8b;
        if (generation == 8 && GameVersion.PLA == version)
            return Gen8a;
        return this[generation];
    }
}
