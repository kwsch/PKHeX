using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic related to the name of a <see cref="Species"/>.
/// </summary>
public static class SpeciesName
{
    /// <summary>
    /// Species name lists indexed by the <see cref="LanguageID"/> value.
    /// </summary>
    private static readonly string[][] SpeciesLang =
    [
        [], // 0 (unused, invalid)
        Util.GetSpeciesList("ja"), // 1
        Util.GetSpeciesList("en"), // 2
        Util.GetSpeciesList("fr"), // 3
        Util.GetSpeciesList("it"), // 4
        Util.GetSpeciesList("de"), // 5
        [], // 6 (reserved for Gen3 KO?, unused)
        Util.GetSpeciesList("es"), // 7
        Util.GetSpeciesList("ko"), // 8
        Util.GetSpeciesList("zh-Hans"), // 9 Simplified
        Util.GetSpeciesList("zh-Hant"), // 10 Traditional
        Util.GetSpeciesList("es-419"), // 11 Spanish 
    ];

    /// <summary>
    /// Egg name list indexed by the <see cref="LanguageID"/> value.
    /// </summary>
    /// <remarks>Indexing matches <see cref="SpeciesLang"/>.</remarks>
    private static string GetEggName(int language) => language switch
    {
        (int)LanguageID.Japanese => "タマゴ",
        (int)LanguageID.English  => "Egg",
        (int)LanguageID.French   => "Œuf",
        (int)LanguageID.Italian  => "Uovo",
        (int)LanguageID.German   => "Ei",

        (int)LanguageID.Spanish  => "Huevo",
        (int)LanguageID.Korean   => "알",
        (int)LanguageID.ChineseS => "蛋",
        (int)LanguageID.ChineseT => "蛋",
        (int)LanguageID.SpanishL => "Huevo",
        _ => string.Empty,
    };

    /// <summary>
    /// <see cref="PKM.Nickname"/> to <see cref="Species"/> table for all <see cref="LanguageID"/> values.
    /// </summary>
    private static readonly Dictionary<string, ushort>.AlternateLookup<ReadOnlySpan<char>>[] SpeciesDict = GetDictionary(SpeciesLang);

    /// <inheritdoc cref="SpeciesDict"/>
    private static readonly Dictionary<string, ushort>.AlternateLookup<ReadOnlySpan<char>>[] SpeciesDictLower = GetDictionary(SpeciesLang, true);

    private static Dictionary<string, ushort>.AlternateLookup<ReadOnlySpan<char>>[] GetDictionary(string[][] names, bool lower = false)
    {
        var result = new Dictionary<string, ushort>.AlternateLookup<ReadOnlySpan<char>>[names.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var speciesList = names[i];
            var capacity = Math.Max(speciesList.Length - 1, 0);
            var dict = new Dictionary<string, ushort>(capacity);
            for (ushort species = 1; species < speciesList.Length; species++)
            {
                var key = speciesList[species];
                dict[lower ? key.ToLowerInvariant() : key] = species;
            }
            result[i] = dict.GetAlternateLookup<ReadOnlySpan<char>>();
        }
        return result;
    }

    /// <summary>
    /// Gets a Pokémon's default name for the desired language ID.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="language">Language ID of the Pokémon</param>
    /// <returns>The Species name if within expected range, else an empty string.</returns>
    /// <remarks>Should only be used externally for message displays; for accurate in-game names use <see cref="GetSpeciesNameGeneration"/>.</remarks>
    public static string GetSpeciesName(ushort species, int language)
    {
        if ((uint)language >= SpeciesLang.Length)
            return string.Empty;

        if (species == 0)
            return GetEggName(language);

        var arr = SpeciesLang[language];
        if (species >= arr.Length)
            return string.Empty;

        return arr[species];
    }

    public static bool IsApostropheFarfetchdLanguage(int language) => language is 2 or 4 or 7;

    /// <summary>
    /// Gets a Pokémon's default name for the desired language ID and generation.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="language">Language ID of the Pokémon</param>
    /// <param name="generation">Generation specific formatting option</param>
    /// <returns>Generation specific default species name</returns>
    public static string GetSpeciesNameGeneration(ushort species, int language, byte generation) => generation switch
    {
        <= 4 => GetSpeciesName1234(species, language, generation),
        5 when species is (int)Species.Farfetchd && IsApostropheFarfetchdLanguage(language) => "Farfetch'd", // Gen5 does not have slanted apostrophes.
        7 when language == (int) LanguageID.ChineseS => GetSpeciesName7ZH(species, language),
        _ => GetSpeciesName(species, language),
    };

