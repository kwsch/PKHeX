using PKHeX.Core;

namespace PKHeX.Application.Abstractions.GiftRecords;

/// <summary>
/// Read/write access to a save file's received Mystery Gift record log.
/// Mutations write directly into the in-memory <see cref="SaveFile"/>; nothing reaches
/// disk until the user saves the file, so operations are applied immediately.
/// </summary>
public interface IGiftRecordStore
{
    /// <summary>Number of record slots (including empty ones).</summary>
    int Count { get; }

    /// <summary>Reads all record slots.</summary>
    IReadOnlyList<GiftRecordEntry> ReadAll();

    /// <summary>Zeroes a record slot.</summary>
    void ClearEntry(int index);

    /// <summary>Whether a wondercard file can be converted into a record entry for this save.</summary>
    bool SupportsImport { get; }

    /// <summary>Whether the specified slot accepts wondercard imports.</summary>
    bool CanImport(int index);

    /// <summary>File-picker patterns for importable wondercard files (e.g. "*.wc9").</summary>
    IReadOnlyList<string> ImportExtensions { get; }

    /// <summary>
    /// Converts a wondercard file into a record and writes it to the given slot.
    /// Returns <see langword="false"/> with a reason key when the file is not usable for this save.
    /// </summary>
    bool TryImport(int index, byte[] data, string extension, DateTime receivedAt, out GiftRecordImportError error);

    /// <summary>Whether records can be exported back to a wondercard file (lossy reconstruction).</summary>
    bool SupportsExport { get; }

    /// <summary>Reconstructs a wondercard from the record at <paramref name="index"/>, or <see langword="null"/> when empty/unsupported.</summary>
    DataMysteryGift? ExportCard(int index);

    /// <summary>Whether this save tracks per-gift received flags alongside the records (BDSP).</summary>
    bool SupportsReceivedFlags { get; }

    /// <summary>Indexes of set received flags.</summary>
    IReadOnlyList<int> GetReceivedFlagIndexes();

    /// <summary>Sets or clears a received flag.</summary>
    void SetReceivedFlag(int flag, bool value);

    /// <summary>Whether this save has a serial-code redemption lock timestamp (BDSP).</summary>
    bool SupportsSerialLock { get; }

    /// <summary>The serial-code lock timestamp, or <see langword="null"/> when not locked/supported.</summary>
    DateTime? SerialLockTimestamp { get; }

    /// <summary>Clears the serial-code redemption lock.</summary>
    void ResetSerialLock();
}

/// <summary>Reason an import was rejected, mapped to a localized message by the caller.</summary>
public enum GiftRecordImportError
{
    None,
    UnreadableFile,
    WrongGame,
    UnsupportedGiftType,
    InvalidTimestamp,
    InvalidSlot,
}
