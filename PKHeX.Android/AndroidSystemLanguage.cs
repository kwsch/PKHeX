using Android.Util;
using Java.Util;

namespace PKHeX.Android;

/// <summary>
/// Maps the phone's locale onto one of the UI languages this app ships, for use on first run.
/// </summary>
/// <remarks>
/// Only consulted when no settings file exists yet. Once the user has a persisted language —
/// including one they deliberately set to English on a Chinese phone — that choice wins, so this
/// must never run again for them.
/// </remarks>
internal static class AndroidSystemLanguage
{
    public static string Resolve(IReadOnlyList<string> supported, string fallback = "en")
    {
        try
        {
            var locale = Locale.Default;
            if (locale is null)
                return fallback;

            var language = locale.Language ?? string.Empty;

            // Chinese needs the script to pick a variant. Android may report it directly; when it
            // does not, the region implies it — Taiwan/Hong Kong/Macau are traditional.
            if (language.Equals("zh", StringComparison.OrdinalIgnoreCase))
            {
                var script = locale.Script ?? string.Empty;
                var country = locale.Country ?? string.Empty;
                var traditional =
                    script.Equals("Hant", StringComparison.OrdinalIgnoreCase) ||
                    country is "TW" or "HK" or "MO";
                var code = traditional ? "zh-Hant" : "zh-Hans";
                return Pick(supported, code, fallback);
            }

            return Pick(supported, language, fallback);
        }
        catch (Exception ex)
        {
            Log.Warn("PKHEX_ANDROID", $"System language unavailable, using {fallback} ({ex.Message})");
            return fallback;
        }
    }

    private static string Pick(IReadOnlyList<string> supported, string code, string fallback)
    {
        foreach (var candidate in supported)
        {
            if (candidate.Equals(code, StringComparison.OrdinalIgnoreCase))
                return candidate;
        }

        return fallback;
    }
}
