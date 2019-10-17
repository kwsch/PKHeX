using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using PKHeX.Core;

namespace PKHeX.Drawing
{
    public static class QRDecode
    {
        // QR Utility
        private const string DecodeAPI = "http://api.qrserver.com/v1/read-qr-code/?fileurl=";

        public static QRDecodeMsg GetQRData(string address, out byte[] result)
        {
            result = Array.Empty<byte>();
            // Fetch data from QR code...

            if (!address.StartsWith("http"))
                return QRDecodeMsg.BadPath;

            string webURL = DecodeAPI + WebUtility.UrlEncode(address);
            string data;
            try
            {
                var str = NetUtil.GetStringFromURL(webURL);
                if (str is null)
                    return QRDecodeMsg.BadConnection;

                data = str;
                if (data.Contains("could not find"))
                    return QRDecodeMsg.BadImage;

                if (data.Contains("filetype not supported"))
                    return QRDecodeMsg.BadType;
            }
            catch { return QRDecodeMsg.BadConnection; }

            // Quickly convert the json response to a data string
            try
            {
                result = DecodeQRJson(data);
                return QRDecodeMsg.Success;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return QRDecodeMsg.BadConversion;
            }
        }

        private static byte[] DecodeQRJson(string data)
        {
            const string cap = "\",\"error\":null}]}]";
            const string intro = "[{\"type\":\"qrcode\",\"symbol\":[{\"seq\":0,\"data\":\"";
            const string qrcode = "nQR-Code:";
            if (!data.StartsWith(intro))
                throw new FormatException();

            string pkstr = data.Substring(intro.Length);
            if (pkstr.Contains(qrcode)) // Remove multiple QR codes in same image
                pkstr = pkstr.Substring(0, pkstr.IndexOf(qrcode, StringComparison.Ordinal));
            pkstr = pkstr.Substring(0, pkstr.IndexOf(cap, StringComparison.Ordinal)); // Trim outro

            if (!pkstr.StartsWith("http") && !pkstr.StartsWith("null")) // G7
            {
                string fstr = Regex.Unescape(pkstr);
                byte[] raw = Encoding.Unicode.GetBytes(fstr);
                // Remove 00 interstitials and retrieve from offset 0x30, take PK7 Stored Size (always)
                return raw.Where((_, i) => i % 2 == 0).Skip(0x30).Take(0xE8).ToArray();
            }
            // All except G7
            pkstr = pkstr.Substring(pkstr.IndexOf('#') + 1); // Trim URL
            pkstr = pkstr.Replace("\\", string.Empty); // Rectify response

            return Convert.FromBase64String(pkstr);
        }

        public static string ConvertMsg(this QRDecodeMsg msg)
        {
            return msg switch
            {
                QRDecodeMsg.Success => string.Empty,
                QRDecodeMsg.BadPath => MessageStrings.MsgQRUrlFailPath,
                QRDecodeMsg.BadImage => MessageStrings.MsgQRUrlFailImage,
                QRDecodeMsg.BadType => MessageStrings.MsgQRUrlFailType,
                QRDecodeMsg.BadConnection => MessageStrings.MsgQRUrlFailConnection,
                QRDecodeMsg.BadConversion => MessageStrings.MsgQRUrlFailConvert,
                _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
            };
        }
    }

    public enum QRDecodeMsg
    {
        Success,
        BadPath,
        BadImage,
        BadType,
        BadConnection,
        BadConversion,
    }
}
