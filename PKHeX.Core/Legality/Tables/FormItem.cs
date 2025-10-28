using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic for relating Forms and the Held Item.
/// </summary>
public static class FormItem
{
    private static ReadOnlySpan<ushort> Arceus_PlateIDs => [303, 306, 304, 305, 309, 308, 310, 313, 298, 299, 301, 300, 307, 302, 311, 312, 644];
    private static ReadOnlySpan<ushort> Arceus_ZCrystal => [782, 785, 783, 784, 788, 787, 789, 792, 777, 778, 780, 779, 786, 781, 790, 791, 793];

    /// <summary>
    /// Gets the form for <see cref="Arceus"/> based on the held item.
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <param name="format">Generation/Format of the entity</param>
    /// <returns>Form ID</returns>
    public static byte GetFormArceus(int item, byte format) => item switch
    {
        (>= 777 and <= 793) => GetFormArceusZCrystal(item),
        (>= 298 and <= 313) or 644 => GetFormArceusPlate(item, format),
        _ => 0,
    };

    /// <summary>
    /// Gets the form for <see cref="Arceus"/> based on the held item (Generation 7 Z-Crystals).
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <returns>Form ID</returns>
    public static byte GetFormArceusZCrystal(int item) => (byte)(Arceus_ZCrystal.IndexOf((ushort)item) + 1);

    /// <summary>
    /// Gets the held item for <see cref="Arceus"/> based on the form (Excluding Generation 7 Z-Crystals).
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <param name="format">Generation/Format of the entity</param>
    /// <returns>Held Item</returns>
    public static byte GetFormArceusPlate(int item, byte format)
    {
        byte form = (byte)(Arceus_PlateIDs.IndexOf((ushort)item) + 1);
        if (format != 4) // No need to consider Curse type
            return form;
        if (form < 9)
            return form;
        return ++form; // ??? type Form shifts everything by 1
    }

    /// <summary>
    /// Gets the held item for <see cref="Arceus"/> based on the form.
    /// </summary>
    /// <param name="form">Form ID</param>
    /// <param name="format">Generation/Format of the entity</param>
    /// <returns>Held Item</returns>
    public static ushort GetItemArceus(byte form, byte format)
    {
        var index = form - 1;
        if (format == 4 && form > 8)
            index--; // ignore curse type

        var arr = Arceus_PlateIDs;
        if ((uint)index >= arr.Length)
            return 0;
        return arr[index];
    }

    /// <summary>
    /// Gets the form for <see cref="Silvally"/> based on the held item.
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <returns>Form ID</returns>
    public static byte GetFormSilvally(int item)
    {
        if (item is >= 904 and <= 920)
            return (byte)(item - 903);
        return 0;
    }

    /// <summary>
    /// Gets the held item for <see cref="Silvally"/> based on the form.
    /// </summary>
    /// <param name="form">Form ID</param>
    /// <returns>Held Item</returns>
    public static ushort GetItemSilvally(byte form)
    {
        if (form is >= 1 and <= 17)
            return (ushort)(form + 903);
        return 0;
    }

    /// <summary>
    /// Gets the form for <see cref="Genesect"/> based on the held item.
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <returns>Form ID</returns>
    public static byte GetFormGenesect(int item)
    {
        if (item is >= 116 and <= 119)
            return (byte)(item - 115);
        return 0;
    }

    /// <summary>
    /// Gets the held item for <see cref="Genesect"/> based on the form.
    /// </summary>
    /// <param name="form">Form ID</param>
    /// <returns>Held Item</returns>
    public static ushort GetItemGenesect(byte form)
    {
        if (form is >= 1 and <= 4)
            return (ushort)(form + 115);
        return 0;
    }

    /// <summary>
    /// Gets the form for <see cref="Ogerpon"/> based on the held item.
    /// </summary>
    /// <param name="item">Held Item</param>
    /// <returns>Form ID</returns>
    public static byte GetFormOgerpon(int item) => item switch
    {
        2407 => 1, // Wellspring Mask
        2408 => 2, // Hearthflame Mask
        2406 => 3, // Cornerstone Mask
        _ => 0, // Teal Mask
    };

    /// <summary>
    /// Gets the held item for <see cref="Ogerpon"/> based on the form.
    /// </summary>
    /// <param name="form">Form ID</param>
    /// <returns>Held Item</returns>
    public static ushort GetItemOgerpon(byte form) => form switch
    {
        1 => 2407, // Wellspring Mask
        2 => 2408, // Hearthflame Mask
        3 => 2406, // Cornerstone Mask
        _ => 0, // Teal Mask (no held item required)
    };

    /// <summary>
    /// Gets the held item for <see cref="Species"/> based on the form.
    /// </summary>
    /// <param name="species">Entity Species</param>
    /// <param name="form">Form ID</param>
    /// <returns>0 if no held item is required</returns>
    public static ushort GetItem(ushort species, byte form) => species switch
    {
        (ushort)Arceus => GetItemArceus(form, 8),
        (ushort)Silvally => GetItemSilvally(form),
        (ushort)Genesect => GetItemGenesect(form),
        (ushort)Ogerpon => GetItemOgerpon(form),
        _ => 0,
    };

    /// <summary>
    /// Gets the form for <see cref="Species"/> based on the held item.
    /// </summary>
    /// <param name="species">Entity Species</param>
    /// <param name="item">Held Item</param>
    /// <param name="format">Generation/Format of the entity</param>
    /// <param name="form">Expected Form ID</param>
    /// <returns>True if the form is required</returns>
    public static bool TryGetForm(ushort species, int item, byte format, out byte form)
    {
        switch (species)
        {
            case (ushort)Arceus: form = GetFormArceus(item, format); return true;
            case (ushort)Silvally: form = GetFormSilvally(item); return true;
            case (ushort)Genesect: form = GetFormGenesect(item); return true;
            case (ushort)Ogerpon: form = GetFormOgerpon(item); return true;
            default: form = 0; return false;
        }
    }
}
