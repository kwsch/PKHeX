using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for exporting and importing <see cref="PKM"/> data in Pokémon Showdown's text format.
    /// </summary>
    public sealed class ShowdownSet : IGigantamax
    {
        private static readonly string[] genders = {"M", "F", ""};
        private static readonly string[] genderForms = {"", "F", ""};
        private static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        private static readonly string[] Splitters = {"\r\n", "\n"};
        private static readonly string[] StatSplitters = { " / ", " " };
        private static readonly string[] LineSplit = {": "};
        private static readonly string[] ItemSplit = {" @ "};
        private static readonly char[] ParenJunk = { '[', ']', '(', ')' };
        private static readonly ushort[] DashedSpecies = {782, 783, 784, 250, 032, 029}; // Kommo-o, Ho-Oh, Nidoran-M, Nidoran-F
        private const int MAX_SPECIES = (int)Core.Species.MAX_COUNT - 1;
        private static readonly GameStrings DefaultStrings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        /// <summary>
        /// <see cref="PKM.Species"/> of the Set entity.
        /// </summary>
        public int Species { get; private set; } = -1;

        /// <summary>
        /// <see cref="PKM.Format"/> of the Set entity it is specific to.
        /// </summary>
        public int Format { get; private set; } = PKMConverter.Format;

        /// <summary>
        /// <see cref="PKM.Nickname"/> of the Set entity.
        /// </summary>
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="PKM.Gender"/> name of the Set entity.
        /// </summary>
        public string Gender { get; private set; } = string.Empty;

        /// <summary>
        /// <see cref="PKM.HeldItem"/> of the Set entity.
        /// </summary>
        public int HeldItem { get; private set; }

        /// <summary>
        /// <see cref="PKM.Ability"/> of the Set entity.
        /// </summary>
        public int Ability { get; private set; } = -1;

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        public int Level { get; private set; } = 100;

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        public bool Shiny { get; private set; }

        /// <summary>
        /// <see cref="PKM.CurrentFriendship"/> of the Set entity.
        /// </summary>
        public int Friendship { get; private set; } = 255;

        /// <summary>
        /// <see cref="PKM.StatNature"/> of the Set entity.
        /// </summary>
        public int Nature { get; set; } = -1;

        /// <summary>
        /// <see cref="PKM.AltForm"/> name of the Set entity, stored in PKHeX style (instead of Showdown's)
        /// </summary>
        public string Form { get; private set; } = string.Empty;

        /// <summary>
        /// <see cref="PKM.AltForm"/> of the Set entity.
        /// </summary>
        public int FormIndex { get; private set; }

        /// <summary>
        /// <see cref="PKM.EVs"/> of the Set entity.
        /// </summary>
        public int[] EVs { get; private set; } = {00, 00, 00, 00, 00, 00};

        /// <summary>
        /// <see cref="PKM.IVs"/> of the Set entity.
        /// </summary>
        public int[] IVs { get; private set; } = {31, 31, 31, 31, 31, 31};

        /// <summary>
        /// <see cref="PKM.HPType"/> of the Set entity.
        /// </summary>
        public int HiddenPowerType { get; set; } = -1;

        /// <summary>
        /// <see cref="PKM.Moves"/> of the Set entity.
        /// </summary>
        public int[] Moves { get; } = {0, 0, 0, 0};

        /// <summary>
        /// <see cref="IGigantamax.CanGigantamax"/> of the Set entity.
        /// </summary>
        public bool CanGigantamax { get; set; }

        /// <summary>
        /// Any lines that failed to be parsed.
        /// </summary>
        public readonly List<string> InvalidLines = new List<string>();

        private GameStrings Strings { get; set; } = DefaultStrings;

        private int[] IVsSpeedFirst => new[] {IVs[0], IVs[1], IVs[2], IVs[5], IVs[3], IVs[4]};
        private int[] IVsSpeedLast => new[] {IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3]};
        private int[] EVsSpeedFirst => new[] {EVs[0], EVs[1], EVs[2], EVs[5], EVs[3], EVs[4]};
        private int[] EVsSpeedLast => new[] {EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3]};

        /// <summary>
        /// Loads a new blank <see cref="ShowdownSet"/>.
        /// </summary>
        public ShowdownSet() { }

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string.
        /// </summary>
        /// <param name="input">Single-line string which will be split before loading.</param>
        public ShowdownSet(string input)
        {
            var lines = input.Split(Splitters, StringSplitOptions.None);
            LoadLines(lines);
        }

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string.
        /// </summary>
        /// <param name="lines">Enumerable list of lines.</param>
        public ShowdownSet(IEnumerable<string> lines)
        {
            LoadLines(lines);
        }

        private void LoadLines(IEnumerable<string> lines)
        {
            lines = lines.Select(z => z.Replace('\'', '’').Replace('–', '-').Trim()); // Sanitize apostrophes & dashes
            lines = lines.Where(z => z.Length > 2);

            ParseLines(lines);

            Form = ConvertFormFromShowdown(Form, Species, Ability);
            // Set Form
            if (Form.Length == 0)
            {
                FormIndex = 0;
                return;
            }
            string[] formStrings = FormConverter.GetFormList(Species, Strings.Types, Strings.forms, genderForms, Format);
            FormIndex = Math.Max(0, Array.FindIndex(formStrings, z => z.Contains(Form)));
        }

        private const int MaxMoveCount = 4;

        private void ParseLines(IEnumerable<string> lines)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            using var e = lines.GetEnumerator();
            if (!e.MoveNext())
                return;

            ParseFirstLine(e.Current);
            int movectr = 0;
            while (e.MoveNext())
            {
                var line = e.Current;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line[0] == '-')
                {
                    string moveString = ParseLineMove(line);
                    int move = StringUtil.FindIndexIgnoreCase(Strings.movelist, moveString);
                    if (move < 0)
                        InvalidLines.Add($"Unknown Move: {moveString}");
                    else if (Moves.Contains(move))
                        InvalidLines.Add($"Duplicate Move: {moveString}");
                    else
                        Moves[movectr++] = move;

                    if (movectr == MaxMoveCount)
                        return; // End of moves, end of set data
                    continue;
                }

                if (movectr != 0)
                    break;

                var split = line.Split(LineSplit, StringSplitOptions.None);
                var valid = split.Length == 1
                    ? ParseSingle(line) // Nature
                    : ParseEntry(split[0].Trim(), split[1].Trim());
                if (!valid)
                    InvalidLines.Add(line);
            }
        }

        private bool ParseSingle(string identifier)
        {
            if (!identifier.EndsWith("Nature"))
                return false;
            var naturestr = identifier.Split(' ')[0].Trim();
            return (Nature = StringUtil.FindIndexIgnoreCase(Strings.natures, naturestr)) >= 0;
        }

        private bool ParseEntry(string identifier, string value)
        {
            switch (identifier)
            {
                case "Trait": case "Ability": return (Ability = StringUtil.FindIndexIgnoreCase(Strings.abilitylist, value)) >= 0;
                case "Shiny": return Shiny = value.Trim() == "Yes";
                case "Nature": return (Nature = StringUtil.FindIndexIgnoreCase(Strings.natures, value)) >= 0;
                case "EV": case "EVs": ParseLineEVs(value); return true;
                case "IV": case "IVs": ParseLineIVs(value); return true;
                case "Level":
                {
                    if (!int.TryParse(value.Trim(), out int val))
                        return false;
                    Level = val;
                    return true;
                }
                case "Happiness": case "Friendship":
                {
                    if (!int.TryParse(value.Trim(), out int val))
                        return false;
                    Friendship = val;
                    return true;
                }
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the standard Text representation of the set details.
        /// </summary>
        public string Text => GetText();

        /// <summary>
        /// Gets the localized Text representation of the set details.
        /// </summary>
        /// <param name="lang">2 character language code</param>
        public string LocalizedText(string lang) => LocalizedText(GameLanguage.GetLanguageIndex(lang));

        /// <summary>
        /// Gets the localized Text representation of the set details.
        /// </summary>
        /// <param name="lang">Language ID</param>
        private string LocalizedText(int lang)
        {
            var strings = GameInfo.GetStrings(lang);
            return GetText(strings);
        }

        private string GetText(GameStrings? strings = null)
        {
            if (Species <= 0 || Species > MAX_SPECIES)
                return string.Empty;

            if (strings != null)
                Strings = strings;

            var result = GetSetLines();
            return string.Join(Environment.NewLine, result);
        }

        public List<string> GetSetLines()
        {
            var result = new List<string>();

            // First Line: Name, Nickname, Gender, Item
            var form = ConvertFormToShowdown(Form, Species);
            result.Add(GetStringFirstLine(form));

            // IVs
            var ivs = GetStringStats(IVsSpeedLast, Format < 3 ? 15 : 31);
            if (ivs.Count > 0)
                result.Add($"IVs: {string.Join(" / ", ivs)}");

            // EVs
            var evs = GetStringStats(EVsSpeedLast, 0);
            if (evs.Count > 0)
                result.Add($"EVs: {string.Join(" / ", evs)}");

            // Secondary Stats
            if ((uint)Ability < Strings.Ability.Count)
                result.Add($"Ability: {Strings.Ability[Ability]}");
            if (Level != 100)
                result.Add($"Level: {Level}");
            if (Shiny)
                result.Add("Shiny: Yes");

            if ((uint)Nature < Strings.Natures.Count)
                result.Add($"{Strings.Natures[Nature]} Nature");

            // Moves
            result.AddRange(GetStringMoves());
            return result;
        }

        private string GetStringFirstLine(string form)
        {
            string specForm = Strings.Species[Species];
            if (form.Length != 0)
                specForm += $"-{form.Replace("Mega ", "Mega-")}";
            else if (Species == (int)Core.Species.NidoranM)
                specForm = specForm.Replace("♂", "-M");
            else if (Species == (int)Core.Species.NidoranF)
                specForm = specForm.Replace("♀", "-F");

            if (CanGigantamax)
                specForm += Gmax;

            string result = GetSpeciesNickname(specForm);
            if (Gender.Length != 0)
                result += $" ({Gender})";
            if (HeldItem > 0)
            {
                var items = Strings.GetItemStrings(Format);
                if ((uint)HeldItem < items.Count)
                    result += $" @ {items[HeldItem]}";
            }
            return result;
        }

        private string GetSpeciesNickname(string specForm)
        {
            if (Nickname.Length == 0)
                return specForm;
            bool isNicknamed = SpeciesName.IsNicknamedAnyLanguage(Species, Nickname, Format);
            if (!isNicknamed)
                return specForm;
            return $"{Nickname} ({specForm})";
        }

        private static IList<string> GetStringStats(int[] stats, int ignore)
        {
            var result = new List<string>();
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i] == ignore)
                    continue; // ignore unused stats
                result.Add($"{stats[i]} {StatNames[i]}");
            }
            return result;
        }

        private IEnumerable<string> GetStringMoves()
        {
            foreach (int move in Moves.Where(move => move != 0 && move < Strings.Move.Count))
            {
                if (move == 237) // Hidden Power
                {
                    yield return $"- {Strings.Move[move]} [{Strings.Types[1 + HiddenPowerType]}]";
                    continue;
                }

                yield return $"- {Strings.Move[move]}";
            }
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
        /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
        /// </summary>
        /// <param name="pkm">PKM to convert to string</param>
        /// <returns>New ShowdownSet object representing the input <see cref="pkm"/></returns>
        public ShowdownSet(PKM pkm)
        {
            if (pkm.Species <= 0)
                return;

            Format = pkm.Format;

            Nickname = pkm.Nickname;
            Species = pkm.Species;
            HeldItem = pkm.HeldItem;
            Ability = pkm.Ability;
            EVs = pkm.EVs;
            IVs = pkm.IVs;
            Moves = pkm.Moves;
            Nature = pkm.StatNature;
            Gender = genders[pkm.Gender < 2 ? pkm.Gender : 2];
            Friendship = pkm.CurrentFriendship;
            Level = Experience.GetLevel(pkm.EXP, pkm.PersonalInfo.EXPGrowth);
            Shiny = pkm.IsShiny;

            if (pkm is IGigantamax g)
                CanGigantamax = g.CanGigantamax;

            HiddenPowerType = HiddenPower.GetType(IVs, Format);
            if (pkm is IHyperTrain h)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (h.GetHT(i))
                        IVs[i] = pkm.MaxIV;
                }
            }

            SetFormString(pkm.AltForm);
        }

        public void SetFormString(int index)
        {
            FormIndex = index;
            if (index <= 0)
            {
                Form = string.Empty;
                return;
            }
            var Forms = FormConverter.GetFormList(Species, Strings.Types, Strings.forms, genderForms, Format);
            Form = FormIndex >= Forms.Length ? string.Empty : Forms[index];
        }

        private void ParseFirstLine(string first)
        {
            if (first.Contains(" @ "))
            {
                string[] pieces = first.Split(ItemSplit, StringSplitOptions.None);
                string itemstr = pieces[pieces.Length - 1].Trim();

                ParseItemStr(itemstr);
                ParseFirstLineNoItem(pieces[0]);
            }
            else
            {
                ParseFirstLineNoItem(first.Trim());
            }
        }

        private void ParseItemStr(string itemstr)
        {
            if (tryGetItem(Format))
                return;
            if (tryGetItem(3))
                return;
            if (tryGetItem(2))
                return;
            InvalidLines.Add($"Unknown Item: {itemstr}");

            bool tryGetItem(int format)
            {
                var items = (string[])Strings.GetItemStrings(format); // ireadonlylist->string[] must be possible for the provided strings
                int item = StringUtil.FindIndexIgnoreCase(items, itemstr);
                if (item < 0)
                    return false;
                HeldItem = item;
                Format = format;
                return true;
            }
        }

        private void ParseFirstLineNoItem(string line)
        {
            // Gender Detection
            string last3 = line.Substring(line.Length - 3);
            if (last3 == "(M)" || last3 == "(F)")
            {
                Gender = last3.Substring(1, 1);
                line = line.Substring(0, line.Length - 3);
            }
            else if (line.Contains(Strings.Species[(int)Core.Species.Meowstic]) || line.Contains(Strings.Species[(int)Core.Species.Indeedee])) // Meowstic Edge Case with no gender provided
            {
                Gender = "M";
            }

            // Nickname Detection
            if (line.Contains('(') && line.Contains(')'))
                ParseSpeciesNickname(line);
            else
                ParseSpeciesForm(line);
        }

        private bool ParseSpeciesForm(string spec)
        {
            spec = spec.Trim();
            if (spec.EndsWith(Gmax))
            {
                CanGigantamax = true;
                spec = spec.Substring(0, spec.Length - Gmax.Length);
            }

            if ((Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, spec)) >= 0) // success, nothing else!
                return true;

            // Forme string present.
            int end = spec.LastIndexOf('-');
            if (end < 0)
                return false;

            Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, spec.Substring(0, end));
            Form = spec.Substring(end + 1);

            if (Species >= 0)
                return true;

            // failure to parse, check edge cases
            foreach (var e in DashedSpecies)
            {
                if (!spec.StartsWith(Strings.Species[e].Replace("♂", "-M").Replace("♀", "-F")))
                    continue;
                Species = e;
                Form = spec.Substring(Strings.Species[e].Length);
                return true;
            }

            // Version Megas
            end = spec.LastIndexOf('-', Math.Max(0, end - 1));
            if (end < 0)
                return false;
            Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, spec.Substring(0, end));
            Form = spec.Substring(end + 1);

            return Species >= 0;
        }

        private void ParseSpeciesNickname(string line)
        {
            int index = line.LastIndexOf('(');
            string n1, n2;
            if (index > 1) // correct format
            {
                n1 = line.Substring(0, index).Trim();
                n2 = line.Substring(index).Trim();
                n2 = RemoveAll(n2, ParenJunk); // Trim out excess data
            }
            else // nickname first (manually created set, incorrect)
            {
                int end = line.IndexOf(')');
                n2 = line.Substring(index + 1, end - 1);
                n1 = line.Substring(end + 2);
            }

            if (ParseSpeciesForm(n2))
            {
                // successful parse on n2=>Species/Form, n1 is nickname
                Nickname = n1;
                return;
            }
            // other case is possibly true (or both invalid).
            Nickname = n2;
            ParseSpeciesForm(n1);
        }

        private string ParseLineMove(string line)
        {
            const int hiddenPower = 237;
            string moveString = line.Substring(line[1] == ' ' ? 2 : 1).Split('/')[0].Trim();
            if (!moveString.StartsWith(Strings.Move[hiddenPower])) // Hidden Power
                return moveString; // regular move

            if (moveString.Length <= 13)
                return Strings.Move[hiddenPower];

            // Defined Hidden Power
            string type = moveString.Substring(13);
            type = RemoveAll(type, ParenJunk); // Trim out excess data
            int hpVal = StringUtil.FindIndexIgnoreCase(Strings.types, type) - 1; // Get HP Type

            HiddenPowerType = hpVal;
            if (IVs.Any(z => z != 31))
            {
                if (!HiddenPower.SetIVsForType(hpVal, IVs, Format))
                    InvalidLines.Add($"Invalid IVs for Hidden Power Type: {type}");
            }
            else if (hpVal >= 0)
            {
                IVs = HiddenPower.SetIVs(hpVal, IVs, Format); // Get IVs
            }
            else
            {
                InvalidLines.Add($"Invalid Hidden Power Type: {type}");
            }
            return Strings.Move[hiddenPower];
        }

        private void ParseLineEVs(string line)
        {
            var list = SplitLineStats(line);
            if ((list.Length & 1) == 1)
                InvalidLines.Add("Unknown EV input.");
            for (int i = 0; i < list.Length / 2; i++)
            {
                int pos = i * 2;
                int index = StringUtil.FindIndexIgnoreCase(StatNames, list[pos + 1]);
                if (index >= 0 && ushort.TryParse(list[pos + 0], out var EV))
                    EVs[index] = EV;
                else
                    InvalidLines.Add($"Unknown EV stat: {list[pos]}");
            }
            EVs = EVsSpeedFirst;
        }

        private void ParseLineIVs(string line)
        {
            var list = SplitLineStats(line);
            if ((list.Length & 1) == 1)
                InvalidLines.Add("Unknown IV input.");
            for (int i = 0; i < list.Length / 2; i++)
            {
                int pos = i * 2;
                int index = StringUtil.FindIndexIgnoreCase(StatNames, list[pos + 1]);
                if (index >= 0 && byte.TryParse(list[pos + 0], out var IV))
                    IVs[index] = IV;
                else
                    InvalidLines.Add($"Unknown IV stat: {list[pos]}");
            }
            IVs = IVsSpeedFirst;
        }

        private const string Minior = "Meteor";
        private const string Gmax = "-Gmax";

        private static string ConvertFormToShowdown(string form, int spec)
        {
            if (form.Length == 0)
            {
                return spec switch
                {
                    (int)Core.Species.Minior => Minior,
                    _ => form
                };
            }

            switch (spec)
            {
                case (int)Core.Species.Basculin when form == "Blue":
                    return "Blue-Striped";
                case (int)Core.Species.Vivillon when form == "Poké Ball":
                    return "Pokeball";
                case (int)Core.Species.Zygarde:
                    form = form.Replace("-C", string.Empty);
                    return form.Replace("50%", string.Empty);
                case (int)Core.Species.Minior:
                    if (form.StartsWith("M-"))
                        return Minior;
                    return form.Replace("C-", string.Empty);
                case (int)Core.Species.Necrozma when form == "Dusk":
                    return $"{form}-Mane";
                case (int)Core.Species.Necrozma when form == "Dawn":
                    return $"{form}-Wings";

                case (int)Core.Species.Furfrou:
                case (int)Core.Species.Greninja:
                case (int)Core.Species.Rockruff:
                case (int)Core.Species.Polteageist:
                case (int)Core.Species.Sinistea:
                    return string.Empty;
                default:
                    if (Legal.Totem_USUM.Contains(spec) && form == "Large")
                        return Legal.Totem_Alolan.Contains(spec) && spec != (int)Core.Species.Mimikyu ? "Alola-Totem" : "Totem";
                    return form.Replace(' ', '-');
            }
        }

        private static string ConvertFormFromShowdown(string form, int spec, int ability)
        {
            if (form.Length == 0)
                form = form.Replace(' ', '-'); // inconsistencies are great

            switch (spec)
            {
                case (int)Core.Species.Basculin when form == "Blue-Striped":
                    return "Blue";
                case (int)Core.Species.Greninja when ability == 210:
                    return "Ash"; // Battle Bond
                case (int)Core.Species.Vivillon when form == "Pokeball":
                    return "Poké Ball";

                // Zygarde
                case (int)Core.Species.Zygarde when form.Length == 0:
                    return ability == 211 ? "50%-C" : "50%";
                case (int)Core.Species.Zygarde when form == "Complete":
                    return form;
                case (int)Core.Species.Zygarde when ability == 211:
                    return "-C"; // Power Construct

                case (int)Core.Species.Rockruff when ability == 020: // Rockruff-1
                    return "Dusk";

                // Minior
                case (int)Core.Species.Minior when form.Length != 0 && form != Minior:
                    return $"C-{form}";

                // Necrozma
                case (int)Core.Species.Necrozma when form == "Dusk-Mane" || form == "Dusk Mane":
                    return "Dusk";
                case (int)Core.Species.Necrozma when form == "Dawn-Wings" || form == "Dawn Wings":
                    return "Dawn";

                // Toxtricity
                case (int)Core.Species.Toxtricity when form == "Low-Key":
                    return "Low Key";

                // Darmanitan
                case (int)Core.Species.Darmanitan:
                    if (form == "Galar-Zen")
                        return "Galar Zen";
                    return form;

                default:
                    if (Legal.Totem_USUM.Contains(spec) && form.EndsWith("Totem"))
                        return "Large";
                    return form;
            }
        }

        private static string RemoveAll(string original, char[] remove) => string.Concat(original.Where(z => !remove.Contains(z)));

        private static string[] SplitLineStats(string line)
        {
            // Because people think they can type sets out...
            return line
                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(StatSplitters, StringSplitOptions.None);
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
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <returns>Consumable list of <see cref="Text"/> lines.</returns>
        public static IEnumerable<string> GetShowdownSets(IEnumerable<PKM> data) => data.Where(p => p.Species != 0).Select(GetShowdownText);

        /// <summary>
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data, and combines it into one string.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <param name="separator">Splitter between each set.</param>
        /// <returns>Single string containing all <see cref="Text"/> lines.</returns>
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
