namespace PKHeX.Web.Tests;

/// <summary>
/// Environment variables that opt in to the <see cref="TestCategory.E2E"/>, <see cref="TestCategory.RealSave"/> and <see cref="TestCategory.Perf"/> tiers and supply their inputs.
/// </summary>
/// <remarks>
/// Those tiers run only when named in <see cref="Tiers"/>; otherwise <see cref="TierFactAttribute"/> and <see cref="TierTheoryAttribute"/> skip them.
/// This keeps runners that ignore the project's default run settings, such as <c>vstest.console</c> on the built assembly, to the self-contained <see cref="TestCategory.Unit"/> tier.
/// Required inputs are read through <see cref="Required"/>, which throws when a variable is missing.
/// An opted-in tier therefore fails rather than skips without its inputs.
/// </remarks>
internal static class TestEnvironment
{
    /// <summary>Opt-in tiers to run, separated by commas, semicolons or spaces, e.g. <c>E2E,RealSave</c>. Case-insensitive.</summary>
    public const string Tiers = "PKHEX_WEB_TEST_TIERS";

    /// <summary>Release publish <c>wwwroot</c> served by <see cref="StaticHost"/>.</summary>
    public const string Published = "PKHEX_WEB_PUBLISHED";

    /// <summary>
    /// Release publish <c>wwwroot</c> made with <c>-p:PKHeXWebSprites=true</c>, for the sprite tests of the <see cref="TestCategory.E2E"/> tier.
    /// </summary>
    public const string PublishedSprites = "PKHEX_WEB_PUBLISHED_SPRITES";

    /// <summary>
    /// Optional browser engines for the browser tiers, separated by commas, semicolons or spaces (e.g. <c>firefox</c>); all of
    /// <see cref="PublishedAppFixture.AllEngines"/> when unset. CI runs one engine per job so the engines run in parallel.
    /// </summary>
    public const string Engines = "PKHEX_WEB_ENGINES";

    /// <summary>Private decrypted X/Y save.</summary>
    public const string XYSave = "PKHEX_XY_SAVE";

    /// <summary>Private decrypted Omega Ruby/Alpha Sapphire save.</summary>
    public const string ORASSave = "PKHEX_ORAS_SAVE";

    /// <summary>Optional directory for sanitised real-save evidence JSON.</summary>
    public const string Evidence = "PKHEX_PROOF_EVIDENCE";

    /// <summary>Directory the <see cref="TestCategory.Perf"/> tier writes <c>boot-baseline.md</c> and <c>boot-baseline.json</c> to.</summary>
    public const string PerfReport = "PKHEX_WEB_PERF_REPORT";

    /// <summary>Optional number of measured boots per configuration in the <see cref="TestCategory.Perf"/> tier; see <see cref="BootBaseline.DefaultRuns"/>.</summary>
    public const string PerfRuns = "PKHEX_WEB_PERF_RUNS";

    /// <summary>Returns the value of <paramref name="name"/>, or throws if it is unset or blank.</summary>
    public static string Required(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Set {name}; the opted-in test tier must not silently skip required evidence.");
        }
        return value;
    }

    /// <summary>Whether <paramref name="tier"/> runs: <see cref="TestCategory.Unit"/> always does, the others only when named in <see cref="Tiers"/>.</summary>
    public static bool IsOptedIn(string tier) => IsOptedIn(tier, Optional(Tiers));

    /// <summary>Whether <paramref name="tier"/> runs when <see cref="Tiers"/> has the value <paramref name="optIn"/>.</summary>
    public static bool IsOptedIn(string tier, string? optIn)
    {
        if (tier == TestCategory.Unit)
        {
            return true;
        }
        var names = optIn?.Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        return names.Contains(tier, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// The engines named in <paramref name="value"/> (see <see cref="Engines"/>), in the order of <paramref name="all"/>, or all of them when it is blank.
    /// </summary>
    /// <exception cref="ArgumentException">A name is not one of <paramref name="all"/>, so a typo cannot quietly run fewer engines.</exception>
    public static string[] SelectEngines(string? value, IReadOnlyList<string> all)
    {
        var names = value?.Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        if (names.Length == 0)
        {
            return [.. all];
        }
        var unknown = names.Where(n => !all.Contains(n, StringComparer.OrdinalIgnoreCase)).ToArray();
        if (unknown.Length != 0)
        {
            throw new ArgumentException($"{Engines} names unknown engines: {string.Join(", ", unknown)}. Use {string.Join(", ", all)}.", nameof(value));
        }
        return [.. all.Where(e => names.Contains(e, StringComparer.OrdinalIgnoreCase))];
    }

    /// <summary>Skip reason for a test in <paramref name="tier"/>, or <see langword="null"/> when the tier is opted in.</summary>
    public static string? SkipUnlessOptedIn(string tier) => IsOptedIn(tier) ? null : $"Opt-in tier; set {Tiers}={tier} to run it.";

    /// <summary>Returns the value of <paramref name="name"/>, or <see langword="null"/> if it is unset or blank.</summary>
    public static string? Optional(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
