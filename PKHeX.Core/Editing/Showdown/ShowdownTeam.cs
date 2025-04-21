using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Logic for retrieving Showdown teams from URLs.
/// </summary>
public static class ShowdownTeam
{
    /// <summary>
    /// Generates the API URL for retrieving a Showdown team based on the supplied team identifier.
    /// </summary>
    /// <param name="team">The numeric identifier of the team.</param>
    /// <returns>A string containing the full URL to access the team data via the API.</returns>
    public static string GetURL(int team) => $"https://play.pokemonshowdown.com/api/getteam?teamid={team}&raw=1";

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
        if (content == null)
            return false;

        return GetFromReply(ref content);
    }

    /// <summary>
    /// Extracts the team data from the API reply and reformats it by replacing escaped newline
    /// characters with system-specific line breaks.
    /// </summary>
    /// <param name="content">
    /// A reference to the API response string. On successful extraction, the value is replaced
    /// with the reformatted team data; otherwise, it remains unchanged.
    /// </param>
    /// <returns>
    /// <c>true</c> if the team data is successfully extracted and reformatted; otherwise, <c>false</c>.
    /// </returns>
    public static bool GetFromReply(ref string content)
    {
        // reformat
        const string startText = """
                                 "team":"
                                 """;
        var start = content.IndexOf(startText, StringComparison.Ordinal);
        if (start == -1)
            return false;
        start += startText.Length; // skip to the start of the team

        var end = content.LastIndexOf("\\n", StringComparison.Ordinal);
        if (end == -1)
            return false;
        var length = end - start;
        if (length < 5) // arbitrary length check
            return false;

        var sb = new StringBuilder();
        sb.Append(content, start, length);
        sb.Replace("\\n", Environment.NewLine);
        content = sb.ToString();
        return true;
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
        if (text.StartsWith("https://psim.us/t/") || // short link
            text.StartsWith("https://teams.pokemonshowdown.com/"))
            return TryCheckWeb(text, out url);

        if (text.StartsWith("https://play.pokemonshowdown.com/api/getteam?teamid="))
            return TryCheckAPI(text, out url);

        url = null;
        return false;
    }

    /// <summary>
    /// Attempts to extract the team identifier from a Showdown web URL and converts it to a standard API URL.
    /// </summary>
    /// <param name="text">The Showdown web URL as a read-only span of characters.</param>
    /// <param name="url">When the method returns, contains the standardized API URL if extraction is successful; otherwise, null.</param>
    /// <returns><c>true</c> if the team index is successfully extracted and converted; otherwise, <c>false</c>.</returns>
    public static bool TryCheckWeb(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? url)
    {
        url = null;
        if (!TryGetIndexWeb(text, out var team))
            return false;
        url = GetURL(team);
        return true;
    }

    /// <summary>
    /// Attempts to extract the team identifier from a Showdown API URL and returns a standardized API URL.
    /// </summary>
    /// <param name="text">The Showdown API URL as a read-only span of characters.</param>
    /// <param name="url">When the method returns, contains the standardized API URL if extraction is successful; otherwise, null.</param>
    /// <returns><c>true</c> if the team index is successfully extracted and the URL normalized; otherwise, <c>false</c>.</returns>
    public static bool TryCheckAPI(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? url)
    {
        url = null;
        if (!TryGetIndexAPI(text, out var team))
            return false;
        url = GetURL(team);
        return true;
    }

    /// <summary>
    /// Extracts the team identifier from a Showdown web URL.
    /// </summary>
    /// <param name="text">The Showdown web URL provided as a read-only span of characters.</param>
    /// <param name="team">When the method returns, contains the extracted team identifier if successful; otherwise, zero.</param>
    /// <returns><c>true</c> if the team identifier is successfully extracted; otherwise, <c>false</c>.</returns>
    public static bool TryGetIndexWeb(ReadOnlySpan<char> text, out int team)
    {
        team = 0;
        if (text.EndsWith('/'))
            text = text[..^1]; // remove trailing slash
        if (text.EndsWith("/raw"))
            text = text[..^4]; // remove trailing /raw

        int start = text.LastIndexOf('/'); // seek back to =
        if (start == -1)
            return false;

        var number = text[(start + 1)..];
        if (!int.TryParse(number, out team))
            return false;
        return true;
    }

    /// <summary>
    /// Extracts the team identifier from a Showdown API URL.
    /// </summary>
    /// <param name="text">The Showdown API URL as a read-only span of characters.</param>
    /// <param name="team">When the method returns, contains the extracted team identifier if successful; otherwise, zero.</param>
    /// <returns><c>true</c> if the team identifier is successfully extracted; otherwise, <c>false</c>.</returns>
    public static bool TryGetIndexAPI(ReadOnlySpan<char> text, out int team)
    {
        team = 0;
        if (!text.EndsWith("&raw=1"))
            return false;

        text = text[..^6];
        int start = text.LastIndexOf('='); // seek back to =
        if (start == -1)
            return false;

        var number = text[(start + 1)..];
        if (!int.TryParse(number, out team))
            return false;
        return true;
    }
}
