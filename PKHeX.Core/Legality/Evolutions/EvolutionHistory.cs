using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the possible evolution bounds for a parsed entity with respect to its origins and game traversal.
/// </summary>
public class EvolutionHistory
{
    private static readonly EvoCriteria[] NONE = Array.Empty<EvoCriteria>();
    public static readonly EvolutionHistory Empty = new();

    public EvoCriteria[] Gen1  = NONE;
    public EvoCriteria[] Gen2  = NONE;
    public EvoCriteria[] Gen3  = NONE;
    public EvoCriteria[] Gen4  = NONE;
    public EvoCriteria[] Gen5  = NONE;
    public EvoCriteria[] Gen6  = NONE;
    public EvoCriteria[] Gen7  = NONE;
    public EvoCriteria[] Gen8  = NONE;

    public EvoCriteria[] Gen7b = NONE;
    public EvoCriteria[] Gen8a = NONE;
    public EvoCriteria[] Gen8b = NONE;

    public bool HasVisitedSWSH => Gen8.Length != 0;
    public bool HasVisitedPLA => Gen8a.Length != 0;
    public bool HasVisitedBDSP => Gen8b.Length != 0;

    public ref EvoCriteria[] Get(EntityContext context)
    {
        if (context == EntityContext.Gen1) return ref Gen1;
        if (context == EntityContext.Gen2) return ref Gen2;
        if (context == EntityContext.Gen3) return ref Gen3;
        if (context == EntityContext.Gen4) return ref Gen4;
        if (context == EntityContext.Gen5) return ref Gen5;
        if (context == EntityContext.Gen6) return ref Gen6;
        if (context == EntityContext.Gen7) return ref Gen7;
        if (context == EntityContext.Gen8) return ref Gen8;

        if (context == EntityContext.Gen7b) return ref Gen7b;
        if (context == EntityContext.Gen8a) return ref Gen8a;
        if (context == EntityContext.Gen8b) return ref Gen8b;

        throw new ArgumentOutOfRangeException(nameof(context));
    }

    public ref EvoCriteria[] Get(int generation, GameVersion version) => ref Get(generation switch
    {
        7 when GameVersion.GG.Contains(version) || version == GameVersion.GO => EntityContext.Gen7b,
        8 when GameVersion.BDSP.Contains(version) => EntityContext.Gen8b,
        8 when GameVersion.PLA == version => EntityContext.Gen8a,
        _ => (EntityContext)generation,
    });

    public void Set(EntityContext context, EvoCriteria[] chain)
    {
        ref var arr = ref Get(context);
        arr = chain;
    }
}
