using PKHeX.Core;
using PKHeX.Web.Interop;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// The text shown for typed outcomes and refusals. State and services report codes only; all their wording lives here.
/// </summary>
/// <remarks>
/// Messages describe what happened and what the user can do. They never claim a file is corrupt when it was only not recognised,
/// never call a family "supported", and never include exception details.
/// </remarks>
public static class UserMessages
{
    /// <summary>What this release opens, as stated in refusals.</summary>
    internal static string OpensOnly { get; } = $"This release opens only raw {string.Join(" or ", SupportMatrix.Families.Select(f => f.Games))} saves.";

    /// <summary>The generic text for a failure that has no typed reason.</summary>
    public const string OperationFailed = "Operation failed. The previous session was retained; no file was overwritten.";

    /// <summary>Text for a failed open.</summary>
    /// <exception cref="ArgumentException"><paramref name="outcome"/> succeeded.</exception>
    public static string For(SaveLoadOutcome outcome) => outcome.Failure switch
    {
        LoadFailure.Empty => "The file is empty.",
        LoadFailure.TooLarge => "The file exceeds the 16 MiB limit.",
        LoadFailure.ReadFailed => "The file could not be read. Choose it again.",
        LoadFailure.Unrecognized => $"This file was not recognised as a save this release can open. {OpensOnly} Use the decrypted main file exported with a save manager on the console, or taken from an emulator.",
        LoadFailure.RecognizedNotEnabled => $"{Describe(outcome.Recognized)} {OpensOnly}",
        LoadFailure.IntegrityFailed => $"This {FamilyName(outcome.Recognized)} save {For(outcome.Integrity)}, so it was not opened. Nothing was repaired or changed.",
        LoadFailure.ParserFault => "The file could not be processed. It was not opened, and nothing was changed.",
        _ => throw new ArgumentException("The outcome is not a failure.", nameof(outcome)),
    };

    /// <summary>The clause naming a failed integrity check.</summary>
    internal static string For(IntegrityProblem? problem) => problem switch
    {
        IntegrityProblem.NotExportable => "is marked by PKHeX as not exportable",
        IntegrityProblem.ChecksumsInvalid => "failed its checksum validation",
        IntegrityProblem.RoundTripMismatch => "would not be reproduced exactly when written back, even without edits",
        _ => "failed an integrity check",
    };

    /// <summary>Text for a refused session, draft or export operation.</summary>
    public static string For(SessionError error) => error switch
    {
        SessionError.SlotNotOccupied => "That position is empty. Choose a Pokémon.",
        SessionError.PartyApplyNotAvailable => "Party members can be inspected but not changed in this release.",
        SessionError.EntityInvalid => "The selected Pokémon is a bad egg (its data fails the game's checks), so it cannot be opened.",
        SessionError.ForeignDraft => "The draft does not belong to the open save. Select the Pokémon again.",
        SessionError.StaleDraft => "The draft is out of date. Select the Pokémon again.",
        SessionError.DraftUnapplied => "Apply or cancel the draft before downloading.",
        SessionError.SlotNotWritable => "The selected slot cannot be edited.",
        SessionError.StagedWriteFailed => "The edit could not be written, so it was not applied.",
        SessionError.StagedEditMismatch => "The edit did not read back as written, so it was not applied.",
        SessionError.FieldNotEditable => "This field cannot be changed for this game in this release.",
        SessionError.NicknameTooLong => "The nickname is longer than this game can store.",
        SessionError.NicknameInvalidCharacters => "The nickname contains control characters.",
        SessionError.NicknameNotRepresentable => "This game cannot store the nickname without changing its text.",
        SessionError.ExportRevalidationFailed => "The exported save did not pass validation when reopened, so it was not downloaded.",
        SessionError.ExportIdentityMismatch => "The exported save reopened as a different game, so it was not downloaded.",
        SessionError.ExportEntityMismatch => "The edited Pokémon did not survive export intact, so the save was not downloaded.",
        _ => OperationFailed,
    };

    /// <summary>Text for a drop refused before anything was read.</summary>
    public static string For(DropRejection rejection) => rejection switch
    {
        DropRejection.MultipleFiles => "Drop a single save file.",
        DropRejection.Directory => "Drop a save file, not a folder.",
        DropRejection.Busy => "Wait for the current operation to finish, then try again.",
        _ => "Only files can be dropped here, not text or links.",
    };

    /// <summary>Describes a recognised save of a family this release does not open, without naming a game Core did not establish.</summary>
    private static string Describe(RecognizedSave? recognized) => recognized switch
    {
        { SaveType: var t } when t == typeof(SAV6AODemo) => "This looks like a save from the Omega Ruby/Alpha Sapphire special demo.",
        { Generation: > 0 and var generation } => $"This looks like a Generation {generation} save.",
        _ => "This looks like a save of a game this release does not open.",
    };

    /// <summary>The display name of an enabled family, taken from <see cref="SupportMatrix"/>.</summary>
    private static string FamilyName(RecognizedSave? recognized) => SupportMatrix.Families.FirstOrDefault(f => f.SaveType == recognized?.SaveType)?.Games ?? "Pokémon";
}
