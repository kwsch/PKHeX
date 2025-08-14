using System.IO;
using System.Text.Json.Serialization;

namespace PKHeX.Core;

public sealed class LocalResourceSettings
{
    [JsonIgnore]
    private string LocalPath = string.Empty;

    public void SetLocalPath(string workingDirectory) => LocalPath = workingDirectory;

    private string Resolve(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return LocalPath;
        return Path.IsPathRooted(path) ? path : Path.Combine(LocalPath, path);
    }

    [LocalizedDescription("Path to the PKM Database folder.")]
    public string DatabasePath { get; set; } = "pkmdb";
    public string GetDatabasePath() => Resolve(DatabasePath);

    [LocalizedDescription("Path to the Mystery Gift Database folder for storing extra mystery gift templates that aren't yet recognized.")]
    public string MGDatabasePath { get; set; } = "mgdb";
    public string GetMGDatabasePath() => Resolve(MGDatabasePath);

    [LocalizedDescription("Path to the backup folder for keeping save file backups.")]
    public string BackupPath { get; set; } = "bak";
    public string GetBackupPath() => Resolve(BackupPath);

    [LocalizedDescription("Path to the sounds folder for sounds to play when hovering over a slot (species cry).")]
    public string SoundPath { get; set; } = "sounds";
    public string GetCryPath() => Resolve(SoundPath);

    [LocalizedDescription("Path to the template folder (with *.pk files) for initializing the PKM editor fields when a save file is loaded.")]
    public string TemplatePath { get; set; } = "template";
    public string GetTemplatePath() => Resolve(TemplatePath);

    [LocalizedDescription("Path to the Trainers folder (with *.pk files) used for generating encounters with known Trainer data.")]
    public string TrainerPath { get; set; } = "trainers";
    public string GetTrainerPath() => Resolve(TrainerPath);

    [LocalizedDescription("Path to the plugins folder.")]
    public string PluginPath { get; set; } = "plugins";
    public string GetPluginPath() => Resolve(PluginPath);
}
