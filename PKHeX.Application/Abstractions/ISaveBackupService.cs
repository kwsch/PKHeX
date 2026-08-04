namespace PKHeX.Application.Abstractions;

/// <summary>One backup snapshot of a save file on disk.</summary>
public readonly record struct SaveBackupEntry(string FilePath, string FileName, DateTime TimestampUtc, long SizeBytes);

/// <summary>
/// Result of listing the backups for a save identity. <see cref="Warnings"/> carries a human-readable
/// note per entry that could not be read/parsed (e.g. a truncated or otherwise corrupt backup file),
/// so the UI can surface a warning without the enumeration ever throwing.
/// </summary>
public readonly record struct BackupListResult(IReadOnlyList<SaveBackupEntry> Entries, IReadOnlyList<string> Warnings);

/// <summary>
/// Port for storing/retrieving timestamped save-file backups. Implemented in the Infrastructure layer
/// on top of the platform data directory (<see cref="IAppPaths.DataDirectory"/>). Backups for a given
/// save are grouped under a stable "save identity" folder (see <see cref="Services.SaveIdentity"/>) so
/// renaming/moving the source file via "Save As" does not scatter its history.
/// </summary>
public interface ISaveBackupService
{
    /// <summary>
    /// Writes <paramref name="data"/> as a new timestamped backup under <paramref name="saveIdentity"/>,
    /// then prunes the oldest backups beyond <paramref name="maxBackups"/>. Returns <see langword="null"/>
    /// and does not throw if the write fails (e.g. disk full/unwritable).
    /// </summary>
    SaveBackupEntry? CreateBackup(string saveIdentity, byte[] data, int maxBackups);

    /// <summary>Lists backups for <paramref name="saveIdentity"/>, newest first. Never throws.</summary>
    BackupListResult ListBackups(string saveIdentity);

    /// <summary>Reads the raw bytes of a backup. Throws if the file is missing/unreadable — callers decide how to report it.</summary>
    byte[] ReadBackup(SaveBackupEntry entry);

    /// <summary>Deletes a backup file. Never throws.</summary>
    void DeleteBackup(SaveBackupEntry entry);

    /// <summary>Full path to the backup folder for a save identity (created on demand).</summary>
    string GetBackupFolder(string saveIdentity);
}
