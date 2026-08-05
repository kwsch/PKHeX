using System.Collections.Concurrent;
using Android.Util;
using Avalonia.Platform.Storage;

namespace PKHeX.Android;

/// <summary>
/// Bridges Android's SAF content URIs to the path-based save/file APIs used by the shared
/// infrastructure. Files are copied into app-private storage while an editor is using them and
/// flushed back to the original SAF item after a successful save.
/// </summary>
internal static class AndroidStorageBridge
{
    private static readonly ConcurrentDictionary<string, IStorageFile> MappedFiles = new(StringComparer.Ordinal);

    public static async Task<string?> MaterializeFileAsync(IStorageFile file)
    {
        var localPath = file.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(localPath) && File.Exists(localPath))
            return localPath;

        try
        {
            var target = CreateCachePath(file.Name);
            using var source = await file.OpenReadAsync();
            using var destination = File.Create(target);
            await source.CopyToAsync(destination);
            await destination.FlushAsync();
            MappedFiles[Normalize(target)] = file;
            return target;
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"SAF read failed for {file.Name}: {ex}");
            return null;
        }
    }

    public static Task<string?> PrepareSaveFileAsync(IStorageFile file)
    {
        try
        {
            var localPath = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(localPath))
                localPath = CreateCachePath(file.Name);

            MappedFiles[Normalize(localPath)] = file;
            return Task.FromResult<string?>(localPath);
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"SAF save target preparation failed for {file.Name}: {ex}");
            return Task.FromResult<string?>(null);
        }
    }

    public static async Task<string?> MaterializeFolderAsync(IStorageFolder folder)
    {
        var localPath = folder.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(localPath) && Directory.Exists(localPath))
            return localPath;

        try
        {
            var target = Path.Combine(GetCacheDirectory(), "folders", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(target);
            await CopyFolderAsync(folder, target);
            return target;
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"SAF folder read failed for {folder.Name}: {ex}");
            return null;
        }
    }

    public static async Task<bool> FlushAsync(string? localPath)
    {
        if (string.IsNullOrWhiteSpace(localPath) || !MappedFiles.TryGetValue(Normalize(localPath), out var file))
            return true;

        try
        {
            using var source = File.OpenRead(localPath);
            using var destination = await file.OpenWriteAsync();
            if (destination.CanSeek)
                destination.SetLength(0);
            await source.CopyToAsync(destination);
            await destination.FlushAsync();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"SAF write failed for {file.Name}: {ex}");
            return false;
        }
    }

    private static async Task CopyFolderAsync(IStorageFolder source, string target)
    {
        await foreach (var item in source.GetItemsAsync())
        {
            var itemName = Sanitize(item.Name);
            if (item is IStorageFile file)
            {
                var path = Path.Combine(target, itemName);
                using var input = await file.OpenReadAsync();
                using var output = File.Create(path);
                await input.CopyToAsync(output);
                await output.FlushAsync();
                MappedFiles[Normalize(path)] = file;
            }
            else if (item is IStorageFolder folder)
            {
                var path = Path.Combine(target, itemName);
                Directory.CreateDirectory(path);
                await CopyFolderAsync(folder, path);
            }
        }
    }

    private static string CreateCachePath(string? name)
    {
        var directory = GetCacheDirectory();
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{Guid.NewGuid():N}-{Sanitize(name)}");
    }

    private static string GetCacheDirectory()
    {
        var root = AndroidHostContext.Activity?.FilesDir?.AbsolutePath;
        if (string.IsNullOrWhiteSpace(root))
            root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(root, "pkhex-storage");
    }

    private static string Normalize(string path) => Path.GetFullPath(path);

    private static string Sanitize(string? name)
    {
        var value = string.IsNullOrWhiteSpace(name) ? "file" : name;
        var chars = value.Where(c => char.IsLetterOrDigit(c) || c is '.' or '-' or '_').ToArray();
        return chars.Length == 0 ? "file" : new string(chars);
    }
}
