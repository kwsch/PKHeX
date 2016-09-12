using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public class ShowdownSet
    {
        // String to Values
        internal static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        public static readonly string[] types = Util.getStringList("types", "en");
        public static readonly string[] forms = Util.getStringList("forms", "en");
        private static readonly string[] species = Util.getSpeciesList("en");
        private static readonly string[] items = Util.getStringList("items", "en");
        private static readonly string[] natures = Util.getStringList("natures", "en");
        private static readonly string[] moves = Util.getMovesList("en");
        private static readonly string[] abilities = Util.getAbilitiesList("en");
        private static readonly string[] hptypes = types.Skip(1).ToArray();

        // Default Set Data
        public string Nickname;
        public int Species;
        public string Form;
        public string Gender;
        public int Item;
        public int Ability;
        public int Level;
        public bool Shiny;
        public int Friendship;
        public int Nature;
        public int[] EVs;
        public int[] IVs;
        public int[] Moves;
        public readonly List<string> InvalidLines = new List<string>();

        // Parsing Utility
        public ShowdownSet(string input = null)
        {
            if (input == null)
                return;

            Nickname = null;
            Species = -1;
            Form = null;
            Gender = null;
            Item = 0;
            Ability = 0;
            Level = 100;
            Shiny = false;
            Friendship = 255;
            Nature = 0;
            EVs = new int[6];
            IVs = new[] { 31, 31, 31, 31, 31, 31 };
            Moves = new int[4];

            string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace("'", "’").Trim(); // Sanitize apostrophes

            if (lines.Length < 3) return;

            // Seek for start of set
            int start = -1;
            for (int i = 0; i < lines.Length; i++)
                if (lines[i].Contains(" @ ")) { start = i; break; }
            lines = lines.Skip(start).Take(lines.Length - start).ToArray();

            // Abort if no text is found
            if (start == -1)
            {
                // Try to parse the first line if it does not have any item
                string ld = lines[0];
                // Gender Detection
                string last3 = ld.Substring(ld.Length - 3);
                if (last3 == "(M)" || last3 == "(F)")
                {
                    Gender = last3.Substring(1, 1);
                    ld = ld.Substring(0, ld.Length - 3);
                }
                // Nickname Detection
                string spec = ld;
                if (spec.Contains("("))
                {
                    int index = spec.LastIndexOf("(", StringComparison.Ordinal);
                    string n1 = spec.Substring(0, index - 1);
                    string n2 = spec.Substring(index).Replace("(", "").Replace(")", "").Replace(" ", "");

                    bool inverted = Array.IndexOf(species, n2.Replace(" ", "")) > -1 || (Species = Array.IndexOf(species, n2.Split('-')[0])) > 0;
                    spec = inverted ? n2 : n1;
                    Nickname = inverted ? n1 : n2;
                }
                Species = Array.IndexOf(species, spec.Replace(" ", ""));
                if (
                    (Species = Array.IndexOf(species, spec)) < 0 // Not an Edge Case
                    &&
                    (Species = Array.IndexOf(species, spec.Replace(" ", ""))) < 0 // Has Form
                    )
                {
                    string[] tmp = spec.Split(new[] { "-" }, StringSplitOptions.None);
                    if (tmp.Length < 2) return;
                    Species = Array.IndexOf(species, tmp[0].Replace(" ", ""));
                    Form = tmp[1].Trim();
                    if (tmp.Length > 2)
                        Form += " " + tmp[2];
                }
                if (Species < -1)
                    return;
                lines = lines.Skip(1).Take(lines.Length - 1).ToArray();
            }
            int movectr = 0;
            // Detect relevant data
            foreach (string line in lines)
            {
                if (line.Length < 2) continue;
                if (line.Contains("- "))
                {
                    string moveString = line.Substring(2);
                    if (moveString.Contains("Hidden Power"))
                    {
                        if (moveString.Length > 13) // Defined Hidden Power
                        {
                            string type = moveString.Remove(0, 13).Replace("[", "").Replace("]", ""); // Trim out excess data
                            int hpVal = Array.IndexOf(hptypes, type); // Get HP Type
                            if (hpVal >= 0)
                                IVs = PKX.setHPIVs(hpVal, IVs); // Get IVs
                            else
                                InvalidLines.Add($"Invalid Hidden Power Type: {type}");
                        }
                        moveString = "Hidden Power";
                    }
                    Moves[movectr++] = Array.IndexOf(moves, moveString);
                    if (movectr == 4)
                        break; // End of moves
                    continue;
                }

                string[] brokenline = line.Split(new[] { ": " }, StringSplitOptions.None);
                switch (brokenline[0])
                {
                    case "Trait":
                    case "Ability": { Ability = Array.IndexOf(abilities, brokenline[1]); break; }
                    case "Level": { Level = Util.ToInt32(brokenline[1]); break; }
                    case "Shiny": { Shiny = brokenline[1] == "Yes"; break; }
                    case "Happiness": { Friendship = Util.ToInt32(brokenline[1]); break; }
                    case "Nature": { Nature = Array.IndexOf(natures, brokenline[1]); break; }
                    case "EVs":
                        {
                            // Get EV list String
                            string[] evlist = brokenline[1]
                                // Because people think they can type sets out...
                                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
                            for (int i = 0; i < evlist.Length/2; i++)
                            {
                                ushort EV;
                                ushort.TryParse(evlist[i * 2 + 0], out EV);
                                int index = Array.IndexOf(StatNames, evlist[i*2 + 1]);
                                if (index > -1)
                                    EVs[index] = EV;
                                else
                                    InvalidLines.Add($"Unknown EV Type input: {evlist[i*2]}");
                            }
                            break;
                        }
                    case "IVs":
                        {
                            // Get IV list String
                            string[] ivlist = brokenline[1]
                                // Because people think they can type sets out...
                                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(new[] { " / ", " " }, StringSplitOptions.None);
                            for (int i = 0; i < ivlist.Length/2; i++)
                            {
                                byte IV;
                                byte.TryParse(ivlist[i*2 + 0], out IV);
                                int index = Array.IndexOf(StatNames, ivlist[i*2 + 1]);
                                if (index > -1)
                                    IVs[index] = IV;
                                else
                                    InvalidLines.Add($"Unknown IV Type input: {ivlist[i * 2]}");
                            }
                            break;
                        }
                    default:
                        {
                            // Either Nature or Gender ItemSpecies
                            if (brokenline[0].Contains(" @ "))
                            {
                                string[] ld = line.Split(new[] { " @ " }, StringSplitOptions.None);
                                Item = Array.IndexOf(items, ld.Last());
                                // Gender Detection
                                string last3 = ld[0].Substring(ld[0].Length - 3);
                                if (last3 == "(M)" || last3 == "(F)")
                                {
                                    Gender = last3.Substring(1, 1);
                                    ld[0] = ld[0].Substring(0, ld[ld.Length - 2].Length - 3);
                                }
                                // Nickname Detection
                                string spec = ld[0];
                                if (spec.Contains("("))
                                {
                                    int index = spec.LastIndexOf("(", StringComparison.Ordinal);
                                    string n1 = spec.Substring(0, index - 1);
                                    string n2 = spec.Substring(index).Replace("(", "").Replace(")", "").Replace(" ", "");

                                    bool inverted = Array.IndexOf(species, n2.Replace(" ", "")) > -1 || (Species = Array.IndexOf(species, n2.Split('-')[0])) > 0;
                                    spec = inverted ? n2 : n1;
                                    Nickname = inverted ? n1 : n2;
                                }
                                if (
                                    (Species = Array.IndexOf(species, spec)) < 0 // Not an Edge Case
                                    &&
                                    (Species = Array.IndexOf(species, spec.Replace(" ", ""))) < 0 // Has Form
                                    )
                                {
                                    string[] tmp = spec.Split(new[] { "-" }, StringSplitOptions.None);
                                    if (tmp.Length < 2) return;
                                    Species = Array.IndexOf(species, tmp[0].Replace(" ", ""));
                                    Form = tmp[1].Trim();
                                    if (tmp.Length > 2)
                                        Form += " " + tmp[2];
                                }
                            }
                            else if (brokenline[0].Contains("Nature"))
                                Nature = Array.IndexOf(natures, line.Split(' ')[0]);
                            else // Fallback
                            {
                                int spec = Array.IndexOf(species, line.Split('(')[0]);
                                if (spec > 0)
                                    Species = spec;
                                else
                                    InvalidLines.Add(line);
                            }
                        }
                        break;
                }
            }
        }
        public string getText()
        {
            if (Species == 0 || Species > 722)
                return "";

            // First Line: Name, Nickname, Gender, Item
            string result = string.Format(Nickname != null && species[Species] != Nickname ? "{0} ({1})" : "{1}", Nickname,
                species[Species] + ((Form ?? "") != "" ? "-" + Form?.Replace("Mega ", "Mega-") : "")) // Species (& Form if necessary)
                            + Gender + (Item > 0 ? " @ " + items[Item] : "") + Environment.NewLine;

            // IVs
            string[] ivstr = new string[6];
            int ivctr = 0;
            int[] sIVs = { IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3] }; // Reorganize speed
            for (int i = 0; i < 6; i++)
            {
                if (sIVs[i] == 31) continue;
                ivstr[ivctr++] += $"{sIVs[i]} {StatNames[i]}";
            }
            if (ivctr > 0)
                result += "IVs: " + string.Join(" / ", ivstr.Take(ivctr)) + Environment.NewLine;

            // EVs
            string[] evstr = new string[6];
            int[] sEVs = { EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3] }; // Reorganize speed
            int evctr = 0;
            for (int i = 0; i < 6; i++)
            {
                if (sEVs[i] == 0) continue;
                evstr[evctr++] += $"{sEVs[i]} {StatNames[i]}";
            }
            if (evctr > 0)
                result += "EVs: " + string.Join(" / ", evstr.Take(evctr)) + Environment.NewLine;

            // Secondary Stats
            if (Ability > -1)
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
                Gender = new[] { " (M)", " (F)", "" }[pkm.Gender],
                Friendship = pkm.CurrentFriendship,
                Level = PKX.getLevel(pkm.Species, pkm.EXP),
                Shiny = pkm.IsShiny,
                Form = pkm.AltForm > 0 ? PKX.getFormList(pkm.Species, types, forms, new[] { "", "F", "" })[pkm.AltForm] : "",
            };
            if (Set.Form == "F") Set.Gender = "";
            return Set.getText();
        }
    }
}
