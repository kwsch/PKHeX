using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Read-only summary of an open save, as Core reports it. Values are typed and carry no display text; the UI formats them.
/// </summary>
/// <remarks>
/// A value the save does not hold is null rather than a guess, and a stored value Core does not recognise is kept raw next to it,
/// so the UI can label it as unknown without inventing a meaning.
/// </remarks>
/// <param name="Version">The game version stored in the save.</param>
/// <param name="VersionValid">True when <paramref name="Version"/> is one of the games of the save's family.</param>
/// <param name="Family">The family the save was opened as.</param>
/// <param name="Generation">The save's generation.</param>
/// <param name="TrainerName">The trainer name, or null when the save stores none.</param>
/// <param name="Language">The stored language value.</param>
/// <param name="LanguageName">Core's name for <paramref name="Language"/>, or null when it is not a language of this generation.</param>
/// <param name="IdFormat">How the game displays trainer IDs.</param>
/// <param name="DisplayTid">The trainer ID as the game displays it.</param>
/// <param name="DisplaySid">The secret ID as the game displays it.</param>
/// <param name="PlayedHours">Hours of play time.</param>
/// <param name="PlayedMinutes">Minutes of play time, after the hours.</param>
/// <param name="PlayedSeconds">Seconds of play time, after the minutes.</param>
/// <param name="Money">Money, in the game's currency.</param>
/// <param name="LastSaved">When the game last saved, or null when the stored date is not a valid date.</param>
/// <param name="FileName">The sanitised name the file was opened as.</param>
/// <param name="SizeBytes">Size of the opened file, in bytes.</param>
public sealed record SaveOverview(
    GameVersion Version,
    bool VersionValid,
    SupportedFamily Family,
    byte Generation,
    string? TrainerName,
    int Language,
    string? LanguageName,
    TrainerIDFormat IdFormat,
    uint DisplayTid,
    uint DisplaySid,
    int PlayedHours,
    int PlayedMinutes,
    int PlayedSeconds,
    uint Money,
    DateTime? LastSaved,
    string FileName,
    int SizeBytes)
{
    /// <summary>
    /// Summarises the current revision of <paramref name="session"/>.
    /// </summary>
    /// <remarks>
    /// Integrity is not part of the summary: a session exists only for a save that passed every open check (see <see cref="SaveLoader"/>),
    /// and <see cref="SaveFile.ChecksumsValid"/> is not meaningful after an in-memory apply, because Core refreshes checksums only when writing.
    /// Raw 32-bit IDs are left out: in the 16-bit format of these games, the displayed IDs are the stored values.
    /// </remarks>
    public static SaveOverview From(SaveSession session)
    {
        var save = session.Working;
        return new SaveOverview(
            save.Version,
            save.IsVersionValid(),
            session.Capabilities.Family,
            save.Generation,
            string.IsNullOrWhiteSpace(save.OT) ? null : save.OT,
            save.Language,
            GameInfo.LanguageDataSource(save.Generation, save.Context).FirstOrDefault(l => l.Value == save.Language)?.Text,
            save.TrainerIDDisplayFormat,
            save.DisplayTID,
            save.DisplaySID,
            save.PlayedHours,
            save.PlayedMinutes,
            save.PlayedSeconds,
            save.Money,
            (save as SAV6)?.Played.LastSavedDate,
            session.FileName,
            session.OriginalLength);
    }
}