    /// <inheritdoc cref="GetSpeciesNameGeneration"/>
    /// <summary>
    /// Gets the initial Species name for HOME imports.
    /// </summary>
    public static string GetSpeciesNameImportHOME(ushort species, int language, byte generation)
    {
        // Default fetched names have the wrong apostrophes.
        var result = GetSpeciesNameGeneration(species, language, generation);
        if (species is (int)Species.Farfetchd && IsApostropheFarfetchdLanguage(language))
            return "Farfetch'd";
        if (species is (int)Species.Sirfetchd && IsApostropheFarfetchdLanguage(language))
            return "Sirfetch'd";
        return result;
    }

    /// <summary>
    /// Gets a Pokémon's egg name for the desired language ID and generation.
    /// </summary>
    /// <param name="language">Language ID of the Pokémon</param>
    /// <param name="generation">Generation specific formatting option</param>
    public static string GetEggName(int language, byte generation) => generation switch
    {
        <= 4 => GetEggName1234(0, language, generation),
        _ => GetEggName(language),
    };

    private static string GetSpeciesName1234(ushort species, int language, byte generation)
    {
        if (species == 0)
            return GetEggName1234(species, language, generation);

        var nick = GetSpeciesName(species, language);
        switch (language)
        {
            case (int)LanguageID.Korean:
                if (generation == 2)
                    StringConverter2KOR.LocalizeKOR2(species, ref nick);
                return nick; // No further processing
            case (int)LanguageID.Japanese:
                return nick; // No further processing
        }

        Span<char> result = stackalloc char[nick.Length];

        // All names are uppercase.
        nick.ToUpperInvariant(result);
        if (language == (int)LanguageID.French)
            StringConverter4Util.StripDiacriticsFR4(result); // strips accents on E and I

        // Gen1/2 species names do not have spaces.
        if (generation >= 3)
        {
            // Gen3/4 use straight apostrophe instead of slanted apostrophe.
            // The only Gen3/4 species with an apostrophe is Farfetch'd.
            if (species is (int)Species.Farfetchd && IsApostropheFarfetchdLanguage(language))
                result[^2] = '\'';

            return new string(result);
        }

        // The only Gen1/2 species with a space is Mr. Mime; different period and no space.
        if (species == (int)Species.MrMime)
        {
            int indexSpace = result.IndexOf(StringConverter1.SPH);
            if (indexSpace > 0)
            {
                // Gen1/2 uses a different period for MR.MIME than user input.
                result[indexSpace - 1] = StringConverter1.DOT;

                // Shift down. Strings have at most 1 occurrence of a space.
                result[(indexSpace + 1)..].CopyTo(result[indexSpace..]);
                result = result[..^1];
            }
        }

        return new string(result);
    }

    private static string GetEggName1234(ushort species, int language, byte generation)
    {
        if (generation == 3)
            return "タマゴ"; // All Gen3 eggs are treated as JPN eggs.

        // Gen2 & Gen4 don't use Œuf like in future games
        if (language == (int)LanguageID.French)
            return generation == 2 ? "OEUF" : "Oeuf";

        var nick = GetSpeciesName(species, language);

        // All Gen4 egg names are Title cased.
        if (generation == 4)
            return nick;

        // Gen2: All Caps
        return nick.ToUpperInvariant();
    }

    /// <summary>
    /// Gets the Generation 7 species name for Chinese games.
    /// </summary>
    /// <remarks>
    /// Species Names for Chinese (Simplified) were revised during Generation 8 Crown Tundra DLC (#2).
    /// For a Gen7 species name request, return the old species name (hardcoded... yay).
    /// In an updated Gen8 game, the species nickname will automatically reset to the correct localization (on save/load ?), fixing existing entries.
    /// We don't differentiate patch revisions, just generation; Gen8 will return the latest localization.
    /// Gen8 did revise CHS species names, but only for Barraskewda, Urshifu, and Zarude. These species are new (Gen8); we can just use the latest.
    /// </remarks>
    private static string GetSpeciesName7ZH(ushort species, int language) => species switch
    {
        // Revised in DLC1 - Isle of Armor
        // https://cn.portal-pokemon.com/topics/event/200323190120_post_19.html
        (int)Species.Porygon2 => "多边兽Ⅱ",  // Later changed to 多边兽２型
        (int)Species.PorygonZ => "多边兽Ｚ", // Later changed to 多边兽乙型
        (int)Species.Mimikyu => "谜拟Ｑ",    // Later changed to 谜拟丘

        // Revised in DLC2 - Crown Tundra
        // https://cn.portal-pokemon.com/topics/event/201020170000_post_21.html
        (int)Species.Cofagrigus => "死神棺", // Later changed to 迭失棺
        (int)Species.Pangoro => "流氓熊猫",  // Later changed to 霸道熊猫
        //(int)Species.Nickit => "偷儿狐",     // Later changed to 狡小狐
        //(int)Species.Thievul => "狐大盗",    // Later changed to 猾大狐
        //(int)Species.Toxel => "毒电婴",      // Later changed to 电音婴
        //(int)Species.Runerigus => "死神板",  // Later changed to 迭失板

        _ => GetSpeciesName(species, language),
    };

