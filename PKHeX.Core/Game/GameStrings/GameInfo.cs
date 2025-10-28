using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Shared instance for fetching <see cref="GameStrings"/> data
/// </summary>
public static class GameInfo
{
    private static readonly LanguageStorage<GameStrings> Languages = new GameStringResourceSet();
    private sealed record GameStringResourceSet : LanguageStorage<GameStrings>
    {
        protected override GameStrings Create(string language) => new(language);
    }

    public static string CurrentLanguage { get; set; } = GameLanguage.DefaultLanguage;
    public static readonly IReadOnlyList<string> GenderSymbolUnicode = ["♂", "♀", "-"];
    public static readonly IReadOnlyList<string> GenderSymbolASCII = ["M", "F", "-"];
    private static GameStrings _strings = GetStrings(CurrentLanguage);
    public static GameDataSource Sources { get; private set; } = new(_strings);
    public static FilteredGameDataSource FilteredSources { get; set; } = new(FakeSaveFile.Default, Sources);

    public static GameStrings Strings
    {
        get => _strings;
        set => Sources = new GameDataSource(_strings = value);
    }

    public static GameStrings GetStrings(string lang) => Languages.Get(lang);

    public static string GetVersionName(GameVersion version)
    {
        foreach (var kvp in Sources.VersionDataSource)
        {
            if (kvp.Value == (int)version)
                return kvp.Text;
        }
        return version.ToString();
    }

    public static IReadOnlyList<ComboItem> LanguageDataSource(byte generation, EntityContext context)
        => Sources.LanguageDataSource(generation, context);

    /// <summary>
    /// Gets the location name for the specified parameters.
    /// </summary>
    /// <param name="isEggLocation">Location is from the <see cref="PKM.EggLocation"/></param>
    /// <param name="location">Location value</param>
    /// <param name="format">Current <see cref="PKM.Format"/></param>
    /// <param name="generation"><see cref="PKM.Generation"/> of origin</param>
    /// <param name="version">Version within <see cref="generation"/>, if needed to differentiate.</param>
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
