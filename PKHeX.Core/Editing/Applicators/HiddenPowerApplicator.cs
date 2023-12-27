using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to apply a Hidden Power type to a <see cref="PKM"/>.
/// </summary>
public static class HiddenPowerApplicator
{
    /// <summary>
    /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hiddenPowerType"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="hiddenPowerType">Desired Hidden Power typing.</param>
    public static void SetHiddenPower(this PKM pk, int hiddenPowerType)
    {
        Span<int> IVs = stackalloc int[6];
        pk.GetIVs(IVs);
        HiddenPower.SetIVsForType(hiddenPowerType, IVs, pk.Context);
        pk.SetIVs(IVs);
    }

    /// <summary>
    /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hiddenPowerType"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="hiddenPowerType">Desired Hidden Power typing.</param>
    public static void SetHiddenPower(this PKM pk, MoveType hiddenPowerType) => pk.SetHiddenPower((int)hiddenPowerType);
}
