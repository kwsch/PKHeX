using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PKHeX.Core;

/// <summary>
/// Logic for retrieving Showdown teams from URLs.
/// </summary>
/// <remarks>
/// <see href="https://pokepast.es/"/>
/// </remarks>
public static class PokepasteTeam
{
    /// <summary>
    /// Generates the raw URL for retrieving a team based on the supplied team identifier.
    /// </summary>
    /// <param name="team">The numeric identifier of the team.</param>
    /// <returns>A string containing the full URL to access the team data.</returns>
    public static string GetURL(ulong team) => $"https://pokepast.es/{team:x16}/raw";

    /// <inheritdoc cref="GetURL"/>
    /// <remarks>For legacy team indexes (first 255 or so), shouldn't ever be triggered non-test team indexes.</remarks>
    public static string GetURLOld(int team) => $"https://pokepast.es/{team}/raw";

    /// <summary>
    /// Attempts to retrieve the Showdown team data from a specified URL, and reformats it.
    /// </summary>
    /// <param name="url">The URL to retrieve the team data from.</param>
    /// <param name="content">When the method returns, contains the processed team data if retrieval and formatting succeed; otherwise, null.</param>
    /// <returns><c>true</c> if the team data is successfully retrieved and reformatted; otherwise, <c>false</c>.</returns>
    public static bool TryGetSets(string url, [NotNullWhen(true)] out string? content)
    {
        content = null;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            return false;

        content = NetUtil.GetStringFromURL(uriResult);
        return content != null;
    }

    /// <summary>
    /// Determines if the provided text is a valid Showdown team URL. If valid, returns a normalized API URL.
    /// </summary>
    /// <param name="text">The text to evaluate.</param>
    /// <param name="url">When the method returns, contains the normalized API URL if the text represents a valid Showdown team URL; otherwise, null.</param>
    /// <returns><c>true</c> if the text is a valid Showdown team URL; otherwise, <c>false</c>.</returns>
    public static bool IsURL(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? url)
    {
        text = text.Trim();
        url = null;
        if (!text.StartsWith("https://pokepast.es/")) // short link
            return false;

        return TryCheckWeb(text, out url);
    }

    /// <summary>
    /// Attempts to extract the team identifier from a Showdown web URL and converts it to a standard API URL.
    /// </summary>
    /// <param name="text">The Showdown web URL as a read-only span of characters.</param>
    /// <param name="url">When the method returns, contains the standardized API URL if extraction is successful; otherwise, null.</param>
    /// <returns><c>true</c> if the team index is successfully extracted and converted; otherwise, <c>false</c>.</returns>
    public static bool TryCheckWeb(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? url)
    {
        // if ends with `/`, remove.
        if (text.EndsWith('/'))
            text = text[..^1]; // remove trailing slash
        // if ends with `/raw`, remove.
        if (text.EndsWith("/raw"))
            text = text[..^4]; // remove trailing /raw

        url = null;

        // seek back to `/`
        int start = text.LastIndexOf('/'); // seek back to /
        if (start == -1)
            return false;

        // get the substring after
        var number = text[(start + 1)..];
        switch (number.Length)
        {
            case 16 when ulong.TryParse(number, NumberStyles.HexNumber, null, out var hash):
                url = GetURL(hash);
                return true;
            case <= 8 when int.TryParse(number, out var team):
                url = GetURLOld(team);
                return true;
            default:
                return false;
        }
    }
}
