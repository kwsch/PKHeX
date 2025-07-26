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
    private const char ItemSplit = '@';
    private const int MAX_SPECIES = (int)MAX_COUNT - 1;
    private const int MaxMoveCount = 4;
    private const string DefaultLanguage = BattleTemplateLocalization.DefaultLanguage; // English
    private static BattleTemplateLocalization DefaultStrings => BattleTemplateLocalization.Default;

    private static ReadOnlySpan<ushort> DashedSpecies =>
    [
        (int)NidoranF, (int)NidoranM,
        (int)HoOh,
        (int)Jangmoo, (int)Hakamoo, (int)Kommoo,
        (int)TingLu, (int)ChienPao, (int)WoChien, (int)ChiYu,
    ];

    public ushort Species { get; private set; }
    public EntityContext Context { get; private set; } = RecentTrainerCache.Context;
    public string Nickname { get; private set; } = string.Empty;
    public byte? Gender { get; private set; }
    public int HeldItem { get; private set; }
    public int Ability { get; private set; } = -1;
    public byte Level { get; private set; } = 100;
    public bool Shiny { get; private set; }
    public byte Friendship { get; private set; } = 255;
    public Nature Nature { get; private set; } = Nature.Random;
    public string FormName { get; private set; } = string.Empty;
    public byte Form { get; private set; }
    public int[] EVs { get; } = [00, 00, 00, 00, 00, 00];
    public int[] IVs { get; } = [31, 31, 31, 31, 31, 31];
    public sbyte HiddenPowerType { get; private set; } = -1;
    public MoveType TeraType { get; private set; } = MoveType.Any;
    public ushort[] Moves { get; } = [0, 0, 0, 0];
    public bool CanGigantamax { get; private set; }
    public byte DynamaxLevel { get; private set; } = 10;

    /// <summary>
    /// Any lines that failed to be parsed.
    /// </summary>
    public readonly List<string> InvalidLines = new(0);

    /// <summary>
    /// Loads a new <see cref="ShowdownSet"/> from the input string.
    /// </summary>
    /// <param name="input">Single-line string which will be split before loading.</param>
    /// <param name="localization">Localization to parse the lines with.</param>
    public ShowdownSet(ReadOnlySpan<char> input, BattleTemplateLocalization? localization = null) => LoadLines(input.EnumerateLines(), localization ?? DefaultStrings);

    /// <summary>
    /// Loads a new <see cref="ShowdownSet"/> from the input string.
    /// </summary>
    /// <param name="lines">Enumerable list of lines.</param>
    /// <param name="localization">Localization to parse the lines with.</param>
    public ShowdownSet(IEnumerable<string> lines, BattleTemplateLocalization? localization = null) => LoadLines(lines, localization ?? DefaultStrings);

    private void LoadLines(SpanLineEnumerator lines, BattleTemplateLocalization localization)
    {
        ParseLines(lines, localization);
        SanitizeResult(localization);
    }

    private void LoadLines(IEnumerable<string> lines, BattleTemplateLocalization localization)
    {
        ParseLines(lines, localization);
        SanitizeResult(localization);
    }

    private void SanitizeResult(BattleTemplateLocalization localization)
    {
        ReviseContextIfPastGenForm(localization.Strings);
        FormName = ShowdownParsing.GetFormNameFromShowdownFormName(Species, FormName, Ability);
        Form = ShowdownParsing.GetFormFromString(FormName, localization.Strings, Species, Context);

        // Handle edge case with fixed-gender forms.
        if (Species is (int)Meowstic or (int)Indeedee or (int)Basculegion or (int)Oinkologne)
            ReviseGenderedForms();
    }

    private void ReviseContextIfPastGenForm(GameStrings strings)
    {
        if (FormName.Length == 0)
            return; // no form name

        if (FormInfo.HasTotemForm(Species) && ShowdownParsing.IsTotemForm(FormName))
            Context = EntityContext.Gen7;
        else if (Species is (int)Pikachu && ShowdownParsing.IsCosplayPikachu(FormName, strings.forms))
            Context = EntityContext.Gen6;
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

    // Skip lines that are too short or too long.
    // Longest line is ~74 (Gen2 EVs)
    // Length permitted: 3-80
    // The shortest Pokémon name in Japanese is "ニ" (Ni) which is the name for the Pokémon, Nidoran♂ (male Nidoran). It has only one letter.
    // We will handle this 1-2 letter edge case only if the line is the first line of the set, in the rare chance we are importing for a non-English language?
    private const int MinLength = 3;
    private const int MaxLength = 80;
    private static bool IsLengthOutOfRange(ReadOnlySpan<char> trim) => IsLengthOutOfRange(trim.Length);
    private static bool IsLengthOutOfRange(int length) => (uint)(length - MinLength) > MaxLength - MinLength;

    private void ParseLines(SpanLineEnumerator lines, BattleTemplateLocalization localization)
    {
        int countMoves = 0;
        bool first = true;
        foreach (var line in lines)
        {
            var trim = line.Trim();
            if (IsLengthOutOfRange(trim))
            {
                // Try for other languages just in case.
                if (first && trim.Length != 0)
                {
                    ParseFirstLine(trim, localization.Strings);
                    first = false;
                    continue;
                }
                InvalidLines.Add(line.ToString());
                continue;
            }

            if (first)
            {
                ParseFirstLine(trim, localization.Strings);
                first = false;
                continue;
            }

            ParseLine(trim, ref countMoves, localization);
        }
    }

    private void ParseLines(IEnumerable<string> lines, BattleTemplateLocalization localization)
    {
        int countMoves = 0;
        bool first = true;
        foreach (var line in lines)
        {
            ReadOnlySpan<char> trim = line.Trim();
            if (IsLengthOutOfRange(trim))
            {
                // Try for other languages just in case.
                if (first && trim.Length != 0)
                {
                    ParseFirstLine(trim, localization.Strings);
                    first = false;
                    continue;
                }
                InvalidLines.Add(line);
                continue;
            }

            if (first)
            {
                ParseFirstLine(trim, localization.Strings);
                first = false;
                continue;
            }

            ParseLine(trim, ref countMoves, localization);
        }
    }

    private void ParseLine(ReadOnlySpan<char> line, ref int movectr, BattleTemplateLocalization localization)
    {
        var moves = Moves.AsSpan();
        var firstChar = line[0];
        if (firstChar is '-' or '–')
        {
            if (movectr >= MaxMoveCount)
            {
                InvalidLines.Add($"Too many moves: {line}");
                return;
            }
            var moveString = ParseLineMove(line, localization.Strings);
            int move = StringUtil.FindIndexIgnoreCase(localization.Strings.movelist, moveString);
            if (move < 0)
                InvalidLines.Add($"Unknown Move: {moveString}");
            else if (moves.Contains((ushort)move))
                InvalidLines.Add($"Duplicate Move: {moveString}");
            else
                moves[movectr++] = (ushort)move;
            return;
        }

        var dirMove = BattleTemplateConfig.GetMoveDisplay(MoveDisplayStyle.Directional);
        var dirMoveIndex = dirMove.IndexOf(firstChar);
        if (dirMoveIndex != -1)
        {
            if (moves[dirMoveIndex] != 0)
            {
                InvalidLines.Add($"Move slot already specified: {line}");
                return;
            }
            var moveString = ParseLineMove(line, localization.Strings);
            int move = StringUtil.FindIndexIgnoreCase(localization.Strings.movelist, moveString);
            if (move < 0)
                InvalidLines.Add($"Unknown Move: {moveString}");
            else if (moves.Contains((ushort)move))
                InvalidLines.Add($"Duplicate Move: {moveString}");
            else
                moves[dirMoveIndex] = (ushort)move;
            movectr++;
            return;
        }

        if (firstChar is '[' or '@') // Ability
        {
            ParseLineAbilityBracket(line, localization.Strings);
            return;
        }

        var token = localization.Config.TryParse(line, out var value);
        if (token == BattleTemplateToken.None)
        {
            InvalidLines.Add($"Unknown Token: {line}");
            return;
        }
        var valid = ParseEntry(token, value, localization);
        if (!valid)
            InvalidLines.Add(line.ToString());
    }

    private void ParseLineAbilityBracket(ReadOnlySpan<char> line, GameStrings localizationStrings)
    {
        // Try to peel off Held Item if it is specified.
        var itemStart = line.IndexOf(ItemSplit);
        if (itemStart != -1)
        {
            var itemName = line[(itemStart + 1)..].TrimStart();
            if (!ParseItemName(itemName, localizationStrings))
                InvalidLines.Add($"Unknown Item: {itemName}");
            line = line[..itemStart];
        }

        // Remainder should be [Ability]
        var abilityEnd = line.IndexOf(']');
        if (abilityEnd == -1 || line.Length == 1) // '[' should be present if ']' is; length check.
        {
            InvalidLines.Add($"Invalid Ability declaration: {line}");
            return; // invalid line
        }

        var abilityName = line[1..abilityEnd].Trim();
        var abilityIndex = StringUtil.FindIndexIgnoreCase(localizationStrings.abilitylist, abilityName);
        if (abilityIndex < 0)
        {
            InvalidLines.Add($"Unknown Ability: {abilityName}");
            return; // invalid line
        }
        Ability = abilityIndex;
    }

    private bool ParseEntry(BattleTemplateToken token, ReadOnlySpan<char> value, BattleTemplateLocalization localization) => token switch
    {
        BattleTemplateToken.Ability       => ParseLineAbility(value, localization.Strings.abilitylist),
        BattleTemplateToken.Nature        => ParseLineNature(value, localization.Strings.natures),
        BattleTemplateToken.Shiny         => Shiny         = true,
        BattleTemplateToken.Gigantamax    => CanGigantamax = true,
        BattleTemplateToken.HeldItem      => ParseItemName(value, localization.Strings),
        BattleTemplateToken.Nickname      => ParseNickname(value),
        BattleTemplateToken.Gender        => ParseGender(value, localization.Config),
        BattleTemplateToken.Friendship    => ParseFriendship(value),
        BattleTemplateToken.EVs           => ParseLineEVs(value, localization),
        BattleTemplateToken.IVs           => ParseLineIVs(value, localization.Config),
        BattleTemplateToken.Level         => ParseLevel(value),
        BattleTemplateToken.DynamaxLevel  => ParseDynamax(value),
        BattleTemplateToken.TeraType      => ParseTeraType(value, localization.Strings.types),
        _ => false,
    };

    private bool ParseLineAbility(ReadOnlySpan<char> value, ReadOnlySpan<string> abilityNames)
    {
        var index = StringUtil.FindIndexIgnoreCase(abilityNames, value);
        if (index < 0)
        {
            InvalidLines.Add($"Unknown Ability: {value}");
            return false;
        }
        if (Ability != -1 && Ability != index)
        {
            InvalidLines.Add($"Different ability already specified: {value}");
            return false;
        }

        Ability = index;
        return true;
    }

    private bool ParseLineNature(ReadOnlySpan<char> value, ReadOnlySpan<string> natureNames)
    {
        var index = StringUtil.FindIndexIgnoreCase(natureNames, value);
        if (index < 0)
            return false;

        var nature = (Nature)index;
        if (!nature.IsFixed())
        {
            InvalidLines.Add($"Invalid Nature: {value}");
            return false;
        }
        if (Nature != Nature.Random && Nature != nature)
        {
            InvalidLines.Add($"Different nature already specified: {value}");
            return false;
        }

        Nature = nature;
        return true;
    }

    private bool ParseNickname(ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return false;
        // ignore length, but generally should be <= the Context's max length
        Nickname = value.ToString();
        return true;
    }

    private bool ParseGender(ReadOnlySpan<char> value, BattleTemplateConfig cfg)
    {
        if (value.Equals(cfg.Male, StringComparison.OrdinalIgnoreCase))
        {
            Gender = EntityGender.Male;
            return true;
        }
        if (value.Equals(cfg.Female, StringComparison.OrdinalIgnoreCase))
        {
            Gender = EntityGender.Female;
            return true;
        }
        return false;
    }

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

    private bool ParseTeraType(ReadOnlySpan<char> value, ReadOnlySpan<string> types)
    {
        Context = EntityContext.Gen9;
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
    public string Text => GetText(BattleTemplateExportSettings.Showdown);

    /// <inheritdoc cref="Text"/>
    /// <param name="language">Language code</param>
    public string GetText(string language = DefaultLanguage) => GetText(new BattleTemplateExportSettings(language));

    /// <inheritdoc cref="Text"/>
    /// <param name="language">Language ID</param>
    public string GetText(LanguageID language) => GetText(new BattleTemplateExportSettings(language));

    /// <inheritdoc cref="Text"/>
    /// <param name="settings">Export settings</param>
    public string GetText(in BattleTemplateExportSettings settings)
    {
        if (Species is 0 or > MAX_SPECIES)
            return string.Empty;

        var result = GetSetLines(settings);
        return string.Join(Environment.NewLine, result);
    }

    /// <inheritdoc cref="GetSetLines(in BattleTemplateExportSettings)"/>
    public List<string> GetSetLines(string language = DefaultLanguage) => GetSetLines(new BattleTemplateExportSettings(language));

    /// <summary>
    /// Gets all lines comprising the exported set details.
    /// </summary>
    /// <param name="settings">Export settings</param>
    /// <returns>List of lines comprising the set</returns>
    public List<string> GetSetLines(in BattleTemplateExportSettings settings)
    {
        var result = new List<string>(DefaultListAllocation);
        if (settings.Order.Length == 0)
            return result;
        GetSetLines(result, settings);
        return result;
    }

    /// <inheritdoc cref="GetSetLines(in BattleTemplateExportSettings)"/>
    public void GetSetLines(List<string> result, in BattleTemplateExportSettings settings)
    {
        var tokens = settings.Order;
        foreach (var token in tokens)
            PushToken(token, result, settings);
    }

    private string GetStringFirstLine(string form, in BattleTemplateExportSettings settings)
    {
        var strings = settings.Localization.Strings;
        var speciesList = strings.specieslist;
        if (Species >= speciesList.Length)
            return string.Empty; // invalid species

        string specForm = speciesList[Species];
        var gender = Gender;
        if (form.Length != 0)
        {
            specForm += $"-{form.Replace("Mega ", "Mega-")}";
        }
        else if (Species == (int)NidoranM)
        {
            specForm = specForm.Replace("♂", "-M");
            if (gender != EntityGender.Female)
                gender = null;
        }
        else if (Species == (int)NidoranF)
        {
            specForm = specForm.Replace("♀", "-F");
            if (gender != EntityGender.Male)
                gender = null;
        }

        var nickname = Nickname;
        if (settings.IsTokenInExport(BattleTemplateToken.Nickname))
            nickname = string.Empty; // omit nickname if not in export

        var result = GetSpeciesNickname(specForm, nickname, Species, Context);

        // Append Gender if not default/random.
        if (gender < EntityGender.Genderless && !settings.IsTokenInExport(BattleTemplateToken.Gender))
        {
            if (gender is 0)
                result += $" {FirstLineMale}";
            else if (gender is 1)
                result += $" {FirstLineFemale}";
        }

        // Append item if specified.
        if (HeldItem > 0 && !settings.IsTokenInExport(BattleTemplateToken.HeldItem) && !settings.IsTokenInExport(BattleTemplateToken.AbilityHeldItem))
        {
            var items = strings.GetItemStrings(Context);
            if ((uint)HeldItem < items.Length)
                result += $" {ItemSplit} {items[HeldItem]}";
        }
        return result;
    }

    private static string GetSpeciesNickname(string specForm, string nickname, ushort species, EntityContext context)
    {
        if (nickname.Length == 0 || nickname == specForm)
            return specForm;
        bool isNicknamed = SpeciesName.IsNicknamedAnyLanguage(species, nickname, context.Generation());
        if (!isNicknamed)
            return specForm;
        return $"{nickname} ({specForm})";
    }

    private void PushToken(BattleTemplateToken token, List<string> result, in BattleTemplateExportSettings settings)
    {
        var cfg = settings.Localization.Config;
        var strings = settings.Localization.Strings;

        switch (token)
        {
            // Core
            case BattleTemplateToken.FirstLine:
                result.Add(GetStringFirstLine(FormName, settings));
                break;
            case BattleTemplateToken.Ability when (uint)Ability < strings.Ability.Count:
                result.Add(cfg.Push(BattleTemplateToken.Ability, strings.Ability[Ability]));
                break;
            case BattleTemplateToken.Nature when (uint)Nature < strings.Natures.Count:
                result.Add(cfg.Push(token, strings.Natures[(byte)Nature]));
                break;

            case BattleTemplateToken.Moves:
                GetStringMoves(result, settings);
                break;

            // Stats
            case BattleTemplateToken.Level when Level != 100:
                result.Add(cfg.Push(token, Level));
                break;
            case BattleTemplateToken.Friendship when Friendship != 255:
                result.Add(cfg.Push(token, Friendship));
                break;
            case BattleTemplateToken.IVs:
                var maxIV = Context.Generation() < 3 ? 15 : 31;
                if (!IVs.AsSpan().ContainsAnyExcept(maxIV))
                    break; // skip if all IVs are maxed
                var nameIVs = cfg.GetStatDisplay(settings.StatsIVs);
                var ivs = GetStringStats(IVs, maxIV, nameIVs);
                if (ivs.Length != 0)
                    result.Add(cfg.Push(BattleTemplateToken.IVs, ivs));
                break;

            // EVs
            case BattleTemplateToken.EVsWithNature:
            case BattleTemplateToken.EVsAppendNature:
            case BattleTemplateToken.EVs when EVs.AsSpan().ContainsAnyExcept(0):
                AddEVs(result, settings, token);
                break;

            // Boolean
            case BattleTemplateToken.Shiny when Shiny:
                result.Add(cfg.Push(token));
                break;

            // Gen8
            case BattleTemplateToken.DynamaxLevel when Context == EntityContext.Gen8 && DynamaxLevel != 10:
                result.Add(cfg.Push(token, DynamaxLevel));
                break;
            case BattleTemplateToken.Gigantamax when Context == EntityContext.Gen8 && CanGigantamax:
                result.Add(cfg.Push(token));
                break;

            // Gen9
            case BattleTemplateToken.TeraType when Context == EntityContext.Gen9 && TeraType != MoveType.Any:
                if ((uint)TeraType <= TeraTypeUtil.MaxType) // Fairy
                    result.Add(cfg.Push(BattleTemplateToken.TeraType, strings.Types[(int)TeraType]));
                else if ((uint)TeraType == TeraTypeUtil.Stellar)
                    result.Add(cfg.Push(BattleTemplateToken.TeraType, strings.Types[TeraTypeUtil.StellarTypeDisplayStringIndex]));
                break;

            // Edge Cases
            case BattleTemplateToken.HeldItem when HeldItem > 0:
                var itemNames = strings.GetItemStrings(Context);
                if ((uint)HeldItem < itemNames.Length)
                    result.Add(cfg.Push(token, itemNames[HeldItem]));
                break;
            case BattleTemplateToken.Nickname when !string.IsNullOrWhiteSpace(Nickname):
                result.Add(cfg.Push(token, Nickname));
                break;
            case BattleTemplateToken.Gender when Gender != EntityGender.Genderless:
                result.Add(cfg.Push(token, Gender == 0 ? cfg.Male : cfg.Female));
                break;

            case BattleTemplateToken.AbilityHeldItem when Ability >= 0 || HeldItem > 0:
                result.Add(GetAbilityHeldItem(strings, Ability, HeldItem, Context));
                break;
        }
    }

    private void AddEVs(List<string> result, in BattleTemplateExportSettings settings, BattleTemplateToken token)
    {
        var cfg = settings.Localization.Config;
        var nameEVs = cfg.GetStatDisplay(settings.StatsEVs);
        var line = token switch
        {
            BattleTemplateToken.EVsWithNature => GetStringStatsNatureAmp(EVs, 0, nameEVs, Nature),
            BattleTemplateToken.EVsAppendNature => GetStringStatsNatureAmp(EVs, 0, nameEVs, Nature),
            _ => GetStringStats(EVs, 0, nameEVs),
        };
        if (token is BattleTemplateToken.EVsAppendNature && Nature.IsFixed())
            line += $" ({settings.Localization.Strings.natures[(int)Nature]})";
        result.Add(cfg.Push(BattleTemplateToken.EVs, line));
    }

    private static string GetAbilityHeldItem(GameStrings strings, int ability, int item, EntityContext context)
    {
        var abilityNames = strings.abilitylist;

        if ((uint)ability >= abilityNames.Length)
            ability = 0; // invalid ability
        var abilityName = abilityNames[ability];

        var itemNames = strings.GetItemStrings(context);
        if ((uint)item >= itemNames.Length)
            item = 0; // invalid item
        var itemName = itemNames[item];

        if (ability <= 0)
            return $"{ItemSplit} {itemName}";
        if (item <= 0)
            return $"[{abilityName}]";
        return $"[{abilityName}] {ItemSplit} {itemName}";
    }

    /// <inheritdoc cref="GetStringStats{T}(ReadOnlySpan{T}, T, StatDisplayConfig)"/>
    /// <remarks>Appends the nature amplification to the stat values, if not a neutral nature.</remarks>
    public static string GetStringStatsNatureAmp<T>(ReadOnlySpan<T> stats, T ignoreValue, StatDisplayConfig statNames, Nature nature) where T : IEquatable<T>
    {
        var (plus, minus) = NatureAmp.GetNatureModification(nature);
        if (plus == minus)
            return GetStringStats(stats, ignoreValue, statNames); // neutral nature won't appear any different

        // Shift as HP is not affected by nature.
        plus++;
        minus++;

        var count = stats.Length;
        if (!statNames.AlwaysShow)
        {
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].Equals(ignoreValue) && i != plus && i != minus)
                    count--; // ignore unused stats
            }
        }
        if (count == 0)
            return string.Empty;

        var result = new StringBuilder();
        int ctr = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            var statIndex = GetStatIndexStored(i);
            var statValue = stats[statIndex];

            var hideValue = statValue.Equals(ignoreValue) && !statNames.AlwaysShow;
            if (hideValue && statIndex != plus && statIndex != minus)
                continue; // ignore unused stats
            var amp = statIndex == plus ? "+" : statIndex == minus ? "-" : string.Empty;
            if (ctr++ != 0)
                result.Append(statNames.Separator);
            statNames.Format(result, i, statValue, amp, hideValue);
        }
        return result.ToString();
    }

    /// <summary>
    /// Gets the string representation of the stats.
    /// </summary>
    /// <param name="stats">Stats to display</param>
    /// <param name="ignoreValue">Value to ignore</param>
    /// <param name="statNames">Stat names to use</param>
    public static string GetStringStats<T>(ReadOnlySpan<T> stats, T ignoreValue, StatDisplayConfig statNames) where T : IEquatable<T>
    {
        var count = stats.Length;
        if (!statNames.AlwaysShow)
        {
            foreach (var stat in stats)
            {
                if (stat.Equals(ignoreValue))
                    count--; // ignore unused stats
            }
        }
        if (count == 0)
            return string.Empty;

        var result = new StringBuilder();
        int ctr = 0;
        for (int i = 0; i < stats.Length; i++)
        {
            var statIndex = GetStatIndexStored(i);
            var statValue = stats[statIndex];
            if (statValue.Equals(ignoreValue) && !statNames.AlwaysShow)
                continue;
            if (ctr++ != 0)
                result.Append(statNames.Separator);
            statNames.Format(result, i, statValue);
        }
        return result.ToString();
    }

    private void GetStringMoves(List<string> result, in BattleTemplateExportSettings settings)
    {
        var strings = settings.Localization.Strings;
        var moveNames = strings.movelist;
        var style = settings.Moves;
        var prefixes = BattleTemplateConfig.GetMoveDisplay(style);

        var added = 0;
        for (var i = 0; i < Moves.Length; i++)
        {
            var move = Moves[i];
            if (move == 0 && !(style is MoveDisplayStyle.Directional && added != 0))
                continue;
            if (move >= moveNames.Length)
                continue;
            var moveName = moveNames[move];

            string line;
            if (move != (int)Move.HiddenPower || HiddenPowerType == -1)
            {
                line = $"{prefixes[i]} {moveName}";
            }
            else
            {
                var type = 1 + HiddenPowerType; // skip Normal
                var typeName = strings.Types[type];
                line = $"{prefixes[i]} {moveName} [{typeName}]";
            }

            result.Add(line);
            added++;
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
    /// <param name="localization">Localization to parse the lines with.</param>
    /// <returns>New ShowdownSet object representing the input <see cref="pk"/></returns>
    public ShowdownSet(PKM pk, BattleTemplateLocalization? localization = null)
    {
        localization ??= DefaultStrings;
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
            HiddenPowerType = (sbyte)HiddenPower.GetType(IVs, Context);

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

        FormName = ShowdownParsing.GetStringFromForm(Form = pk.Form, localization.Strings, Species, Context);
    }

    private void ParseFirstLine(ReadOnlySpan<char> first, GameStrings strings)
    {
        int itemSplit = first.IndexOf(ItemSplit);
        if (itemSplit != -1)
        {
            var itemName = first[(itemSplit + 1)..].TrimStart();
            var speciesName = first[..itemSplit].TrimEnd();

            if (!ParseItemName(itemName, strings))
                InvalidLines.Add($"Unknown Item: {itemName}");
            ParseFirstLineNoItem(speciesName, strings);
        }
        else
        {
            ParseFirstLineNoItem(first, strings);
        }
    }

    private bool ParseItemName(ReadOnlySpan<char> itemName, GameStrings strings)
    {
        if (TryGetItem(itemName, strings, Context))
            return true;
        if (TryGetItem(itemName, strings, EntityContext.Gen3))
            return true;
        if (TryGetItem(itemName, strings, EntityContext.Gen2))
            return true;
        return false;
    }

    private bool TryGetItem(ReadOnlySpan<char> itemName, GameStrings strings, EntityContext context)
    {
        var items = strings.GetItemStrings(context);
        var item = StringUtil.FindIndexIgnoreCase(items, itemName);
        if (item < 0)
            return false;
        Context = context;
        HeldItem = item;
        return true;
    }

    private const string FirstLineMale = "(M)";
    private const string FirstLineFemale = "(F)";

    private void ParseFirstLineNoItem(ReadOnlySpan<char> line, GameStrings strings)
    {
        // Gender Detection
        if (line.EndsWith(FirstLineMale, StringComparison.Ordinal))
        {
            line = line[..^3].TrimEnd();
            Gender = 0;
        }
        else if (line.EndsWith(FirstLineFemale, StringComparison.Ordinal))
        {
            line = line[..^3].TrimEnd();
            Gender = 1;
        }

        // Nickname Detection
        if (line.IndexOf('(') != -1 && line.IndexOf(')') != -1)
            ParseSpeciesNickname(line, strings);
        else
            ParseSpeciesForm(line, strings);
    }

    private const string Gmax = "-Gmax";

    /// <summary>
    /// Average count of lines in a Showdown set.
    /// </summary>
    /// <remarks>Optimization to skip 1 size update allocation (from 4). Usually first-line, ability, (ivs, evs, shiny, level) 4*moves </remarks>
    public const int DefaultListAllocation = 8;

    private bool ParseSpeciesForm(ReadOnlySpan<char> speciesLine, GameStrings strings)
    {
        speciesLine = speciesLine.Trim();
        if (speciesLine.Length == 0)
            return false;

        if (speciesLine.EndsWith(Gmax, StringComparison.Ordinal))
        {
            CanGigantamax = true;
            speciesLine = speciesLine[..^Gmax.Length];
        }

        var speciesIndex = StringUtil.FindIndexIgnoreCase(strings.specieslist, speciesLine);
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

        speciesIndex = StringUtil.FindIndexIgnoreCase(strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..].ToString();
            return true;
        }

        // failure to parse, check edge cases
        foreach (var e in DashedSpecies)
        {
            var sn = strings.Species[e];
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

        speciesIndex = StringUtil.FindIndexIgnoreCase(strings.specieslist, speciesLine[..end]);
        if (speciesIndex > 0)
        {
            Species = (ushort)speciesIndex;
            FormName = speciesLine[(end + 1)..].ToString();
            return true;
        }
        return false;
    }

    private void ParseSpeciesNickname(ReadOnlySpan<char> line, GameStrings strings)
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

        if (ParseSpeciesForm(species, strings))
            Nickname = nickname.ToString();
        else if (ParseSpeciesForm(nickname, strings))
            Nickname = species.ToString();
    }

    private ReadOnlySpan<char> ParseLineMove(ReadOnlySpan<char> line, GameStrings strings)
    {
        line = line[1..].TrimStart();

        // Discard any multi-move options; keep only first.
        var option = line.IndexOf('/');
        if (option != -1)
            line = line[..option].TrimEnd();

        var moveString = line;

        var hiddenPowerName = strings.Move[(int)Move.HiddenPower];
        if (!moveString.StartsWith(hiddenPowerName, StringComparison.OrdinalIgnoreCase))
            return moveString; // regular move

        if (moveString.Length == hiddenPowerName.Length)
            return hiddenPowerName;

        // Defined Hidden Power
        var type = GetHiddenPowerType(moveString[(hiddenPowerName.Length + 1)..]);
        var types = strings.HiddenPowerTypes;
        int hpVal = StringUtil.FindIndexIgnoreCase(types, type); // Get HP Type
        if (hpVal == -1)
            return hiddenPowerName;

        HiddenPowerType = (sbyte)hpVal;
        var maxIV = Context.Generation() < 3 ? 15 : 31;
        if (IVs.AsSpan().ContainsAnyExcept(maxIV))
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

        // Allow for both (Type) and [Type]
        if (type[0] == '(' && type[^1] == ')')
            return type[1..^1].Trim();
        if (type[0] == '[' && type[^1] == ']')
            return type[1..^1].Trim();

        return type;
    }

    private bool ParseLineEVs(ReadOnlySpan<char> line, BattleTemplateLocalization localization)
    {
        // If nature is present, parse it first.
        var nature = line.IndexOf('(');
        if (nature != -1)
        {
            var natureName = line[(nature + 1)..];
            var end = natureName.IndexOf(')');
            if (end == -1)
            {
                InvalidLines.Add($"Invalid EV nature: {natureName}");
                return false; // invalid line
            }
            natureName = natureName[..end].Trim();
            var natureIndex = StringUtil.FindIndexIgnoreCase(localization.Strings.natures, natureName);
            if (natureIndex == -1)
            {
                InvalidLines.Add($"Invalid EV nature: {natureName}");
                return false; // invalid line
            }

            if (Nature != Nature.Random) // specified in a separate Nature line
                InvalidLines.Add($"EV nature ignored, specified previously: {natureName}");
            else
                Nature = (Nature)natureIndex;

            line = line[..nature].TrimEnd();
        }

        var result = localization.Config.TryParseStats(line, EVs);
        var success = result.IsParsedAllStats;
        if (result is { HasAmps: false })
            return success;

        // Use the amp nature ONLY if nature was not specified.
        // Only indicate invalid if it differs from the current nature.
        var currentNature = Nature;
        result.TreatAmpsAsSpeedNotLast();
        var ampNature = AdjustNature(result.Plus, result.Minus);
        success &= ampNature;
        if (ampNature && currentNature != Nature.Random && currentNature != Nature)
        {
            InvalidLines.Add($"EV +/- nature does not match specified nature: {currentNature}");
            Nature = currentNature; // revert to original
        }
        return success;
    }

    private bool ParseLineIVs(ReadOnlySpan<char> line, BattleTemplateConfig config)
    {
        // Parse stats, with unspecified name representation (try all).
        var result = config.TryParseStats(line, IVs);
        return result.IsParsedAllStats;
    }

    private bool AdjustNature(sbyte plus, sbyte minus)
    {
        if (plus == StatParseResult.NoStatAmp)
            InvalidLines.Add("Invalid Nature adjustment, missing plus stat.");
        if (minus == StatParseResult.NoStatAmp)
            InvalidLines.Add("Invalid Nature adjustment, missing minus stat.");
        else
            Nature = NatureAmp.CreateNatureFromAmps(plus, minus);
        return true;
    }
}
