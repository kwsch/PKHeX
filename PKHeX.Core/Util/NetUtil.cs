using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace PKHeX.Core
{
    public static class NetUtil
    {
        public static string? GetStringFromURL(string url)
        {
            try
            {
                var stream = GetStreamFromURL(url);
                if (stream == null)
                    return null;

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

        private static Stream? GetStreamFromURL(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            // The GitHub API will fail if no user agent is provided
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            var httpWebResponse = httpWebRequest.GetResponse();
            return httpWebResponse.GetResponseStream();
        }
    }
}
