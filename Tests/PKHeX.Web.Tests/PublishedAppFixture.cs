using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Shares one <see cref="PublishedAppFixture"/> across every browser test class. Tests in a collection run serially.
/// </summary>
[CollectionDefinition(Name)]
public sealed class PublishedAppCollection : ICollectionFixture<PublishedAppFixture>
{
    /// <summary>Collection name used by <c>[Collection(PublishedAppCollection.Name)]</c>.</summary>
    public const string Name = "PublishedApp";
}

/// <summary>
/// Serves the Release publish output (<see cref="TestEnvironment.Published"/>) from a loopback <see cref="StaticHost"/>
/// and boots it in Playwright browsers, checking deployment headers, static-only boot requests and privacy.
/// Tests of the sprite atlas boot the publish made with sprites (<see cref="TestEnvironment.PublishedSprites"/>) instead, from a second host
/// started on first use.
/// </summary>
/// <remarks>
/// xUnit creates this fixture only when a test in <see cref="PublishedAppCollection"/> is selected, so Unit-only runs never need a publish or browsers.
/// A run that ignores the category filter, such as <c>vstest.console</c> on the assembly, still creates it for the skipped tests of a tier that is not opted in;
/// initialisation then throws, which xUnit does not report against skipped tests.
/// When a tier is opted in and the publish output is missing, initialisation throws and every test in the collection fails rather than skips.
/// </remarks>
public sealed class PublishedAppFixture : IAsyncLifetime
{
    /// <summary>Every browser engine the E2E and RealSave tiers cover.</summary>
    public static readonly string[] AllEngines = ["chromium", "firefox", "webkit"];

    /// <summary>The engines this run covers: <see cref="AllEngines"/>, or those named in <see cref="TestEnvironment.Engines"/>.</summary>
    public static readonly string[] Engines = TestEnvironment.SelectEngines(TestEnvironment.Optional(TestEnvironment.Engines), AllEngines);

    /// <summary>Hosting paths: the site root and the <c>/PKHeX/</c> subpath deployment.</summary>
    public static readonly string[] Prefixes = ["", "PKHeX/"];

