namespace PKHeX.Core;

/// <summary>
/// Stores details about <see cref="GameVersion.GO"/> encounters relevant for legality.
/// </summary>
public interface IPogoSlot : IPogoDateRange
{
    /// <summary> Possibility of shiny for the encounter. </summary>
    Shiny Shiny { get; }

    /// <summary> Method the Pokémon may be encountered with. </summary>
    PogoType Type { get; }

    /// <summary> Gender the Pokémon may be encountered with. </summary>
    Gender Gender { get; }
}

/// <summary>
/// Contains details about an encounter that can be found in <see cref="GameVersion.GO"/>.
/// </summary>
public static class PogoSlotExtensions
{
    public static bool GetIVsAboveMinimum(this IPogoSlot slot, PKM pk)
    {
        int min = slot.Type.GetMinIV();
        if (min == 0)
            return true;
        return GetIVsAboveMinimum(pk, min);
    }

    private static bool GetIVsAboveMinimum(PKM pk, int min)
    {
        if (pk.IV_ATK >> 1 < min) // ATK
            return false;
        if (pk.IV_DEF >> 1 < min) // DEF
            return false;
        return pk.IV_HP >> 1 >= min; // HP
    }

    public static bool GetIVsValid(this IPogoSlot slot, PKM pk)
    {
        if (!slot.GetIVsAboveMinimum(pk))
            return false;

        // HP * 2 | 1 -> HP
        // ATK * 2 | 1 -> ATK&SPA
        // DEF * 2 | 1 -> DEF&SPD
        // Speed is random.

        // All IVs must be odd (except speed) and equal to their counterpart.
        if ((pk.IV_ATK & 1) != 1 || pk.IV_ATK != pk.IV_SPA) // ATK=SPA
            return false;
        if ((pk.IV_DEF & 1) != 1 || pk.IV_DEF != pk.IV_SPD) // DEF=SPD
            return false;
        return (pk.IV_HP & 1) == 1; // HP
    }
}
