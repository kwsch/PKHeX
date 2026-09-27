using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PKHeX.Web.Tests;

/// <summary>Serves only a supplied Release wwwroot. No save-processing endpoints.</summary>
/// <remarks>
/// Responses carry deployment-like headers; <see cref="PublishedAppFixture"/> asserts them independently.
/// With <c>deploymentCaching</c> the host also behaves like a caching static host (see <see cref="StaticHost(string, bool)"/>),
/// which <see cref="BootBaseline"/> needs for realistic transfer sizes and warm boots.
/// </remarks>
internal sealed partial class StaticHost : IDisposable
{
    /// <summary>Content Security Policy header sent with every response.</summary>
    public const string ContentSecurityPolicy = "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self'; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'";

    /// <summary><c>Cache-Control</c> for fingerprinted assets, whose content never changes under the same name.</summary>
    public const string ImmutableCacheControl = "public, max-age=31536000, immutable";

    /// <summary><c>Cache-Control</c> for everything else: the browser may keep it but must revalidate before use.</summary>
    public const string RevalidateCacheControl = "no-cache";

    private readonly HttpListener listener = new();
    private readonly string root;
    private readonly bool deploymentCaching;
    private readonly ConcurrentDictionary<(string File, bool Rewritten), string> entityTags = new();
    private ConcurrentQueue<ServedResponse> log = [];

    public string Url { get; }

    /// <summary>Starts serving <paramref name="root"/> on a free loopback port.</summary>
    /// <param name="root">The published <c>wwwroot</c>.</param>
    /// <param name="deploymentCaching">
    /// When set, the host serves the precompressed <c>.br</c>/<c>.gz</c> sibling the browser accepts (with <c>Content-Encoding</c> and <c>Vary</c>),
    /// sends a strong <c>ETag</c> and answers a matching <c>If-None-Match</c> with 304, and sends <see cref="CacheControlFor"/>.
    /// This is the policy M20's checked-in <c>_headers</c> is planned to give a real deployment. When not set, every request gets the raw file with no caching headers.
    /// </param>
    public StaticHost(string root, bool deploymentCaching = false)
    {
        this.root = Path.GetFullPath(root);
        this.deploymentCaching = deploymentCaching;
        var reservation = new TcpListener(IPAddress.Loopback, 0);
        reservation.Start();
        var port = ((IPEndPoint)reservation.LocalEndpoint).Port;
        reservation.Stop();
        Url = $"http://127.0.0.1:{port}/";
        listener.Prefixes.Add(Url);
        listener.Start();
        _ = Serve();
    }

    /// <summary>One response the host sent: requested path, method, status, content coding (<see langword="null"/> for none) and body bytes written.</summary>
    public sealed record ServedResponse(string Path, string Method, int Status, string? Encoding, long BodyBytes);

    /// <summary>Atomically returns and clears the responses sent since the previous call.</summary>
    public IReadOnlyList<ServedResponse> TakeLog() => Interlocked.Exchange(ref log, []).ToArray();

    /// <summary>
    /// Whether a published file carries a content fingerprint in its name, like <c>_framework/PKHeX.Core.qlok0qw4y5.wasm</c>.
    /// The .NET publish fingerprints every <c>_framework</c> file except the loaders <c>blazor.webassembly.js</c> and <c>dotnet.js</c>.
    /// This is a heuristic for the test host only; M20's <c>_headers</c> will name the paths explicitly.
    /// </summary>
    /// <param name="relativePath">Path relative to <c>wwwroot</c>, with <c>/</c> separators.</param>
    public static bool IsFingerprinted(string relativePath) => FingerprintedPath().IsMatch(relativePath);

    /// <summary>The <c>Cache-Control</c> value for a published file in deployment-caching mode.</summary>
    /// <param name="relativePath">Path relative to <c>wwwroot</c>, with <c>/</c> separators.</param>
    public static string CacheControlFor(string relativePath) => IsFingerprinted(relativePath) ? ImmutableCacheControl : RevalidateCacheControl;

    /// <summary>
    /// Picks the content coding to serve: Brotli, then gzip, among those the <c>Accept-Encoding</c> header allows and a sibling exists for; otherwise none.
    /// A coding with <c>q=0</c> is refused.
    /// </summary>
    /// <returns><c>br</c>, <c>gzip</c> or <see langword="null"/> for the identity coding.</returns>
    public static string? ChooseEncoding(string? acceptEncoding, bool hasBrotli, bool hasGzip)
    {
        var accepted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var part in (acceptEncoding ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pieces = part.Split(';', StringSplitOptions.TrimEntries);
            var refused = pieces.Skip(1).Any(p => p.Replace(" ", "") is "q=0" or "q=0.0" or "q=0.00" or "q=0.000");
            if (!refused)
            {
                accepted.Add(pieces[0]);
            }
        }
        if (hasBrotli && accepted.Contains("br"))
        {
            return "br";
        }
        if (hasGzip && accepted.Contains("gzip"))
        {
            return "gzip";
        }
        return null;
    }

