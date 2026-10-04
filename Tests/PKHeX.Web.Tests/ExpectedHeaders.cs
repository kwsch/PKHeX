using System.Text.RegularExpressions;

namespace PKHeX.Web.Tests;

/// <summary>
/// The response headers a deployment must send, written out independently of the shipped <c>_headers</c> file.
/// </summary>
/// <remarks>
/// The test host serves whatever <c>_headers</c> says (<see cref="HostHeaders"/>); the boot checks and <see cref="DeploymentHeadersTests"/>
/// compare what was served with these values, so a wrong rule in the file fails a test instead of agreeing with itself.
/// </remarks>
internal static partial class ExpectedHeaders
{
    /// <summary>The app's Content-Security-Policy: the meta policy in <c>index.html</c> plus <c>frame-ancestors</c>, which only a header can carry.</summary>
    public const string AppPolicy = "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self'; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'";

    /// <summary>
    /// The policy for the license and notices: they run no script, and the browser's own text viewer may style itself (WebKit's sets an inline style)
    /// and ask for the site's icon (Firefox requests <c>/favicon.ico</c> for a text document).
    /// </summary>
    public const string TextPolicy = "default-src 'none'; img-src 'self'; style-src 'unsafe-inline'; frame-ancestors 'none'";

    /// <summary><c>Cache-Control</c> for fingerprinted assets, whose content never changes under the same name.</summary>
    public const string Immutable = "public, max-age=31536000, immutable";

    /// <summary><c>Cache-Control</c> for everything else: the browser may keep it but must revalidate before use.</summary>
    public const string Revalidate = "no-cache";

    /// <summary><c>X-Content-Type-Options</c> on every response.</summary>
    public const string ContentTypeOptions = "nosniff";

    /// <summary><c>Referrer-Policy</c> on every response.</summary>
    public const string ReferrerPolicy = "no-referrer";

    /// <summary>
    /// Whether a published file carries a content fingerprint in its name: a <c>_framework</c> file like <c>PKHeX.Core.qlok0qw4y5.wasm</c>
    /// (the .NET publish fingerprints all but the loaders <c>blazor.webassembly.js</c> and <c>dotnet.js</c>), or the sprite atlas and stylesheet,
    /// named by a hash of their content.
    /// </summary>
    /// <param name="relativePath">Path relative to <c>wwwroot</c>, with <c>/</c> separators.</param>
    public static bool IsFingerprinted(string relativePath) => FingerprintedPath().IsMatch(relativePath);

    /// <summary>Whether a published file is the license, the notices or an upstream notice, which get <see cref="TextPolicy"/>.</summary>
    /// <param name="relativePath">Path relative to <c>wwwroot</c>, with <c>/</c> separators.</param>
    public static bool IsLicenseText(string relativePath)
        => relativePath is "LICENSE.txt" or "THIRD-PARTY-NOTICES.md" || relativePath.StartsWith("licenses/", StringComparison.Ordinal);

    /// <summary>The <c>Cache-Control</c> a published file must be served with.</summary>
    public static string CacheControlFor(string relativePath) => IsFingerprinted(relativePath) ? Immutable : Revalidate;

    /// <summary>The Content-Security-Policy a published file must be served with.</summary>
    public static string PolicyFor(string relativePath) => IsLicenseText(relativePath) ? TextPolicy : AppPolicy;

    [GeneratedRegex(@"^(_framework/[^/]+\.[a-z0-9]{10}\.(wasm|js|dat)|sprites/(pokemon\.[0-9a-f]{16}\.png|sprites\.[0-9a-f]{16}\.css))$")]
    private static partial Regex FingerprintedPath();
}
