using PKHeX.Application.Abstractions;
using PKHeX.Core;
using PKHeX.Infrastructure;

namespace PKHeX.Android;

/// <summary>
/// Keeps the shared path-based <see cref="SaveFileService"/> unchanged while synchronizing its
/// app-local working copy with the Android SAF document selected by the user.
/// </summary>
public sealed class AndroidSaveFileGateway : ISaveFileGateway
{
    private readonly SaveFileService _inner;

    public AndroidSaveFileGateway(SaveFileService inner) => _inner = inner;

    public SaveFile? CurrentSave => _inner.CurrentSave;
    public bool HasSave => _inner.HasSave;
    public string? CurrentPath => _inner.CurrentPath;

    public event Action<SaveFile?>? SaveFileChanged
    {
        add => _inner.SaveFileChanged += value;
        remove => _inner.SaveFileChanged -= value;
    }

    public Task<bool> LoadSaveFileAsync(string path) => _inner.LoadSaveFileAsync(path);

    public void OpenLoadedSave(SaveFile sav, string? path = null) => _inner.OpenLoadedSave(sav, path);

    public async Task<bool> SaveFileAsync(string? path = null)
    {
        var target = path ?? _inner.CurrentPath;
        if (!await _inner.SaveFileAsync(path))
            return false;
        return await AndroidStorageBridge.FlushAsync(target);
    }

    public void CloseSave() => _inner.CloseSave();
}
