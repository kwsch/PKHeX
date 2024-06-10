using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic related to the name of a <see cref="Species"/>.
/// </summary>
public static class SpeciesName
{
    private const int LatestGeneration = PKX.Generation;

    /// <summary>
    /// Species name lists indexed by the <see cref="LanguageID"/> value.
    /// </summary>
    private static readonly IReadOnlyList<IReadOnlyList<string>> SpeciesLang =
    [
        Util.GetSpeciesList("ja"), // 0 (unused, invalid)
        Util.GetSpeciesList("ja"), // 1
        Util.GetSpeciesList("en"), // 2
        Util.GetSpeciesList("fr"), // 3
        Util.GetSpeciesList("it"), // 4
        Util.GetSpeciesList("de"), // 5
        Util.GetSpeciesList("es"), // 6 (reserved for Gen3 KO?, unused)
        Util.GetSpeciesList("es"), // 7
        Util.GetSpeciesList("ko"), // 8
        Util.GetSpeciesList("zh"), // 9 Simplified
        Util.GetSpeciesList("zh2"), // 10 Traditional
    ];

    /// <summary>
    /// Gets the maximum valid species ID stored in the <see cref="SpeciesLang"/> list.
    /// </summary>
    public static readonly ushort MaxSpeciesID = (ushort)(SpeciesLang[0].Count - 1);

    /// <summary>
    /// Egg name list indexed by the <see cref="LanguageID"/> value.
    /// </summary>
    /// <remarks>Indexing matches <see cref="SpeciesLang"/>.</remarks>
    private static readonly string[] EggNames =
    [
        "タマゴ",
        "タマゴ",
        "Egg",
        "Œuf",
        "Uovo",
        "Ei",
        "Huevo",
        "Huevo",
        "알",
        "蛋",
        "蛋",
    ];

    /// <summary>
    /// <see cref="PKM.Nickname"/> to <see cref="Species"/> table for all <see cref="LanguageID"/> values.
    /// </summary>
    private static readonly Dictionary<int, ushort>[] SpeciesDict = GetDictionary(SpeciesLang);

    private static Dictionary<int, ushort>[] GetDictionary(IReadOnlyList<IReadOnlyList<string>> names)
    {
        var result = new Dictionary<int, ushort>[names.Count];
        for (int i = 0; i < result.Length; i++)
        {
            var speciesList = names[i];
            var dict = new Dictionary<int, ushort>(speciesList.Count - 1);
            for (ushort species = 1; species < speciesList.Count; species++)
            {
                var name = speciesList[species];
                var hash = string.GetHashCode(name);
                dict.Add(hash, species);
            }
            result[i] = dict;
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
        if ((uint)language >= SpeciesLang.Count)
            return string.Empty;

        if (species == 0)
            return EggNames[language];

        var arr = SpeciesLang[language];
        if (species >= arr.Count)
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
    public static string GetEggName(int language, byte generation = LatestGeneration) => generation switch
    {
        <= 4 => GetEggName1234(0, language, generation),
        _ => (uint)language >= EggNames.Length ? string.Empty : EggNames[language],
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
        nick.CopyTo(result);

        // All names are uppercase.
        foreach (ref var c in result)
            c = char.ToUpperInvariant(c);
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
    /// Gen8 did revise CHT species names, but only for Barraskewda, Urshifu, and Zarude. These species are new (Gen8); we can just use the latest.
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
    /// <param name="generation">Generation specific formatting option</param>
    /// <returns>True if it does not match any language name, False if not nicknamed</returns>
    public static bool IsNicknamedAnyLanguage(ushort species, ReadOnlySpan<char> nickname, byte generation = LatestGeneration)
    {
        var langs = Language.GetAvailableGameLanguages(generation);
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
    public static bool IsNicknamed(ushort species, ReadOnlySpan<char> nickname, int language, byte generation = LatestGeneration)
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
    /// <param name="generation">Generation specific formatting option</param>
    /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
    public static int GetSpeciesNameLanguage(ushort species, int priorityLanguage, ReadOnlySpan<char> nickname, byte generation = LatestGeneration)
    {
        var langs = Language.GetAvailableGameLanguages(generation);
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
    /// <param name="generation">Generation specific formatting option</param>
    /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
    public static int GetSpeciesNameLanguage(ushort species, ReadOnlySpan<char> nickname, byte generation = LatestGeneration)
    {
        var langs = Language.GetAvailableGameLanguages(generation);
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
        // Eventually we'll refactor this once Dictionary<string, ushort> supports ReadOnlySpan<char> fetching in NET 9 runtime/#27229
        var hash = string.GetHashCode(speciesName);
        var dict = SpeciesDict[language];
        if (!dict.TryGetValue(hash, out species))
            return false;
        // Double-check the species name
        var arr = SpeciesLang[language];
        var expect = arr[species];
        return speciesName.SequenceEqual(expect);
    }
}
