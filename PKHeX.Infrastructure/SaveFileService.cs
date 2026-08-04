using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Core;

namespace PKHeX.Infrastructure;

public sealed class SaveFileService : ISaveFileGateway
{
    private readonly ISaveBackupService _backupService;
    private readonly AppSettings _settings;

    public SaveFile? CurrentSave { get; private set; }
    public bool HasSave => CurrentSave is not null;
    public string? CurrentPath => _currentPath;

    private string? _currentPath;

    public event Action<SaveFile?>? SaveFileChanged;

    public SaveFileService(ISaveBackupService backupService, AppSettings settings)
    {
        _backupService = backupService;
        _settings = settings;
    }

    public async Task<bool> LoadSaveFileAsync(string path)
    {
        var sav = await Task.Run(() =>
        {
            try
            {
                var obj = FileUtil.GetSupportedFile(path);
                return obj as SaveFile;
            }
            catch
            {
                return null;
            }
        });

        if (sav is null)
            return false;

        CurrentSave = sav;
        _currentPath = path;
        SaveFileChanged?.Invoke(CurrentSave);

        return true;
    }

    public void OpenLoadedSave(SaveFile sav, string? path = null)
    {
        CurrentSave = sav;
        _currentPath = path ?? sav.Metadata.FilePath;
        SaveFileChanged?.Invoke(CurrentSave);
    }

    public Task<bool> SaveFileAsync(string? path = null)
    {
        return Task.Run(() =>
        {
            if (CurrentSave is null)
                return false;

            try
            {
                var savePath = path ?? _currentPath;
                if (string.IsNullOrEmpty(savePath))
                    return false;

                // Automatic backup (issue #135): snapshot whatever is currently on disk *before*
                // it gets overwritten, so a bad edit is always recoverable. Never blocks the save
                // itself — a failed backup is swallowed by SaveBackupService.
                if (_settings.Backup.BAKEnabled && File.Exists(savePath))
                {
                    try
                    {
                        var existing = File.ReadAllBytes(savePath);
                        var identity = SaveIdentity.Compute(savePath);
                        _backupService.CreateBackup(identity, existing, _settings.SaveBackup.MaxBackupsPerSave);
                    }
                    catch
                    {
                        // Backing up must never prevent the user from saving.
                    }
                }

                var data = CurrentSave.Write().ToArray();
                File.WriteAllBytes(savePath, data);

                if (path is not null)
                    _currentPath = path;

                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public void CloseSave()
    {
        CurrentSave = null;
        _currentPath = null;
        SaveFileChanged?.Invoke(null);
    }
}
