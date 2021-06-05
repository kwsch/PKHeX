using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for exporting and importing <see cref="PKM"/> data in Pokémon Showdown's text format.
    /// </summary>
    public sealed class ShowdownSet : IBattleTemplate
    {
        private static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        private static readonly string[] Splitters = {"\r\n", "\n"};
        private static readonly string[] StatSplitters = { " / ", " " };
        private static readonly string[] LineSplit = {": "};
        private static readonly string[] ItemSplit = {" @ "};
        private static readonly char[] ParenJunk = { '[', ']', '(', ')' };
        private static readonly ushort[] DashedSpecies = {782, 783, 784, 250, 032, 029}; // Kommo-o, Ho-Oh, Nidoran-M, Nidoran-F
        private const int MAX_SPECIES = (int)MAX_COUNT - 1;
        private static readonly GameStrings DefaultStrings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        /// <inheritdoc/>
        public int Species { get; private set; } = -1;

        /// <inheritdoc/>
        public int Format { get; private set; } = PKMConverter.Format;

        /// <inheritdoc/>
        public string Nickname { get; set; } = string.Empty;

        /// <inheritdoc/>
        public int Gender { get; private set; } = -1;

        /// <inheritdoc/>
        public int HeldItem { get; private set; }

        /// <inheritdoc/>
        public int Ability { get; private set; } = -1;

        /// <inheritdoc/>
        public int Level { get; private set; } = 100;

        /// <inheritdoc/>
        public bool Shiny { get; private set; }

        /// <inheritdoc/>
        public int Friendship { get; private set; } = 255;

        /// <inheritdoc/>
        public int Nature { get; set; } = -1;

        /// <inheritdoc/>
        public string FormName { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public int Form { get; private set; }

        /// <inheritdoc/>
        public int[] EVs { get; private set; } = {00, 00, 00, 00, 00, 00};

        /// <inheritdoc/>
        public int[] IVs { get; private set; } = {31, 31, 31, 31, 31, 31};

        /// <inheritdoc/>
        public int HiddenPowerType { get; set; } = -1;

        /// <inheritdoc/>
        public int[] Moves { get; } = {0, 0, 0, 0};

        /// <inheritdoc/>
        public bool CanGigantamax { get; set; }

        /// <summary>
        /// Any lines that failed to be parsed.
        /// </summary>
        public readonly List<string> InvalidLines = new();

        private GameStrings Strings { get; set; } = DefaultStrings;

        private int[] IVsSpeedFirst => new[] {IVs[0], IVs[1], IVs[2], IVs[5], IVs[3], IVs[4]};
        private int[] IVsSpeedLast => new[] {IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3]};
        private int[] EVsSpeedFirst => new[] {EVs[0], EVs[1], EVs[2], EVs[5], EVs[3], EVs[4]};
        private int[] EVsSpeedLast => new[] {EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3]};

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string.
        /// </summary>
        /// <param name="input">Single-line string which will be split before loading.</param>
        public ShowdownSet(string input) : this(input.Split(Splitters, 0)) { }

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string.
        /// </summary>
        /// <param name="lines">Enumerable list of lines.</param>
        public ShowdownSet(IEnumerable<string> lines) => LoadLines(lines);

        private void LoadLines(IEnumerable<string> lines)
        {
            lines = lines.Select(z => z.Replace('\'', '’').Replace('–', '-').Trim()); // Sanitize apostrophes & dashes
            lines = lines.Where(z => z.Length > 2);

            ParseLines(lines);

            FormName = ShowdownParsing.SetShowdownFormName(Species, FormName, Ability);
            Form = ShowdownParsing.GetFormFromString(FormName, Strings, Species, Format);

            // Handle edge case with fixed-gender forms.
            if (Species is (int) Meowstic or (int) Indeedee)
            {
                if (Gender == 1) // Recognized with (F)
                {
                    FormName = "F";
                    Form = 1;
                }
                else
                {
                    FormName = Form == 1 ? "F" : "M";
                    Gender = Form;
                }
            }
        }

        private const int MaxMoveCount = 4;

        private void ParseLines(IEnumerable<string> lines)
        {
            using var e = lines.GetEnumerator();
            if (!e.MoveNext())
                return;

            ParseFirstLine(e.Current!);
            int movectr = 0;
            while (e.MoveNext())
            {
                var line = e.Current!;
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
                case "Ability" or "Trait": return (Ability = StringUtil.FindIndexIgnoreCase(Strings.abilitylist, value)) >= 0;
                case "Shiny": return Shiny = value.Trim() == "Yes";
                case "Gigantamax": return CanGigantamax = value.Trim() == "Yes";
                case "Nature": return (Nature = StringUtil.FindIndexIgnoreCase(Strings.natures, value)) >= 0;
                case "EV" or "EVs": ParseLineEVs(value); return true;
                case "IV" or "IVs": ParseLineIVs(value); return true;
                case "Level":
                {
                    if (!int.TryParse(value.Trim(), out int val))
                        return false;
                    Level = val;
                    return true;
                }
                case "Friendship" or "Happiness":
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
            var form = ShowdownParsing.GetShowdownFormName(Species, FormName);
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
            if (CanGigantamax)
                result.Add("Gigantamax: Yes");
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
            else if (Species == (int)NidoranM)
                specForm = specForm.Replace("♂", "-M");
            else if (Species == (int)NidoranF)
                specForm = specForm.Replace("♀", "-F");

            string result = GetSpeciesNickname(specForm);

            // omit genderless or nonspecific
            if (Gender is 1)
                result += " (F)";
            else if (Gender is 0)
                result += " (M)";

            if (HeldItem > 0)
            {
                var items = Strings.GetItemStrings(Format);
                if ((uint)HeldItem < items.Length)
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
            Gender = pkm.Gender < 2 ? pkm.Gender : 2;
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
                    if (h.IsHyperTrained(i))
                        IVs[i] = pkm.MaxIV;
                }
            }

            FormName = ShowdownParsing.GetStringFromForm(Form = pkm.Form, Strings, Species, Format);
        }

        private void ParseFirstLine(string first)
        {
            if (first.Contains(" @ "))
            {
                string[] pieces = first.Split(ItemSplit, StringSplitOptions.None);
                string itemName = pieces[^1].Trim();

                ParseItemName(itemName);
                ParseFirstLineNoItem(pieces[0]);
            }
            else
            {
                ParseFirstLineNoItem(first);
            }
        }

        private void ParseItemName(string itemName)
        {
            if (TrySetItem(Format))
                return;
            if (TrySetItem(3))
                return;
            if (TrySetItem(2))
                return;
            InvalidLines.Add($"Unknown Item: {itemName}");

            bool TrySetItem(int format)
            {
                var items = Strings.GetItemStrings(format);
                int item = StringUtil.FindIndexIgnoreCase(items, itemName);
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
            if (line.EndsWith("(M)"))
            {
                line = line[..^3];
                Gender = 0;
            }
            else if (line.EndsWith("(F)"))
            {
                line = line[..^3];
                Gender = 1;
            }

            // Nickname Detection
            if (line.Contains('(') && line.Contains(')'))
                ParseSpeciesNickname(line);
            else
                ParseSpeciesForm(line);
        }

        private const string Gmax = "-Gmax";

        private bool ParseSpeciesForm(string speciesLine)
        {
            speciesLine = speciesLine.Trim();
            if (speciesLine.Length == 0)
                return false;

            if (speciesLine.EndsWith(Gmax))
            {
                CanGigantamax = true;
                speciesLine = speciesLine[..^Gmax.Length];
            }

            if ((Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine)) >= 0) // success, nothing else!
                return true;

            // Form string present.
            int end = speciesLine.LastIndexOf('-');
            if (end < 0)
                return false;

            Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
            FormName = speciesLine[(end + 1)..];

            if (Species >= 0)
                return true;

            // failure to parse, check edge cases
            foreach (var e in DashedSpecies)
            {
                var sn = Strings.Species[e];
                if (!speciesLine.StartsWith(sn.Replace("♂", "-M").Replace("♀", "-F")))
                    continue;
                Species = e;
                FormName = speciesLine[sn.Length..];
                return true;
            }

            // Version Megas
            end = speciesLine.LastIndexOf('-', Math.Max(0, end - 1));
            if (end < 0)
                return false;
            Species = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
            FormName = speciesLine[(end + 1)..];

            return Species >= 0;
        }

        private void ParseSpeciesNickname(string line)
        {
            int index = line.LastIndexOf('(');
            string species, nickname;
            if (index > 1) // parenthesis value after: Nickname (Species), correct.
            {
                nickname = line[..index].Trim();
                species = line[index..].Trim();
                species = RemoveAll(species, ParenJunk); // Trim out excess data
            }
            else // parenthesis value before: (Species) Nickname, incorrect
            {
                int start = index + 1;
                int end = line.IndexOf(')');
                var tmp = line[start..end];
                if (end < line.Length - 2)
                {
                    nickname = line[(end + 2)..];
                    species = tmp;
                }
                else // (Species), or garbage
                {
                    species = tmp;
                    nickname = string.Empty;
                }
            }

            if (ParseSpeciesForm(species))
                Nickname = nickname;
            else if (ParseSpeciesForm(nickname))
                Nickname = species;
        }

        private string ParseLineMove(string line)
        {
            const int hiddenPower = 237;
            string moveString = line[(line[1] == ' ' ? 2 : 1)..].Split('/')[0].Trim();
            if (!moveString.StartsWith(Strings.Move[hiddenPower])) // Hidden Power
                return moveString; // regular move

            if (moveString.Length <= 13)
                return Strings.Move[hiddenPower];

            // Defined Hidden Power
            string type = moveString[13..];
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
                if (index >= 0 && byte.TryParse(list[pos + 0], out var iv))
                    IVs[index] = iv;
                else
                    InvalidLines.Add($"Unknown IV stat: {list[pos]}");
            }
            IVs = IVsSpeedFirst;
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
    }
}
