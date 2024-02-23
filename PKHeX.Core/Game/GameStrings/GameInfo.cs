using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Shared instance for fetching <see cref="GameStrings"/> data
/// </summary>
public static class GameInfo
{
    private static readonly GameStrings?[] Languages = new GameStrings[GameLanguage.LanguageCount];

    public static string CurrentLanguage { get; set; } = GameLanguage.DefaultLanguage;
    public static readonly IReadOnlyList<string> GenderSymbolUnicode = ["♂", "♀", "-"];
    public static readonly IReadOnlyList<string> GenderSymbolASCII = ["M", "F", "-"];
    private static GameStrings _strings = GetStrings(CurrentLanguage);

    public static GameStrings GetStrings(string lang)
    {
        int index = GameLanguage.GetLanguageIndex(lang);
        return GetStrings(index);
    }

    public static GameStrings GetStrings(int index)
    {
        return Languages[index] ??= new GameStrings(GameLanguage.Language2Char(index));
    }

    public static GameStrings Strings
    {
        get => _strings;
        set => Sources = new GameDataSource(_strings = value);
    }

    public static GameDataSource Sources { get; private set; } = new(_strings);
    public static FilteredGameDataSource FilteredSources { get; set; } = new(FakeSaveFile.Default, Sources);

    public static string GetVersionName(GameVersion version)
    {
        foreach (var kvp in VersionDataSource)
        {
            if (kvp.Value == (int)version)
                return kvp.Text;
        }
        return version.ToString();
    }

    // DataSource providing
    public static IReadOnlyList<ComboItem> ItemDataSource => FilteredSources.Items;
    public static IReadOnlyList<ComboItem> SpeciesDataSource => Sources.SpeciesDataSource;
    public static IReadOnlyList<ComboItem> BallDataSource => Sources.BallDataSource;
    public static IReadOnlyList<ComboItem> NatureDataSource => Sources.NatureDataSource;
    public static IReadOnlyList<ComboItem> AbilityDataSource => Sources.AbilityDataSource;
    public static IReadOnlyList<ComboItem> VersionDataSource => Sources.VersionDataSource;
    public static IReadOnlyList<ComboItem> MoveDataSource => Sources.HaXMoveDataSource;
    public static IReadOnlyList<ComboItem> GroundTileDataSource => Sources.GroundTileDataSource;
    public static IReadOnlyList<ComboItem> Regions => GameDataSource.Regions;

    public static IReadOnlyList<ComboItem> LanguageDataSource(byte generation)
        => GameDataSource.LanguageDataSource(generation);

    /// <summary>
    /// Gets the location name for the specified parameters.
    /// </summary>
    /// <param name="isEggLocation">Location is from the <see cref="PKM.EggLocation"/></param>
    /// <param name="location">Location value</param>
    /// <param name="format">Current <see cref="PKM.Format"/></param>
    /// <param name="generation"><see cref="PKM.Generation"/> of origin</param>
    /// <param name="version">Current GameVersion (only applicable for <see cref="GameVersion.Gen7b"/> differentiation)</param>
    /// <returns>Location name</returns>
    public static string GetLocationName(bool isEggLocation, ushort location, byte format, byte generation, GameVersion version)
    {
        return Strings.GetLocationName(isEggLocation, location, format, generation, version);
    }

    /// <summary>
    /// Gets the location list for a specific version, which can retrieve either met locations or egg locations.
    /// </summary>
    /// <param name="version">Version to retrieve for</param>
    /// <param name="context">Current format context</param>
    /// <param name="egg">Egg Locations are to be retrieved instead of regular Met Locations</param>
    /// <returns>Consumable list of met locations</returns>
    public static IReadOnlyList<ComboItem> GetLocationList(GameVersion version, EntityContext context, bool egg = false)
    {
        return Sources.Met.GetLocationList(version, context, egg);
    }
}
