using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace PKHeX.Core;

/// <summary>
/// Logic for fetching data from the internet, such as text files or JSON data.
/// </summary>
public static class NetUtil
{
    /// <summary>
    /// Retrieves the content of the specified URL as a string.
    /// </summary>
    /// <remarks>
    /// This method attempts to fetch the content of the specified URL and return it as a string.
    /// If the URL is inaccessible or an error occurs during the operation, the method returns  <see langword="null"/>.
    /// The caller should handle the possibility of a <see langword="null"/> return value.
    /// </remarks>
    /// <param name="url">The <see cref="Uri"/> of the resource to retrieve.</param>
    /// <returns>A string containing the content of the resource, or <see langword="null"/> if the resource could not be retrieved or an error occurred.</returns>
    public static string? GetStringFromURL(Uri url)
    {
        try
        {
            var stream = GetStreamFromURL(url);
            if (stream is null)
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

    // The GitHub API will fail if no user agent is provided. Use a hardcoded one to avoid issues.
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

    private static Stream? GetStreamFromURL(Uri url)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(3);
        client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        var response = client.GetAsync(url).Result;
        return response.IsSuccessStatusCode ? response.Content.ReadAsStream() : null;
    }
}
