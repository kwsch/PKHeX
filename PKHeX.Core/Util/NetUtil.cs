using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace PKHeX.Core;

public static class NetUtil
{
    public static string? GetStringFromURL(Uri url)
    {
        try
        {
            var stream = GetStreamFromURL(url);
            if (stream == null)
                return null;

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        // No internet?
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return null;
        }
    }

    private static Stream? GetStreamFromURL(Uri url)
    {
        // The GitHub API will fail if no user agent is provided
        using var client = new HttpClient();
        const string agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
        client.DefaultRequestHeaders.Add("User-Agent", agent);
        var response = client.GetAsync(url).Result;
        return response.IsSuccessStatusCode ? response.Content.ReadAsStream() : null;
    }
}
