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

        public static QRDecodeResult GetQRData(string address, out byte[] result)
        {
            result = Array.Empty<byte>();
            // Fetch data from QR code...

            if (!address.StartsWith("http"))
                return QRDecodeResult.BadPath;

            string url = DecodeAPI + WebUtility.UrlEncode(address);
            string data;
            try
            {
                var str = NetUtil.GetStringFromURL(url);
                if (str is null)
                    return QRDecodeResult.BadConnection;

                data = str;
                if (data.Contains("could not find"))
                    return QRDecodeResult.BadImage;

                if (data.Contains("filetype not supported"))
                    return QRDecodeResult.BadType;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { return QRDecodeResult.BadConnection; }
#pragma warning restore CA1031 // Do not catch general exception types

            // Quickly convert the json response to a data string
            try
            {
                result = DecodeQRJson(data);
                return QRDecodeResult.Success;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Debug.WriteLine(e.Message);
                return QRDecodeResult.BadConversion;
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

        public static string ConvertMsg(this QRDecodeResult result) => result switch
        {
            QRDecodeResult.Success => string.Empty,
            QRDecodeResult.BadPath => MessageStrings.MsgQRUrlFailPath,
            QRDecodeResult.BadImage => MessageStrings.MsgQRUrlFailImage,
            QRDecodeResult.BadType => MessageStrings.MsgQRUrlFailType,
            QRDecodeResult.BadConnection => MessageStrings.MsgQRUrlFailConnection,
            QRDecodeResult.BadConversion => MessageStrings.MsgQRUrlFailConvert,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }
}
