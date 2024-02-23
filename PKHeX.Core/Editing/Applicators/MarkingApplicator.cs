using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the <see cref="IAppliedMarkings"/>.
/// </summary>
public static class MarkingApplicator
{
    /// <summary>
    /// Default <see cref="MarkingMethod"/> when applying markings.
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static Func<PKM, Func<int, int, int>> MarkingMethod { get; set; } = FlagHighLow;

    /// <summary>
    /// Sets the applied Markings to indicate flawless (or near-flawless) <see cref="PKM.IVs"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void SetMarkings(this PKM pk)
    {
        if (pk is not IAppliedMarkings { MarkingCount: 6 })
            return; // insufficient marking indexes

        if (pk is IAppliedMarkings<MarkingColor> c)
            SetMarkings(c, pk);
        else if (pk is IAppliedMarkings<bool> b)
            SetMarkings(b, pk);
    }

    private static void SetMarkings(this IAppliedMarkings<bool> mark, PKM pk)
    {
        var method = MarkingMethod(pk);
        mark.SetMarking(0, method(pk.IV_HP , 0) == 1);
        mark.SetMarking(1, method(pk.IV_ATK, 1) == 1);
        mark.SetMarking(2, method(pk.IV_DEF, 2) == 1);
        mark.SetMarking(3, method(pk.IV_SPA, 3) == 1);
        mark.SetMarking(4, method(pk.IV_SPD, 4) == 1);
        mark.SetMarking(5, method(pk.IV_SPE, 5) == 1);
    }

    private static void SetMarkings(this IAppliedMarkings<MarkingColor> mark, PKM pk)
    {
        var method = MarkingMethod(pk);
        mark.SetMarking(0, (MarkingColor)method(pk.IV_HP, 0));
        mark.SetMarking(1, (MarkingColor)method(pk.IV_ATK, 1));
        mark.SetMarking(2, (MarkingColor)method(pk.IV_DEF, 2));
        mark.SetMarking(3, (MarkingColor)method(pk.IV_SPA, 3));
        mark.SetMarking(4, (MarkingColor)method(pk.IV_SPD, 4));
        mark.SetMarking(5, (MarkingColor)method(pk.IV_SPE, 5));
    }

    /// <summary>
    /// Toggles the marking at a given index.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="index">Marking index to toggle</param>
    /// <returns>Current marking value</returns>
    public static void ToggleMarking(this PKM pk, int index)
    {
        if (pk is IAppliedMarkings<MarkingColor> c)
            c.SetMarking(index, c.GetMarking(index).Next());
        else if (pk is IAppliedMarkings<bool> b)
            b.SetMarking(index, !b.GetMarking(index));
    }

    private static MarkingColor Next(this MarkingColor value) => value switch
    {
        MarkingColor.Blue => MarkingColor.Pink,
        MarkingColor.Pink => MarkingColor.None,
        _ => MarkingColor.Blue,
    };

    private static Func<int, int, int> FlagHighLow(PKM pk)
    {
        if (pk.Format < 7)
            return GetSimpleMarking;
        return GetComplexMarking;

        static int GetSimpleMarking(int val, int _) => val == 31 ? 1 : 0;
        static int GetComplexMarking(int val, int _) => val switch
        {
            31 or 1 => 1,
            30 or 0 => 2,
            _ => 0,
        };
    }
}
