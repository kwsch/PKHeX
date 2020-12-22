using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PKHeX.Core
{
    public static class NetUtil
    {
        public static string? GetStringFromURL(string url)
        {
            try
            {
                var stream = GetStreamFromURL(url);
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // No internet?
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        private static Stream GetStreamFromURL(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            // The GitHub API will fail if no user agent is provided
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            var httpWebResponse = httpWebRequest.GetResponse();
            return httpWebResponse.GetResponseStream();
        }

        private static readonly Regex LatestGitTagRegex = new("\\\"tag_name\"\\s*\\:\\s*\\\"([0-9]+\\.[0-9]+\\.[0-9]+)\\\""); // Match `"tag_name": "18.12.02"`. Group 1 is `18.12.02`

        /// <summary>
        /// Gets the latest version of PKHeX according to the Github API
        /// </summary>
        /// <returns>A version representing the latest available version of PKHeX, or null if the latest version could not be determined</returns>
        public static Version? GetLatestPKHeXVersion()
        {
            const string apiEndpoint = "https://api.github.com/repos/kwsch/pkhex/releases/latest";
            var responseJson = GetStringFromURL(apiEndpoint);
            if (string.IsNullOrEmpty(responseJson))
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
