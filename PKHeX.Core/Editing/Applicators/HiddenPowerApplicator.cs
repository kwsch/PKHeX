using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to apply a Hidden Power type to a <see cref="PKM"/>.
/// </summary>
public static class HiddenPowerApplicator
{
    extension(PKM pk)
    {
        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hiddenPowerType"/>.
        /// </summary>
        /// <param name="hiddenPowerType">Desired Hidden Power typing.</param>
        public void SetHiddenPower(int hiddenPowerType)
        {
            Span<int> IVs = stackalloc int[6];
            pk.GetIVs(IVs);
            HiddenPower.SetIVsForType(hiddenPowerType, IVs, pk.Context);
            pk.SetIVs(IVs);
        }

        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hiddenPowerType"/>.
        /// </summary>
        /// <param name="hiddenPowerType">Desired Hidden Power typing.</param>
        public void SetHiddenPower(MoveType hiddenPowerType) => pk.SetHiddenPower((int)hiddenPowerType);
    }
}
