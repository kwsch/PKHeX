using static PKHeX.Core.GameVersion;

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
        /// <returns>Usually 0...</returns>
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
        /// Met Level which eggs are given to the player. May change if transferred to future games.
        /// </summary>
        /// <param name="version">Game the egg is obtained in</param>
        /// <param name="generation">Generation the egg is given in</param>
        public static int GetEggLevelMet(GameVersion version, int generation) => generation switch
        {
            2 => version is C ? 1 : 0, // GS do not store met data
            3 or 4 => 0,
            _ => 1,
        };

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

        /// <summary>
        /// Gets a valid <see cref="PKM.Met_Location"/> for an egg hatched in the origin game, accounting for future format transfers altering the data.
        /// </summary>
        public static int GetEggHatchLocation(GameVersion game, int format) => game switch
        {
            R or S or E or FR or LG => format switch
            {
                3 => game is FR or LG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE,
                4 => Locations.Transfer3, // Pal Park
                _ => Locations.Transfer4,
            },

            D or P or Pt => format > 4 ? Locations.Transfer4 : Locations.HatchLocationDPPt,
            HG or SS => format > 4 ? Locations.Transfer4 : Locations.HatchLocationHGSS,

            B or W or B2 or W2 => Locations.HatchLocation5,

            X or Y => Locations.HatchLocation6XY,
            AS or OR => Locations.HatchLocation6AO,
            SN or MN or US or UM => Locations.HatchLocation7,

            GSC or C when format <= 2 => Locations.HatchLocationC,
            RD or BU or GN or YW => Locations.Transfer1,
            GD or SI or C => Locations.Transfer2,

            SW or SH => Locations.HatchLocation8,
            BD or SP => Locations.HatchLocation8b,
            _ => -1,
        };
    }
}
