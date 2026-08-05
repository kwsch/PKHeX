using Android.App;
using Android.Content;
using Android.Util;
using Avalonia.Platform.Storage;
using PKHeX.Application.Abstractions;

namespace PKHeX.Android;

public sealed class AndroidDialogService : IDialogService
{
    public async Task<string?> OpenFileAsync(string title, string[]? filters = null)
    {
        Log.Info("PKHEX_ANDROID", $"AndroidDialogService.OpenFileAsync: {title}");
        var topLevel = AndroidHostContext.GetTopLevel();
        Log.Info("PKHEX_ANDROID", $"AndroidDialogService.OpenFileAsync: topLevel={topLevel?.GetType().Name ?? "null"}");
        var provider = topLevel?.StorageProvider;
        if (provider is null)
        {
            Log.Warn("PKHEX_ANDROID", "AndroidDialogService.OpenFileAsync: storage provider unavailable");
            return null;
        }

        try
        {
            Log.Info("PKHEX_ANDROID", "AndroidDialogService.OpenFileAsync: opening picker");
            var result = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = BuildAndroidFileTypes(filters),
            });

            var file = result.FirstOrDefault();
            Log.Info("PKHEX_ANDROID", $"AndroidDialogService.OpenFileAsync: picker result={file?.Name ?? "cancelled"}");
            return file is null ? null : await AndroidStorageBridge.MaterializeFileAsync(file);
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"AndroidDialogService.OpenFileAsync failed: {ex}");
            return null;
        }
    }

    public async Task<string?> OpenFolderAsync(string title)
    {
        var provider = AndroidHostContext.GetTopLevel()?.StorageProvider;
        if (provider is null)
            return null;

        var result = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        });

        var folder = result.FirstOrDefault();
        return folder is null ? null : await AndroidStorageBridge.MaterializeFolderAsync(folder);
    }

    public async Task<string?> SaveFileAsync(string title, string? defaultFileName = null, string[]? filters = null)
    {
        var provider = AndroidHostContext.GetTopLevel()?.StorageProvider;
        if (provider is null)
            return null;

        var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName,
            FileTypeChoices = filters is null
                ? null
                : BuildAndroidFileTypes(filters),
            ShowOverwritePrompt = true,
        });

        return file is null ? null : await AndroidStorageBridge.PrepareSaveFileAsync(file);
    }

    public Task ShowErrorAsync(string title, string message) => ShowAlertAsync(title, message);

    public Task ShowInformationAsync(string title, string message) => ShowAlertAsync(title, message);

    public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Yes", string cancelText = "Cancel")
    {
        if (AndroidHostContext.Activity is not { } activity)
            return false;

        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = (global::Android.Content.Context)activity;
        activity.RunOnUiThread(() =>
        {
            try
            {
                var builder = new AlertDialog.Builder(context);
                builder.SetTitle(title);
                builder.SetMessage(message);
                builder.SetNegativeButton(cancelText, (_, _) => completion.TrySetResult(false));
                builder.SetPositiveButton(confirmText, (_, _) => completion.TrySetResult(true));
                var dialog = builder.Create()!;
                dialog.CancelEvent += (_, _) => completion.TrySetResult(false);
                dialog.Show();
            }
            catch (Exception ex)
            {
                Log.Error("PKHEX_ANDROID", $"Confirmation dialog failed: {ex}");
                completion.TrySetResult(false);
            }
        });

        return await completion.Task;
    }

    public void RevealInFileManager(string path)
    {
        // Android's SAF owns the document provider and does not offer a portable "reveal path"
        // operation. The file picker remains the authoritative way to locate the document.
    }

    public Task<string?> GetClipboardTextAsync() => new AndroidClipboardService().GetTextAsync();

    public Task SetClipboardTextAsync(string text) => new AndroidClipboardService().SetTextAsync(text);

    private static Task ShowAlertAsync(string title, string message)
    {
        if (AndroidHostContext.Activity is not { } activity)
            return Task.CompletedTask;

        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = (global::Android.Content.Context)activity;
        activity.RunOnUiThread(() =>
        {
            try
            {
                var builder = new AlertDialog.Builder(context);
                builder.SetTitle(title);
                builder.SetMessage(message);
                builder.SetPositiveButton("OK", (_, _) => completion.TrySetResult(true));
                var dialog = builder.Create()!;
                dialog.CancelEvent += (_, _) => completion.TrySetResult(true);
                dialog.Show();
            }
            catch (Exception ex)
            {
                Log.Error("PKHEX_ANDROID", $"Alert dialog failed: {ex}");
                completion.TrySetResult(true);
            }
        });

        return completion.Task;
    }

    private static List<FilePickerFileType> BuildAndroidFileTypes(string[]? filters)
    {
        var types = global::PKHeX.Avalonia.Services.FileDialogFilterFactory.BuildOpenFileTypes(filters);

        // Save formats such as .main, .sav and .pk* are not registered MIME types on Android.
        // Keep the desktop glob metadata for other backends, but use a wildcard MIME type here so
        // DocumentsUI never hides a valid PKHeX file merely because its extension is unfamiliar.
        foreach (var type in types)
            type.MimeTypes = ["*/*"];
        return types;
    }
}
