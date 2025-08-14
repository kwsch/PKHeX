using System.ComponentModel;

namespace PKHeX.Core;

public sealed class SaveLanguageSettings
{
    [LocalizedDescription("Gen1: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen1 { get; set; } = new();

    [LocalizedDescription("Gen2: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen2 { get; set; } = new();

    [LocalizedDescription("Gen3 R/S: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen3RS { get; set; } = new();

    [LocalizedDescription("Gen3 FR/LG: If unable to detect a language or version for a save file, use these instead.")]
    public LangVersion OverrideGen3FRLG { get; set; } = new();

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed record LangVersion
    {
        public LanguageID Language { get; set; } = LanguageID.English;
        public GameVersion Version { get; set; }
    }

    public void Apply()
    {
        SaveLanguage.OverrideLanguageGen1 = OverrideGen1.Language;
        if (OverrideGen1.Version.IsGen1())
            SaveLanguage.OverrideVersionGen1 = OverrideGen1.Version;

        SaveLanguage.OverrideLanguageGen2 = OverrideGen2.Language;
        if (OverrideGen2.Version is GameVersion.GD or GameVersion.SI)
            SaveLanguage.OverrideVersionGen2 = OverrideGen2.Version;

        SaveLanguage.OverrideLanguageGen3RS = OverrideGen3RS.Language;
        if (OverrideGen3RS.Version is GameVersion.R or GameVersion.S)
            SaveLanguage.OverrideVersionGen3RS = OverrideGen3RS.Version;

        SaveLanguage.OverrideLanguageGen3FRLG = OverrideGen3FRLG.Language;
        if (OverrideGen3FRLG.Version is GameVersion.FR or GameVersion.LG)
            SaveLanguage.OverrideVersionGen3FRLG = OverrideGen3FRLG.Version;
    }
}
