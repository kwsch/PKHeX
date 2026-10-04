using Microsoft.Playwright;

namespace PKHeX.Web.Tests;

/// <summary>
/// Chooses and launches the browsers the <see cref="TestCategory.Perf"/> measurements run in, so every Perf harness honours the same
/// <see cref="TestEnvironment.PerfChannel"/>.
/// </summary>
/// <remarks>
/// Without a channel the Chromium rows use Playwright's own headless Chromium build, as CI does. With one, they use the installed release
/// browser of that channel (e.g. Google Chrome), which is what a support report can name. Firefox and WebKit have no channels in Playwright.
/// </remarks>
internal static class PerfBrowser
{
    /// <summary>The engines measured, in report order.</summary>
    public static readonly string[] Engines = ["chromium", "firefox", "webkit"];

    /// <summary>The Chromium release channels Playwright can launch from an installed browser.</summary>
    public static readonly string[] Channels = ["chrome", "chrome-beta", "chrome-dev", "chrome-canary", "msedge", "msedge-beta", "msedge-dev", "msedge-canary"];

    /// <summary>
    /// Reads <see cref="TestEnvironment.PerfChannel"/>: one of <see cref="Channels"/> (case-insensitive, returned in lower case), or
    /// <see langword="null"/> when unset.
    /// </summary>
    /// <exception cref="InvalidOperationException">The value is not a known channel, so a typo cannot quietly measure the wrong browser.</exception>
    public static string? ParseChannel(string? value)
    {
        if (value is null)
        {
            return null;
        }
        var channel = value.Trim().ToLowerInvariant();
        if (!Channels.Contains(channel, StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"{TestEnvironment.PerfChannel} must be one of {string.Join(", ", Channels)}; it is '{value}'.");
        }
        return channel;
    }

    /// <summary>The Playwright browser type of <paramref name="engine"/>.</summary>
    public static IBrowserType TypeOf(IPlaywright playwright, string engine) => engine switch
    {
        "chromium" => playwright.Chromium,
        "firefox" => playwright.Firefox,
        "webkit" => playwright.Webkit,
        _ => throw new ArgumentOutOfRangeException(nameof(engine), engine, null),
    };

    /// <summary>The channel <paramref name="engine"/> launches from: <paramref name="channel"/> for Chromium, none for the others.</summary>
    public static string? ChannelFor(string engine, string? channel) => engine == "chromium" ? channel : null;

    /// <summary>How a report names the browser: the channel when Chromium runs from one (e.g. <c>chrome</c>), otherwise the engine.</summary>
    public static string Label(string engine, string? channel) => ChannelFor(engine, channel) ?? engine;

    /// <summary>Launches <paramref name="engine"/> headless, from <paramref name="channel"/> when it is Chromium and a channel is given.</summary>
    public static Task<IBrowser> LaunchAsync(IPlaywright playwright, string engine, string? channel)
        => TypeOf(playwright, engine).LaunchAsync(new() { Headless = true, Channel = ChannelFor(engine, channel) });
}
