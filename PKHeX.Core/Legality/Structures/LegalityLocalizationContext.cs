using System;

namespace PKHeX.Core;

public readonly ref struct LegalityLocalizationContext
{
    public required LegalityAnalysis Analysis { get; init; }
    public required BattleTemplateExportSettings Settings { get; init; }

    public GameStrings Strings => Settings.Localization.Strings;

    public string GetRibbonMessage() => RibbonVerifier.GetMessage(Analysis, Strings.Ribbons);
    public string GetStatName(int displayIndex) => GetStatName(Settings, displayIndex);
    public string GetMoveName(ushort move) => GetSafe(Strings.movelist, move);
    public string GetSpeciesName(ushort species) => GetSafe(Strings.specieslist, species);
    public string GetConsoleRegion3DS(int index) => GetSafe(Strings.console3ds, index);
    public string GetRibbonName(int index) => GetSafe(Strings.ribbons, index);
    public string GetLanguageName(int index) => GetSafe(Strings.languageNames, index);

    private static string GetStatName(BattleTemplateExportSettings settings, int displayIndex) => settings.Localization.Config.StatNamesFull.Names[displayIndex];
    private static string GetSafe(ReadOnlySpan<string> arr, int index)
    {
        if ((uint)index >= arr.Length)
            return string.Empty;
        return arr[index];
    }
}
