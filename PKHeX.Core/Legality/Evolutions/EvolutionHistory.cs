using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the possible evolution bounds for a parsed entity with respect to its origins and game traversal.
/// </summary>
public sealed class EvolutionHistory
{
    private static readonly EvoCriteria[] NONE = [];

    public EvoCriteria[] Gen1  = NONE;
    public EvoCriteria[] Gen2  = NONE;
    public EvoCriteria[] Gen3  = NONE;
    public EvoCriteria[] Gen4  = NONE;
    public EvoCriteria[] Gen5  = NONE;
    public EvoCriteria[] Gen6  = NONE;
    public EvoCriteria[] Gen7  = NONE;
    public EvoCriteria[] Gen8  = NONE;
    public EvoCriteria[] Gen9  = NONE;

    public EvoCriteria[] Gen7b = NONE;
    public EvoCriteria[] Gen8a = NONE;
    public EvoCriteria[] Gen8b = NONE;
    public EvoCriteria[] Gen9a = NONE;

    public bool HasVisitedGen1 => Gen1.Length != 0;
    public bool HasVisitedGen2 => Gen2.Length != 0;
    public bool HasVisitedGen3 => Gen3.Length != 0;
    public bool HasVisitedGen4 => Gen4.Length != 0;
    public bool HasVisitedGen5 => Gen5.Length != 0;
    public bool HasVisitedGen6 => Gen6.Length != 0;
    public bool HasVisitedGen7 => Gen7.Length != 0;
    public bool HasVisitedSWSH => Gen8.Length != 0;
    public bool HasVisitedGen9 => Gen9.Length != 0;

    public bool HasVisitedLGPE => Gen7b.Length != 0;
    public bool HasVisitedPLA => Gen8a.Length != 0;
    public bool HasVisitedBDSP => Gen8b.Length != 0;
    public bool HasVisitedZA => Gen9a.Length != 0;

    public ReadOnlySpan<EvoCriteria> Get(EntityContext context) => context switch
    {
        EntityContext.Gen1 => Gen1,
        EntityContext.Gen2 => Gen2,
        EntityContext.Gen3 => Gen3,
        EntityContext.Gen4 => Gen4,
        EntityContext.Gen5 => Gen5,
        EntityContext.Gen6 => Gen6,
        EntityContext.Gen7 => Gen7,
        EntityContext.Gen8 => Gen8,
        EntityContext.Gen9 => Gen9,

        EntityContext.Gen7b => Gen7b,
        EntityContext.Gen8a => Gen8a,
        EntityContext.Gen8b => Gen8b,
        EntityContext.Gen9a => Gen9a,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    public void Set(EntityContext context, EvoCriteria[] evos)
    {
        ref var arr = ref GetByRef(context);
        arr = evos;
    }

    private ref EvoCriteria[] GetByRef(EntityContext context)
    {
        ref EvoCriteria[] arr = ref Gen1;
             if (context == EntityContext.Gen2) arr = ref Gen2;
        else if (context == EntityContext.Gen3) arr = ref Gen3;
        else if (context == EntityContext.Gen4) arr = ref Gen4;
        else if (context == EntityContext.Gen5) arr = ref Gen5;
        else if (context == EntityContext.Gen6) arr = ref Gen6;
        else if (context == EntityContext.Gen7) arr = ref Gen7;
        else if (context == EntityContext.Gen8) arr = ref Gen8;
        else if (context == EntityContext.Gen9) arr = ref Gen9;
        else if (context == EntityContext.Gen7b) arr = ref Gen7b;
        else if (context == EntityContext.Gen8a) arr = ref Gen8a;
        else if (context == EntityContext.Gen8b) arr = ref Gen8b;
        else if (context == EntityContext.Gen9a) arr = ref Gen9a;
        else throw new ArgumentOutOfRangeException(nameof(context), context, null);
        return ref arr;
    }

    public static bool HasVisited(in ReadOnlySpan<EvoCriteria> evos, ushort species)
    {
        foreach (ref readonly var evo in evos)
        {
            if (evo.Species == species)
                return true;
        }
        return false;
    }

    public EvolutionHistory AsSingle(EntityContext context)
    {
        var single = new EvolutionHistory();
        single.Set(context, Get(context).ToArray());
        return single;
    }

    public EvolutionHistory PruneKeepPreEvolutions(ushort species) => new()
    {
        Gen1 = PruneKeepPreEvolutions(Gen1, species),
        Gen2 = PruneKeepPreEvolutions(Gen2, species),
        Gen3 = PruneKeepPreEvolutions(Gen3, species),
        Gen4 = PruneKeepPreEvolutions(Gen4, species),
        Gen5 = PruneKeepPreEvolutions(Gen5, species),
        Gen6 = PruneKeepPreEvolutions(Gen6, species),
        Gen7 = PruneKeepPreEvolutions(Gen7, species),
        Gen8 = PruneKeepPreEvolutions(Gen8, species),
        Gen9 = PruneKeepPreEvolutions(Gen9, species),
        Gen7b = PruneKeepPreEvolutions(Gen7b, species),
        Gen8a = PruneKeepPreEvolutions(Gen8a, species),
        Gen8b = PruneKeepPreEvolutions(Gen8b, species),
        Gen9a = PruneKeepPreEvolutions(Gen9a, species),
    };

    private static EvoCriteria[] PruneKeepPreEvolutions(EvoCriteria[] src, ushort species)
    {
        // Most evolved species is at the lowest index.
        // If `species` is at the current index, only keep indexes after.
        var start = GetSpeciesIndex(src, species);
        if (start == -1)
            return src;
        if (start == src.Length - 1)
            return NONE;
        return src[(start + 1)..];
    }

    private static int GetSpeciesIndex(ReadOnlySpan<EvoCriteria> array, ushort species)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Species == species)
                return i;
        }
        return -1;
    }
}
