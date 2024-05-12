using System;
using System.Collections.Generic;
using System.Text;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic for exporting and importing <see cref="PKM"/> data in Pokémon Showdown's text format.
/// </summary>
public sealed class ShowdownSet : IBattleTemplate
{
    private static readonly string[] StatNames = ["HP", "Atk", "Def", "Spe", "SpA", "SpD"];
    private const string LineSplit = ": ";
    private const string ItemSplit = " @ ";
    private const int MAX_SPECIES = (int)MAX_COUNT - 1;
    internal const string DefaultLanguage = GameLanguage.DefaultLanguage;
    private static readonly GameStrings DefaultStrings = GameInfo.GetStrings(DefaultLanguage);

    private static ReadOnlySpan<ushort> DashedSpecies =>
    [
        (int)NidoranF, (int)NidoranM,
        (int)HoOh,
        (int)Jangmoo, (int)Hakamoo, (int)Kommoo,
        (int)TingLu, (int)ChienPao, (int)WoChien, (int)ChiYu,
    ];

    /// <inheritdoc/>
    public ushort Species { get; private set; }

    /// <inheritdoc/>
    public EntityContext Context { get; private set; } = RecentTrainerCache.Context;

    /// <inheritdoc/>
    public string Nickname { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public byte? Gender { get; private set; }

    /// <inheritdoc/>
    public int HeldItem { get; private set; }

    /// <inheritdoc/>
    public int Ability { get; private set; } = -1;

    /// <inheritdoc/>
    public byte Level { get; private set; } = 100;

    /// <inheritdoc/>
    public bool Shiny { get; private set; }

    /// <inheritdoc/>
    public byte Friendship { get; private set; } = 255;

    /// <inheritdoc/>
    public Nature Nature { get; private set; } = Nature.Random;

    /// <inheritdoc/>
    public string FormName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public byte Form { get; private set; }

    /// <inheritdoc/>
    public int[] EVs { get; } = [00, 00, 00, 00, 00, 00];

    /// <inheritdoc/>
    public int[] IVs { get; } = [31, 31, 31, 31, 31, 31];

    /// <inheritdoc/>
    public int HiddenPowerType { get; private set; } = -1;

    public MoveType TeraType { get; private set; } = MoveType.Any;

    /// <inheritdoc/>
    public ushort[] Moves { get; } = [0, 0, 0, 0];

    /// <inheritdoc/>
    public bool CanGigantamax { get; private set; }

    /// <inheritdoc/>
    public byte DynamaxLevel { get; private set; } = 10;

    /// <summary>
    /// Any lines that failed to be parsed.
    /// </summary>
    public readonly List<string> InvalidLines = new(0);

    private GameStrings Strings { get; set; } = DefaultStrings;

    /// <summary>
    /// Loads a new <see cref="ShowdownSet"/> from the input string.
    /// </summary>
    /// <param name="input">Single-line string which will be split before loading.</param>
    public ShowdownSet(ReadOnlySpan<char> input) => LoadLines(input.EnumerateLines());

    /// <summary>
    /// Loads a new <see cref="ShowdownSet"/> from the input string.
    /// </summary>
    /// <param name="lines">Enumerable list of lines.</param>
    public ShowdownSet(IEnumerable<string> lines) => LoadLines(lines);

    private void LoadLines(SpanLineEnumerator lines)
    {
        ParseLines(lines);
        SanitizeResult();
    }

    private void LoadLines(IEnumerable<string> lines)
    {
        ParseLines(lines);
        SanitizeResult();
    }

    private void SanitizeResult()
    {
        FormName = ShowdownParsing.SetShowdownFormName(Species, FormName, Ability);
        Form = ShowdownParsing.GetFormFromString(FormName, Strings, Species, Context);

        // Handle edge case with fixed-gender forms.
        if (Species is (int)Meowstic or (int)Indeedee or (int)Basculegion or (int)Oinkologne)
            ReviseGenderedForms();
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

    // Skip lines that are too short or too long.
    // Longest line is ~74 (Gen2 EVs)
    // Length permitted: 3-80
    // The shortest Pokémon name in Japanese is "ニ" (Ni) which is the name for the Pokémon, Nidoran♂ (male Nidoran). It has only one letter.
    // We will handle this 1-2 letter edge case only if the line is the first line of the set, in the rare chance we are importing for a non-English language?
    private const int MinLength = 3;
    private const int MaxLength = 80;
    private static bool IsLengthOutOfRange(ReadOnlySpan<char> trim) => IsLengthOutOfRange(trim.Length);
    private static bool IsLengthOutOfRange(int length) => (uint)(length - MinLength) > MaxLength - MinLength;

    private void ParseLines(SpanLineEnumerator lines)
    {
        int movectr = 0;
        bool first = true;
        foreach (var line in lines)
        {
            ReadOnlySpan<char> trim = line.Trim();
            if (IsLengthOutOfRange(trim))
            {
                // Try for other languages just in case.
                if (first && trim.Length != 0)
                {
                    ParseFirstLine(trim);
                    first = false;
                    continue;
                }
                InvalidLines.Add(line.ToString());
                continue;
            }

            if (first)
            {
                ParseFirstLine(trim);
                first = false;
                continue;
            }
            if (ParseLine(trim, ref movectr))
                return; // End of moves, end of set data
        }
    }

    private void ParseLines(IEnumerable<string> lines)
    {
        int movectr = 0;
        bool first = true;
        foreach (var line in lines)
        {
            ReadOnlySpan<char> trim = line.Trim();
            if (IsLengthOutOfRange(trim))
            {
                // Try for other languages just in case.
                if (first && trim.Length != 0)
                {
                    ParseFirstLine(trim);
                    first = false;
                    continue;
                }
                InvalidLines.Add(line);
                continue;
            }

            if (first)
            {
                ParseFirstLine(trim);
                first = false;
                continue;
            }
            if (ParseLine(trim, ref movectr))
                return; // End of moves, end of set data
        }
    }

    private bool ParseLine(ReadOnlySpan<char> line, ref int movectr)
    {
        var moves = Moves.AsSpan();
        if (line[0] is '-' or '–')
        {
            var moveString = ParseLineMove(line);
            int move = StringUtil.FindIndexIgnoreCase(Strings.movelist, moveString);
            if (move < 0)
                InvalidLines.Add($"Unknown Move: {moveString}");
            else if (moves.Contains((ushort)move))
                InvalidLines.Add($"Duplicate Move: {moveString}");
            else
                moves[movectr++] = (ushort)move;

            return movectr == MaxMoveCount;
        }

        if (movectr != 0)
            return true;

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
            InvalidLines.Add(line.ToString());
        return false;
    }

    private bool ParseSingle(ReadOnlySpan<char> identifier)
    {
        if (!identifier.EndsWith("Nature", StringComparison.OrdinalIgnoreCase))
            return false;
        var firstSpace = identifier.IndexOf(' ');
        if (firstSpace == -1)
            return false;
        var nature = identifier[..firstSpace];
        return (Nature = (Nature)StringUtil.FindIndexIgnoreCase(Strings.natures, nature)).IsFixed();
    }

    private bool ParseEntry(ReadOnlySpan<char> identifier, ReadOnlySpan<char> value) => identifier switch
    {
        "Ability"       => (Ability = StringUtil.FindIndexIgnoreCase(Strings.abilitylist, value)) >= 0,
        "Nature"        => (Nature  = (Nature)StringUtil.FindIndexIgnoreCase(Strings.natures    , value)).IsFixed(),
        "Shiny"         => Shiny         = StringUtil.IsMatchIgnoreCase("Yes", value),
        "Gigantamax"    => CanGigantamax = StringUtil.IsMatchIgnoreCase("Yes", value),
        "Friendship"    => ParseFriendship(value),
        "EVs"           => ParseLineEVs(value),
        "IVs"           => ParseLineIVs(value),
        "Level"         => ParseLevel(value),
        "Dynamax Level" => ParseDynamax(value),
        "Tera Type"     => ParseTeraType(value),
        _ => false,
    };

    private bool ParseLevel(ReadOnlySpan<char> value)
    {
        if (!byte.TryParse(value.Trim(), out var val))
            return false;
        if ((uint)val is 0 or > 100)
            return false;
        Level = val;
        return true;
    }

    private bool ParseFriendship(ReadOnlySpan<char> value)
    {
        if (!byte.TryParse(value.Trim(), out var val))
            return false;
        Friendship = val;
        return true;
    }

    private bool ParseDynamax(ReadOnlySpan<char> value)
    {
        Context = EntityContext.Gen8;
        var val = Util.ToInt32(value);
        if ((uint)val > 10)
            return false;
        DynamaxLevel = (byte)val;
        return true;
    }

    private bool ParseTeraType(ReadOnlySpan<char> value)
    {
        Context = EntityContext.Gen9;
        var types = Strings.types;
        var val = StringUtil.FindIndexIgnoreCase(types, value);
        if (val < 0)
            return false;
        if (val == TeraTypeUtil.StellarTypeDisplayStringIndex)
            val = TeraTypeUtil.Stellar;
        TeraType = (MoveType)val;
        return true;
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
        if (Species is 0 or > MAX_SPECIES)
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
        var maxIV = Context.Generation() < 3 ? 15 : 31;
        var ivs = GetStringStats(IVs, maxIV);
        if (ivs.Length != 0)
            result.Add($"IVs: {string.Join(" / ", ivs)}");

        // EVs
        var evs = GetStringStats(EVs, 0);
        if (evs.Length != 0)
            result.Add($"EVs: {string.Join(" / ", evs)}");

        // Secondary Stats
        if ((uint)Ability < Strings.Ability.Count)
            result.Add($"Ability: {Strings.Ability[Ability]}");
        if (Context == EntityContext.Gen9 && TeraType != MoveType.Any)
        {
            if ((uint)TeraType <= TeraTypeUtil.MaxType) // Fairy
                result.Add($"Tera Type: {Strings.Types[(int)TeraType]}");
            else if ((uint)TeraType == TeraTypeUtil.Stellar)
                result.Add($"Tera Type: {Strings.Types[TeraTypeUtil.StellarTypeDisplayStringIndex]}");
        }

        if (Level != 100)
            result.Add($"Level: {Level}");
        if (Shiny)
            result.Add("Shiny: Yes");
        if (Context == EntityContext.Gen8 && DynamaxLevel != 10)
            result.Add($"Dynamax Level: {DynamaxLevel}");
        if (Context == EntityContext.Gen8 && CanGigantamax)
            result.Add("Gigantamax: Yes");

        if ((uint)Nature < Strings.Natures.Count)
            result.Add($"{Strings.Natures[(byte)Nature]} Nature");

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
        if (Nickname.Length == 0 || Nickname == specForm)
            return specForm;
        bool isNicknamed = SpeciesName.IsNicknamedAnyLanguage(Species, Nickname, Context.Generation());
        if (!isNicknamed)
            return specForm;
        return $"{Nickname} ({specForm})";
    }

    public static string[] GetStringStats<T>(ReadOnlySpan<T> stats, T ignoreValue) where T : IEquatable<T>
    {
        var count = stats.Length - stats.Count(ignoreValue);
        if (count == 0)
            return [];

        var result = new string[count];
        int ctr = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            var statIndex = GetStatIndexStored(i);
            var statValue = stats[statIndex];
            if (statValue.Equals(ignoreValue))
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

            if (move != (int)Move.HiddenPower || HiddenPowerType == -1)
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
    /// Forces some properties to indicate the set for future display values.
    /// </summary>
    /// <param name="pk">PKM to convert to string</param>
    public void InterpretAsPreview(PKM pk)
    {
        if (pk.Format <= 2) // Nature preview from IVs
            Nature = Experience.GetNatureVC(pk.EXP);
    }

    /// <summary>
    /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
    /// </summary>
    /// <param name="pk">PKM to convert to string</param>
    /// <returns>New ShowdownSet object representing the input <see cref="pk"/></returns>
    public ShowdownSet(PKM pk)
    {
        if (pk.Species == 0)
            return;

        Context = pk.Context;

        Nickname = pk.Nickname;
        Species = pk.Species;
        HeldItem = pk.HeldItem;
        Ability = pk.Ability;
        pk.GetEVs(EVs);
        pk.GetIVs(IVs);

        var moves = Moves.AsSpan();
        pk.GetMoves(moves);
        if (moves.Contains((ushort)Move.HiddenPower))
            HiddenPowerType = HiddenPower.GetType(IVs, Context);

        Nature = pk.StatNature;
        Gender = pk.Gender < 2 ? pk.Gender : (byte)2;
        Friendship = pk.CurrentFriendship;
        Level = pk.CurrentLevel;
        Shiny = pk.IsShiny;

        if (pk is PK8 g) // Only set Gigantamax if it is a PK8
        {
            CanGigantamax = g.CanGigantamax;
            DynamaxLevel = g.DynamaxLevel;
        }

        if (pk is ITeraType t)
            TeraType = t.TeraType;
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

    private void ParseFirstLine(ReadOnlySpan<char> first)
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

    private void ParseItemName(ReadOnlySpan<char> itemName)
    {
        if (TrySetItem(Context, itemName))
            return;
        if (TrySetItem(EntityContext.Gen3, itemName))
            return;
        if (TrySetItem(EntityContext.Gen2, itemName))
            return;
        InvalidLines.Add($"Unknown Item: {itemName}");

        bool TrySetItem(EntityContext context, ReadOnlySpan<char> span)
        {
            var items = Strings.GetItemStrings(context);
            int item = StringUtil.FindIndexIgnoreCase(items, span);
            if (item < 0)
                return false;
            HeldItem = item;
            Context = context;
            return true;
        }
    }

    private void ParseFirstLineNoItem(ReadOnlySpan<char> line)
    {
        // Gender Detection
        if (line.EndsWith("(M)", StringComparison.Ordinal))
        {
            line = line[..^3].TrimEnd();
            Gender = 0;
        }
        else if (line.EndsWith("(F)", StringComparison.Ordinal))
        {
            line = line[..^3].TrimEnd();
            Gender = 1;
        }

        // Nickname Detection
        if (line.IndexOf('(') != -1 && line.IndexOf(')') != -1)
            ParseSpeciesNickname(line);
        else
            ParseSpeciesForm(line);
    }

    private const string Gmax = "-Gmax";

    private bool ParseSpeciesForm(ReadOnlySpan<char> speciesLine)
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
        int end = speciesLine.IndexOf('-');
        if (end < 0)
            return false;

        speciesIndex = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..].ToString();
            return true;
        }

        // failure to parse, check edge cases
        foreach (var e in DashedSpecies)
        {
            var sn = Strings.Species[e];
            if (!speciesLine.StartsWith(sn.Replace("♂", "-M").Replace("♀", "-F"), StringComparison.Ordinal))
                continue;
            Species = e;
            FormName = speciesLine[sn.Length..].ToString();
            return true;
        }

        // Version Megas
        end = speciesLine[Math.Max(0, end - 1)..].LastIndexOf('-');
        if (end < 0)
            return false;

        speciesIndex = StringUtil.FindIndexIgnoreCase(Strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..].ToString();
            return true;
        }
        return false;
    }

