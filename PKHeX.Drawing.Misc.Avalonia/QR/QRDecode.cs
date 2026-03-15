using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using PKHeX.Core;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class QRDecode
{
    private const string DecodeAPI = "http://api.qrserver.com/v1/read-qr-code/?fileurl=";

    public static QRDecodeResult GetQRData(string address, out byte[] result)
    {
        result = [];

        if (!address.StartsWith("http"))
            return QRDecodeResult.BadPath;

        string url = DecodeAPI + WebUtility.UrlEncode(address);
        string data;
        try
        {
            var str = NetUtil.GetStringFromURL(new Uri(url));
            if (str is null)
                return QRDecodeResult.BadConnection;

            data = str;
            if (data.Contains("could not find"))
                return QRDecodeResult.BadImage;

            if (data.Contains("filetype not supported"))
                return QRDecodeResult.BadType;
        }
        catch { return QRDecodeResult.BadConnection; }

        try
        {
            result = DecodeQRJson(data);
            return QRDecodeResult.Success;
        }
        catch (Exception e)
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

        string pkstr = data[intro.Length..];

        var qr = pkstr.IndexOf(qrcode, StringComparison.Ordinal);
        if (qr != -1)
            pkstr = pkstr[..qr];

        var outroIndex = pkstr.IndexOf(cap, StringComparison.Ordinal);
        if (outroIndex == -1)
            throw new FormatException();

        pkstr = pkstr[..outroIndex];

        if (!pkstr.StartsWith("http") && !pkstr.StartsWith("null"))
        {
            string fstr = Regex.Unescape(pkstr);
            byte[] raw = Encoding.Unicode.GetBytes(fstr);

            byte[] result = new byte[0xE8];
            for (int i = 0; i < result.Length; i++)
                result[i] = raw[(i + 0x30) * 2];
            return result;
        }
        pkstr = pkstr[(pkstr.IndexOf('#') + 1)..];
        pkstr = pkstr.Replace("\\", string.Empty);

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
        _ => throw new ArgumentOutOfRangeException(nameof(result), result, null),
    };
}
