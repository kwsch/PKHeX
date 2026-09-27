using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Opens untrusted save bytes into a <see cref="SaveSession"/>.
/// </summary>
/// <remarks>
/// Input is bounded, copied before parsing, restricted to the enabled save families and integrity-checked.
/// Every failure throws <see cref="InvalidDataException"/> with a message that is safe to show to the user.
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
    /// <returns>A new session with revision 0.</returns>
    /// <exception cref="InvalidDataException">The input is out of bounds, unrecognised, not enabled or fails integrity checks.</exception>
    public static SaveSession Load(ReadOnlySpan<byte> bytes, string? fileName = null)
    {
        if (bytes.Length == 0 || bytes.Length > MaxInputBytes)
        {
            throw new InvalidDataException("The file is empty or exceeds the 16 MiB limit.");
        }

        var original = bytes.ToArray();
        // Core normalises its input buffer in place (e.g. Gen 7 zeroes the MemeCrypto signature block), so parse a copy.
        var save = SaveUtil.GetSaveFile(original.ToArray());
        // Release allowlist: only these concrete save types are enabled.
        if (save is not (SAV6XY or SAV6AO))
        {
            throw new InvalidDataException("Only raw XY/ORAS saves are supported.");
        }
        if (!save.State.Exportable || !save.ChecksumsValid)
        {
            throw new InvalidDataException("Save integrity validation failed. No changes were made.");
        }
        return new SaveSession(original, save, FileNaming.Sanitize(fileName));
    }
}
