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

    [LocalizedDescription("Gen3: Decode two-byte Chinese characters from unofficial fan-translated games when displaying names, and encode them back when saving. Auto enables this only when the loaded save file looks fan-translated.")]
    public Gen3ChineseFanTextMode Gen3ChineseFanText { get; set; } = Gen3ChineseFanTextMode.Auto;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed record LangVersion
    {
        public LanguageID Language { get; set; } = LanguageID.English;
        public GameVersion Version { get; set; }
    }

    public enum Gen3ChineseFanTextMode : byte
    {
        /// <summary> Enable only when the loaded save file looks like a Chinese fan translation. </summary>
        Auto = 0,
        /// <summary> Always decode fan-translation sequences for Gen3 strings. </summary>
        Always = 1,
        /// <summary> Never decode; behave like the official games. </summary>
        Never = 2,
    }

    public void Apply()
    {
        StringConverter3Zh.Enabled = Gen3ChineseFanText switch
        {
            Gen3ChineseFanTextMode.Always => true,
            Gen3ChineseFanTextMode.Never => false,
            _ => StringConverter3Zh.Enabled, // Auto: decided per save file when loaded
        };

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
