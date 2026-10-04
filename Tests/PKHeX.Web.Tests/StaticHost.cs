using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Web.Tests;

/// <summary>Serves only a supplied Release wwwroot. No save-processing endpoints.</summary>
/// <remarks>
/// Responses carry the headers the published <c>_headers</c> file gives them (<see cref="HostHeaders"/>), as Cloudflare Pages serves them;
/// <see cref="PublishedAppFixture"/> and <see cref="DeploymentHeadersTests"/> check them against <see cref="ExpectedHeaders"/>.
/// Also as Cloudflare does, <c>_headers</c> itself is never served, and a missing file gets the published <c>404.html</c> with status 404.
/// With <c>deploymentCaching</c> the host also behaves like a caching static host (see <see cref="StaticHost(string, bool)"/>),
/// which <see cref="BootBaseline"/> needs for realistic transfer sizes and warm boots.
/// </remarks>
internal sealed class StaticHost : IDisposable
{
    /// <summary>The published file a host serves, with status 404, for a missing file.</summary>
    public const string NotFoundPage = "404.html";

    /// <summary><c>Cache-Control</c> Cloudflare puts on every 404, in place of any the rules give, so a missing file is never cached.</summary>
    public const string NotFoundCacheControl = "no-store";

    private readonly HttpListener listener = new();
    private readonly string root;
    private readonly HostHeaders rules;
    private readonly bool deploymentCaching;
    private readonly ConcurrentDictionary<(string File, bool Rewritten), string> entityTags = new();
    private ConcurrentQueue<ServedResponse> log = [];

    public string Url { get; }

    /// <summary>The <c>wwwroot</c> being served, as a full path.</summary>
    public string Root => root;

    /// <summary>Starts serving <paramref name="root"/> on a free loopback port.</summary>
    /// <param name="root">The published <c>wwwroot</c>.</param>
    /// <param name="deploymentCaching">
    /// When set, the host serves the precompressed <c>.br</c>/<c>.gz</c> sibling the browser accepts (with <c>Content-Encoding</c> and <c>Vary</c>),
    /// sends a strong <c>ETag</c> and answers a matching <c>If-None-Match</c> with 304, and sends the <c>Cache-Control</c> the <c>_headers</c> file gives.
    /// When not set, every request gets the raw file with no caching headers, so the browser tests never depend on what an earlier test cached.
    /// </param>
    /// <exception cref="InvalidOperationException"><paramref name="root"/> has no <c>_headers</c> file, or the file uses syntax <see cref="HostHeaders"/> does not accept.</exception>
    public StaticHost(string root, bool deploymentCaching = false)
    {
        this.root = Path.GetFullPath(root);
        this.deploymentCaching = deploymentCaching;
        rules = HostHeaders.Load(this.root);
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

    /// <summary>
    /// Answers one request and records it in the log. Never throws: a failed response is aborted and logged with status 0.
    /// </summary>
    /// <remarks>
    /// Each entry is recorded before the response is sent, because a client can finish reading it before <c>Close</c> returns: recorded
    /// afterwards, a test that reads the log as soon as its request completes could miss the entry. A response that then fails to send is
    /// followed by a status 0 entry.
    /// </remarks>
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
            if (!file.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) || method != "GET")
            {
                Volatile.Read(ref log).Enqueue(new(requestPath, method, 404, null, 0));
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }
            if (!File.Exists(file) || file == Path.Combine(root, "_headers"))
            {
                await RespondNotFoundAsync(context.Response, requestPath, urlPath);
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
                ".json" => "application/json", ".wasm" => "application/wasm", ".png" => "image/png",
                // The license and notices linked from the About panel; the types Cloudflare Pages serves them with.
                ".md" => "text/markdown; charset=utf-8", ".txt" => "text/plain; charset=utf-8",
                _ => "application/octet-stream",
            };
            ApplyRules(response, urlPath);

            if (deploymentCaching)
            {
                // The tag is computed over the bytes actually served, so each coding and the rewritten page get their own.
                var entityTag = entityTags.GetOrAdd((source, rewrite), _ => $"\"{Convert.ToHexStringLower(SHA256.HashData(bytes))[..32]}\"");
                response.Headers["ETag"] = entityTag;
                response.Headers["Vary"] = "Accept-Encoding";
                if (encoding is not null)
                {
                    response.Headers["Content-Encoding"] = encoding;
                }
                if (MatchesIfNoneMatch(context.Request.Headers["If-None-Match"], entityTag))
                {
                    Volatile.Read(ref log).Enqueue(new(requestPath, method, 304, encoding, 0));
                    response.StatusCode = 304;
                    response.Close();
                    return;
                }
            }

            Volatile.Read(ref log).Enqueue(new(requestPath, method, 200, encoding, bytes.Length));
            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes);
            response.Close();
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

    /// <summary>
    /// Sends the headers the <c>_headers</c> rules give <paramref name="urlPath"/>; <c>Cache-Control</c> only in deployment-caching mode.
    /// </summary>
    /// <param name="urlPath">The request path from the deployment root (the <c>/PKHeX</c> subpath removed).</param>
    private void ApplyRules(HttpListenerResponse response, string urlPath)
    {
        foreach (var (name, value) in rules.Resolve(urlPath))
        {
            if (deploymentCaching || !string.Equals(name, "Cache-Control", StringComparison.OrdinalIgnoreCase))
            {
                response.Headers[name] = value;
            }
        }
    }

    /// <summary>
    /// Answers a missing file, or the never-served <c>_headers</c>, as Cloudflare Pages does: the published <see cref="NotFoundPage"/> with status 404 and the rules' headers,
    /// with <see cref="NotFoundCacheControl"/> in place of any <c>Cache-Control</c> in deployment-caching mode. Without a <see cref="NotFoundPage"/> the body is empty.
    /// </summary>
    private async Task RespondNotFoundAsync(HttpListenerResponse response, string requestPath, string urlPath)
    {
        var page = Path.Combine(root, NotFoundPage);
        var bytes = File.Exists(page) ? await File.ReadAllBytesAsync(page) : [];
        response.StatusCode = 404;
        response.ContentType = "text/html";
        ApplyRules(response, urlPath);
        if (deploymentCaching)
        {
            response.Headers["Cache-Control"] = NotFoundCacheControl;
        }
        Volatile.Read(ref log).Enqueue(new(requestPath, "GET", 404, null, bytes.Length));
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes);
        response.Close();
    }

    public void Dispose()
    {
        listener.Stop();
        listener.Close();
    }
}
