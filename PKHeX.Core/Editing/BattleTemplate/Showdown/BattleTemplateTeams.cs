using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Logic for retrieving teams from URLs.
/// </summary>
public static class BattleTemplateTeams
{
    /// <summary>
    /// Tries to check if the input text is a valid URL for a team, and if so, retrieves the team data.
    /// </summary>
    /// <param name="text">The input text to check.</param>
    /// <param name="content">When the method returns, contains the retrieved team data if the text is a valid URL; otherwise, null.</param>
    /// <returns><see langword="true"/> if the text is a valid URL and the team data was successfully retrieved; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetSetLines(string text, [NotNullWhen(true)] out string? content)
    {
        if (ShowdownTeam.IsURL(text, out var url))
            return ShowdownTeam.TryGetSets(url, out content);
        if (PokepasteTeam.IsURL(text, out url))
            return PokepasteTeam.TryGetSets(url, out content);
        content = text;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve sets from the provided text. If the text is a valid URL, it retrieves the team data from the URL.
    /// </summary>
    /// <param name="text">The input text to check.</param>
    /// <returns>An enumerable collection of <see cref="ShowdownSet"/> objects representing the sets.</returns>
    public static IEnumerable<ShowdownSet> TryGetSets(string text)
    {
        var ingest = TryGetSetLines(text, out var many) ? many : text;
        return ShowdownParsing.GetShowdownSets(ingest);
    }
}
