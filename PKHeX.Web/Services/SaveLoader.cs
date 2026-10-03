using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Opens untrusted save bytes into a <see cref="SaveSession"/>.
/// </summary>
/// <remarks>
/// Input is bounded, copied before parsing, restricted to the families in <see cref="SupportMatrix"/>, integrity-checked,
/// and must write back unchanged. Every failure is returned as a typed <see cref="SaveLoadOutcome"/>; nothing is repaired.
/// </remarks>
public static class SaveLoader
{
    /// <summary>Largest input accepted, in bytes (16 MiB).</summary>
    public const int MaxInputBytes = 16 * 1024 * 1024;

    /// <summary>
    /// Parses <paramref name="bytes"/> into a new session. The caller's buffer is never mutated.
    /// </summary>
    /// <param name="bytes">Raw file contents.</param>
    /// <param name="fileName">Name the file was opened as. It is sanitised, and <see cref="FileNaming.DefaultSaveName"/> is used when it is missing.</param>
    /// <returns>A new session with revision 0, or the reason none was created.</returns>
    public static SaveLoadOutcome Load(ReadOnlySpan<byte> bytes, string? fileName = null) => Load(bytes, fileName, Parse, WriteForComparison);

    /// <summary>
    /// <see cref="Load(ReadOnlySpan{byte}, string?)"/> with the Core parse and write steps replaceable, so tests can inject faults Core does not produce on demand.
    /// </summary>
    internal static SaveLoadOutcome Load(ReadOnlySpan<byte> bytes, string? fileName, Func<byte[], SaveFile?> parse, Func<SaveFile, ReadOnlyMemory<byte>> write)
    {
        if (bytes.Length == 0)
        {
            return SaveLoadOutcome.Failed(LoadFailure.Empty);
        }
        if (bytes.Length > MaxInputBytes)
        {
            return SaveLoadOutcome.Failed(LoadFailure.TooLarge);
        }

        var original = bytes.ToArray();
        try
        {
            // Core normalises its input buffer in place (e.g. Gen 7 zeroes the MemeCrypto signature block), so parse a copy.
            var save = parse(original.ToArray());
            if (save is null)
            {
                return SaveLoadOutcome.Failed(LoadFailure.Unrecognized);
            }
            var recognized = RecognizedSave.From(save);
            if (!SupportMatrix.IsEnabled(save))
            {
                return SaveLoadOutcome.NotEnabled(recognized);
            }
            if (!save.State.Exportable)
            {
                return SaveLoadOutcome.IntegrityFailed(recognized, IntegrityProblem.NotExportable);
            }
            if (!save.ChecksumsValid)
            {
                return SaveLoadOutcome.IntegrityFailed(recognized, IntegrityProblem.ChecksumsInvalid);
            }
            // A save that passes its checksums but that Core would not write back identically would be changed by the first export, even with no edits.
            if (!write(save).Span.SequenceEqual(original))
            {
                return SaveLoadOutcome.IntegrityFailed(recognized, IntegrityProblem.RoundTripMismatch);
            }
            return SaveLoadOutcome.Opened(new SaveSession(original, save, FileNaming.Sanitize(fileName)));
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            // The input is untrusted; any exception while reading it is a parser fault. Only its redacted code is kept, for the caller to record;
            // its message, which can carry values from the file, is never shown or logged.
            return SaveLoadOutcome.Faulted(ex);
        }
    }

    private static SaveFile? Parse(byte[] data) => SaveUtil.GetSaveFile(data);

    /// <summary>The bytes Core would export for <paramref name="save"/>, without changing it.</summary>
    /// <remarks>It writes a clone, because <see cref="SaveFile.Write"/> refreshes checksums in the save's own buffer.</remarks>
    internal static ReadOnlyMemory<byte> WriteForComparison(SaveFile save) => save.Clone().Write();
}
