using System;
using System.Buffers;

namespace PKHeX.Core;

/// <summary>
/// QR Message reading &amp; writing logic
/// </summary>
public static class QRMessageUtil
{
    private const string QR6PathBad = "null/#";
    private const string QR6Path = "http://lunarcookies.github.io/b1s1.html#";
    private const string QR6PathWC = "http://lunarcookies.github.io/wc.html#";
    private static string GetExploitURLPrefixPKM(byte format) => format == 6 ? QR6Path : QR6PathBad;
    private static string GetExploitURLPrefixWC(byte format) => format == 6 ? QR6PathWC : QR6PathBad;

    /// <summary>
    /// Gets the <see cref="PKM"/> data from the message that is encoded in a QR.
    /// </summary>
    /// <param name="message">QR Message</param>
    /// <param name="context">Preferred <see cref="PKM.Context"/> to expect.</param>
    /// <returns>Decoded <see cref="PKM"/> object, null if invalid.</returns>
    public static PKM? GetPKM(ReadOnlySpan<char> message, EntityContext context)
    {
        var data = DecodeMessagePKM(message);
        if (data == null)
            return null;
        return EntityFormat.GetFromBytes(data, context);
    }

    /// <summary>
    /// Gets a QR Message from the input <see cref="PKM"/> data.
    /// </summary>
    /// <param name="pk">Pokémon to encode</param>
    /// <returns>QR Message</returns>
    public static string GetMessage(PKM pk)
    {
        if (pk is PK7 pk7)
        {
            Span<byte> payload = stackalloc byte[QR7.SIZE];
            QR7.SetQRData(pk7, payload);
            return GetMessage(payload);
        }

        var server = GetExploitURLPrefixPKM(pk.Format);
        var data = pk.EncryptedBoxData;
        return GetMessageBase64(data, server);
    }

    /// <summary>
    /// Gets a QR Message from the input <see cref="byte"/> data.
    /// </summary>
    /// <param name="payload">Data to encode</param>
    /// <returns>QR Message</returns>
    public static string GetMessage(ReadOnlySpan<byte> payload)
    {
        Span<char> result = stackalloc char[payload.Length];
        for (int i = 0; i < payload.Length; i++)
            result[i] = (char)payload[i];
        return new string(result);
    }

    /// <summary>
    /// Gets a QR Message from the input <see cref="MysteryGift"/> data.
    /// </summary>
    /// <param name="mg">Gift data to encode</param>
    /// <returns>QR Message</returns>
    public static string GetMessage(DataMysteryGift mg)
    {
        var server = GetExploitURLPrefixWC(mg.Generation);
        var data = mg.Write();
        return GetMessageBase64(data, server);
    }

    /// <summary>
    /// Gets a QR Message from the input <see cref="byte"/> data.
    /// </summary>
    /// <param name="data">Data to include in the payload</param>
    /// <param name="server">Website URL that sources the payload</param>
    /// <returns>QR Message</returns>
    public static string GetMessageBase64(ReadOnlySpan<byte> data, string server)
    {
        string payload = Convert.ToBase64String(data);
        return server + payload;
    }

    private static byte[]? DecodeMessagePKM(ReadOnlySpan<char> message)
    {
        if (message.Length < 32) // arbitrary length check; everything should be greater than this
            return null;
        if (message.StartsWith(QR6PathBad, StringComparison.Ordinal)) // fake url
            return DecodeMessageDataBase64(message);
        if (message.StartsWith("http", StringComparison.Ordinal)) // inject url
            return DecodeMessageDataBase64(message);

        const int g7size = 0xE8;
        const int g7intro = 0x30;
        if (message.StartsWith("POKE", StringComparison.Ordinal) && message.Length > g7intro + g7size) // G7 data
            return GetBytesFromMessage(message[g7intro..], g7size);
        return null;
    }

    private static byte[]? DecodeMessageDataBase64(ReadOnlySpan<char> url)
    {
        int payloadBegin = url.IndexOf('#');
        if (payloadBegin == -1)
            return null; // bad URL, need the payload separator
        if (payloadBegin == url.Length - 1)
            return null; // no payload

        url = url[(payloadBegin + 1)..]; // Trim URL to right after #
        // Decode unicode string -- size might be pretty big (user input), so just rent instead of stackalloc
        var tmp = ArrayPool<byte>.Shared.Rent(url.Length);
        var result = Convert.TryFromBase64Chars(url, tmp, out int bytesWritten) ? tmp[..bytesWritten] : null;
        ArrayPool<byte>.Shared.Return(tmp, true);
        return result;
    }

    private static byte[] GetBytesFromMessage(ReadOnlySpan<char> input, int count)
    {
        byte[] data = new byte[count];
        for (int i = data.Length - 1; i >= 0; i--)
            data[i] = (byte)input[i];
        return data;
    }
}
