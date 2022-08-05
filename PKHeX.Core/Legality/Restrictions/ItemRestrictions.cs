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
    /// Checks if an <see cref="item"/> is available to be held in <see cref="context"/>.
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

        EntityContext.Gen8b => ReleasedHeldItems_8b,
        _ => Array.Empty<bool>(), // lgp/e, pla, etc
    };
}
