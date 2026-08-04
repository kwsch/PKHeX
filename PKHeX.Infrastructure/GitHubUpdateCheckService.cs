using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using PKHeX.Application.Abstractions;

namespace PKHeX.Infrastructure;

/// <summary>
/// Queries the GitHub Releases API for doctorllll/PKHeX. Any failure (offline, DNS,
/// timeout, rate limit, malformed JSON) is caught and logged — never surfaced to the caller as an
/// exception — so a failed startup check never blocks or interrupts the user.
/// </summary>
public sealed class GitHubUpdateCheckService : IUpdateCheckService
{
    private const string ReleasesUrl = "https://api.github.com/repos/doctorllll/PKHeX/releases";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client;

    public GitHubUpdateCheckService() : this(CreateDefaultClient())
    {
    }

    // Internal for testability: lets tests inject a client with a fake handler instead of hitting the network.
    internal GitHubUpdateCheckService(HttpClient client)
    {
        _client = client;
    }

    private static HttpClient CreateDefaultClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PKHeX-Avalonia-UpdateChecker", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        return client;
    }

    public async Task<IReadOnlyList<ReleaseInfo>?> GetReleasesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _client.GetAsync(ReleasesUrl, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                Trace.TraceWarning($"Update check failed: GitHub API returned {(int)response.StatusCode} {response.StatusCode}.");
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var releases = await JsonSerializer.DeserializeAsync<List<GitHubRelease>>(stream, JsonOptions, cancellationToken).ConfigureAwait(false);
            if (releases is null)
                return null;

            return releases
                .Where(r => !r.Draft)
                .Select(r => new ReleaseInfo(
                    r.TagName ?? string.Empty,
                    string.IsNullOrEmpty(r.Name) ? r.TagName ?? string.Empty : r.Name,
                    r.Body ?? string.Empty,
                    r.HtmlUrl ?? string.Empty,
                    r.Prerelease,
                    (r.Assets ?? [])
                        .Where(a => !string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.BrowserDownloadUrl))
                        .Select(a => new ReleaseAsset(a.Name!, a.BrowserDownloadUrl!, StripDigestPrefix(a.Digest), a.Size))
                        .ToList()))
                .ToList();
        }
        catch (Exception ex)
        {
            // Offline, DNS failure, timeout, rate-limit, malformed response, etc. — stay silent, just log.
            Trace.TraceWarning($"Update check failed: {ex.Message}");
            return null;
        }
    }

    private sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("assets")]
        public List<GitHubAsset>? Assets { get; set; }
    }

    private sealed class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? BrowserDownloadUrl { get; set; }

        /// <summary>e.g. "sha256:abcdef…". Only populated on releases uploaded after GitHub added digest reporting.</summary>
        [JsonPropertyName("digest")]
        public string? Digest { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }

    private static string? StripDigestPrefix(string? digest)
    {
        if (string.IsNullOrEmpty(digest))
            return null;

        const string prefix = "sha256:";
        return digest.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? digest[prefix.Length..] : digest;
    }
}
