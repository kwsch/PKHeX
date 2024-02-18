using System;

namespace PKHeX.Core;

/// <summary>
/// Modifications using <see cref="BatchInfo"/> legality.
/// </summary>
internal static class BatchModifications
{
    private static bool IsAll(ReadOnlySpan<char> p) => p.EndsWith("All", StringComparison.OrdinalIgnoreCase);
    private static bool IsNone(ReadOnlySpan<char> p) => p.EndsWith("None", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Sets a suggested legal moveset for the Entity.
    /// </summary>
    public static ModifyResult SetSuggestedMoveset(BatchInfo info, bool random = false)
    {
        Span<ushort> moves = stackalloc ushort[4];
        info.Legality.GetMoveSet(moves, random);
        return SetMoves(info.Entity, moves);
    }

    /// <summary>
    /// Sets a suggested legal relearn moveset for the Entity.
    /// </summary>
    public static ModifyResult SetSuggestedRelearnData(BatchInfo info, ReadOnlySpan<char> propValue)
    {
        var pk = info.Entity;
        if (pk is ITechRecord t)
        {
            t.ClearRecordFlags();
            if (IsAll(propValue))
            {
                t.SetRecordFlagsAll(info.Legality.Info.EvoChainsAllGens.Get(pk.Context)); // all
            }
            else if (!IsNone(propValue))
            {
                Span<ushort> moves = stackalloc ushort[4];
                pk.GetMoves(moves);
                t.SetRecordFlags(moves, info.Legality.Info.EvoChainsAllGens.Get(pk.Context)); // whatever fit the current moves
            }
        }

        pk.SetRelearnMoves(info.Legality);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets all legal Move Mastery flag data for the Entity.
    /// </summary>
    /// <remarks>Only applicable for <see cref="IMoveShop8Mastery"/>.</remarks>
    public static ModifyResult SetSuggestedMasteryData(BatchInfo info, ReadOnlySpan<char> propValue)
    {
        var pk = info.Entity;
        if (pk is not IMoveShop8Mastery t)
            return ModifyResult.Invalid;

        t.ClearMoveShopFlags();
        if (IsNone(propValue))
            return ModifyResult.Modified;

        var e = info.Legality.EncounterMatch;
        if (e is IMasteryInitialMoveShop8 enc)
            enc.SetInitialMastery(pk);
        if (IsAll(propValue))
            t.SetMoveShopFlagsAll(pk);
        else
            t.SetMoveShopFlags(pk);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets suggested ribbon data for the Entity.
    /// </summary>
    /// <remarks>If None, removes all ribbons possible.</remarks>
    public static ModifyResult SetSuggestedRibbons(BatchInfo info, ReadOnlySpan<char> value)
    {
        if (IsNone(value))
            RibbonApplicator.RemoveAllValidRibbons(info.Legality);
        else // All
            RibbonApplicator.SetAllValidRibbons(info.Legality);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets suggested met data for the Entity.
    /// </summary>
    public static ModifyResult SetSuggestedMetData(BatchInfo info)
    {
        var pk = info.Entity;
        var encounter = EncounterSuggestion.GetSuggestedMetInfo(pk);
        if (encounter == null)
            return ModifyResult.Error;

        var location = encounter.Location;
        var level = encounter.LevelMin;
        var minimumLevel = EncounterSuggestion.GetLowestLevel(pk, level);

        pk.MetLevel = level;
        pk.MetLocation = location;
        pk.CurrentLevel = Math.Max(minimumLevel, level);

        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets the lowest current level for the Entity.
    /// </summary>
    public static ModifyResult SetMinimumCurrentLevel(BatchInfo info)
    {
        var result = EncounterSuggestion.IterateMinimumCurrentLevel(info.Entity, info.Legal);
        return result ? ModifyResult.Modified : ModifyResult.Filtered;
    }

    /// <summary>
    /// Sets the provided moves in a random order.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="moves">Moves to apply.</param>
    public static ModifyResult SetMoves(PKM pk, ReadOnlySpan<ushort> moves)
    {
        pk.SetMoves(moves);
        return ModifyResult.Modified;
    }

    public static ModifyResult SetEVs(PKM pk)
    {
        Span<int> evs = stackalloc int[6];
        EffortValues.SetMax(evs, pk);
        pk.SetEVs(evs);
        return ModifyResult.Modified;
    }

    /// <summary>
    /// Sets the contests stats as requested.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="la">Legality Information matched to.</param>
    /// <param name="option">Option to apply with</param>
    public static ModifyResult SetContestStats(PKM pk, LegalityAnalysis la, ReadOnlySpan<char> option)
    {
        if (option.Length != 0 && option[BatchEditing.CONST_SUGGEST.Length..] is not "0")
            pk.SetMaxContestStats(la.EncounterMatch, la.Info.EvoChainsAllGens);
        else
            pk.SetSuggestedContestStats(la.EncounterMatch, la.Info.EvoChainsAllGens);
        return ModifyResult.Modified;
    }
}
