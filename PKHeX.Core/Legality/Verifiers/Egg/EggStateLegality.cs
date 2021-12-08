namespace PKHeX.Core
{
    /// <summary>
    /// Provides information for what values an Egg can have, while it still is an egg.
    /// </summary>
    public static class EggStateLegality
    {
        /// <summary>
        /// Checks if the Egg Entity's hatch counter value is within the confines of game legality.
        /// </summary>
        /// <param name="pk">Egg Entity</param>
        /// <param name="enc">Encounter the egg was generated from</param>
        /// <returns>True if valid, false if invalid.</returns>
        public static bool GetIsEggHatchCyclesValid(PKM pk, IEncounterTemplate enc)
        {
            var hatchCounter = pk.OT_Friendship;
            var max = GetMaximumEggHatchCycles(pk, enc);
            if (hatchCounter > max)
                return false;
            var min = GetMinimumEggHatchCycles(pk);
            if (hatchCounter < min)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the minimum hatch counter value allowed for an Egg Entity.
        /// </summary>
        /// <param name="pk">Egg Entity</param>
        /// <returns>Usually 1...</returns>
        public static int GetMinimumEggHatchCycles(PKM pk) => pk switch
        {
            PK2 or PB8 => 1, // no grace period between 1 step remaining and hatch
            _ => 0, // having several Eggs in your party and then hatching one will give the rest 0... they can then be boxed!
        };

        /// <inheritdoc cref="GetMaximumEggHatchCycles(PKM, IEncounterTemplate)"/>
        /// <remarks>Will create a new <see cref="LegalityAnalysis"/> to find the encounter.</remarks>
        public static int GetMaximumEggHatchCycles(PKM pk)
        {
            var la = new LegalityAnalysis(pk);
            var enc = la.EncounterMatch;
            return GetMaximumEggHatchCycles(pk, enc);
        }

        /// <summary>
        /// Gets the original Hatch Cycle value for an Egg Entity.
        /// </summary>
        /// <param name="pk">Egg Entity</param>
        /// <param name="enc">Encounter the egg was generated from</param>
        /// <returns>Maximum value the Hatch Counter can be.</returns>
        public static int GetMaximumEggHatchCycles(PKM pk, IEncounterTemplate enc)
        {
            if (enc is EncounterStatic { EggCycles: not 0 } s)
                return s.EggCycles;
            return pk.PersonalInfo.HatchCycles;
        }

        /// <summary>
        /// Level which eggs are given to the player.
        /// </summary>
        /// <param name="generation">Generation the egg is given in</param>
        public static int GetEggLevel(int generation) => generation >= 4 ? 1 : 5;

        /// <summary>
        /// Checks if the <see cref="PKM.HT_Name"/> and associated details can be set for the provided egg <see cref="pk"/>.
        /// </summary>
        /// <param name="pk">Egg Entity</param>
        /// <returns>True if valid, false if invalid.</returns>
        public static bool IsValidHTEgg(PKM pk) => pk switch
        {
            PB8 { Met_Location: Locations.LinkTrade6NPC } pb8 when pb8.HT_Friendship == PersonalTable.BDSP[pb8.Species].BaseFriendship => true,
            _ => false,
        };

        /// <summary>
        /// Indicates if the <see cref="PKM.IsNicknamed"/> flag should be set for an Egg entity.
        /// </summary>
        /// <param name="enc">Encounter the egg was generated with</param>
        /// <param name="pk">Egg Entity</param>
        /// <returns>True if the <see cref="PKM.IsNicknamed"/> flag should be set, otherwise false.</returns>
        public static bool IsNicknameFlagSet(IEncounterTemplate enc, PKM pk) => enc switch
        {
            EncounterStatic7 => false,
            WB8 or EncounterStatic8b when pk.IsUntraded => false,
            { Generation: 4 } => false,
            _ => true,
        };

        /// <inheritdoc cref="IsNicknameFlagSet(IEncounterTemplate,PKM)"/>
        public static bool IsNicknameFlagSet(PKM pk) => IsNicknameFlagSet(new LegalityAnalysis(pk).EncounterMatch, pk);
    }
}
