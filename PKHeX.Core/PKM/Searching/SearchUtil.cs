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

    public static bool SatisfiesFilterContext(PKM pk, EntityContext context, SearchComparison contextOperand) => contextOperand switch
    {
        SearchComparison.GreaterThanEquals when pk.Context.IsGenerationLessThan(context) => false,
        SearchComparison.Equals when pk.Context != context => false,
        SearchComparison.LessThanEquals when pk.Context.IsGenerationGreaterThan(context) => false,
        _ => contextOperand != SearchComparison.None || CanReachContext(pk, context),
    };

    private static bool CanReachContext(PKM pk, EntityContext context)
    {
        var generation = context.Generation;
        if (generation <= 2)
            return pk.Format <= 2; // 1-2 can reach 1-2
        if (generation <= 6)
            return pk.Format >= 3; // 3-6 can reach 3-6
        return true; // 7+ can reach all contexts
    }

    public static bool SatisfiesFilterGeneration(PKM pk, byte generation) => generation switch
    {
        1 => pk.VC || pk.Format < 3,
        2 => pk.VC || pk.Format < 3,
        _ => pk.Generation == generation,
    };

    public static bool SatisfiesFilterLevel(PKM pk, SearchComparison option, byte level)
    {
        var current = pk.Stat_Level;
        if (current == 0)
            current = pk.CurrentLevel;
        return option switch
        {
            SearchComparison.LessThanEquals => current <= level,
            SearchComparison.Equals => current == level,
            SearchComparison.GreaterThanEquals => current >= level,
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

    public static bool SatisfiesFilterNickname(PKM pk, ReadOnlySpan<char> nicknameSubstring)
    {
        Span<char> name = stackalloc char[pk.MaxStringLengthNickname];
        int length = pk.LoadString(pk.NicknameTrash, name);
        name = name[..length];

        // Compare the nickname filter against the Entity's nickname, ignoring case.
        return name.Contains(nicknameSubstring, StringComparison.OrdinalIgnoreCase);
    }

    public static bool TrySeekNext(SaveFile sav, Func<PKM, bool> searchFilter, out (int Box, int Slot) result, int currentBox = -1, int currentSlot = -1, bool reverse = false)
    {
        // Search from next slot, wrapping around
        if (currentBox == -1)
            currentBox = 0;

        var step = reverse ? -1 : 1;
        if (currentSlot == -1)
            currentSlot = 0;
        else
            currentSlot += step;

        var totalSlots = sav.SlotCount;
        var index = currentBox * sav.BoxSlotCount + currentSlot;
        if (index < 0)
            index = totalSlots - 1;
        else if (index >= totalSlots)
            index = 0;

        for (var i = 0; i < totalSlots; i++)
        {
            var actualIndex = (index + i * step + totalSlots) % totalSlots;
            var pk = sav.GetBoxSlotAtIndex(actualIndex);
            if (pk.Species == 0)
                continue;

            if (!searchFilter(pk))
                continue;

            // Match found. Seek to the box, and Focus on the slot.
            sav.GetBoxSlotFromIndex(actualIndex, out var box, out var slot);
            result = (box, slot);
            return true;
        }

        // None found.
        result = default;
        return false;
    }
}
