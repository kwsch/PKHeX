using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class AltFormInfo
    {
        /// <summary>
        /// Checks if the form cannot exist outside of a Battle.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        /// <returns>True if it can only exist in a battle, false if it can exist outside of battle.</returns>
        public static bool IsBattleOnlyForm(int species, int form, int format)
        {
            if (!BattleOnly.Contains(species))
                return false;

            // Some species have battle only forms as well as out-of-battle forms (other than base form).
            switch (species)
            {
                case (int)Species.Slowbro when form == 2 && format >= 8: // Only mark Ultra Necrozma as Battle Only
                case (int)Species.Darmanitan when form == 2 && format >= 8: // this one is OK, Galarian Slowbro (not a Mega)
                case (int)Species.Zygarde when form < 4: // Zygarde Complete
                case (int)Species.Mimikyu when form == 2: // Totem disguise Mimikyu
                case (int)Species.Necrozma when form < 3: // this one is OK, Galarian non-Zen
                case (int)Species.Minior when form >= 7: // Minior Shields-Down
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Reverts the Battle Form to the form it would have outside of Battle.
        /// </summary>
        /// <remarks>Only call this if you've already checked that <see cref="IsBattleOnlyForm"/> returns true.</remarks>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        /// <returns>Suggested alt form value.</returns>
        public static int GetOutOfBattleForm(int species, int form, int format)
        {
            return species switch
            {
                (int)Species.Darmanitan => form & 2,
                (int)Species.Zygarde when format > 6 => 3,
                (int)Species.Minior => form + 7,
                _ => 0
            };
        }

        /// <summary>
        /// Checks if the <see cref="form"/> is a fused form, which indicates it cannot be traded away.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        /// <returns>True if it is a fused species-form, false if it is not fused.</returns>
        public static bool IsFusedForm(int species, int form, int format)
        {
            return species switch
            {
                (int)Species.Kyurem when form != 0 && format >= 5 => true,
                (int)Species.Necrozma when form != 0 && format >= 7 => true,
                (int)Species.Calyrex when form != 0 && format >= 8 => true,
                _ => false
            };
        }

        /// <summary>
        /// Species that have an alternate form that cannot exist outside of battle.
        /// </summary>
        private static readonly HashSet<int> BattleForms = new HashSet<int>
        {
            (int)Species.Castform,
            (int)Species.Cherrim,
            (int)Species.Darmanitan,
            (int)Species.Meloetta,
            (int)Species.Aegislash,
            (int)Species.Xerneas,
            (int)Species.Zygarde,

            (int)Species.Wishiwashi,
            (int)Species.Mimikyu,

            (int)Species.Cramorant,
            (int)Species.Morpeko,
            (int)Species.Eiscue,

            (int)Species.Zacian,
            (int)Species.Zamazenta,
            (int)Species.Eternatus,
        };

        /// <summary>
        /// Species that have a mega form that cannot exist outside of battle.
        /// </summary>
        /// <remarks>Using a held item to change form during battle, via an in-battle transformation feature.</remarks>
        private static readonly HashSet<int> BattleMegas = new HashSet<int>
        {
            // XY
            (int)Species.Venusaur, (int)Species.Charizard, (int)Species.Blastoise,
            (int)Species.Alakazam, (int)Species.Gengar, (int)Species.Kangaskhan, (int)Species.Pinsir,
            (int)Species.Gyarados, (int)Species.Aerodactyl, (int)Species.Mewtwo,

            (int)Species.Ampharos, (int)Species.Scizor, (int)Species.Heracross, (int)Species.Houndoom, (int)Species.Tyranitar,

            (int)Species.Blaziken, (int)Species.Gardevoir, (int)Species.Mawile, (int)Species.Aggron, (int)Species.Medicham,
            (int)Species.Manectric, (int)Species.Banette, (int)Species.Absol, (int)Species.Latios, (int)Species.Latias,

            (int)Species.Garchomp, (int)Species.Lucario, (int)Species.Abomasnow,

            // AO
            (int)Species.Beedrill, (int)Species.Pidgeot, (int)Species.Slowbro,

            (int)Species.Steelix,

            (int)Species.Sceptile, (int)Species.Swampert, (int)Species.Sableye, (int)Species.Sharpedo, (int)Species.Camerupt,
            (int)Species.Altaria, (int)Species.Glalie, (int)Species.Salamence, (int)Species.Metagross, (int)Species.Rayquaza,

            (int)Species.Lopunny, (int)Species.Gallade,
            (int)Species.Audino, (int)Species.Diancie,

            // USUM
            (int)Species.Necrozma, // Ultra Necrozma
        };

        /// <summary>
        /// Species that have a primal form that cannot exist outside of battle.
        /// </summary>
        private static readonly HashSet<int> BattlePrimals = new HashSet<int> { 382, 383 };

        private static readonly HashSet<int> BattleOnly = GetBattleFormSet();

        private static HashSet<int> GetBattleFormSet()
        {
            var hs = new HashSet<int>(BattleForms);
            hs.UnionWith(BattleMegas);
            hs.UnionWith(BattlePrimals);
            return hs;
        }
    }
}