    /// <summary>Expected <c>Content-Type</c> per published extension. An extension missing here fails the header check, so new asset types get a deliberate mapping.</summary>
    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".html"] = "text/html",
        [".js"] = "text/javascript",
        [".css"] = "text/css",
        [".json"] = "application/json",
        [".wasm"] = "application/wasm",
        [".dat"] = "application/octet-stream",
        [".md"] = "text/markdown",
        [".txt"] = "text/plain",
        [".png"] = "image/png",
    };

    /// <summary>Requested by some browsers on their own; the published app has none, so it is the only 404 allowed.</summary>
    private const string FaviconPath = "/favicon.ico";

    private readonly Dictionary<string, IBrowser> browsers = [];
    private StaticHost? host;
    private StaticHost? spriteHost;
    private IPlaywright? playwright;

    /// <summary>The published <c>wwwroot</c> being served.</summary>
    public string Root { get; private set; } = "";

    /// <summary>The <c>wwwroot</c> of the publish made with sprites; required, and served, only once a test asks for it.</summary>
    public string SpriteRoot => (spriteHost ??= StartSpriteHost()).Root;

    /// <summary>All engine × hosting-path combinations, for <c>[MemberData]</c>.</summary>
    public static IEnumerable<object[]> BrowserCases()
    {
        foreach (var engine in Engines)
        {
            foreach (var prefix in Prefixes)
            {
                yield return [engine, prefix];
            }
        }
    }

    public async Task InitializeAsync()
    {
        Root = PublishRoot(TestEnvironment.Published);
        host = new StaticHost(Root);
        playwright = await Playwright.CreateAsync();
    }

    /// <summary>The publish <c>wwwroot</c> named by <paramref name="variable"/>, checked to be one.</summary>
    private static string PublishRoot(string variable)
    {
        var root = Path.GetFullPath(TestEnvironment.Required(variable));
        if (!File.Exists(Path.Combine(root, "_framework", "blazor.webassembly.js")))
        {
            throw new InvalidOperationException($"Point {variable} to the Release publish wwwroot.");
        }
        return root;
    }

    private static StaticHost StartSpriteHost()
    {
        var root = PublishRoot(TestEnvironment.PublishedSprites);
        if (!File.Exists(Path.Combine(root, "sprites", "manifest.json")))
        {
            throw new InvalidOperationException($"{TestEnvironment.PublishedSprites} is not a publish made with -p:PKHeXWebSprites=true.");
        }
        return new StaticHost(root);
    }

    public async Task DisposeAsync()
    {
        foreach (var browser in browsers.Values)
        {
            await browser.DisposeAsync();
        }
        browsers.Clear();
        playwright?.Dispose();
        host?.Dispose();
        spriteHost?.Dispose();
    }

    /// <summary>
    /// Opens a fresh browser context, boots the app at <paramref name="prefix"/> and checks the boot with <see cref="AssertStaticBootAsync"/>.
    /// </summary>
    /// <param name="engine">One of <see cref="Engines"/>.</param>
    /// <param name="prefix">One of <see cref="Prefixes"/>.</param>
    /// <param name="sprites">Boot the publish made with sprites rather than the default one.</param>
    /// <param name="timezoneId">The browser's time zone (an IANA name), or null for the machine's.</param>
    public async Task<AppSession> BootAsync(string engine, string prefix, bool sprites = false, string? timezoneId = null)
    {
        var session = await CreateSessionAsync(engine, prefix, sprites, timezoneId);
        try
        {
            await session.Page.GotoAsync(session.AppUrl);
            await Expect(session.Page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
            session.BootMs = session.ElapsedMs;
            await AssertStaticBootAsync(session);
            return session;
        }
        catch
        {
            await session.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Opens a fresh browser context with the recorders installed, without navigating, for tests that change how the page loads
    /// (routes or init scripts) before going to <see cref="AppSession.AppUrl"/>. Nothing about the boot is checked.
    /// </summary>
    /// <param name="engine">One of <see cref="Engines"/>.</param>
    /// <param name="prefix">One of <see cref="Prefixes"/>.</param>
    /// <param name="sprites">Serve the publish made with sprites rather than the default one.</param>
    /// <param name="timezoneId">The browser's time zone (an IANA name), or null for the machine's.</param>
    public async Task<AppSession> CreateSessionAsync(string engine, string prefix, bool sprites = false, string? timezoneId = null)
    {
        var served = sprites ? spriteHost ??= StartSpriteHost() : host!;
        var browser = await GetBrowserAsync(engine);
        var context = await browser.NewContextAsync(new() { AcceptDownloads = true, TimezoneId = timezoneId });
        try
        {
            return await AppSession.CreateAsync(context, prefix, browser.Version, served.Url + prefix, served.Root);
        }
        catch
        {
            await context.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Fetches <paramref name="path"/> (relative to the app at <paramref name="prefix"/>) straight from the host, outside any browser,
    /// so the request is not counted as page activity.
    /// </summary>
    public async Task<HttpResponseMessage> FetchAsync(string prefix, string path)
    {
        using var client = new HttpClient();
        return await client.GetAsync(host!.Url + prefix + path);
    }

    /// <summary>
    /// Waits for the network to go idle, then checks everything recorded since the previous check (or since the session started):
    /// only static requests for published files, none failed, deployment headers on every response, and no CSP violations.
    /// Recording restarts atomically before the checks, so every later request counts as activity after boot.
    /// Call it again after a reload to check the second boot.
    /// </summary>
    public async Task AssertStaticBootAsync(AppSession session)
    {
        await session.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var (requests, responses, failures) = session.TakeRecorded();
        session.BootRequests = [.. requests.Select(r => new Uri(r.Url).AbsolutePath)];
        AssertOnlyStaticRequests(requests, session.Root, session.Prefix);
        Assert.True(failures.Count == 0, $"Boot requests failed without a response: {string.Join(", ", failures)}");
        await AssertDeploymentHeadersAsync(responses, session.Prefix);
        await session.AssertNoCspViolationsAsync();
    }

    private async Task<IBrowser> GetBrowserAsync(string engine)
    {
        // A browser that crashed in an earlier test is replaced, so one failure does not cascade through the engine.
        if (browsers.TryGetValue(engine, out var existing))
        {
            if (existing.IsConnected)
            {
                return existing;
            }
            await existing.DisposeAsync();
            browsers.Remove(engine);
        }
        var type = engine switch
        {
            "chromium" => playwright!.Chromium,
            "firefox" => playwright!.Firefox,
            "webkit" => playwright!.Webkit,
            _ => throw new ArgumentOutOfRangeException(nameof(engine), engine, null),
        };
        var browser = await type.LaunchAsync(new() { Headless = true });
        browsers[engine] = browser;
        return browser;
    }

    /// <summary>
    /// Boot may only fetch published files (plus the root and favicon) from the loopback host, with plain GETs.
    /// </summary>
    private static void AssertOnlyStaticRequests(IReadOnlyCollection<IRequest> requests, string root, string prefix)
    {
        var allowed = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(p => "/" + prefix + Path.GetRelativePath(root, p).Replace('\\', '/'))
            .ToHashSet();
        allowed.Add("/" + prefix);
        allowed.Add(FaviconPath);

        Assert.True(requests.Count > 0, "No boot requests were recorded.");
        foreach (var request in requests)
        {
            var uri = new Uri(request.Url);
            var isStatic = uri.Host == "127.0.0.1" && uri.Query.Length == 0 && request.Method == "GET"
                && request.PostData is null && allowed.Contains(uri.AbsolutePath);
            Assert.True(isStatic, "A non-static network request occurred during boot (details withheld).");
        }
    }

    /// <summary>
    /// Every boot response must carry the deployment headers (<c>nosniff</c>, CSP, referrer policy) and the MIME type expected for its extension.
    /// </summary>
    private static async Task AssertDeploymentHeadersAsync(IReadOnlyCollection<IResponse> responses, string prefix)
    {
        var document = "/" + prefix;
        var problems = new List<string>();
        var sawDocument = false;
        foreach (var response in responses)
        {
            var path = new Uri(response.Url).AbsolutePath;
            if (path == FaviconPath && response.Status == 404)
            {
                continue;
            }
            if (response.Status != 200)
            {
                problems.Add($"{path}: status {response.Status}");
                continue;
            }

            var headers = await response.AllHeadersAsync();
            var isDocument = path == document;
            sawDocument |= isDocument;
            var extension = isDocument ? ".html" : Path.GetExtension(path);
            if (!ContentTypes.TryGetValue(extension, out var expected))
            {
                problems.Add($"{path}: no expected Content-Type for '{extension}'");
            }
            else if (MediaType(headers.GetValueOrDefault("content-type")) != expected)
            {
                problems.Add($"{path}: Content-Type '{headers.GetValueOrDefault("content-type")}', expected '{expected}'");
            }
            if (headers.GetValueOrDefault("x-content-type-options") != "nosniff")
            {
                problems.Add($"{path}: missing X-Content-Type-Options: nosniff");
            }
            if (headers.GetValueOrDefault("content-security-policy") != StaticHost.ContentSecurityPolicy)
            {
                problems.Add($"{path}: Content-Security-Policy header differs");
            }
            if (headers.GetValueOrDefault("referrer-policy") != "no-referrer")
            {
                problems.Add($"{path}: missing Referrer-Policy: no-referrer");
            }
        }

        if (!sawDocument)
        {
            problems.Add($"{document}: document response not recorded");
        }
        Assert.True(problems.Count == 0, $"Deployment header problems:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
    }

    private static string? MediaType(string? contentType) => contentType?.Split(';')[0].Trim().ToLowerInvariant();
}

/// <summary>
/// One booted page in its own browser context, with every request, response, failed request and CSP violation recorded.
/// </summary>
public sealed class AppSession : IAsyncDisposable
{
    /// <summary>Page binding that receives CSP violations. Bindings live on the context, so records survive reloads and navigation.</summary>
    private const string CspBinding = "__pkhexCspViolation";

    /// <summary>
    /// Reports <c>securitypolicyviolation</c> events from page start. Playwright injects init scripts outside the page's CSP.
    /// Only the directive is sent, never the blocked value.
    /// </summary>
    private const string CspViolationRecorder = $$"""
        document.addEventListener('securitypolicyviolation', e => window.{{CspBinding}}(e.effectiveDirective));
        """;

    private readonly Stopwatch timer = Stopwatch.StartNew();
    private readonly ConcurrentQueue<string> cspViolations = [];
    private ConcurrentQueue<IRequest> requests = [];
    private ConcurrentQueue<IResponse> responses = [];
    private ConcurrentQueue<string> failures = [];
    private int pageErrors;

    private AppSession(IBrowserContext context, IPage page, string prefix, string browserVersion, string appUrl, string root)
    {
        Context = context;
        Page = page;
        Prefix = prefix;
        AppUrl = appUrl;
        Root = root;
        BrowserVersion = browserVersion;
        page.PageError += (_, _) => Interlocked.Increment(ref pageErrors);
        page.Dialog += async (_, dialog) =>
        {
            Dialogs.Enqueue(dialog.Type);
            await dialog.AcceptAsync();
        };
    }

    /// <summary>Opens the context's page and installs the recorders. Nothing has been navigated yet.</summary>
    internal static async Task<AppSession> CreateAsync(IBrowserContext context, string prefix, string browserVersion, string appUrl, string root)
    {
        var session = new AppSession(context, await context.NewPageAsync(), prefix, browserVersion, appUrl, root);
        context.Request += (_, request) => Volatile.Read(ref session.requests).Enqueue(request);
        context.Response += (_, response) => Volatile.Read(ref session.responses).Enqueue(response);
        context.RequestFailed += (_, request) => Volatile.Read(ref session.failures).Enqueue(request.Failure ?? "unknown");

        // Context-level binding and init script apply to the existing page from its first navigation, and after every reload.
        await context.ExposeFunctionAsync(CspBinding, (string? directive) =>
        {
            if (directive is not null)
            {
                session.cspViolations.Enqueue(directive);
            }
        });
        await context.AddInitScriptAsync(CspViolationRecorder);
        return session;
    }

    /// <summary>The isolated browser context; closing it discards downloads and all page state.</summary>
    public IBrowserContext Context { get; }

    /// <summary>The app page.</summary>
    public IPage Page { get; }

    /// <summary>Hosting path the app was booted at: empty for the root, or <c>PKHeX/</c>.</summary>
    public string Prefix { get; }

    /// <summary>Absolute URL of the app's page on the test host, including the <see cref="Prefix"/>.</summary>
    public string AppUrl { get; }

    /// <summary>The published <c>wwwroot</c> this session's host serves.</summary>
    public string Root { get; }

    /// <summary>Paths requested during the boot, as recorded by <see cref="PublishedAppFixture.AssertStaticBootAsync"/>; the latest boot's when called again.</summary>
    public IReadOnlyList<string> BootRequests { get; internal set; } = [];

    /// <summary>Version string of the browser engine, for evidence.</summary>
    public string BrowserVersion { get; }

    /// <summary>Milliseconds from navigation until the file input was visible.</summary>
    public long BootMs { get; internal set; }

    /// <summary>Milliseconds since the session was created, just before navigation.</summary>
    public long ElapsedMs => timer.ElapsedMilliseconds;

    /// <summary>Uncaught page errors since the session started.</summary>
    public int PageErrors => Volatile.Read(ref pageErrors);

    /// <summary>
    /// Types of the dialogs raised so far (<c>alert</c>, <c>confirm</c>, <c>beforeunload</c>, …). Every dialog is accepted,
    /// so tests that care whether one appeared, such as an unsaved-changes warning, assert on this list.
    /// </summary>
    public ConcurrentQueue<string> Dialogs { get; } = [];

    /// <summary>
    /// Atomically swaps in empty recorders and returns what was recorded before: requests, responses,
    /// and failure reasons (such as <c>net::ERR_BLOCKED_BY_CSP</c>) of requests that ended without a response. URLs of failures are not kept.
    /// </summary>
    internal (IReadOnlyCollection<IRequest> Requests, IReadOnlyCollection<IResponse> Responses, IReadOnlyCollection<string> Failures) TakeRecorded() =>
    (
        Interlocked.Exchange(ref requests, []).ToArray(),
        Interlocked.Exchange(ref responses, []).ToArray(),
        Interlocked.Exchange(ref failures, []).ToArray()
    );

    /// <summary>
    /// Fails if anything since the last boot check reached the network, if the browser persisted state
    /// (web storage, IndexedDB, Cache Storage, service workers or cookies), or if a page violated the CSP.
    /// </summary>
    public async Task AssertNoNetworkOrPersistenceAsync()
    {
        // Recording restarted when the boot was checked, so any request here came from using the app.
        Assert.True(Volatile.Read(ref requests).IsEmpty, "File processing triggered network activity after application boot.");

        foreach (var page in Context.Pages)
        {
            var empty = await page.EvaluateAsync<bool>("""
                async () => localStorage.length === 0 && sessionStorage.length === 0 &&
                    (await indexedDB.databases()).length === 0 && (await caches.keys()).length === 0 &&
                    (await navigator.serviceWorker.getRegistrations()).length === 0
                """);
            Assert.True(empty, "Browser save persistence was detected.");
        }
        Assert.Empty(await Context.CookiesAsync());
        await AssertNoCspViolationsAsync();
    }

    /// <summary>Fails if any page reported a Content Security Policy violation since the session started, across reloads; the message lists only the directives.</summary>
    public async Task AssertNoCspViolationsAsync()
    {
        foreach (var page in Context.Pages)
        {
            // Binding calls are delivered in order, so awaiting a sentinel call flushes earlier reports. It also fails if the recorder is missing.
            var installed = await page.EvaluateAsync<bool>($"async () => typeof window.{CspBinding} === 'function' && (await window.{CspBinding}(null), true)");
            Assert.True(installed, "The CSP violation recorder is missing.");
        }
        var directives = cspViolations.ToArray();
        Assert.True(directives.Length == 0, $"Content Security Policy violations: {string.Join(", ", directives)}");
    }

    /// <summary>
    /// Returns the directives of the CSP violations reported so far and forgets them, for tests that provoke a violation on purpose; a later
    /// <see cref="AssertNoCspViolationsAsync"/> then checks only what came after.
    /// </summary>
    public async Task<IReadOnlyList<string>> TakeCspViolationsAsync()
    {
        foreach (var page in Context.Pages)
        {
            // Flushes reports still in flight, as in AssertNoCspViolationsAsync.
            await page.EvaluateAsync($"async () => await window.{CspBinding}(null)");
        }
        var taken = new List<string>();
        while (cspViolations.TryDequeue(out var directive))
        {
            taken.Add(directive);
        }
        return taken;
    }

    public async ValueTask DisposeAsync() => await Context.DisposeAsync();
}
