using System.IO;
using System.Net.Http;

namespace PKHeX.Core.AutoMod;

public static class InternalNetUtil
{
    /// <summary>
    /// Gets the html string from the requested <see cref="address"/>.
    /// </summary>
    /// <param name="address">Address to fetch from</param>
    /// <returns>Page response</returns>
    public static string GetPageText(string address)
    {
        var stream = GetStreamFromURL(address);
        return GetStringResponse(stream);
    }

    private static Stream GetStreamFromURL(string url)
    {
        using var client = new HttpClient();
        const string agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
        client.DefaultRequestHeaders.Add("User-Agent", agent);
        var response = client.GetAsync(url).Result;
        return response.Content.ReadAsStreamAsync().Result;
    }

    /// <summary>
    /// Downloads a string response with hard-coded address parameters.
    /// </summary>
    /// <param name="address">Address to fetch from</param>
    /// <returns>Page response</returns>
    public static string DownloadString(string address)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "PKHeX-Auto-Legality-Mod");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        var response = client.GetAsync(address).Result;
        var stream = response.Content.ReadAsStreamAsync().Result;
        return GetStringResponse(stream);
    }

    private static string GetStringResponse(Stream? dataStream)
    {
        if (dataStream == null)
            return string.Empty;

        using var reader = new StreamReader(dataStream);
        return reader.ReadToEnd();
    }
}
