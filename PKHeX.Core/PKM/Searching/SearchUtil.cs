using System;
using System.Collections.Generic;

namespace PKHeX.Core.Searching;

/// <summary>
/// <see cref="PKM"/> searching utility
/// </summary>
public static class SearchUtil
{
    // Future: Might need to clamp down further for generations that cannot exist in the current format.
    public static bool SatisfiesFilterFormat(PKM pk, byte format, SearchComparison formatOperand) => formatOperand switch
    {
        SearchComparison.GreaterThanEquals when pk.Format <  format => false,
        SearchComparison.Equals            when pk.Format != format => false,
        SearchComparison.LessThanEquals    when pk.Format >  format => false,

        _ when format <= 2 => pk.Format <= 2, // 1-2
        _ when format <= 6 => pk.Format >= 3, // 3-6
        _ => true,
    };

    public static bool SatisfiesFilterGeneration(PKM pk, byte generation) => generation switch
    {
        1 => pk.VC || pk.Format < 3,
        2 => pk.VC || pk.Format < 3,
        _ => pk.Generation == generation,
    };

    public static bool SatisfiesFilterLevel(PKM pk, SearchComparison option, int level)
    {
        if (level > 100)
            return true; // why???

        return option switch
        {
            SearchComparison.LessThanEquals =>    pk.Stat_Level <= level,
            SearchComparison.Equals =>            pk.Stat_Level == level,
            SearchComparison.GreaterThanEquals => pk.Stat_Level >= level,
            _ => true,
        };
    }

    public static bool SatisfiesFilterEVs(PKM pk, int option) => option switch
    {
        1 => pk.EVTotal == 0, // None (0)
        2 => pk.EVTotal is (not 0) and < 128, // Some (127-1)
        3 => pk.EVTotal is >= 128 and < EffortValues.MaxEffective, // Half (128-507)
        4 => pk.EVTotal >= EffortValues.MaxEffective, // Full (508+)
        _ => true,
    };

    public static bool SatisfiesFilterIVs(PKM pk, int option) => option switch
    {
        1 => pk.IVTotal <= 90, // <= 90
        2 => pk.IVTotal is >  90 and <= 120, // 91-120
        3 => pk.IVTotal is > 120 and <= 150, // 121-150
        4 => pk.IVTotal is > 150 and <  180, // 151-179
        5 => pk.IVTotal >= 180, // 180+
        6 => pk.IVTotal == 186, // == 186
        _ => true,
    };

    public static bool SatisfiesFilterMoves(PKM pk, ReadOnlySpan<ushort> requiredMoves)
    {
        foreach (var m in requiredMoves)
        {
            if (!pk.HasMove(m))
                return false;
        }
        return true;
    }

    public static bool SatisfiesFilterBatchInstruction(PKM pk, IReadOnlyList<StringInstruction> filters)
    {
        return BatchEditing.IsFilterMatch(filters, pk); // Compare across all filters
    }

    public static Func<PKM, string> GetCloneDetectMethod(CloneDetectionMethod method) => method switch
    {
        CloneDetectionMethod.HashPID => HashByPID,
        _ => HashByDetails,
    };

    public static string HashByDetails(PKM pk) => pk switch
    {
        GBPKM gb => $"{pk.Species:000}{gb.DV16:X4}",
        _ => $"{pk.Species:0000}{pk.PID:X8}{GetIVString(pk)}{pk.Form:00}",
    };

    // Use a space as our separator -- don't merge single digit IVs and potentially get incorrect collisions
    private static string GetIVString(PKM pk) => $"{pk.IV_HP} {pk.IV_ATK} {pk.IV_DEF} {pk.IV_SPE} {pk.IV_SPA} {pk.IV_SPD}";

    public static string HashByPID(PKM pk) => pk switch
    {
        GBPKM gb => $"{gb.DV16:X4}",
        _ => $"{pk.PID:X8}",
    };

    public static IEnumerable<PKM> GetClones(IEnumerable<PKM> res, CloneDetectionMethod type = CloneDetectionMethod.HashDetails)
    {
        var method = GetCloneDetectMethod(type);
        return GetExtraClones(res, method);
    }

    public static IEnumerable<T> GetUniques<T>(IEnumerable<T> db, Func<T, string> method)
    {
        var hs = new HashSet<string>();
        foreach (var t in db)
        {
            var hash = method(t);
            if (hs.Contains(hash))
                continue;
            yield return t;
            hs.Add(hash);
        }
    }

    public static IEnumerable<T> GetExtraClones<T>(IEnumerable<T> db, Func<T, string> method)
    {
        var hs = new HashSet<string>();
        foreach (var t in db)
        {
            var hash = method(t);
            if (!hs.Add(hash))
                yield return t;
        }
    }
}
