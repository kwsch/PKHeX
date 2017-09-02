using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ShowdownSet
    {
        // String to Values
        private static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        private const string Language = "en";
        private static readonly string[] types = Util.GetTypesList(Language);
        private static readonly string[] forms = Util.GetFormsList(Language);
        private static readonly string[] species = Util.GetSpeciesList(Language);
        private static readonly string[] items = Util.GetItemsList(Language);
        private static readonly string[] natures = Util.GetNaturesList(Language);
        private static readonly string[] moves = Util.GetMovesList(Language);
        private static readonly string[] abilities = Util.GetAbilitiesList(Language);
        private static readonly string[] hptypes = types.Skip(1).ToArray();
        private const int MAX_SPECIES = 802;

        // Default Set Data
        public string Nickname { get; set; }
        public int Species { get; private set; } = -1;
        public string Form { get; private set; }
        public string Gender { get; private set; }
        public int HeldItem { get; private set; }
        public int Ability { get; private set; }
        public int Level { get; private set; } = 100;
        public bool Shiny { get; private set; }
        public int Friendship { get; private set; } = 255;
        public int Nature { get; private set; }
        public int[] EVs { get; private set; } = {00, 00, 00, 00, 00, 00};
        public int[] IVs { get; private set; } = {31, 31, 31, 31, 31, 31};
        public int[] Moves { get; private set; } = {0, 0, 0, 0};
        public readonly List<string> InvalidLines = new List<string>();

        private int[] IVsSpeedFirst => new[] {IVs[0], IVs[1], IVs[2], IVs[5], IVs[3], IVs[4]};
        private int[] IVsSpeedLast => new[] {IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3]};
        private int[] EVsSpeedFirst => new[] {EVs[0], EVs[1], EVs[2], EVs[5], EVs[3], EVs[4]};
        private int[] EVsSpeedLast => new[] {EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3]};

        // Parsing Utility
        public ShowdownSet(string input = null)
        {
            if (input == null)
                return;

            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace("'", "’").Trim(); // Sanitize apostrophes

            lines = lines.Where(line => line.Length > 2).ToArray();

            if (lines.Length < 3) return;

            // Seek for start of set
            int start = Array.FindIndex(lines, line => line.Contains(" @ "));

            if (start != -1) // Has Item -- skip to start.
                lines = lines.Skip(start).Take(lines.Length - start).ToArray();
            else // Has no Item -- try parsing the first line anyway.
            {
                ParseFirstLine(lines[0]);
                if (Species < -1)
                    return; // Abort if no text is found

                lines = lines.Skip(1).Take(lines.Length - 1).ToArray();
            }
            int movectr = 0;
            // Detect relevant data
            foreach (string line in lines)
            {
                if (line.StartsWith("-"))
                {
                    string moveString = ParseLineMove(line);
                    int move = Array.IndexOf(moves, moveString);
                    if (move < 0)
                        InvalidLines.Add($"Unknown Move: {moveString}");
                    else
                        Moves[movectr++] = move;

                    if (movectr == 4)
                        break; // End of moves
                    continue;
                }

                string[] brokenline = line.Split(new[] { ": " }, StringSplitOptions.None);
                if (brokenline.Length == 1)
                    brokenline = new[] {brokenline[0], ""};
                switch (brokenline[0])
                {
                    case "Trait":
                    case "Ability": { Ability = Array.IndexOf(abilities, brokenline[1].Trim()); break; }
                    case "Level": { if (int.TryParse(brokenline[1].Trim(), out int val)) Level = val; else InvalidLines.Add(line); break; }
                    case "Shiny": { Shiny = brokenline[1].Trim() == "Yes"; break; }
                    case "Happiness": { if (int.TryParse(brokenline[1].Trim(), out int val)) Friendship = val; else InvalidLines.Add(line); break; }
                    case "Nature": { Nature = Array.IndexOf(natures, brokenline[1].Trim()); break; }
                    case "EV":
                    case "EVs": { ParseLineEVs(brokenline[1].Trim()); break; }
                    case "IV":
                    case "IVs": { ParseLineIVs(brokenline[1].Trim()); break; }
                    case "Type": { brokenline = new[] {line}; goto default; } // Type: Null edge case
                    default:
                    {
                        // Either Nature or Gender ItemSpecies
                        if (brokenline[0].Contains(" @ "))
                        {
                            string[] pieces = line.Split(new[] {" @ "}, StringSplitOptions.None);
                            string itemstr = pieces.Last().Trim();
                            int item = Array.IndexOf(items, itemstr);
                            if (item < 0)
                                InvalidLines.Add($"Unknown Item: {itemstr}");
                            else
                                HeldItem = item;

                            ParseFirstLine(pieces[0]);
                        }
                        else if (brokenline[0].Contains("Nature"))
                        {
                            string naturestr = line.Split(' ')[0].Trim();
                            int nature = Array.IndexOf(natures, naturestr);
                            if (nature < 0)
                                InvalidLines.Add($"Unknown Nature: {naturestr}");
                            else
                                Nature = nature;
                        }
                        else // Fallback
                        {
                            string speciesstr = line.Split('(')[0].Trim();
                            int spec = Array.IndexOf(species, speciesstr);
                            if (spec < 1)
                                InvalidLines.Add(speciesstr);
                            else
                                Species = spec;
                        }
                        break;
                    }
                }
            }

            IVs = IVsSpeedFirst;
            EVs = EVsSpeedFirst;

            // Showdown Quirks
            Form = ConvertFormFromShowdown(Form, Species, Ability);
        }

        public string Text => GetText();
        private string GetText()
        {
            if (Species == 0 || Species > MAX_SPECIES)
                return "";

            var result = new List<string>();

            // First Line: Name, Nickname, Gender, Item
            string form = ConvertFormToShowdown(Form, Species);
            result.Add(GetStringFirstLine(form));

            // IVs
            if (GetStringStats(out IEnumerable<string> ivstr, IVsSpeedLast, 31))
                result.Add($"IVs: {string.Join(" / ", ivstr)}");

            // EVs
            if (GetStringStats(out IEnumerable<string> evstr, EVsSpeedLast, 0))
                result.Add($"EVs: {string.Join(" / ", evstr)}");

            // Secondary Stats
            if (Ability > -1 && Ability < abilities.Length)
                result.Add($"Ability: {abilities[Ability]}");
            result.Add($"Level: {Level}");
            if (Shiny)
                result.Add("Shiny: Yes");

            if (Nature > -1)
                result.Add($"{natures[Nature]} Nature");

            // Moves
            result.AddRange(GetStringMoves());

            return string.Join(Environment.NewLine, result);
        }
        private string GetStringFirstLine(string form)
        {
            string specForm = species[Species];
            if (!string.IsNullOrWhiteSpace(form))
                specForm += "-" + form.Replace("Mega ", "Mega-");

            string result = Nickname != null && species[Species] != Nickname ? $"{Nickname} ({specForm})" : $"{specForm}";
            if (!string.IsNullOrEmpty(Gender))
                result += $" ({Gender})";
            if (HeldItem > 0 && HeldItem < items.Length)
                result += " @ " + items[HeldItem];
            return result;
        }
        private static bool GetStringStats(out IEnumerable<string> result, int[] stats, int ignore)
        {
            var list = new List<string>();
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i] == ignore) continue; // ignore unused EVs
                list.Add($"{stats[i]} {StatNames[i]}");
            }
            result = list;
            return list.Count > 0;
        }
        private IEnumerable<string> GetStringMoves()
        {
            foreach (int move in Moves.Where(move => move != 0 && move < moves.Length))
            {
                var str = $"- {moves[move]}";
                if (move == 237) // Hidden Power
                {
                    int hp = 0;
                    for (int i = 0; i < 6; i++)
                        hp |= (IVs[i] & 1) << i;
                    hp *= 0xF;
                    hp /= 0x3F;
                    str += $" [{hptypes[hp]}]";
                }
                yield return str;
            }
        }

        public static string GetShowdownText(PKM pkm)
        {
            if (pkm.Species == 0) return "";

            string[] Forms = PKX.GetFormList(pkm.Species, types, forms, new[] {"", "F", ""}, pkm.Format);
            ShowdownSet Set = new ShowdownSet
            {
                Nickname = pkm.Nickname,
                Species = pkm.Species,
                HeldItem = pkm.HeldItem,
                Ability = pkm.Ability,
                EVs = pkm.EVs,
                IVs = pkm.IVs,
                Moves = pkm.Moves,
                Nature = pkm.Nature,
                Gender = new[] { "M", "F", "" }[pkm.Gender < 2 ? pkm.Gender : 2],
                Friendship = pkm.CurrentFriendship,
                Level = PKX.GetLevel(pkm.Species, pkm.EXP),
                Shiny = pkm.IsShiny,
                Form = pkm.AltForm > 0 && pkm.AltForm < Forms.Length ? Forms[pkm.AltForm] : "",
            };

            if (Set.Form == "F")
                Set.Gender = "";

            return Set.Text;
        }
        private void ParseFirstLine(string line)
        {
            // Gender Detection
            string last3 = line.Substring(line.Length - 3);
            if (last3 == "(M)" || last3 == "(F)")
            {
                Gender = last3.Substring(1, 1);
                line = line.Substring(0, line.Length - 3);
            }

            // Nickname Detection
            string spec = line;
            if (spec.Contains("(") && spec.Contains(")"))
                ParseSpeciesNickname(ref spec);

            spec = spec.Trim();
            if ((Species = Array.IndexOf(species, spec)) >= 0) // success, nothing else!
                return;

            string[] tmp = spec.Split(new[] { "-" }, StringSplitOptions.None);
            if (tmp.Length < 2)
                return;

            Species = Array.IndexOf(species, tmp[0].Trim());
            Form = tmp[1].Trim();
            if (tmp.Length > 2)
                Form += " " + tmp[2];
        }
        private void ParseSpeciesNickname(ref string line)
        {
            int index = line.LastIndexOf("(", StringComparison.Ordinal);
            string n1, n2;
            if (index > 1) // correct format
            {
                n1 = line.Substring(0, index - 1);
                n2 = line.Substring(index).Trim();
                ReplaceAll(ref n2, "", "[", "]", "(", ")"); // Trim out excess data
            }
            else // nickname first (manually created set, incorrect)
            {
                int end = line.IndexOf(")", StringComparison.Ordinal);
                n2 = line.Substring(index + 1, end - 1);
                n1 = line.Substring(end + 2);
            }

            bool inverted = Array.IndexOf(species, n2.Replace(" ", "")) > -1 || (Species = Array.IndexOf(species, n2.Split('-')[0])) > 0;
            line = inverted ? n2 : n1;
            Nickname = inverted ? n1 : n2;
        }
        private string ParseLineMove(string line)
        {
            string moveString = line.Substring(line[1] == ' ' ? 2 : 1);
            if (!moveString.Contains("Hidden Power"))
                return moveString;

            // Defined Hidden Power
            if (moveString.Length > 13)
            {
                string type = moveString.Remove(0, 13);
                ReplaceAll(ref type, "", "[", "]", "(", ")"); // Trim out excess data
                int hpVal = Array.IndexOf(hptypes, type); // Get HP Type
                if (hpVal >= 0)
                    IVs = PKX.SetHPIVs(hpVal, IVs); // Get IVs
                else
                    InvalidLines.Add($"Invalid Hidden Power Type: {type}");
            }
            moveString = "Hidden Power";
            return moveString;
        }
        private void ParseLineEVs(string line)
        {
            string[] evlist = SplitLineStats(line);
            if (evlist.Length == 1)
                InvalidLines.Add("Unknown EV input.");
            for (int i = 0; i < evlist.Length / 2; i++)
            {
                bool valid = ushort.TryParse(evlist[i * 2 + 0], out ushort EV);
                int index = Array.IndexOf(StatNames, evlist[i * 2 + 1]);
                if (valid && index > -1)
                    EVs[index] = EV;
                else
                    InvalidLines.Add($"Unknown EV Type input: {evlist[i * 2]}");
            }
        }
        private void ParseLineIVs(string line)
        {
            string[] ivlist = SplitLineStats(line);
            if (ivlist.Length == 1)
                InvalidLines.Add("Unknown IV input.");
            for (int i = 0; i < ivlist.Length / 2; i++)
            {
                bool valid = byte.TryParse(ivlist[i * 2 + 0], out byte IV);
                int index = Array.IndexOf(StatNames, ivlist[i * 2 + 1]);
                if (valid && index > -1)
                    IVs[index] = IV;
                else
                    InvalidLines.Add($"Unknown IV Type input: {ivlist[i * 2]}");
            }
        }
        private static string ConvertFormToShowdown(string form, int spec)
        {
            if (string.IsNullOrWhiteSpace(form))
            {
                if (spec == 774) // Minior
                    form = "Meteor";
                return form;
            }

            switch (spec)
            {
                case 550 when form == "Blue":
                    return "Blue Striped";
                case 666 when form == "Poké Ball":
                    return "Pokeball"; // Vivillon
                case 676:
                    return ""; // Furfrou
                case 658: // Greninja
                    return "";
                case 718: // Zygarde
                    form = form.Replace("-C", "");
                    form = form.Replace("50%", "");
                    return form.Replace("100%", "Complete");
                case 774: // Minior
                    if (form.StartsWith("M-"))
                        return "Meteor";
                    return form.Replace("C-", "");

                default:
                    return form;
            }
        }
        private static string ConvertFormFromShowdown(string form, int spec, int ability)
        {
            switch (spec)
            {
                case 550 when form == "Blue Striped": // Basculin
                    return "Blue";
                case 658 when ability == 210: // Greninja
                    return "Ash"; // Battle Bond
                case 666 when form == "Pokeball": // Vivillon
                    return "Poké Ball";

                // Zygarde
                case 718 when string.IsNullOrWhiteSpace(form):
                    return "50%";
                case 718 when form == "Complete":
                    return "100%";
                case 718 when ability == 211:
                    return "-C"; // Power Construct

                // Minior
                case 774 when !string.IsNullOrWhiteSpace(form) && form != "Meteor":
                    return "C-" + form;

                default:
                    return form;
            }
        }

        private static string[] SplitLineStats(string line)
        {
            // Because people think they can type sets out...
            return line
                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
        }
        private static void ReplaceAll(ref string rv, string o, params string[] i)
        {
            rv = i.Aggregate(rv, (current, v) => current.Replace(v, o));
        }
    }
}
