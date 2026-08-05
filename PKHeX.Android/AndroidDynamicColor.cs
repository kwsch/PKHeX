using Android.Util;
using Avalonia.Media;

namespace PKHeX.Android;

/// <summary>
/// Repoints the app's accent palette at Android's Material You system colors, so PKHeX picks up
/// the wallpaper-derived accent the rest of the phone uses instead of shipping a fixed red.
/// </summary>
/// <remarks>
/// The <c>system_accent1_*</c> resources are guaranteed from API 31, which is this host's minimum,
/// but an OEM skin can still surprise us — any failure leaves the bundled palette in place.
/// </remarks>
internal static class AndroidDynamicColor
{
    public static void Apply(global::Avalonia.Application app)
    {
        var context = AndroidHostContext.Activity;
        if (context is null)
        {
            Log.Warn("PKHEX_ANDROID", "DynamicColor: no activity yet, keeping the bundled palette");
            return;
        }

        try
        {
            // Avalonia's Fluent theme derives its accent shades from these seven keys.
            Set(app, context, "SystemAccentColor", global::Android.Resource.Color.SystemAccent1500);
            Set(app, context, "SystemAccentColorDark1", global::Android.Resource.Color.SystemAccent1600);
            Set(app, context, "SystemAccentColorDark2", global::Android.Resource.Color.SystemAccent1700);
            Set(app, context, "SystemAccentColorDark3", global::Android.Resource.Color.SystemAccent1800);
            Set(app, context, "SystemAccentColorLight1", global::Android.Resource.Color.SystemAccent1300);
            Set(app, context, "SystemAccentColorLight2", global::Android.Resource.Color.SystemAccent1200);
            Set(app, context, "SystemAccentColorLight3", global::Android.Resource.Color.SystemAccent1100);
            Log.Info("PKHEX_ANDROID", "DynamicColor: applied Material You accents");
        }
        catch (Exception ex)
        {
            Log.Warn("PKHEX_ANDROID", $"DynamicColor: unavailable, keeping the bundled palette ({ex.Message})");
        }
    }

    private static void Set(global::Avalonia.Application app, global::Android.Content.Context context, string key, int colorResourceId)
    {
        var argb = context.GetColor(colorResourceId);
        app.Resources[key] = Color.FromArgb(
            (byte)((argb >> 24) & 0xFF),
            (byte)((argb >> 16) & 0xFF),
            (byte)((argb >> 8) & 0xFF),
            (byte)(argb & 0xFF));
    }
}
