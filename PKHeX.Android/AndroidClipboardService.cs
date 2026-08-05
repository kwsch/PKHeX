using Android.Content;
using Android.Util;
using PKHeX.Application.Abstractions;

namespace PKHeX.Android;

public sealed class AndroidClipboardService : IClipboardService
{
    public Task<string?> GetTextAsync()
    {
        try
        {
            var activity = AndroidHostContext.Activity;
            var clipboard = activity?.GetSystemService(Context.ClipboardService) as ClipboardManager;
            var item = clipboard?.PrimaryClip?.GetItemAt(0);
            return Task.FromResult<string?>(item?.CoerceToText(activity)?.ToString());
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"Clipboard read failed: {ex}");
            return Task.FromResult<string?>(null);
        }
    }

    public Task SetTextAsync(string text)
    {
        try
        {
            var activity = AndroidHostContext.Activity;
            var clipboard = activity?.GetSystemService(Context.ClipboardService) as ClipboardManager;
            if (clipboard is not null)
                clipboard.PrimaryClip = ClipData.NewPlainText("PKHeX", text);
        }
        catch (Exception ex)
        {
            Log.Error("PKHEX_ANDROID", $"Clipboard write failed: {ex}");
        }

        return Task.CompletedTask;
    }
}