    private void ParseSpeciesNickname(ReadOnlySpan<char> line)
    {
        // Entering into this method requires both ( and ) to be present within the input line.
        int index = line.LastIndexOf('(');
        ReadOnlySpan<char> species;
        ReadOnlySpan<char> nickname;
        if (index > 1) // parenthesis value after: Nickname (Species), correct.
        {
            nickname = line[..index].TrimEnd();
            species = line[(index + 1)..];
            if (species.Length != 0 && species[^1] == ')')
                species = species[..^1];
        }
        else // parenthesis value before: (Species) Nickname, incorrect
        {
            int start = index + 1;
            int end = line.LastIndexOf(')');
            var tmp = line[start..end];
            if (end < line.Length - 2)
            {
                nickname = line[(end + 2)..];
                species = tmp;
            }
            else // (Species), or garbage
            {
                species = tmp;
                nickname = [];
            }
        }

        if (ParseSpeciesForm(species))
            Nickname = nickname.ToString();
        else if (ParseSpeciesForm(nickname))
            Nickname = species.ToString();
    }

    private ReadOnlySpan<char> ParseLineMove(ReadOnlySpan<char> line)
    {
        var startSearch = line[1] == ' ' ? 2 : 1;
        var option = line.IndexOf('/');
        line = option != -1 ? line[startSearch..option] : line[startSearch..];

        var moveString = line.Trim();

        var hiddenPowerName = Strings.Move[(int)Move.HiddenPower];
        if (!moveString.StartsWith(hiddenPowerName, StringComparison.OrdinalIgnoreCase))
            return moveString; // regular move

        if (moveString.Length == hiddenPowerName.Length)
            return hiddenPowerName;

        // Defined Hidden Power
        var type = GetHiddenPowerType(moveString[(hiddenPowerName.Length + 1)..]);
        var types = Strings.types.AsSpan(1, HiddenPower.TypeCount);
        int hpVal = StringUtil.FindIndexIgnoreCase(types, type); // Get HP Type
        if (hpVal == -1)
            return hiddenPowerName;

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

    private static ReadOnlySpan<char> GetHiddenPowerType(ReadOnlySpan<char> line)
    {
        var type = line.Trim();
        if (type.Length == 0)
            return type;

        if (type[0] == '(' && type[^1] == ')')
            return type[1..^1].Trim();
        if (type[0] == '[' && type[^1] == ']')
            return type[1..^1].Trim();

        return type;
    }

    private bool ParseLineEVs(ReadOnlySpan<char> line)
    {
        int start = 0;
        while (true)
        {
            var chunk = line[start..];
            var separator = chunk.IndexOf('/');
            var len = separator == -1 ? chunk.Length : separator;
            var tuple = chunk[..len].Trim();
            if (!AbsorbValue(tuple))
                InvalidLines.Add($"Invalid EV tuple: {tuple}");
            if (separator == -1)
                break; // no more stats
            start += separator + 1;
        }
        return true;

        bool AbsorbValue(ReadOnlySpan<char> text)
        {
            var space = text.IndexOf(' ');
            if (space == -1)
                return false;
            var stat = text[(space + 1)..].Trim();
            var statIndex = StringUtil.FindIndexIgnoreCase(StatNames, stat);
            if (statIndex == -1)
                return false;
            var value = text[..space].Trim();
            if (!ushort.TryParse(value, out var statValue))
                return false;
            EVs[statIndex] = statValue;
            return true;
        }
    }

    private bool ParseLineIVs(ReadOnlySpan<char> line)
    {
        int start = 0;
        while (true)
        {
            var chunk = line[start..];
            var separator = chunk.IndexOf('/');
            var len = separator == -1 ? chunk.Length : separator;
            var tuple = chunk[..len].Trim();
            if (!AbsorbValue(tuple))
                InvalidLines.Add($"Invalid IV tuple: {tuple}");
            if (separator == -1)
                break; // no more stats
            start += separator + 1;
        }
        return true;

        bool AbsorbValue(ReadOnlySpan<char> text)
        {
            var space = text.IndexOf(' ');
            if (space == -1)
                return false;
            var stat = text[(space + 1)..].Trim();
            var statIndex = StringUtil.FindIndexIgnoreCase(StatNames, stat);
            if (statIndex == -1)
                return false;
            var value = text[..space].Trim();
            if (!byte.TryParse(value, out var statValue))
                return false;
            IVs[statIndex] = statValue;
            return true;
        }
    }
}
