using Microsoft.Extensions.Logging;
using PKHeX.Web.State;

namespace PKHeX.Web.Services.Diagnostics;

/// <summary>One recorded failure.</summary>
/// <param name="Time">When it was recorded, in UTC.</param>
/// <param name="Operation">What the user was doing.</param>
/// <param name="Code">The redacted failure.</param>
public sealed record DiagnosticEntry(DateTimeOffset Time, DiagnosticOperation Operation, DiagnosticCode Code);

/// <summary>
/// The failures recorded in this tab, for an opt-in diagnostic report, and the one place failures are written to the browser console.
/// </summary>
/// <remarks>
/// It holds <see cref="DiagnosticCode"/>s only, never save data, so it is kept across sessions in the tab; it lives in memory and is never stored,
/// so a reload empties it. The console receives the same redacted text as the report, never an exception's message.
/// Refusals of user input (an out-of-range level, say) are not failures and are not recorded.
/// </remarks>
public sealed class DiagnosticLog(ILogger<DiagnosticLog> logger, TimeProvider clock)
{
    /// <summary>Most entries kept; the oldest is dropped first.</summary>
    public const int Capacity = 20;

    private readonly Queue<DiagnosticEntry> entries = new(Capacity);

    /// <summary>The recorded failures, oldest first.</summary>
    public IReadOnlyList<DiagnosticEntry> Entries => [.. entries];

    /// <summary>Raised after an entry is recorded or the log is cleared.</summary>
    public event Action? Changed;

    /// <summary>Records <paramref name="code"/> for <paramref name="operation"/> and writes it to the browser console.</summary>
    public void Record(DiagnosticOperation operation, DiagnosticCode code)
    {
        if (entries.Count == Capacity)
        {
            entries.Dequeue();
        }
        entries.Enqueue(new(clock.GetUtcNow(), operation, code));
        logger.LogError("{Operation} failed: {Code}", operation, code.ToString());
        Changed?.Invoke();
    }

    /// <summary>Records an exception, redacted (<see cref="DiagnosticCode.For(Exception)"/>).</summary>
    public void Record(DiagnosticOperation operation, Exception exception) => Record(operation, DiagnosticCode.For(exception));

    /// <summary>
    /// Records <paramref name="exception"/> when it is a failure: anything other than a <see cref="SessionException"/>, or one whose error
    /// <see cref="IsFailure"/>. A refusal of the user's input is not recorded.
    /// </summary>
    /// <returns>True when it was recorded.</returns>
    public bool RecordIfFailure(DiagnosticOperation operation, Exception exception)
    {
        if (exception is SessionException refused && !IsFailure(refused.Error))
        {
            return false;
        }
        Record(operation, exception);
        return true;
    }

    /// <summary>
    /// True for a refusal that means something went wrong rather than that the input was refused: a staged write or an export that did not
    /// verify, a draft that no longer matches its session, or a reset whose original bytes no longer open.
    /// </summary>
    public static bool IsFailure(SessionError error) => error is SessionError.StagedWriteFailed or SessionError.StagedEditMismatch
        or SessionError.PartyCountChanged or SessionError.UntargetedSlotChanged or SessionError.SlotNotWritable
        or SessionError.ForeignDraft or SessionError.StaleDraft or SessionError.ResetFailed
        or SessionError.ExportRevalidationFailed or SessionError.ExportIdentityMismatch or SessionError.ExportEntityMismatch;

    /// <summary>Forgets every entry.</summary>
    public void Clear()
    {
        entries.Clear();
        Changed?.Invoke();
    }
}
