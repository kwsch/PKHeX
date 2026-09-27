namespace PKHeX.Web.Tests;

/// <summary>
/// Environment variables that supply the inputs of the <see cref="TestCategory.E2E"/> and <see cref="TestCategory.RealSave"/> tiers.
/// </summary>
/// <remarks>
/// Required inputs are read through <see cref="Required"/>, which throws when a variable is missing.
/// A selected tier therefore fails rather than skips without its inputs.
/// </remarks>
internal static class TestEnvironment
{
    /// <summary>Release publish <c>wwwroot</c> served by <see cref="StaticHost"/>.</summary>
    public const string Published = "PKHEX_WEB_PUBLISHED";

    /// <summary>Private decrypted X/Y save.</summary>
    public const string XYSave = "PKHEX_XY_SAVE";

    /// <summary>Private decrypted Omega Ruby/Alpha Sapphire save.</summary>
    public const string ORASSave = "PKHEX_ORAS_SAVE";

    /// <summary>Optional directory for sanitised real-save evidence JSON.</summary>
    public const string Evidence = "PKHEX_PROOF_EVIDENCE";

    /// <summary>Returns the value of <paramref name="name"/>, or throws if it is unset or blank.</summary>
    public static string Required(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Set {name}; the selected test tier must not silently skip required evidence.");
        }
        return value;
    }

    /// <summary>Returns the value of <paramref name="name"/>, or <see langword="null"/> if it is unset or blank.</summary>
    public static string? Optional(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
