using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic for exporting and importing <see cref="PKM"/> data in Pokémon Showdown's text format.
/// </summary>
public sealed class ShowdownSet : IBattleTemplate
{
    private static readonly string[] StatNames = { "HP", "Atk", "Def", "Spe", "SpA", "SpD" };
    private static readonly string[] Splitters = {"\r\n", "\n"};
    private static readonly string[] StatSplitters = { " / ", " " };
    private const string LineSplit = ": ";
    private const string ItemSplit = " @ ";
    private static readonly char[] ParenJunk = { '(', ')', '[', ']' };
    private static readonly ushort[] DashedSpecies = {782, 783, 784, 250, 032, 029}; // Kommo-o, Ho-Oh, Nidoran-M, Nidoran-F
    private const int MAX_SPECIES = (int)MAX_COUNT - 1;
    internal const string DefaultLanguage = GameLanguage.DefaultLanguage;
    private static readonly GameStrings DefaultStrings = GameInfo.GetStrings(DefaultLanguage);

    /// <inheritdoc/>
    public ushort Species { get; private set; }

    /// <inheritdoc/>
    public EntityContext Context { get; private set; } = RecentTrainerCache.Context;

    /// <inheritdoc/>
    public string Nickname { get; private set; } = string.Empty;

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
    public byte Form { get; private set; }

    /// <inheritdoc/>
    public int[] EVs { get; } = {00, 00, 00, 00, 00, 00};

    /// <inheritdoc/>
    public int[] IVs { get; } = {31, 31, 31, 31, 31, 31};

    /// <inheritdoc/>
    public int HiddenPowerType { get; set; } = -1;

    /// <inheritdoc/>
    public ushort[] Moves { get; } = {0, 0, 0, 0};

    /// <inheritdoc/>
    public bool CanGigantamax { get; set; }

    /// <inheritdoc/>
    public byte DynamaxLevel { get; set; } = 10;

    /// <summary>
    /// Any lines that failed to be parsed.
    /// </summary>
    public readonly List<string> InvalidLines = new();

    private GameStrings Strings { get; set; } = DefaultStrings;

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
        ParseLines(lines);

        FormName = ShowdownParsing.SetShowdownFormName(Species, FormName, Ability);
        Form = ShowdownParsing.GetFormFromString(FormName, Strings, Species, Context);