    /// <summary>Whether an <c>If-None-Match</c> header matches <paramref name="entityTag"/> (weak comparison, as for GET).</summary>
    public static bool MatchesIfNoneMatch(string? ifNoneMatch, string entityTag)
    {
        if (string.IsNullOrWhiteSpace(ifNoneMatch))
        {
            return false;
        }
        foreach (var candidate in ifNoneMatch.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (candidate == "*" || (candidate.StartsWith("W/", StringComparison.Ordinal) ? candidate[2..] : candidate) == entityTag)
            {
                return true;
            }
        }
        return false;
    }

    private async Task Serve()
    {
        while (listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await listener.GetContextAsync();
            }
            catch (Exception) when (!listener.IsListening)
            {
                return;
            }
            // Browsers fetch boot assets in parallel; serving them one at a time would stall every request behind the largest one.
            _ = RespondAsync(context);
        }
    }

    /// <summary>Answers one request and records it in the log. Never throws: a failed response is aborted and logged with status 0.</summary>
    private async Task RespondAsync(HttpListenerContext context)
    {
        var requestPath = context.Request.Url!.AbsolutePath;
        var method = context.Request.HttpMethod;
        try
        {
            var urlPath = Uri.UnescapeDataString(requestPath);
            var subpath = urlPath.StartsWith("/PKHeX/", StringComparison.Ordinal);
            if (subpath)
            {
                urlPath = urlPath[6..];
            }
            var file = Path.GetFullPath(Path.Combine(root, urlPath.TrimStart('/')));
            if (file == root)
            {
                file = Path.Combine(root, "index.html");
            }
            if (!file.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) || !File.Exists(file) || method != "GET")
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                Volatile.Read(ref log).Enqueue(new(requestPath, method, 404, null, 0));
                return;
            }

            // The only deployment transformation is the documented static base href; the rewritten page is always served uncompressed.
            var rewrite = subpath && Path.GetFileName(file) == "index.html";
            string? encoding = null;
            var source = file;
            if (deploymentCaching && !rewrite)
            {
                encoding = ChooseEncoding(context.Request.Headers["Accept-Encoding"], File.Exists(file + ".br"), File.Exists(file + ".gz"));
                source = encoding switch
                {
                    "br" => file + ".br",
                    "gzip" => file + ".gz",
                    _ => file,
                };
            }
            var bytes = await File.ReadAllBytesAsync(source);
            if (rewrite)
            {
                bytes = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes).Replace("<base href=\"/\"", "<base href=\"/PKHeX/\"", StringComparison.Ordinal));
            }

            var response = context.Response;
            response.ContentType = Path.GetExtension(file) switch
            {
                ".html" => "text/html", ".js" => "text/javascript", ".css" => "text/css",
                ".json" => "application/json", ".wasm" => "application/wasm", _ => "application/octet-stream",
            };
            response.Headers["X-Content-Type-Options"] = "nosniff";
            response.Headers["Referrer-Policy"] = "no-referrer";
            response.Headers["Content-Security-Policy"] = ContentSecurityPolicy;

            if (deploymentCaching)
            {
                // The tag is computed over the bytes actually served, so each coding and the rewritten page get their own.
                var entityTag = entityTags.GetOrAdd((source, rewrite), _ => $"\"{Convert.ToHexStringLower(SHA256.HashData(bytes))[..32]}\"");
                response.Headers["ETag"] = entityTag;
                response.Headers["Cache-Control"] = CacheControlFor(Path.GetRelativePath(root, file).Replace('\\', '/'));
                response.Headers["Vary"] = "Accept-Encoding";
                if (encoding is not null)
                {
                    response.Headers["Content-Encoding"] = encoding;
                }
                if (MatchesIfNoneMatch(context.Request.Headers["If-None-Match"], entityTag))
                {
                    response.StatusCode = 304;
                    response.Close();
                    Volatile.Read(ref log).Enqueue(new(requestPath, method, 304, encoding, 0));
                    return;
                }
            }

            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes);
            response.Close();
            Volatile.Read(ref log).Enqueue(new(requestPath, method, 200, encoding, bytes.Length));
        }
        catch
        {
            Volatile.Read(ref log).Enqueue(new(requestPath, method, 0, null, 0));
            try
            {
                context.Response.Abort();
            }
            catch
            {
                // Nothing awaits this task, so an exception here would only surface as unobserved; the failure is already logged above.
            }
        }
    }

    /// <summary><c>_framework/{name}.{10 lowercase letters or digits}.{wasm|js|dat}</c>.</summary>
    [GeneratedRegex(@"^_framework/[^/]+\.[a-z0-9]{10}\.(wasm|js|dat)$")]
    private static partial Regex FingerprintedPath();

    public void Dispose()
    {
        listener.Stop();
        listener.Close();
    }
}
