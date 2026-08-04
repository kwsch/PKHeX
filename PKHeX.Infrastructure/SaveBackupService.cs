using PKHeX.Application.Abstractions;

namespace PKHeX.Infrastructure;

/// <summary>
/// Default <see cref="ISaveBackupService"/> implementation. Backups are stored as timestamped files
/// under <c>IAppPaths.DataDirectory/backups/&lt;save-identity&gt;/</c>. Every operation is defensive:
/// a corrupt/unreadable backup is skipped (with a warning) rather than throwing, and write/delete
/// failures are swallowed since backups must never block the primary save operation.
/// </summary>
public sealed class SaveBackupService : ISaveBackupService
{
    private const string BackupsFolderName = "backups";
    private const string TimestampFormat = "yyyyMMdd_HHmmss_fff";
    private const string Extension = ".bak";

    private readonly IAppPaths _paths;

    public SaveBackupService(IAppPaths paths) => _paths = paths;

    public string GetBackupFolder(string saveIdentity)
        => Path.Combine(_paths.DataDirectory, BackupsFolderName, saveIdentity);

    public SaveBackupEntry? CreateBackup(string saveIdentity, byte[] data, int maxBackups)
    {
        try
        {
            var folder = GetBackupFolder(saveIdentity);
            Directory.CreateDirectory(folder);

            var fileName = $"{DateTime.UtcNow.ToString(TimestampFormat)}{Extension}";
            var filePath = Path.Combine(folder, fileName);

            // Guard against two backups landing in the same millisecond (fast retry loops in tests).
            var attempt = 1;
            while (File.Exists(filePath))
            {
                fileName = $"{DateTime.UtcNow.ToString(TimestampFormat)}_{attempt++}{Extension}";
                filePath = Path.Combine(folder, fileName);
            }

            File.WriteAllBytes(filePath, data);
            PruneOldBackups(folder, maxBackups);

            var info = new FileInfo(filePath);
            return new SaveBackupEntry(filePath, fileName, info.LastWriteTimeUtc, info.Length);
        }
        catch
        {
            return null;
        }
    }

    public BackupListResult ListBackups(string saveIdentity)
    {
        var entries = new List<SaveBackupEntry>();
        var warnings = new List<string>();

        var folder = GetBackupFolder(saveIdentity);
        if (!Directory.Exists(folder))
            return new BackupListResult(entries, warnings);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, $"*{Extension}");
        }
        catch (Exception ex)
        {
            warnings.Add($"Could not read the backup folder: {ex.Message}");
            return new BackupListResult(entries, warnings);
        }

        foreach (var file in files)
        {
            try
            {
                var info = new FileInfo(file);
                if (!info.Exists)
                {
                    warnings.Add($"{info.Name}: file disappeared while listing backups.");
                    continue;
                }
                if (info.Length == 0)
                {
                    // A zero-byte backup can never be a valid save (truncated write, disk-full,
                    // crash mid-copy, etc.) — surface it as a warning instead of a bogus "0 B" entry.
                    warnings.Add($"{info.Name}: backup file is empty/corrupt and was skipped.");
                    continue;
                }
                entries.Add(new SaveBackupEntry(info.FullName, info.Name, info.LastWriteTimeUtc, info.Length));
            }
            catch (Exception ex)
            {
                warnings.Add($"{Path.GetFileName(file)}: could not read backup ({ex.Message}).");
            }
        }

        // Primary: newest write time first. Secondary (tie-break): filename descending, ordinal.
        // Backups created inside the same millisecond (fast loops / rapid saves) share an identical
        // LastWriteTimeUtc, so the timestamp alone is ambiguous; the filename encodes creation order
        // (timestamp + "_N" collision suffix), and an ordinal compare keeps ordering deterministic and
        // culture-invariant across platforms (NTFS vs APFS/ext4 enumerate directories differently).
        entries.Sort((a, b) =>
        {
            var byTime = b.TimestampUtc.CompareTo(a.TimestampUtc);
            return byTime != 0 ? byTime : string.CompareOrdinal(b.FileName, a.FileName);
        });
        return new BackupListResult(entries, warnings);
    }

    public byte[] ReadBackup(SaveBackupEntry entry) => File.ReadAllBytes(entry.FilePath);

    public void DeleteBackup(SaveBackupEntry entry)
    {
        try
        {
            File.Delete(entry.FilePath);
        }
        catch
        {
            // Best effort — nothing sensible to do here beyond letting the UI re-list and notice it's gone.
        }
    }

    private static void PruneOldBackups(string folder, int maxBackups)
    {
        if (maxBackups <= 0)
            return;

        try
        {
            // Same tie-break as ListBackups: within an identical LastWriteTimeUtc, order by filename
            // (ordinal) so the retained/pruned set is deterministic regardless of platform directory
            // enumeration order — otherwise "keep newest N" is ambiguous for same-millisecond backups.
            var files = Directory.GetFiles(folder, $"*{Extension}")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .ThenByDescending(f => f.Name, StringComparer.Ordinal)
                .ToList();

            for (int i = maxBackups; i < files.Count; i++)
            {
                try { files[i].Delete(); }
                catch { /* best effort */ }
            }
        }
        catch
        {
            // Pruning is best-effort; never let it block the backup that was just created.
        }
    }
}
