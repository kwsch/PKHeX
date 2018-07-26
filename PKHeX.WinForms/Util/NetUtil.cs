using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

namespace PKHeX.WinForms
{
    public static class NetUtil
    {
        public static string GetStringFromURL(string webURL)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webURL);
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
    }
}
