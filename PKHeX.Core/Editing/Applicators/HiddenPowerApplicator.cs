namespace PKHeX.Core
{
    public static class HiddenPowerApplicator
    {
        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hptype"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="hptype">Desired Hidden Power typing.</param>
        public static void SetHiddenPower(this PKM pk, int hptype)
        {
            var IVs = pk.IVs;
            HiddenPower.SetIVsForType(hptype, pk.IVs, pk.Format);
            pk.IVs = IVs;
        }

        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hptype"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="hptype">Desired Hidden Power typing.</param>
        public static void SetHiddenPower(this PKM pk, MoveType hptype) => pk.SetHiddenPower((int)hptype);
    }
}