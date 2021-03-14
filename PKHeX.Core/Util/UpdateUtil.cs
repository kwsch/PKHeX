using System;
using System.Text.RegularExpressions;

namespace PKHeX.Core
{
    public static class UpdateUtil
    {
        private static readonly Regex LatestGitTagRegex = new("\\\"tag_name\"\\s*\\:\\s*\\\"([0-9]+\\.[0-9]+\\.[0-9]+)\\\""); // Match `"tag_name": "18.12.02"`. Group 1 is `18.12.02`

        /// <summary>
        /// Gets the latest version of PKHeX according to the Github API
        /// </summary>
        /// <returns>A version representing the latest available version of PKHeX, or null if the latest version could not be determined</returns>
        public static Version? GetLatestPKHeXVersion()
        {
            const string apiEndpoint = "https://api.github.com/repos/kwsch/pkhex/releases/latest";
            var responseJson = NetUtil.GetStringFromURL(apiEndpoint);
            if (responseJson is null)
                return null;

            // Using a regex to get the tag to avoid importing an entire JSON parsing library
            var tagMatch = LatestGitTagRegex.Match(responseJson);
            if (!tagMatch.Success)
                return null;

            var tagString = tagMatch.Groups[1].Value;
            return !Version.TryParse(tagString, out var latestVersion) ? null : latestVersion;
        }
    }
}
