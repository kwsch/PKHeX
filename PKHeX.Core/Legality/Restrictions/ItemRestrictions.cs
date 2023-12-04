using System;

using static PKHeX.Core.Legal;

namespace PKHeX.Core;

/// <summary>
/// Information about Held Item Restrictions
/// </summary>
public static class ItemRestrictions
{
    /// <summary>
    /// Checks if a <see cref="PKM.HeldItem"/> is available to be held in the current <see cref="PKM.Format"/>.
    /// </summary>
    /// <param name="pk">Entity data</param>
    /// <returns>True if able to be held, false if not</returns>
    public static bool IsHeldItemAllowed(PKM pk)
    {
        return IsHeldItemAllowed(pk.HeldItem, pk.Context);
    }

    /// <summary>
    /// Checks if an item is available to be held in <see cref="context"/>.
    /// </summary>
    /// <param name="item">Held Item ID</param>
    /// <param name="context">Entity context to check</param>
    /// <returns>True if able to be held, false if not</returns>
    public static bool IsHeldItemAllowed(int item, EntityContext context)
    {
        if (item == 0)
            return true;
        var items = GetReleasedHeldItems(context);
        return (uint)item < items.Length && items[item];
    }

    private static ReadOnlySpan<bool> GetReleasedHeldItems(EntityContext context) => context switch
    {
        EntityContext.Gen2 => ReleasedHeldItems_2,
        EntityContext.Gen3 => ReleasedHeldItems_3,
        EntityContext.Gen4 => ReleasedHeldItems_4,
        EntityContext.Gen5 => ReleasedHeldItems_5,
        EntityContext.Gen6 => ReleasedHeldItems_6,
        EntityContext.Gen7 => ReleasedHeldItems_7,
        EntityContext.Gen8 => ReleasedHeldItems_8,
        EntityContext.Gen9 => ReleasedHeldItems_9,

        EntityContext.Gen8b => ReleasedHeldItems_8b,
        _ => [], // lgp/e, pla, etc
    };

    private static readonly bool[] ReleasedHeldItems_2 = GetPermitList(MaxItemID_2, HeldItems_GSC);
    private static readonly bool[] ReleasedHeldItems_3 = GetPermitList(MaxItemID_3, HeldItems_RS, ItemStorage3RS.Unreleased); // Safari Ball
    private static readonly bool[] ReleasedHeldItems_4 = GetPermitList(MaxItemID_4_HGSS, HeldItems_HGSS, ItemStorage4.Unreleased);
    private static readonly bool[] ReleasedHeldItems_5 = GetPermitList(MaxItemID_5_B2W2, HeldItems_BW, ItemStorage5.Unreleased);
    private static readonly bool[] ReleasedHeldItems_6 = GetPermitList(MaxItemID_6_AO, HeldItems_AO, ItemStorage6XY.Unreleased);
    private static readonly bool[] ReleasedHeldItems_7 = GetPermitList(MaxItemID_7_USUM, HeldItems_USUM, ItemStorage7SM.Unreleased);
    private static readonly bool[] ReleasedHeldItems_8 = GetPermitList(MaxItemID_8, HeldItems_SWSH, ItemStorage8SWSH.Unreleased, ItemStorage8SWSH.DynamaxCrystalBCAT);
    private static readonly bool[] ReleasedHeldItems_8b = GetPermitList(MaxItemID_8b, HeldItems_BS, ItemStorage8BDSP.Unreleased, ItemStorage8BDSP.DisallowHeldTreasure);
    private static readonly bool[] ReleasedHeldItems_9 = GetPermitList(MaxItemID_9, HeldItems_SV, ItemStorage9SV.Unreleased);

    /// <summary>
    /// Gets a permit list with the permitted indexes, then un-flags the indexes that are not permitted.
    /// </summary>
    /// <param name="max">Maximum index expected to allow</param>
    /// <param name="allowed">Allowed indexes</param>
    private static bool[] GetPermitList(int max, ReadOnlySpan<ushort> allowed)
    {
        var result = new bool[max + 1];
        foreach (var index in allowed)
            result[index] = true;
        return result;
    }

    /// <summary>
    /// Gets a permit list with the permitted indexes, then un-flags the indexes that are not permitted.
    /// </summary>
    /// <param name="max">Maximum index expected to allow</param>
    /// <param name="allowed">Allowed indexes (may have some disallowed)</param>
    /// <param name="disallow">Disallowed indexes</param>
    private static bool[] GetPermitList(int max, ReadOnlySpan<ushort> allowed, ReadOnlySpan<ushort> disallow)
    {
        var result = GetPermitList(max, allowed);
        foreach (var index in disallow)
            result[index] = false;
        return result;
    }

    /// <inheritdoc cref="GetPermitList(int,ReadOnlySpan{ushort})"/>
    private static bool[] GetPermitList(int max, ReadOnlySpan<ushort> allowed, ReadOnlySpan<ushort> disallow1, ReadOnlySpan<ushort> disallow2)
    {
        var result = GetPermitList(max, allowed);
        foreach (var index in disallow1)
            result[index] = false;
        foreach (var index in disallow2)
            result[index] = false;
        return result;
    }

    /// <inheritdoc cref="GetPermitList(int,ReadOnlySpan{ushort})"/>
    private static bool[] GetPermitList(int max, ReadOnlySpan<ushort> allowed, ReadOnlySpan<ushort> disallow1, Range disallow2)
    {
        var result = GetPermitList(max, allowed);
        foreach (var index in disallow1)
            result[index] = false;
        for (int i = disallow2.Start.Value; i < disallow2.End.Value; i++)
            result[i] = false;
        return result;
    }
}
