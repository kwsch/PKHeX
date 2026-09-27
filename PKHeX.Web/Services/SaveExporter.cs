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
    /// integrity checks, keep the family and version, and keep the drafted slot's checksum and nickname fields.
    /// </summary>
    /// <remarks>
    /// This does not mark the session as exported. Once the download has been started, call
    /// <see cref="SaveSession.MarkExported"/> with the revision captured before calling this.
    /// </remarks>
    /// <param name="session">Session to export.</param>
    /// <param name="draft">The open draft, if any. It must belong to <paramref name="session"/>, be current and have no unapplied changes.</param>
    /// <returns>The validated file bytes.</returns>
    /// <exception cref="InvalidDataException">The draft is foreign, stale or unapplied, or the output fails validation.</exception>
    public static byte[] Export(SaveSession session, EditorDraft? draft)
    {
        if (draft is not null)
        {
            session.EnsureOwns(draft);
            if (draft.IsDirty)
            {
                throw new InvalidDataException("Apply or cancel the draft before downloading.");
            }
            session.EnsureCurrent(draft);
        }

        var working = session.Working;
        var bytes = working.Clone().Write().ToArray();
        var reopened = SaveLoader.Load(bytes);
        if (reopened.Family != session.Family || reopened.Working.Version != working.Version)
        {
            throw new InvalidDataException("Exported save identity validation failed.");
        }
        if (draft is not null)
        {
            var before = SaveSession.GetSlot(working, draft.SlotIndex).Read(working);
            var after = SaveSession.GetSlot(reopened.Working, draft.SlotIndex).Read(reopened.Working);
            if (!after.ChecksumValid || before.Nickname != after.Nickname || before.IsNicknamed != after.IsNicknamed)
            {
                throw new InvalidDataException("Exported Pokémon validation failed.");
            }
        }
        return bytes;
    }
}