        // Handle edge case with fixed-gender forms.
        if (Species is (int)Meowstic or (int)Indeedee or (int)Basculegion)
            ReviseGenderedForms();
    }

    private static IEnumerable<string> GetSanitizedLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var trim = line.Trim();
            if (trim.Length <= 2)
                continue;

            // Sanitize apostrophes & dashes
            if (trim.IndexOf('\'') != -1)
                trim = trim.Replace('\'', '’');
            if (trim.IndexOf('–') != -1)
                trim = trim.Replace('–', '-');
            yield return trim;
        }
    }

    private void ReviseGenderedForms()
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

    private const int MaxMoveCount = 4;

    private void ParseLines(IEnumerable<string> lines)
    {
        lines = GetSanitizedLines(lines);
        using var e = lines.GetEnumerator();
        if (!e.MoveNext())
            return;

        ParseFirstLine(e.Current!);
        int movectr = 0;
        while (e.MoveNext())
        {
            var line = e.Current!;
            if (line.Length < 3)
                continue;

            if (line[0] == '-')
            {
                string moveString = ParseLineMove(line);
                int move = StringUtil.FindIndexIgnoreCase(Strings.movelist, moveString);
                if (move < 0)
                    InvalidLines.Add($"Unknown Move: {moveString}");
                else if (Array.IndexOf(Moves, (ushort)move) != -1)
                    InvalidLines.Add($"Duplicate Move: {moveString}");
                else
                    Moves[movectr++] = (ushort)move;

                if (movectr == MaxMoveCount)
                    return; // End of moves, end of set data
                continue;
            }

            if (movectr != 0)
                break;

            bool valid;
            var split = line.IndexOf(LineSplit, StringComparison.Ordinal);
            if (split == -1)
            {
                valid = ParseSingle(line); // Nature
            }
            else
            {
                var left = line[..split].Trim();
                var right = line[(split + LineSplit.Length)..].Trim();
                valid = ParseEntry(left, right);
            }
            if (!valid)
                InvalidLines.Add(line);
        }
    }

    private bool ParseSingle(string identifier)
    {
        if (!identifier.EndsWith("Nature", StringComparison.OrdinalIgnoreCase))
            return false;
        var firstSpace = identifier.IndexOf(' ');
        if (firstSpace == -1)
            return false;
        var naturestr = identifier[..firstSpace];
        return (Nature = StringUtil.FindIndexIgnoreCase(Strings.natures, naturestr)) >= 0;
    }

    private bool ParseEntry(string identifier, string value)
    {
        switch (identifier)
        {
            case "Ability" or "Trait": return (Ability = StringUtil.FindIndexIgnoreCase(Strings.abilitylist, value)) >= 0;
            case "Shiny": return Shiny = StringUtil.IsMatchIgnoreCase("Yes", value);
            case "Gigantamax": return CanGigantamax = StringUtil.IsMatchIgnoreCase("Yes", value);
            case "Nature": return (Nature = StringUtil.FindIndexIgnoreCase(Strings.natures, value)) >= 0;
            case "EV" or "EVs": ParseLineEVs(value); return true;
            case "IV" or "IVs": ParseLineIVs(value); return true;
            case "Dynamax Level": return ParseDynamax(value);
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

    private bool ParseDynamax(string value)
    {
        Context = EntityContext.Gen8;
        var val = Util.ToInt32(value);
        if ((uint)val > 10)
            return false;
        return (DynamaxLevel = (byte)val) is (>= 0 and <= 10);
    }

    /// <summary>
    /// Gets the standard Text representation of the set details.
    /// </summary>
    public string Text => GetText();

    /// <summary>
    /// Gets the localized Text representation of the set details.
    /// </summary>
    /// <param name="lang">2 character language code</param>
    public string LocalizedText(string lang = DefaultLanguage) => LocalizedText(GameLanguage.GetLanguageIndex(lang));

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
        if (Species is <= 0 or > MAX_SPECIES)
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
        var ivs = GetStringStats(IVs, Context.Generation() < 3 ? 15 : 31);
        if (ivs.Length != 0)
            result.Add($"IVs: {string.Join(" / ", ivs)}");

        // EVs
        var evs = GetStringStats(EVs, 0);
        if (evs.Length != 0)
            result.Add($"EVs: {string.Join(" / ", evs)}");

        // Secondary Stats
        if ((uint)Ability < Strings.Ability.Count)
            result.Add($"Ability: {Strings.Ability[Ability]}");
        if (Level != 100)
            result.Add($"Level: {Level}");
        if (Shiny)
            result.Add("Shiny: Yes");
        if (DynamaxLevel != 10 && Context == EntityContext.Gen8)
            result.Add($"Dynamax Level: {DynamaxLevel}");
        if (CanGigantamax)
            result.Add("Gigantamax: Yes");

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
            var items = Strings.GetItemStrings(Context);
            if ((uint)HeldItem < items.Length)
                result += $" @ {items[HeldItem]}";
        }
        return result;
    }

    private string GetSpeciesNickname(string specForm)
    {
        if (Nickname.Length == 0)
            return specForm;
        bool isNicknamed = SpeciesName.IsNicknamedAnyLanguage(Species, Nickname, Context.Generation());
        if (!isNicknamed)
            return specForm;
        return $"{Nickname} ({specForm})";
    }

    private static string[] GetStringStats(ReadOnlySpan<int> stats, int ignoreValue)
    {
        var count = stats.Length - stats.Count(ignoreValue);
        if (count == 0)
            return Array.Empty<string>();

        var result = new string[count];
        int ctr = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            var statIndex = GetStatIndexStored(i);
            var statValue = stats[statIndex];
            if (statValue == ignoreValue)
                continue; // ignore unused stats
            var statName = StatNames[statIndex];
            result[ctr++] = $"{statValue} {statName}";
        }
        return result;
    }

    private IEnumerable<string> GetStringMoves()
    {
        var moves = Strings.Move;
        foreach (var move in Moves)
        {
            if (move == 0 || move >= moves.Count)
                continue;

            if (move != (int)Move.HiddenPower)
            {
                yield return $"- {moves[move]}";
                continue;
            }

            var type = 1 + HiddenPowerType; // skip Normal
            var typeName = Strings.Types[type];
            yield return $"- {moves[move]} [{typeName}]";
        }
    }

    private static int GetStatIndexStored(int displayIndex) => displayIndex switch
    {
        3 => 4,
        4 => 5,
        5 => 3,
        _ => displayIndex,
    };

    /// <summary>
    /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
    /// </summary>
    /// <param name="pk">PKM to convert to string</param>
    /// <returns>New ShowdownSet object representing the input <see cref="pk"/></returns>
    public ShowdownSet(PKM pk)
    {
        if (pk.Species <= 0)
            return;

        Context = pk.Context;

        Nickname = pk.Nickname;
        Species = pk.Species;
        HeldItem = pk.HeldItem;
        Ability = pk.Ability;
        pk.GetEVs(EVs);
        pk.GetIVs(IVs);
        pk.GetMoves(Moves);
        Nature = pk.StatNature;
        Gender = (uint)pk.Gender < 2 ? pk.Gender : 2;
        Friendship = pk.CurrentFriendship;
        Level = pk.CurrentLevel;
        Shiny = pk.IsShiny;

        if (pk is IGigantamax g)
            CanGigantamax = g.CanGigantamax;
        if (pk is IDynamaxLevel d)
            DynamaxLevel = d.DynamaxLevel;

        if (Array.IndexOf(Moves, (ushort)Move.HiddenPower) != -1)
            HiddenPowerType = HiddenPower.GetType(IVs, Context);
        if (pk is IHyperTrain h)
        {
            for (int i = 0; i < 6; i++)
            {
                if (h.IsHyperTrained(i))
                    IVs[i] = pk.MaxIV;
            }
        }

        FormName = ShowdownParsing.GetStringFromForm(Form = pk.Form, Strings, Species, Context);
    }

    private void ParseFirstLine(string first)
    {
        int itemSplit = first.IndexOf(ItemSplit, StringComparison.Ordinal);
        if (itemSplit != -1)
        {
            var itemName = first[(itemSplit + ItemSplit.Length)..];
            var speciesName = first[..itemSplit];

            ParseItemName(itemName);
            ParseFirstLineNoItem(speciesName);
        }
        else
        {
            ParseFirstLineNoItem(first);
        }
    }

    private void ParseItemName(string itemName)
    {
        if (TrySetItem(Context))
            return;
        if (TrySetItem(EntityContext.Gen3))
            return;
        if (TrySetItem(EntityContext.Gen2))
            return;
        InvalidLines.Add($"Unknown Item: {itemName}");

        bool TrySetItem(EntityContext context)
        {
            var items = Strings.GetItemStrings(context);
            int item = StringUtil.FindIndexIgnoreCase(items, itemName);
            if (item < 0)
                return false;
            HeldItem = item;
            Context = context;
            return true;
        }
    }

    private void ParseFirstLineNoItem(string line)
    {
        // Gender Detection
        if (line.EndsWith("(M)", StringComparison.Ordinal))
        {
            line = line[..^3];
            Gender = 0;
        }
        else if (line.EndsWith("(F)", StringComparison.Ordinal))
        {
            line = line[..^3];
            Gender = 1;
        }

        // Nickname Detection
        if (line.IndexOf('(') != -1 && line.IndexOf(')') != -1)
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

        if (speciesLine.EndsWith(Gmax, StringComparison.Ordinal))
        {
            CanGigantamax = true;
            speciesLine = speciesLine[..^Gmax.Length];
        }

        var speciesIndex = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine);
        if (speciesIndex > 0)
        {
            // success, nothing else !
            Species = (ushort)speciesIndex;
            return true;
        }

        // Form string present.
        int end = speciesLine.LastIndexOf('-');
        if (end < 0)
            return false;

        speciesIndex = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..];
            return true;
        }

        // failure to parse, check edge cases
        foreach (var e in DashedSpecies)
        {
            var sn = Strings.Species[e];
            if (!speciesLine.StartsWith(sn.Replace("♂", "-M").Replace("♀", "-F"), StringComparison.Ordinal))
                continue;
            Species = e;
            FormName = speciesLine[sn.Length..];
            return true;
        }

        // Version Megas
        end = speciesLine.LastIndexOf('-', Math.Max(0, end - 1));
        if (end < 0)
            return false;

        speciesIndex = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..];
            return true;
        }
        return false;
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
        var startSearch = line[1] == ' ' ? 2 : 1;
        var option = line.IndexOf('/');
        line = option != -1 ? line[startSearch..option] : line[startSearch..];

        string moveString = line.Trim();

        var hiddenPowerName = Strings.Move[(int)Move.HiddenPower];
        if (!moveString.StartsWith(hiddenPowerName, StringComparison.OrdinalIgnoreCase))
            return moveString; // regular move

        if (moveString.Length == hiddenPowerName.Length)
            return hiddenPowerName;

        // Defined Hidden Power
        string type = moveString[13..];
        type = RemoveAll(type, ParenJunk); // Trim out excess data
        int hpVal = StringUtil.FindIndexIgnoreCase(Strings.types, type) - 1; // Get HP Type

        HiddenPowerType = hpVal;
        if (!Array.TrueForAll(IVs, z => z == 31))
        {
            if (!HiddenPower.SetIVsForType(hpVal, IVs, Context))
                InvalidLines.Add($"Invalid IVs for Hidden Power Type: {type}");
        }
        else if (hpVal >= 0)
        {
            HiddenPower.SetIVs(hpVal, IVs, Context); // Alter IVs
        }
        else
        {
            InvalidLines.Add($"Invalid Hidden Power Type: {type}");
        }
        return hiddenPowerName;
    }

    private void ParseLineEVs(string line)
    {
        var list = SplitLineStats(line);
        if ((list.Length & 1) == 1)
            InvalidLines.Add("Unknown EV input.");
        for (int i = 0; i < list.Length / 2; i++)
        {
            int pos = i * 2;
            var statName = list[pos + 1];
            int index = StringUtil.FindIndexIgnoreCase(StatNames, statName);
            if (index < 0 || !ushort.TryParse(list[pos + 0], out var value))
            {
                InvalidLines.Add($"Unknown EV stat: {list[pos]}");
                continue;
            }
            EVs[index] = value;
        }
    }

    private void ParseLineIVs(string line)
    {
        var list = SplitLineStats(line);
        if ((list.Length & 1) == 1)
            InvalidLines.Add("Unknown IV input.");
        for (int i = 0; i < list.Length / 2; i++)
        {
            int pos = i * 2;
            var statName = list[pos + 1];
            int index = StringUtil.FindIndexIgnoreCase(StatNames, statName);
            if (index < 0 || !byte.TryParse(list[pos + 0], out var value))
            {
                InvalidLines.Add($"Unknown IV stat: {list[pos]}");
                continue;
            }
            IVs[index] = value;
        }
    }

    private static string RemoveAll(string original, ReadOnlySpan<char> remove)
    {
        Span<char> result = stackalloc char[original.Length];
        int ctr = 0;
        foreach (var c in original)
        {
            if (remove.IndexOf(c) == -1)
                result[ctr++] = c;
        }
        if (ctr == original.Length)
            return original;
        return new string(result[..ctr].ToArray());
    }

    private static string[] SplitLineStats(string line)
    {
        // Because people think they can type sets out...
        return line
            .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
            .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
            .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(StatSplitters, StringSplitOptions.None);
    }
}
