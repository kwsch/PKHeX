using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Serialises a session's working save and checks the output before it is handed to the user.
/// </summary>
public static class SaveExporter
{
    /// <summary>
    /// Writes a clone of the working save and reopens it through <see cref="SaveLoader"/>. The output must pass the loader's
    /// integrity and round-trip checks, keep the family, version and party count, and keep the drafted slot's entity byte for byte, with a
    /// valid checksum (for a party member, its stored battle stats, HP and status too).
    /// </summary>
    /// <remarks>
    /// This does not mark the session as exported. Once the download has been started, call
    /// <see cref="SaveSession.MarkExported"/> with the revision captured before calling this.
    /// </remarks>
    /// <param name="session">Session to export.</param>
    /// <param name="draft">The open draft, if any. It must belong to <paramref name="session"/>, be current and have no unapplied changes.</param>
    /// <returns>The validated file bytes.</returns>
    /// <exception cref="SessionException">
    /// The session holds flagged changes the user has not acknowledged for this revision (<see cref="SaveSession.ExportNeedsAcknowledgement"/>),
    /// the draft is foreign, stale or unapplied, or the output fails validation.
    /// </exception>
    public static byte[] Export(SaveSession session, EditorDraft? draft)
    {
        if (session.ExportNeedsAcknowledgement)
        {
            throw new SessionException(SessionError.ExportNotAcknowledged);
        }
        if (draft is not null)
        {
            session.EnsureOwns(draft);
            if (draft.IsDirty)
            {
                throw new SessionException(SessionError.DraftUnapplied);
            }
            session.EnsureCurrent(draft);
        }

        var working = session.Working;
        var bytes = working.Clone().Write().ToArray();
        if (SaveLoader.Load(bytes).Session is not { } reopened)
        {
            throw new SessionException(SessionError.ExportRevalidationFailed);
        }
        if (reopened.Working.GetType() != working.GetType() || reopened.Working.Version != working.Version
            || reopened.Working.PartyCount != working.PartyCount)
        {
            throw new SessionException(SessionError.ExportIdentityMismatch);
        }
        if (draft is not null)
        {
            var before = draft.Slot.ToSlotInfo(working).Read(working);
            var after = draft.Slot.ToSlotInfo(reopened.Working).Read(reopened.Working);
            if (!SaveSession.StoresExactly(after, before, draft.Slot.IsParty))
            {
                throw new SessionException(SessionError.ExportEntityMismatch);
            }
        }
        return bytes;
    }
}
