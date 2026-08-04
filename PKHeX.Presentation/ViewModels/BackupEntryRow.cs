using PKHeX.Application.Abstractions;

namespace PKHeX.Presentation.ViewModels;

/// <summary>Display-ready wrapper around a <see cref="SaveBackupEntry"/> for the Backup Manager grid.</summary>
public sealed class BackupEntryRow(SaveBackupEntry entry)
{
    public SaveBackupEntry Entry { get; } = entry;

    public string FileName => Entry.FileName;
    public string FilePath => Entry.FilePath;

    /// <summary>Local time the backup was taken, formatted for display.</summary>
    public string Timestamp => Entry.TimestampUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

    public string Size => FormatSize(Entry.SizeBytes);

    private static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return $"{size:0.#} {units[unit]}";
    }
}
