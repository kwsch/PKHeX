using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public class ShowdownSet
    {
        // String to Values
        private static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        private static readonly string[] types = Util.getTypesList("en");
        private static readonly string[] forms = Util.getFormsList("en");
        private static readonly string[] species = Util.getSpeciesList("en");
        private static readonly string[] items = Util.getItemsList("en");
        private static readonly string[] natures = Util.getNaturesList("en");
        private static readonly string[] moves = Util.getMovesList("en");
        private static readonly string[] abilities = Util.getAbilitiesList("en");
        private static readonly string[] hptypes = types.Skip(1).ToArray();
        private const int MAX_SPECIES = 802;

        // Default Set Data
        public string Nickname { get; set; }
        public int Species { get; private set; } = -1;
        public string Form { get; private set; }
        public string Gender { get; private set; }
        public int Item { get; private set; }
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
                parseFirstLine(lines[0]);
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
                    string moveString = parseLineMove(line);
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
                switch (brokenline[0])
                {
                    case "Trait":
                    case "Ability": { Ability = Array.IndexOf(abilities, brokenline[1].Trim()); break; }
                    case "Level": { Level = Util.ToInt32(brokenline[1].Trim()); break; }
                    case "Shiny": { Shiny = brokenline[1].Trim() == "Yes"; break; }
                    case "Happiness": { Friendship = Util.ToInt32(brokenline[1].Trim()); break; }
                    case "Nature": { Nature = Array.IndexOf(natures, brokenline[1].Trim()); break; }
                    case "EV":
                    case "EVs": { parseLineEVs(brokenline[1].Trim()); break; }
                    case "IV":
                    case "IVs": { parseLineIVs(brokenline[1].Trim()); break; }
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
                                Item = item;

                            parseFirstLine(pieces[0]);
                        }
                        else if (brokenline[0].Contains("Nature"))
                        {
                            string naturestr = line.Split(' ')[0].Trim();
                            int nature = Array.IndexOf(natures, naturestr);
                            if (Nature < 0)
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
        }
        public string getText()
        {
            if (Species == 0 || Species > MAX_SPECIES)
                return "";

            // First Line: Name, Nickname, Gender, Item
            string specForm = species[Species];
            if (!string.IsNullOrWhiteSpace(Form))
                specForm += "-" + Form.Replace("Mega ", "Mega-");

            string result = Nickname != null && species[Species] != Nickname ? $"{Nickname} ({specForm})" : $"{specForm}"; 
            if (!string.IsNullOrEmpty(Gender))
                result += $" ({Gender})";
            if (Item > 0 && Item < items.Length)
                result += " @ " + items[Item];
            result += Environment.NewLine;

            // IVs
            string[] ivstr = new string[6];
            int ivctr = 0;
            int[] sIVs = IVsSpeedLast; // Reorganize speed
            for (int i = 0; i < 6; i++)
            {
                if (sIVs[i] == 31) continue;
                ivstr[ivctr++] += $"{sIVs[i]} {StatNames[i]}";
            }
            if (ivctr > 0)
                result += "IVs: " + string.Join(" / ", ivstr.Take(ivctr)) + Environment.NewLine;

            // EVs
            string[] evstr = new string[6];
            int[] sEVs = EVsSpeedLast; // Reorganize speed
            int evctr = 0;
            for (int i = 0; i < 6; i++)
            {
                if (sEVs[i] == 0) continue;
                evstr[evctr++] += $"{sEVs[i]} {StatNames[i]}";
            }
            if (evctr > 0)
                result += "EVs: " + string.Join(" / ", evstr.Take(evctr)) + Environment.NewLine;

            // Secondary Stats
            if (Ability > -1 && Ability < abilities.Length)
                result += "Ability: " + abilities[Ability] + Environment.NewLine;
            result += "Level: " + Level + Environment.NewLine;
            if (Shiny)
                result += "Shiny: Yes" + Environment.NewLine;

            if (Nature > -1)
                result += natures[Nature] + " Nature" + Environment.NewLine;
            // Add in Moves
            string[] MoveLines = new string[Moves.Length];
            int movectr = 0;
            foreach (int move in Moves.Where(move => move != 0 && move < moves.Length))
            {
                MoveLines[movectr] += "- " + moves[move];
                if (move == 237) // Hidden Power
                {
                    int hp = 0;
                    for (int i = 0; i < 6; i++)
                        hp |= (IVs[i] & 1) << i;
                    hp *= 0xF; hp /= 0x3F;
                    MoveLines[movectr] += $" [{hptypes[hp]}]";
                }
                movectr++;
            }
            result += string.Join(Environment.NewLine, MoveLines.Take(movectr));

            return result;
        }
        internal static string getShowdownText(PKM pkm)
        {
            if (pkm.Species == 0) return "";

            string[] Forms = PKX.getFormList(pkm.Species, types, forms, new[] {"", "F", ""});
            ShowdownSet Set = new ShowdownSet
            {
                Nickname = pkm.Nickname,
                Species = pkm.Species,
                Item = pkm.HeldItem,
                Ability = pkm.Ability,
                EVs = pkm.EVs,
                IVs = pkm.IVs,
                Moves = pkm.Moves,
                Nature = pkm.Nature,
                Gender = new[] { "M", "F", "" }[pkm.Gender < 2 ? pkm.Gender : 2],
                Friendship = pkm.CurrentFriendship,
                Level = PKX.getLevel(pkm.Species, pkm.EXP),
                Shiny = pkm.IsShiny,
                Form = pkm.AltForm > 0 && pkm.AltForm < Forms.Length ? Forms[pkm.AltForm] : "",
            };
            if (Set.Form == "F") Set.Gender = "";
            return Set.getText();
        }

        private void parseFirstLine(string line)
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
                parseSpeciesNickname(ref spec);

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
        private void parseSpeciesNickname(ref string line)
        {
            int index = line.LastIndexOf("(", StringComparison.Ordinal);
            string n1, n2;
            if (index != 0) // correct format
            {
                n1 = line.Substring(0, index - 1);
                n2 = line.Substring(index).Trim();
                replaceAll(ref n2, "", "[", "]", "(", ")"); // Trim out excess data
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
        private string parseLineMove(string line)
        {
            string moveString = line.Substring(line[1] == ' ' ? 2 : 1);
            if (!moveString.Contains("Hidden Power"))
                return moveString;

            // Defined Hidden Power
            if (moveString.Length > 13)
            {
                string type = moveString.Remove(0, 13);
                replaceAll(ref type, "", "[", "]", "(", ")"); // Trim out excess data
                int hpVal = Array.IndexOf(hptypes, type); // Get HP Type
                if (hpVal >= 0)
                    IVs = PKX.setHPIVs(hpVal, IVs); // Get IVs
                else
                    InvalidLines.Add($"Invalid Hidden Power Type: {type}");
            }
            moveString = "Hidden Power";
            return moveString;
        }
        private void parseLineEVs(string line)
        {
            string[] evlist = splitLineStats(line);
            for (int i = 0; i < evlist.Length / 2; i++)
            {
                ushort EV;
                ushort.TryParse(evlist[i * 2 + 0], out EV);
                int index = Array.IndexOf(StatNames, evlist[i * 2 + 1]);
                if (index > -1)
                    EVs[index] = EV;
                else
                    InvalidLines.Add($"Unknown EV Type input: {evlist[i * 2]}");
            }
        }
        private void parseLineIVs(string line)
        {
            string[] ivlist = splitLineStats(line);
            for (int i = 0; i < ivlist.Length / 2; i++)
            {
                byte IV;
                byte.TryParse(ivlist[i * 2 + 0], out IV);
                int index = Array.IndexOf(StatNames, ivlist[i * 2 + 1]);
                if (index > -1)
                    IVs[index] = IV;
                else
                    InvalidLines.Add($"Unknown IV Type input: {ivlist[i * 2]}");
            }
        }

        private static string[] splitLineStats(string line)
        {
            // Because people think they can type sets out...
            return line
                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
        }
        private static void replaceAll(ref string rv, string o, params string[] i)
        {
            rv = i.Aggregate(rv, (current, v) => current.Replace(v, o));
        }
    }
}
