using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// QR Message reading &amp; writing logic
    /// </summary>
    public static class QRMessageUtil
    {
        private const string QR6PathBad = "null/#";
        private const string QR6Path = "http://lunarcookies.github.io/b1s1.html#";
        private const string QR6PathWC = "http://lunarcookies.github.io/wc.html#";
        private static string GetExploitURLPrefixPKM(int format) => format == 6 ? QR6Path : QR6PathBad;
        private static string GetExploitURLPrefixWC(int format) => format == 6 ? QR6PathWC : QR6PathBad;

        /// <summary>
        /// Gets the <see cref="PKM"/> data from the message that is encoded in a QR.
        /// </summary>
        /// <param name="message">QR Message</param>
        /// <param name="format">Preferred <see cref="PKM.Format"/> to expect.</param>
        /// <returns>Decoded <see cref="PKM"/> object, null if invalid.</returns>
        public static PKM? GetPKM(string message, int format)
        {
            var data = DecodeMessagePKM(message);
            if (data == null)
                return null;
            return PKMConverter.GetPKMfromBytes(data, format);
        }

        /// <summary>
        /// Gets a QR Message from the input <see cref="PKM"/> data.
        /// </summary>
        /// <param name="pkm">Pokémon to encode</param>
        /// <returns>QR Message</returns>
        public static string GetMessage(PKM pkm)
        {
            if (pkm is PK7 pk7)
            {
                byte[] payload = QR7.GenerateQRData(pk7);
                return GetMessage(payload);
            }

            var server = GetExploitURLPrefixPKM(pkm.Format);
            var data = pkm.EncryptedBoxData;
            return GetMessageBase64(data, server);
        }

        /// <summary>
        /// Gets a QR Message from the input <see cref="byte"/> data.
        /// </summary>
        /// <param name="payload">Data to encode</param>
        /// <returns>QR Message</returns>
        public static string GetMessage(byte[] payload) => string.Concat(payload.Select(z => (char) z));

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

        public static string GetMessageBase64(byte[] data, string server)
        {
            string payload = Convert.ToBase64String(data);
            return server + payload;
        }

        private static byte[]? DecodeMessagePKM(string message)
        {
            if (message.Length < 32) // arbitrary length check; everything should be greater than this
                return null;
            if (message.StartsWith(QR6PathBad)) // fake url
                return DecodeMessageDataBase64(message);
            if (message.StartsWith("http")) // inject url
                return DecodeMessageDataBase64(message);
            if (message.StartsWith("POKE") && message.Length > 0x30 + 0xE8) // G7 data
                return GetBytesFromMessage(message, 0x30, 0xE8);
            return null;
        }

        private static byte[]? DecodeMessageDataBase64(string url)
        {
            try
            {
                int payloadBegin = url.IndexOf('#');
                if (payloadBegin < 0) // bad URL, need the payload separator
                    return null;
                url = url[(payloadBegin + 1)..]; // Trim URL to right after #
                return Convert.FromBase64String(url);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        private static byte[] GetBytesFromMessage(string seed, int skip, int take)
        {
            byte[] data = new byte[take];
            for (int i = 0; i < take; i++)
                data[i] = (byte)seed[i + skip];
            return data;
        }
    }
}