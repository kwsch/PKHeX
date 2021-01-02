using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains logic for Alternate Form information.
    /// </summary>
    public static class FormInfo
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
                case (int)Slowbro when form == 2 && format >= 8: // Only mark Ultra Necrozma as Battle Only
                case (int)Darmanitan when form == 2 && format >= 8: // this one is OK, Galarian Slowbro (not a Mega)
                case (int)Zygarde when form < 4: // Zygarde Complete
                case (int)Mimikyu when form == 2: // Totem disguise Mimikyu
                case (int)Necrozma when form < 3: // this one is OK, Galarian non-Zen
                    return false;
                case (int)Minior: return form < 7; // Minior Shields-Down

                default:
                    return form != 0;
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
        public static int GetOutOfBattleForm(int species, int form, int format) => species switch
        {
            (int)Darmanitan => form & 2,
            (int)Zygarde when format > 6 => 3,
            (int)Minior => form + 7,
            _ => 0
        };

        /// <summary>
        /// Checks if the <see cref="form"/> is a fused form, which indicates it cannot be traded away.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        /// <returns>True if it is a fused species-form, false if it is not fused.</returns>
        public static bool IsFusedForm(int species, int form, int format) => species switch
        {
            (int)Kyurem when form != 0 && format >= 5 => true,
            (int)Necrozma when form != 0 && format >= 7 => true,
            (int)Calyrex when form != 0 && format >= 8 => true,
            _ => false
        };

        /// <summary>Checks if the form may be different than the original encounter detail.</summary>
        /// <param name="species">Original species</param>
        /// <param name="oldForm">Original form</param>
        /// <param name="newForm">Current form</param>
        /// <param name="format">Current format</param>
        public static bool IsFormChangeable(int species, int oldForm, int newForm, int format)
        {
            if (FormChange.Contains(species))
                return true;

            // Zygarde Form Changing
            // Gen6: Introduced; no form changing.
            // Gen7: Form changing introduced; can only change to Form 2/3 (Power Construct), never to 0/1 (Aura Break). A form-1 can be boosted to form-0.
            // Gen8: Form changing improved; can pick any Form & Ability combination.
            if (species == (int)Zygarde)
            {
                return format switch
                {
                    6 => false,
                    7 => newForm >= 2 || (oldForm == 1 && newForm == 0),
                    _ => true,
                };
            }
            return false;
        }

        /// <summary>
        /// Species that can be captured normally in the wild and can change between their forms.
        /// </summary>
        public static readonly HashSet<int> WildChangeFormAfter = new()
        {
            412, // Burmy
            479, // Rotom
            676, // Furfrou
            741, // Oricorio
        };

        /// <summary>
        /// Species that can change between their forms, regardless of origin.
        /// </summary>
        /// <remarks>Excludes Zygarde as it has special conditions. Check separately.</remarks>
        private static readonly HashSet<int> FormChange = new(WildChangeFormAfter)
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            493, // Arceus
            641, // Tornadus
            642, // Thundurus
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            649, // Genesect
            720, // Hoopa
            773, // Silvally
            800, // Necrozma
            898, // Calyrex
        };

        /// <summary>
        /// Species that have an alternate form that cannot exist outside of battle.
        /// </summary>
        private static readonly HashSet<int> BattleForms = new()
        {
            (int)Castform,
            (int)Cherrim,
            (int)Darmanitan,
            (int)Meloetta,
            (int)Aegislash,
            (int)Xerneas,
            (int)Zygarde,

            (int)Wishiwashi,
            (int)Minior,
            (int)Mimikyu,

            (int)Cramorant,
            (int)Morpeko,
            (int)Eiscue,

            (int)Zacian,
            (int)Zamazenta,
            (int)Eternatus,
        };

        /// <summary>
        /// Species that have a mega form that cannot exist outside of battle.
        /// </summary>
        /// <remarks>Using a held item to change form during battle, via an in-battle transformation feature.</remarks>
        private static readonly HashSet<int> BattleMegas = new()
        {
            // XY
            (int)Venusaur, (int)Charizard, (int)Blastoise,
            (int)Alakazam, (int)Gengar, (int)Kangaskhan, (int)Pinsir,
            (int)Gyarados, (int)Aerodactyl, (int)Mewtwo,

            (int)Ampharos, (int)Scizor, (int)Heracross, (int)Houndoom, (int)Tyranitar,

            (int)Blaziken, (int)Gardevoir, (int)Mawile, (int)Aggron, (int)Medicham,
            (int)Manectric, (int)Banette, (int)Absol, (int)Latios, (int)Latias,

            (int)Garchomp, (int)Lucario, (int)Abomasnow,

            // AO
            (int)Beedrill, (int)Pidgeot, (int)Slowbro,

            (int)Steelix,

            (int)Sceptile, (int)Swampert, (int)Sableye, (int)Sharpedo, (int)Camerupt,
            (int)Altaria, (int)Glalie, (int)Salamence, (int)Metagross, (int)Rayquaza,

            (int)Lopunny, (int)Gallade,
            (int)Audino, (int)Diancie,

            // USUM
            (int)Necrozma, // Ultra Necrozma
        };

        /// <summary>
        /// Species that have a primal form that cannot exist outside of battle.
        /// </summary>
        private static readonly HashSet<int> BattlePrimals = new() { (int)Kyogre, (int)Groudon };

        private static readonly HashSet<int> BattleOnly = GetBattleFormSet();

        private static HashSet<int> GetBattleFormSet()
        {
            var hs = new HashSet<int>(BattleForms);
            hs.UnionWith(BattleMegas);
            hs.UnionWith(BattlePrimals);
            return hs;
        }

        /// <summary>
        /// Chhecks if the <see cref="form"/> for the <see cref="species"/> is a Totem form.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        public static bool IsTotemForm(int species, int form, int format)
        {
            if (format != 7)
                return false;
            if (form == 0)
                return false;
            if (!Legal.Totem_USUM.Contains(species))
                return false;
            if (species == (int)Mimikyu)
                return form is 2 or 3;
            if (Legal.Totem_Alolan.Contains(species))
                return form == 2;
            return form == 1;
        }

        /// <summary>
        /// Gets the base <see cref="form"/> for the <see cref="species"/> when the Totem form is reverted (on transfer).
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        public static int GetTotemBaseForm(int species, int form)
        {
            if (species == (int)Mimikyu)
                return form - 2;
            return form - 1;
        }

        /// <summary>
        /// Checks if the <see cref="form"/> exists for the <see cref="species"/> without having an associated <see cref="PersonalInfo"/> index.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="format">Current generation format</param>
        /// <seealso cref="HasFormValuesNotIndicatedByPersonal"/>
        public static bool IsValidOutOfBoundsForm(int species, int form, int format) => (Species) species switch
        {
            Unown => form < (format == 2 ? 26 : 28), // A-Z : A-Z?!
            Mothim => form < 3, // Burmy base form is kept

            Scatterbug => form < 18, // Vivillon Pre-evolutions
            Spewpa => form < 18, // Vivillon Pre-evolutions

            _ => false
        };

        /// <summary>
        /// Checks if the <see cref="PKM"/> data should have a drop-down selection visible for the <see cref="PKM.Form"/> value.
        /// </summary>
        /// <param name="pi">Game specific personal info</param>
        /// <param name="species"><see cref="Species"/> ID</param>
        /// <param name="format"><see cref="PKM.Form"/> ID</param>
        /// <returns>True if has formes that can be provided by <see cref="FormConverter.GetFormList"/>, otherwise false for none.</returns>
        public static bool HasFormSelection(PersonalInfo pi, int species, int format)
        {
            if (format <= 3 && species != (int)Unown)
                return false;

            if (HasFormValuesNotIndicatedByPersonal.Contains(species))
                return true;

            int count = pi.FormCount;
            return count > 1;
        }

        /// <summary>
        /// <seealso cref="IsValidOutOfBoundsForm"/>
        /// </summary>
        private static readonly HashSet<int> HasFormValuesNotIndicatedByPersonal = new()
        {
            (int)Unown,
            (int)Mothim, // (Burmy forme carried over, not cleared)
            (int)Scatterbug, (int)Spewpa, // Vivillon pre-evos
        };
    }
}
