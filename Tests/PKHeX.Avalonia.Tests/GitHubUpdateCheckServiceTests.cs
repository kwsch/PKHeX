using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PKHeX.Avalonia.Tests;

public class GitHubUpdateCheckServiceTests
{
    private const string ReleasesJson = """
        [
          {
            "tag_name": "v1.39.0",
            "name": "v1.39.0",
            "body": "Notes",
            "html_url": "https://example.com/releases/v1.39.0",
            "draft": false,
            "prerelease": false,
            "assets": [
              {
                "name": "PKHeX-Avalonia-win-x64.zip",
                "browser_download_url": "https://example.com/PKHeX-Avalonia-win-x64.zip",
                "digest": "sha256:aabbccddeeff00112233445566778899aabbccddeeff00112233445566778899",
                "size": 123456
              },
              {
                "name": "PKHeX-Avalonia-legacy-asset.zip",
                "browser_download_url": "https://example.com/legacy.zip"
              }
            ]
          }
        ]
        """;

    [Fact]
    public async Task GetReleasesAsync_parses_digest_and_size_and_strips_the_sha256_prefix()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ReleasesJson),
        });
        var service = new GitHubUpdateCheckService(new HttpClient(handler));

        var releases = await service.GetReleasesAsync();

        Assert.NotNull(releases);
        var asset = Assert.Single(releases!)!.Assets[0];
        Assert.Equal("aabbccddeeff00112233445566778899aabbccddeeff00112233445566778899", asset.Sha256);
        Assert.Equal(123456, asset.Size);
    }

    [Fact]
    public async Task GetReleasesAsync_leaves_sha256_null_when_no_digest_is_published()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ReleasesJson),
        });
        var service = new GitHubUpdateCheckService(new HttpClient(handler));

        var releases = await service.GetReleasesAsync();

        var legacyAsset = releases![0].Assets[1];
        Assert.Null(legacyAsset.Sha256);
        Assert.Equal(0, legacyAsset.Size);
    }
}
