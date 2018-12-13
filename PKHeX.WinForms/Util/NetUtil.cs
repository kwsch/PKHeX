using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PKHeX.WinForms
{
    public static class NetUtil
    {
        private static Regex LatestGitTagRegex = new Regex("\\\"tag_name\"\\s*\\:\\s*\\\"([0-9]+\\.[0-9]+\\.[0-9]+)\\\""); // Match `"tag_name": "18.12.02"`. Group 1 is `18.12.02`
        
        public static string GetStringFromURL(string webURL)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webURL);

                // The GitHub API will fail if no user agent is provided
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var reader = new StreamReader(httpWebResponse.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static Image GetImageFromURL(string webURL)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webURL);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream stream = httpWebResponse.GetResponseStream();
                return stream != null ? Image.FromStream(stream) : null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Gets the latest version of PKHeX according to the Github API
        /// </summary>
        /// <returns>A version representing the latest available version of PKHeX, or null if the latest version could not be determined</returns>
        public static Version GetLatestPKHeXVersion()
        {
            var apiEndpoint = "https://api.github.com/repos/kwsch/pkhex/releases/latest";
            var responseJson = GetStringFromURL(apiEndpoint);
            if (string.IsNullOrEmpty(responseJson))
            {
                return null;
            }

            // Using a regex to get the tag to avoid importing an entire JSON parsing library
            var tagMatch = LatestGitTagRegex.Match(responseJson);
            if (!tagMatch.Success)
            {
                return null;
            }
            
            var tagString = tagMatch.Groups[1].Value;
            if (!Version.TryParse(tagString, out var latestVersion))
            {
                return null;
            }

            return latestVersion;
        }
    }
}
