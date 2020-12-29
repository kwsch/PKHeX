using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for parsing details for <see cref="ShowdownSet"/> objects.
    /// </summary>
    public static class ShowdownParsing
    {
        private static readonly string[] genderForms = { "", "F", "" };

        /// <summary>
        /// Gets the Form ID from the input <see cref="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strings"></param>
        /// <param name="species">Species ID the form belongs to</param>
        /// <param name="format">Format the form name should appear in</param>
        /// <returns>Zero (base form) if no form matches the input string.</returns>
        public static int GetFormFromString(string name, GameStrings strings, int species, int format)
        {
            if (name.Length == 0)
                return 0;

            string[] formStrings = FormConverter.GetFormList(species, strings.Types, strings.forms, genderForms, format);
            return Math.Max(0, Array.FindIndex(formStrings, z => z.Contains(name)));
        }

        /// <summary>
        /// Converts a Form ID to string.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="strings"></param>
        /// <param name="species"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetStringFromForm(int form, GameStrings strings, int species, int format)
        {
            if (form <= 0)
                return string.Empty;

            var forms = FormConverter.GetFormList(species, strings.Types, strings.forms, genderForms, format);
            return form >= forms.Length ? string.Empty : forms[form];
        }

        private const string MiniorFormName = "Meteor";

        /// <summary>
        /// Converts the PKHeX standard form name to Showdown's form name.
        /// </summary>
        /// <param name="species">Species ID</param>
        /// <param name="form">PKHeX form name</param>
        public static string GetShowdownFormName(int species, string form)
        {
            if (form.Length == 0)
            {
                return species switch
                {
                    (int)Minior => MiniorFormName,
                    _ => form
                };
            }

            return species switch
            {
                (int)Basculin when form is "Blue"         => "Blue-Striped",
                (int)Vivillon when form is "Poké Ball"    => "Pokeball",
                (int)Zygarde                              => form.Replace("-C", string.Empty).Replace("50%", string.Empty),
                (int)Minior   when form.StartsWith("M-")  => MiniorFormName,
                (int)Minior                               => form.Replace("C-", string.Empty),
                (int)Necrozma when form is "Dusk"         => $"{form}-Mane",
                (int)Necrozma when form is "Dawn"         => $"{form}-Wings",
                (int)Polteageist or (int)Sinistea         => form == "Antique" ? form : string.Empty,

                (int)Furfrou or (int)Greninja or (int)Rockruff => string.Empty,

                _ => Legal.Totem_USUM.Contains(species) && form == "Large"
                    ? Legal.Totem_Alolan.Contains(species) && species != (int)Mimikyu ? "Alola-Totem" : "Totem"
                    : form.Replace(' ', '-')
            };
        }

        /// <summary>
        /// Converts the Showdown form name to PKHeX's form name.
        /// </summary>
        /// <param name="species">Species ID</param>
        /// <param name="form">Showdown form name</param>
        /// <param name="ability">Showdown ability ID</param>
        public static string SetShowdownFormName(int species, string form, int ability)
        {
            if (form.Length != 0)
                form = form.Replace(' ', '-'); // inconsistencies are great

            return species switch
            {
                (int)Basculin   when form == "Blue-Striped" => "Blue",
                (int)Vivillon   when form == "Pokeball"     => "Poké Ball",
                (int)Necrozma   when form == "Dusk-Mane"    => "Dusk",
                (int)Necrozma   when form == "Dawn-Wings"   => "Dawn",
                (int)Toxtricity when form == "Low-Key"      => "Low Key",
                (int)Darmanitan when form == "Galar-Zen"    => "Galar Zen",
                (int)Minior     when form != MiniorFormName => $"C-{form}",
                (int)Zygarde    when form == "Complete"     => form,
                (int)Zygarde    when ability == 211         => $"{(string.IsNullOrWhiteSpace(form) ? "50%" : "10%")}-C",
                (int)Greninja   when ability == 210         => "Ash", // Battle Bond
                (int)Rockruff   when ability == 020         => "Dusk", // Rockruff-1
                (int)Urshifu => form.Replace('-', ' '),

                _ => Legal.Totem_USUM.Contains(species) && form.EndsWith("Totem") ? "Large" : form,
            };
        }

        /// <summary>
        /// Fetches <see cref="ShowdownSet"/> data from the input <see cref="lines"/>.
        /// </summary>
        /// <param name="lines">Raw lines containing numerous multi-line set data.</param>
        /// <returns><see cref="ShowdownSet"/> objects until <see cref="lines"/> is consumed.</returns>
        public static IEnumerable<ShowdownSet> GetShowdownSets(IEnumerable<string> lines)
        {
            // exported sets always have >4 moves; new List will always require 1 resizing, allocate 2x to save 1 reallocation.
            // intro, nature, ability, (ivs, evs, shiny, level) 4*moves
            var setLines = new List<string>(8);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    setLines.Add(line);
                    continue;
                }
                if (setLines.Count == 0)
                    continue;
                yield return new ShowdownSet(setLines);
                setLines.Clear();
            }
            if (setLines.Count != 0)
                yield return new ShowdownSet(setLines);
        }

        /// <summary>
        /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
        /// </summary>
        /// <param name="pkm">PKM to convert to string</param>
        /// <returns>Multi line set data</returns>
        public static string GetShowdownText(PKM pkm)
        {
            if (pkm.Species == 0)
                return string.Empty;
            return new ShowdownSet(pkm).Text;
        }

        /// <summary>
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <returns>Consumable list of <see cref="ShowdownSet.Text"/> lines.</returns>
        public static IEnumerable<string> GetShowdownSets(IEnumerable<PKM> data) => data.Where(p => p.Species != 0).Select(GetShowdownText);

        /// <summary>
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data, and combines it into one string.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <param name="separator">Splitter between each set.</param>
        /// <returns>Single string containing all <see cref="ShowdownSet.Text"/> lines.</returns>
        public static string GetShowdownSets(IEnumerable<PKM> data, string separator) => string.Join(separator, GetShowdownSets(data));

        /// <summary>
        /// Gets a localized string preview of the provided <see cref="pk"/>.
        /// </summary>
        /// <param name="pk">Pokémon data</param>
        /// <param name="language">Language code</param>
        /// <returns>Multi-line string</returns>
        public static string GetLocalizedPreviewText(PKM pk, string language)
        {
            var set = new ShowdownSet(pk);
            if (pk.Format <= 2) // Nature preview from IVs
                set.Nature = Experience.GetNatureVC(pk.EXP);
            return set.LocalizedText(language);
        }
    }
}