    /// <summary>
    /// Checks if the input <see cref="nickname"/> is not the species name for all languages.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="nickname">Current name</param>
    /// <param name="context">Generation specific formatting option</param>
    /// <returns>True if it does not match any language name, False if not nicknamed</returns>
    public static bool IsNicknamedAnyLanguage(ushort species, ReadOnlySpan<char> nickname, EntityContext context = Latest.Context)
    {
        var langs = Language.GetAvailableGameLanguages(context);
        var generation = context.Generation;
        foreach (var language in langs)
        {
            if (!IsNicknamed(species, nickname, language, generation))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the input <see cref="nickname"/> is not the species name.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="nickname">Current name</param>
    /// <param name="language">Language ID of the Pokémon</param>
    /// <param name="generation">Generation specific formatting option</param>
    /// <returns>True if it does not match the language name, False if not nicknamed (matches).</returns>
    public static bool IsNicknamed(ushort species, ReadOnlySpan<char> nickname, int language, byte generation = Latest.Generation)
    {
        var expect = GetSpeciesNameGeneration(species, language, generation);
        return !nickname.SequenceEqual(expect);
    }

    /// <summary>
    /// Gets the Species name Language ID for the current name and generation.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="priorityLanguage">Language ID with a higher priority</param>
    /// <param name="nickname">Current name</param>
    /// <param name="context">Generation specific formatting option</param>
    /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
    public static int GetSpeciesNameLanguage(ushort species, int priorityLanguage, ReadOnlySpan<char> nickname, EntityContext context = Latest.Context)
    {
        var langs = Language.GetAvailableGameLanguages(context);
        var generation = context.Generation;
        var priorityIndex = langs.IndexOf((byte)priorityLanguage);
        if (priorityIndex != -1)
        {
            var expect = GetSpeciesNameGeneration(species, priorityLanguage, generation);
            if (nickname.SequenceEqual(expect))
                return priorityLanguage;
        }

        return GetSpeciesNameLanguage(species, nickname, generation, langs);
    }

    /// <summary>
    /// Gets the Species name Language ID for the current name and generation.
    /// </summary>
    /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
    /// <param name="nickname">Current name</param>
    /// <param name="context">Generation specific formatting option</param>
    /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
    public static int GetSpeciesNameLanguage(ushort species, ReadOnlySpan<char> nickname, EntityContext context = Latest.Context)
    {
        var langs = Language.GetAvailableGameLanguages(context);
        var generation = context.Generation;
        return GetSpeciesNameLanguage(species, nickname, generation, langs);
    }

    private static int GetSpeciesNameLanguage(ushort species, ReadOnlySpan<char> nickname, byte generation, ReadOnlySpan<byte> langs)
    {
        foreach (var lang in langs)
        {
            var expect = GetSpeciesNameGeneration(species, lang, generation);
            if (nickname.SequenceEqual(expect))
                return lang;
        }
        return -1;
    }

    /// <summary>
    /// Gets the Species ID for the specified <see cref="speciesName"/> and <see cref="language"/>.
    /// </summary>
    /// <param name="speciesName">Species Name</param>
    /// <param name="language">Language the name is from</param>
    /// <param name="species">Species ID</param>
    /// <returns>True if the species was found, False if not</returns>
    public static bool TryGetSpecies(ReadOnlySpan<char> speciesName, int language, out ushort species)
    {
        return SpeciesDict[language].TryGetValue(speciesName, out species);
    }

    public static bool TryGetSpeciesAnyLanguage(ReadOnlySpan<char> speciesName, out ushort species, EntityContext context = Latest.Context)
    {
        foreach (var language in Language.GetAvailableGameLanguages(context))
        {
            if (SpeciesDict[language].TryGetValue(speciesName, out species))
                return true;
        }
        species = 0;
        return false;
    }

    public static bool TryGetSpeciesAnyLanguageCaseInsensitive(ReadOnlySpan<char> speciesName, out ushort species, EntityContext context = Latest.Context)
    {
        Span<char> lowercase = stackalloc char[speciesName.Length];
        speciesName.ToLowerInvariant(lowercase);

        foreach (var language in Language.GetAvailableGameLanguages(context))
        {
            if (SpeciesDictLower[language].TryGetValue(lowercase, out species))
                return true;
        }
        species = 0;
        return false;
    }
}
