using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core.AutoMod;

/// <summary>
/// Parser for Smogon webpage <see cref="ShowdownSet"/> data.
/// </summary>
public sealed class SmogonSetGenerator
{
    public readonly bool Valid;
    public readonly string URL;
    public readonly string Species;
    public readonly string Form;
    public readonly string ShowdownSpeciesName;
    public readonly string Page;
    public readonly bool LetsGo;
    public readonly bool BDSP;
    public readonly List<string> SetFormat = [];
    public readonly List<string> SetName = [];
    public readonly List<string> SetConfig = [];
    public readonly List<string> SetText = [];
    public readonly List<ShowdownSet> Sets = [];

    public static readonly string[] IllegalFormats =
    [
        "Almost Any Ability", // Generates illegal abilities
        "BH", // Balanced Hackmons
        "Mix and Mega", // Assumes Pokémon can mega evolve that cannot
        "STABmons", // Generates illegal movesets
        "National Dex", // Adds Megas to Generation VIII
        "National Dex AG", // Adds Megas to Generation VIII
    ];

    private static bool IsIllegalFormat(ReadOnlySpan<char> format)
    {
        foreach (var f in IllegalFormats)
        {
            if (format.Equals(f, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public string Summary => AlertText(ShowdownSpeciesName, SetText.Count, GetTitles());

    public SmogonSetGenerator(PKM pk)
    {
        var baseURL = GetBaseURL(pk.GetType().Name);
        if (string.IsNullOrWhiteSpace(baseURL))
        {
            URL = Species = Form = ShowdownSpeciesName = Page = string.Empty;
            return;
        }

        var set = new ShowdownSet(pk);
        Species = GameInfo.GetStrings("en").Species[pk.Species];
        Form = ConvertFormToURLForm(set.FormName, Species);
        var psform = ConvertFormToShowdown(set.FormName, set.Species);

        URL = GetURL(Species, Form, baseURL);
        Page = InternalNetUtil.GetPageText(URL);

        LetsGo = pk is PB7;
        BDSP = pk is PB8;
        Valid = true;
        ShowdownSpeciesName = GetShowdownName(Species, psform);

        LoadSetsFromPage();
    }

    private static string GetShowdownName(string species, ReadOnlySpan<char> form)
    {
        return form.Length == 0 || IsInvalidForm(form) ? species : $"{species}-{form}";
    }

    private void LoadSetsFromPage()
    {
        var split1 = Page.Split("\",\"abilities\":");
        var format = "";
        for (int i = 1; i < split1.Length; i++)
        {
            var w = split1[i - 1];
            var shiny = w.Contains("\"shiny\":true");
            if (w.Contains("\"format\":\""))
            {
                var len = w.IndexOf("\"format\":\"", StringComparison.Ordinal) + "\"format\":\"".Length;
                format = w[len..].Split('\"')[0];
            }

            if (IsIllegalFormat(format))
                continue;

            if (LetsGo != format.StartsWith("LGPE", StringComparison.OrdinalIgnoreCase))
                continue;

            if (BDSP != format.StartsWith("BDSP", StringComparison.OrdinalIgnoreCase))
                continue;

            var level = format.StartsWith("LC") ? 5 : 100;
            if (!w.Contains("\"name\":"))
                continue;

            var name = w[(w.LastIndexOf("\"name\":\"", StringComparison.Ordinal) + "\"name\":\"".Length)..].Split('\"')[0];
            var setSpecies = w[(w.LastIndexOf("\"pokemon\":\"", StringComparison.Ordinal) + "\"pokemon\":\"".Length)..].Split('\"')[0];
            SetFormat.Add(format);
            SetName.Add(name);

            if (!w.Contains("\"level\":0,") && w.Contains("\"level\":"))
            {
                _ = int.TryParse(w.Split("\"level\":")[1].Split(',')[0], out level);
            }

            var split2 = split1[i].Split("\"]}");
            var tmp = split2[0];
            SetConfig.Add(tmp);

            var morphed = ConvertSetToShowdown(tmp, setSpecies, shiny, level);
            SetText.Add(morphed);

            var converted = new ShowdownSet(morphed);
            Sets.Add(converted);
        }
    }

    private static string GetBaseURL(string type) => type switch
    {
        nameof(PK1)                               => "https://www.smogon.com/dex/rb/pokemon",
        nameof(PK2) or nameof(SK2)                => "https://www.smogon.com/dex/gs/pokemon",
        nameof(PK3) or nameof(CK3) or nameof(XK3) => "https://www.smogon.com/dex/rs/pokemon",
        nameof(PK4) or nameof(BK4) or nameof(RK4) => "https://www.smogon.com/dex/dp/pokemon",
        nameof(PK5)                               => "https://www.smogon.com/dex/bw/pokemon",
        nameof(PK6)                               => "https://www.smogon.com/dex/xy/pokemon",
        nameof(PK7) or nameof(PB7)                => "https://www.smogon.com/dex/sm/pokemon",
        nameof(PK8) or nameof(PB8)                => "https://www.smogon.com/dex/ss/pokemon",
        nameof(PK9)                               => "https://www.smogon.com/dex/sv/pokemon",
        _ => string.Empty,
    };

    private static string ConvertSetToShowdown(string set, string species, bool shiny, int level)
    {
        var result = GetSetLines(set, species, shiny, level);
        return string.Join(Environment.NewLine, result);
    }

    private static readonly string[] statNames = ["HP", "Atk", "Def", "SpA", "SpD", "Spe"];

    private static List<string> GetSetLines(string set, string species, bool shiny, int level)
    {
        TryGetToken(set, "\"items\":[\"", "\"", out var item);
        TryGetToken(set, "\"moveslots\":", ",\"evconfigs\":", out var movesets);
        TryGetToken(set, "\"evconfigs\":[{", "}],\"ivconfigs\":", out var evstr);
        TryGetToken(set, "\"ivconfigs\":[{", "}],\"natures\":", out var ivstr);
        TryGetToken(set, "\"natures\":[\"", "\"", out var nature);
        TryGetToken(set, "\"teratypes\":[\"", "\"", out var teratype);

        if (teratype.StartsWith(']'))
            teratype = null;

        var evs = ParseEVIVs(evstr, false);
        var ivs = ParseEVIVs(ivstr, true);
        var ability = set[1] == ']' ? string.Empty : set.Split('\"')[1];

        if (item == "No Item") // LGPE actually lists an item, RBY sets have an empty [].
            item = string.Empty;

        var result = new List<string>(9)
        {
            item.Length == 0 ? species : $"{species} @ {item}",
        };
        if (level != 100)
            result.Add($"Level: {level}");

        if (shiny)
            result.Add("Shiny: Yes");

        if (!string.IsNullOrWhiteSpace(ability))
            result.Add($"Ability: {ability}");

        if (!string.IsNullOrWhiteSpace(teratype))
            result.Add($"Tera Type: {teratype}");

        if (evstr.Length >= 3)
            result.Add($"EVs: {string.Join(" / ", statNames.Select((z, i) => $"{evs[i]} {z}"))}");

        if (ivstr.Length >= 3)
            result.Add($"IVs: {string.Join(" / ", statNames.Select((z, i) => $"{ivs[i]} {z}"))}");

        if (!string.IsNullOrWhiteSpace(nature))
            result.Add($"{nature} Nature");

        result.AddRange(GetMoves(movesets).Select(move => $"- {move}"));
        return result;
    }

    /// <summary>
    /// Tries to rip out a substring between the provided <see cref="prefix"/> and <see cref="suffix"/>.
    /// </summary>
    /// <param name="line">Line</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="suffix">Suffix</param>
    /// <param name="result">Substring within prefix-suffix.</param>
    /// <returns>True if found a substring, false if no prefix found.</returns>
    private static bool TryGetToken(string line, string prefix, string suffix, out string result)
    {
        var prefixStart = line.IndexOf(prefix, StringComparison.Ordinal);
        if (prefixStart < 0)
        {
            result = string.Empty;
            return false;
        }
        prefixStart += prefix.Length;

        var suffixStart = line.IndexOf(suffix, prefixStart, StringComparison.Ordinal);
        if (suffixStart < 0)
            suffixStart = line.Length;

        result = line[prefixStart..suffixStart];
        return true;
    }

    private static List<string> GetMoves(string movesets)
    {
        var moves = new List<string>();
        var slots = movesets.Split("],[");
        foreach (var slot in slots)
        {
            var choices = slot.Split("\"move\":\"")[1..];
            foreach (var choice in choices)
            {
                var move = GetMove(choice);
                if (moves.Contains(move))
                    continue;

                if (move.Equals("Hidden Power", StringComparison.OrdinalIgnoreCase))
                    move = $"{move} [{choice.Split("\"type\":\"")[1].Split('\"')[0]}]";

                moves.Add(move);
                break;
            }

            if (moves.Count == 4)
                break;
        }

        return moves;

        static string GetMove(string s) => s.Split('"')[0];
    }

    private static readonly string[] ivdefault = ["31", "31", "31", "31", "31", "31"];
    private static readonly string[] evdefault = ["0", "0", "0", "0", "0", "0"];

    private static string[] ParseEVIVs(string liststring, bool isParseIV)
    {
        var val = isParseIV ? ivdefault : evdefault;
        if (string.IsNullOrWhiteSpace(liststring))
            return val;

        string getStat(string v) => liststring.Split(v)[1].Split(',')[0];
        val[0] = getStat("\"hp\":");
        val[1] = getStat("\"atk\":");
        val[2] = getStat("\"def\":");
        val[3] = getStat("\"spa\":");
        val[4] = getStat("\"spd\":");
        val[5] = getStat("\"spe\":");

        return val;
    }

    // Smogon Quirks
    private static string ConvertSpeciesToURLSpecies(string spec) => spec switch
    {
        "Nidoran♂" => "nidoran-m",
        "Nidoran♀" => "nidoran-f",
        "Farfetch’d" => "farfetchd",
        "Flabébé" => "flabebe",
        "Sirfetch’d" => "sirfetchd",
        _ => spec,
    };

    // Smogon Quirks
    private static string ConvertFormToURLForm(string form, string spec) => spec switch
    {
        "Necrozma" when form == "Dusk" => "dusk_mane",
        "Necrozma" when form == "Dawn" => "dawn_wings",
        "Oricorio" when form == "Pa’u" => "pau",
        "Darmanitan" when form == "Galarian Standard" => "galar",
        "Meowstic" when form.Length == 0 => "m",
        "Gastrodon" => "",
        "Vivillon" => "",
        "Sawsbuck" => "",
        "Deerling" => "",
        "Furfrou" => "",
        _ => form,
    };

    private static string ConvertFormToShowdown(string form, ushort spec)
    {
        if (form.Length == 0)
        {
            return spec switch
            {
                (int)Core.Species.Minior => "Meteor",
                _ => form,
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
                    return "Meteor";

                return form.Replace("C-", string.Empty);
            case (int)Core.Species.Necrozma when form == "Dusk":
                return $"{form}-Mane";
            case (int)Core.Species.Necrozma when form == "Dawn":
                return $"{form}-Wings";

            case (int)Core.Species.Furfrou:
            case (int)Core.Species.Greninja:
            case (int)Core.Species.Rockruff:
                return string.Empty;

            case (int)Core.Species.Polteageist:
            case (int)Core.Species.Sinistea:
                return form == "Antique" ? form : string.Empty;

            default:
                if (FormInfo.HasTotemForm(spec) && form == "Large")
                    return IsTotemAlolan(spec) && spec != (int)Core.Species.Mimikyu ? "Alola-Totem" : "Totem";

                return form.Replace(' ', '-');
        }
    }

    // Raticate (Normal, Alolan, Totem)
    // Marowak (Normal, Alolan, Totem)
    // Mimikyu (Normal, Busted, Totem, Totem_Busted)
    private static bool IsTotemAlolan(ushort species) => species is 20 or 105 or 778; // 0, Alolan, Totem

    private static string GetURL(string speciesName, string form, string baseURL)
    {
        if (string.IsNullOrWhiteSpace(form) || (IsInvalidForm(form) && form != "Crowned")) // Crowned Formes have separate pages
        {
            var spec = ConvertSpeciesToURLSpecies(speciesName).ToLower();
            return $"{baseURL}/{spec}/";
        }

        var urlSpecies = ConvertSpeciesToURLSpecies(speciesName);
        {
            var spec = urlSpecies.ToLower();
            var f = form.ToLower();
            return $"{baseURL}/{spec}-{f}/";
        }
    }

    private Dictionary<string, List<string>> GetTitles()
    {
        var titles = new Dictionary<string, List<string>>();
        for (int i = 0; i < Sets.Count; i++)
        {
            var format = SetFormat[i];
            var name = SetName[i];
            if (titles.TryGetValue(format, out var list))
                list.Add(name);
            else
                titles.Add(format, [name]);
        }

        return titles;
    }

    private static string AlertText(ReadOnlySpan<char> showdownSpec, int count, Dictionary<string, List<string>> titles)
    {
        var sb = new StringBuilder();
        sb.Append(showdownSpec).Append(':');
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        foreach (var entry in titles)
        {
            sb.Append(entry.Key).Append(": ").AppendJoin(", ", entry.Value);
            sb.Append(Environment.NewLine);
        }
        sb.Append(Environment.NewLine);
        sb.Append(count).Append(" sets generated for ").Append(showdownSpec);
        return sb.ToString();
    }

    public static bool IsInvalidForm(ReadOnlySpan<char> form) => form switch
    {
        "Primal" => true,
        "Busted" => true,
        "Crowned" => true,
        "Noice" => true,
        "Gulping" => true,
        "Gorging" => true,
        "Zen" => true,
        "Galar-Zen" => true,
        "Hangry" => true,
        "Complete" => true,
        _ => !form.Contains("Mega", StringComparison.Ordinal),
    };
}
