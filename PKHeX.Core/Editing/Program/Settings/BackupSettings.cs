using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class BackupSettings
{
    [LocalizedDescription("Automatic Backups of Save Files are copied to the backup folder when true.")]
    public bool BAKEnabled { get; set; } = true;

    [LocalizedDescription("Tracks if the \"Create Backup\" prompt has been issued to the user.")]
    public bool BAKPrompt { get; set; }

    [LocalizedDescription("List of extra locations to look for Save Files.")]
    public List<string> OtherBackupPaths { get; set; } = [];

    [LocalizedDescription("Save File file-extensions (no period) that the program should also recognize.")]
    public List<string> OtherSaveFileExtensions { get; set; } = [];
}
