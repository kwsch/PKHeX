using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.Components;

/// <summary>
/// The text shown for each value of a <see cref="SaveOverview"/>.
/// </summary>
/// <remarks>
/// Numbers are formatted with the invariant culture, so the text does not depend on the browser's locale, and they are never shortened:
/// full values are shown with their units. A value the save does not hold, or holds in a form Core does not recognise, is labelled as such.
/// </remarks>
public static class OverviewText
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>What the overview says about integrity. It restates what <see cref="SaveLoader"/> checked before the session existed.</summary>
    public const string Integrity = "Checksums valid and unchanged round trip verified when opened. Every download is revalidated.";

    /// <summary>The game stored in the save, by Core's English name.</summary>
    public static string Game(SaveOverview overview) => overview.VersionValid
        ? GameInfo.GetVersionName(overview.Version)
        : Unknown((int)overview.Version);

    /// <summary>The family the save was opened as.</summary>
    public static string Family(SaveOverview overview) => overview.Family.Games;

    /// <summary>The trainer name, or a label when the save stores none.</summary>
    public static string Trainer(SaveOverview overview) => overview.TrainerName ?? "Not set in this save";

    /// <summary>The language name, or a label with the stored value when Core does not recognise it.</summary>
    public static string Language(SaveOverview overview) => overview.LanguageName ?? Unknown(overview.Language);

    /// <summary>The trainer ID, padded the way the game displays it.</summary>
    public static string TrainerId(SaveOverview overview) => overview.DisplayTid.ToString(overview.IdFormat.GetTrainerIDFormatStringTID(), Invariant);

    /// <summary>The secret ID, padded the way the game displays it.</summary>
    public static string SecretId(SaveOverview overview) => overview.DisplaySid.ToString(overview.IdFormat.GetTrainerIDFormatStringSID(), Invariant);

    /// <summary>Play time in hours, minutes and seconds; the hours are never capped.</summary>
    public static string PlayTime(SaveOverview overview) =>
        string.Create(Invariant, $"{overview.PlayedHours} h {overview.PlayedMinutes:00} min {overview.PlayedSeconds:00} s");

    /// <summary>Money in full, with thousands separators.</summary>
    public static string Money(SaveOverview overview) => string.Create(Invariant, $"{overview.Money:N0} Pokédollars");

    /// <summary>When the game last saved, or a label when the stored date is not valid.</summary>
    public static string LastSaved(SaveOverview overview) => overview.LastSaved?.ToString("yyyy-MM-dd HH:mm", Invariant) ?? "Not recorded";

    /// <summary>The exact size in bytes, followed by the size in KiB.</summary>
    public static string Size(SaveOverview overview) =>
        string.Create(Invariant, $"{overview.SizeBytes:N0} bytes ({overview.SizeBytes / 1024.0:#,0.#} KiB)");

    /// <summary>The save format. Only raw, decrypted saves are opened, and these games have no save revision to show.</summary>
    public static string Format(SaveOverview overview) => string.Create(Invariant, $"Raw Generation {overview.Generation} save");

    /// <summary>A stored value Core does not recognise.</summary>
    private static string Unknown(int value) => string.Create(Invariant, $"Unknown (stored value {value})");
}
