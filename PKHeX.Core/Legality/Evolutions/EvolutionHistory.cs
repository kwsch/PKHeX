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
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    public static bool HasVisited(in ReadOnlySpan<EvoCriteria> evos, ushort species)
    {
        foreach (ref readonly var evo in evos)
        {
            if (evo.Species == species)
                return true;
        }
        return false;
    }
}
