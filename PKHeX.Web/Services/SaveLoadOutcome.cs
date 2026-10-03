using PKHeX.Core;
using PKHeX.Web.Services.Diagnostics;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Why a file was not opened. The UI maps each value to text; nothing here is shown directly.
/// </summary>
public enum LoadFailure
{
    /// <summary>The file has no content.</summary>
    Empty,

    /// <summary>The file is larger than <see cref="SaveLoader.MaxInputBytes"/>.</summary>
    TooLarge,

    /// <summary>The browser could not read the file.</summary>
    ReadFailed,

    /// <summary>Core does not recognise the bytes as any save it knows. This says nothing about whether the file is damaged.</summary>
    Unrecognized,

    /// <summary>Core recognises the save, but its family is not in <see cref="SupportMatrix"/>.</summary>
    RecognizedNotEnabled,

    /// <summary>The save is of an enabled family, but fails an integrity check; see <see cref="IntegrityProblem"/>.</summary>
    IntegrityFailed,

    /// <summary>An exception was thrown while parsing or checking the save, or while building its session.</summary>
    ParserFault,
}

/// <summary>
/// Which integrity check an enabled save failed.
/// </summary>
public enum IntegrityProblem
{
    /// <summary>
    /// Core marks the save as not exportable. Current Core does this only for blank saves built without data, never for one parsed from bytes,
    /// so this is a defensive check.
    /// </summary>
    NotExportable,

    /// <summary>At least one block checksum does not match its data.</summary>
    ChecksumsInvalid,

    /// <summary>Writing the freshly parsed, unmodified save does not reproduce the original bytes.</summary>
    RoundTripMismatch,
}

/// <summary>
/// What Core recognised a refused file as, taken from the parsed save. It is only a description; no session exists for it.
/// </summary>
/// <param name="SaveType">The concrete Core save type.</param>
/// <param name="Version">The game version Core assigned.</param>
/// <param name="Generation">The save's generation.</param>
public sealed record RecognizedSave(Type SaveType, GameVersion Version, byte Generation)
{
    /// <summary>Describes <paramref name="save"/>.</summary>
    public static RecognizedSave From(SaveFile save) => new(save.GetType(), save.Version, save.Generation);
}

/// <summary>
/// Result of opening a file: either a new session, or a typed reason why none was created.
/// </summary>
/// <remarks>
/// It carries no display text and no exception message, so it is safe to log or turn into a diagnostic code. A parser fault keeps only the
/// redacted <see cref="DiagnosticCode"/> of what was thrown.
/// </remarks>
public sealed record SaveLoadOutcome
{
    /// <summary>The opened session, when <see cref="Succeeded"/>.</summary>
    public SaveSession? Session { get; private init; }

    /// <summary>Why nothing was opened, or null on success.</summary>
    public LoadFailure? Failure { get; private init; }

    /// <summary>Which check failed, when <see cref="Failure"/> is <see cref="LoadFailure.IntegrityFailed"/>.</summary>
    public IntegrityProblem? Integrity { get; private init; }

    /// <summary>What the file was recognised as, when <see cref="Failure"/> is <see cref="LoadFailure.RecognizedNotEnabled"/> or <see cref="LoadFailure.IntegrityFailed"/>.</summary>
    public RecognizedSave? Recognized { get; private init; }

    /// <summary>The redacted exception, when <see cref="Failure"/> is <see cref="LoadFailure.ParserFault"/> and one was caught.</summary>
    public DiagnosticCode? Fault { get; private init; }

    /// <summary>True when a session was opened.</summary>
    public bool Succeeded => Session is not null;

    private SaveLoadOutcome() { }

    /// <summary>A successful open.</summary>
    public static SaveLoadOutcome Opened(SaveSession session) => new() { Session = session };

    /// <summary>A failure that has no recognised save behind it.</summary>
    /// <exception cref="ArgumentException"><paramref name="failure"/> needs more detail; use <see cref="NotEnabled"/> or <see cref="IntegrityFailed"/>.</exception>
    public static SaveLoadOutcome Failed(LoadFailure failure)
    {
        if (failure is LoadFailure.RecognizedNotEnabled or LoadFailure.IntegrityFailed)
        {
            throw new ArgumentException("This failure needs the recognised save.", nameof(failure));
        }
        return new() { Failure = failure };
    }

    /// <summary>A parser fault, keeping only the redacted <paramref name="exception"/> (never its message).</summary>
    public static SaveLoadOutcome Faulted(Exception exception) => new()
    {
        Failure = LoadFailure.ParserFault, Fault = DiagnosticCode.FromException("open.parser-fault", exception),
    };

    /// <summary>A save Core recognises, of a family this release does not open.</summary>
    public static SaveLoadOutcome NotEnabled(RecognizedSave recognized) => new() { Failure = LoadFailure.RecognizedNotEnabled, Recognized = recognized };

    /// <summary>An enabled save that failed <paramref name="problem"/>.</summary>
    public static SaveLoadOutcome IntegrityFailed(RecognizedSave recognized, IntegrityProblem problem) => new()
    {
        Failure = LoadFailure.IntegrityFailed, Integrity = problem, Recognized = recognized,
    };
}
