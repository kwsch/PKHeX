using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for converting a <see cref="string"/> for Generation 1 &amp; 2 games.
    /// </summary>
    public static class StringConverter12
    {
        public static bool GetIsG1Japanese(string str) => str.All(z => U2RBY_J.ContainsKey(z));
        public static bool GetIsG1English(string str) => str.All(z => U2RBY_U.ContainsKey(z));

        public const char G1Terminator = '\0';
        public const byte G1TradeOTCode = 0x5D;
        public const char G1TradeOT = '*';
        public const string G1TradeOTStr = "*";

        /// <summary>
        /// Checks if the input byte array is definitely of German origin (any ÄÖÜäöü)
        /// </summary>
        /// <param name="data">Raw string bytes</param>
        /// <returns>Indication if the data is from a definitely-german string</returns>
        public static bool IsG12German(IEnumerable<byte> data) => data.Any(z => z >= 0xC0 && z <= 0xC6);

        /// <summary>
        /// Checks if the input byte array is definitely of German origin (any ÄÖÜäöü)
        /// </summary>
        /// <param name="data">Input string</param>
        /// <returns>Indication if the data is from a definitely-german string</returns>
        public static bool IsG12German(string data) => IsG12German(SetString1(data, data.Length, false));

        /// <summary>
        /// Converts Generation 1 encoded data into a string.
        /// </summary>
        /// <param name="strdata">Encoded data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count"></param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString1(byte[] strdata, int offset, int count, bool jp)
        {
            var dict = jp ? RBY2U_J : RBY2U_U;

            var s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                var val = strdata[offset + i];
                if (!dict.TryGetValue(val, out var c)) // Take valid values
                    break;
                if (c == '\0') // Stop if Terminator
                    break;
                s.Append(c);
            }
            return StringConverter.SanitizeString(s.ToString());
        }

        /// <summary>
        /// Converts Generation 1 encoded data the same way Bank converts.
        /// </summary>
        /// <param name="strdata">Generation 1 encoded data.</param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string GetG1ConvertedString(byte[] strdata, bool jp)
        {
            var table = jp ? jp_table : us_table;
            return string.Concat(strdata.TakeWhile(b => b != 0).Select(b => (char)table[b]).TakeWhile(b => b != 0));
        }

        /// <summary>
        /// Converts a string to Generation 1 encoded data.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="jp">Data destination is Japanese.</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString1(string value, int maxLength, bool jp, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap

            var dict = jp ? U2RBY_J : U2RBY_U;
            if (value.StartsWith(G1TradeOTStr)) // Handle "[TRAINER]"
                return new[] { dict[G1TradeOT], dict[G1Terminator] };

            var arr = new List<byte>(padTo);
            foreach (char c in value)
            {
                if (!dict.TryGetValue(c, out byte val))
                    break;
                arr.Add(val);
            }
            var term = dict[G1Terminator]; // terminator
            arr.Add(term);
            while (arr.Count < padTo)
                arr.Add((byte)padWith);
            return arr.ToArray();
        }

        /// <summary>
        /// Converts Generation 1 encoded character to string.
        /// </summary>
        /// <param name="key">Encoded character.</param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string GetG1Char(byte key, bool jp)
        {
            var dict = jp ? RBY2U_J : RBY2U_U;
            if (dict.TryGetValue(key, out var val))
                return val.ToString();
            return string.Empty;
        }

        #region Gen 1/2 Character Tables
        /// <summary>
        /// International 1->7 character translation table
        /// </summary>
        private static readonly ushort[] us_table =
        {
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 0
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 1
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 2
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 3
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 4
            0x0000, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 5
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 6
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // 7
            0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, // 8
            0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x0028, 0x0029, 0x003A, 0x003B, 0x0028, 0x0029, // 9
            0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, // A
            0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // B
            0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // C
            0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // D
            0x0020, 0x0050, 0x004D, 0x002D, 0x0020, 0x0020, 0x003F, 0x0021, 0x002D, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0xE08E, // E
            0x0020, 0x0078, 0x002E, 0x002F, 0x002C, 0xE08F, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, // F
        };

        /// <summary>
        /// Japanese 1->7 character translation table
        /// </summary>
        private static readonly ushort[] jp_table =
        {
            0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30AC, 0x30AE, 0x30B0, 0x30B2, 0x30B4, 0x30B6, 0x30B8, 0x30BA, 0x30BC, 0x30BE, 0x30C0, // 0
            0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30D0, 0x30D3, 0x30D6, 0x30DC, 0x3000, 0x3000, 0x3000, // 1
            0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x304C, 0x304E, 0x3050, 0x3052, 0x3054, 0x3056, 0x3058, 0x305A, 0x305C, 0x305E, // 2
            0x3060, 0x3062, 0x3065, 0x3067, 0x3069, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3070, 0x3073, 0x3076, 0x30D9, 0x307C, 0x3000, // 3
            0x30D1, 0x30D4, 0x30D7, 0x30DD, 0x3071, 0x3074, 0x3077, 0x30DA, 0x307D, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 4
            0x0000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 5
            0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 6
            0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // 7
            0x30A2, 0x30A4, 0x30A6, 0x30A8, 0x30AA, 0x30AB, 0x30AD, 0x30AF, 0x30B1, 0x30B3, 0x30B5, 0x30B7, 0x30B9, 0x30BB, 0x30BD, 0x30BF, // 8
            0x30C1, 0x30C4, 0x30C6, 0x30C8, 0x30CA, 0x30CB, 0x30CC, 0x30CD, 0x30CE, 0x30CF, 0x30D2, 0x30D5, 0x30DB, 0x30DE, 0x30DF, 0x30E0, // 9
            0x30E1, 0x30E2, 0x30E4, 0x30E6, 0x30E8, 0x30E9, 0x30EB, 0x30EC, 0x30ED, 0x30EF, 0x30F2, 0x30F3, 0x30C3, 0x30E3, 0x30E5, 0x30E7, // A
            0x30A3, 0x3042, 0x3044, 0x3046, 0x3048, 0x304A, 0x304B, 0x304D, 0x304F, 0x3051, 0x3053, 0x3055, 0x3057, 0x3059, 0x305B, 0x305D, // B
            0x305F, 0x3061, 0x3064, 0x3066, 0x3068, 0x306A, 0x306B, 0x306C, 0x306D, 0x306E, 0x306F, 0x3072, 0x3075, 0x30D8, 0x307B, 0x307E, // C
            0x307F, 0x3080, 0x3081, 0x3082, 0x3084, 0x3086, 0x3088, 0x3089, 0x30EA, 0x308B, 0x308C, 0x308D, 0x308F, 0x3092, 0x3093, 0x3063, // D
            0x3083, 0x3085, 0x3087, 0x30FC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A1, 0x30A5, 0x30A7, 0x3000, 0x3000, 0x3000, 0x2642, // E
            0x3000, 0x3000, 0x3000, 0x3000, 0x30A9, 0x2640, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, // F
        };

        internal static readonly Dictionary<byte, char> RBY2U_U = new Dictionary<byte, char>{
            {0x50, G1Terminator},
            {0x5D, G1TradeOT},
            {0x7F, ' '},
            {0x80, 'A'},
            {0x81, 'B'},
            {0x82, 'C'},
            {0x83, 'D'},
            {0x84, 'E'},
            {0x85, 'F'},
            {0x86, 'G'},
            {0x87, 'H'},
            {0x88, 'I'},
            {0x89, 'J'},
            {0x8A, 'K'},
            {0x8B, 'L'},
            {0x8C, 'M'},
            {0x8D, 'N'},
            {0x8E, 'O'},
            {0x8F, 'P'},
            {0x90, 'Q'},
            {0x91, 'R'},
            {0x92, 'S'},
            {0x93, 'T'},
            {0x94, 'U'},
            {0x95, 'V'},
            {0x96, 'W'},
            {0x97, 'X'},
            {0x98, 'Y'},
            {0x99, 'Z'},
            {0x9A, '('},
            {0x9B, ')'},
            {0x9C, ':'},
            {0x9D, ';'},
            {0x9E, '['},
            {0x9F, ']'},
            {0xA0, 'a'},
            {0xA1, 'b'},
            {0xA2, 'c'},
            {0xA3, 'd'},
            {0xA4, 'e'},
            {0xA5, 'f'},
            {0xA6, 'g'},
            {0xA7, 'h'},
            {0xA8, 'i'},
            {0xA9, 'j'},
            {0xAA, 'k'},
            {0xAB, 'l'},
            {0xAC, 'm'},
            {0xAD, 'n'},
            {0xAE, 'o'},
            {0xAF, 'p'},
            {0xB0, 'q'},
            {0xB1, 'r'},
            {0xB2, 's'},
            {0xB3, 't'},
            {0xB4, 'u'},
            {0xB5, 'v'},
            {0xB6, 'w'},
            {0xB7, 'x'},
            {0xB8, 'y'},
            {0xB9, 'z'},

            // unused characters
            {0xBA, 'à'},
            {0xBB, 'è'},
            {0xBC, 'é'},
            {0xBD, 'ù'},
            {0xBE, 'À'},
            {0xBF, 'Á'}, // Used in Spanish FALCÁN in-game trade, inaccessible from keyboard

            {0xC0, 'Ä'},
            {0xC1, 'Ö'},
            {0xC2, 'Ü'},
            {0xC3, 'ä'},
            {0xC4, 'ö'},
            {0xC5, 'ü'},

            // unused characters
            {0xC6, 'È'},
            {0xC7, 'É'},
            {0xC8, 'Ì'},
            {0xC9, 'Í'}, // Used in Spanish MANÍA in-game trade, inaccessible from keyboard
            {0xCA, 'Ñ'},
            {0xCB, 'Ò'},
            {0xCC, 'Ó'},
            {0xCD, 'Ù'},
            {0xCE, 'Ú'},
            {0xCF, 'á'},
            {0xD0, 'ì'},
            {0xD1, 'í'},
            {0xD2, 'ñ'},
            {0xD3, 'ò'},
            {0xD4, 'ó'},
            {0xD5, 'ú'},

            {0xE0, '’'},
            {0xE1, '{'}, /* Pk */
            {0xE2, '}'}, /* Mn */
            {0xE3, '-'},
            {0xE6, '?'},
            {0xE7, '!'},
            {0xE8, '.'}, // Alias decimal point to .
            {0xEF, '♂'},
            {0xF1, '×'},
            {0xF2, '.'},
            {0xF3, '/'},
            {0xF4, ','},
            {0xF5, '♀'},
            {0xF6, '0'},
            {0xF7, '1'},
            {0xF8, '2'},
            {0xF9, '3'},
            {0xFA, '4'},
            {0xFB, '5'},
            {0xFC, '6'},
            {0xFD, '7'},
            {0xFE, '8'},
            {0xFF, '9'}
        };

        private static readonly Dictionary<byte, char> RBY2U_J = new Dictionary<byte, char> {
            {0x05, 'ガ'},
            {0x06, 'ギ'},
            {0x07, 'グ'},
            {0x08, 'ゲ'},
            {0x09, 'ゴ'},
            {0x0A, 'ザ'},
            {0x0B, 'ジ'},
            {0x0C, 'ズ'},
            {0x0D, 'ゼ'},
            {0x0E, 'ゾ'},
            {0x0F, 'ダ'},
            {0x10, 'ヂ'},
            {0x11, 'ヅ'},
            {0x12, 'デ'},
            {0x13, 'ド'},
            {0x19, 'バ'},
            {0x1A, 'ビ'},
            {0x1B, 'ブ'},
            {0x1C, 'ボ'},
            {0x26, 'が'},
            {0x27, 'ぎ'},
            {0x28, 'ぐ'},
            {0x29, 'げ'},
            {0x2A, 'ご'},
            {0x2B, 'ざ'},
            {0x2C, 'じ'},
            {0x2D, 'ず'},
            {0x2E, 'ぜ'},
            {0x2F, 'ぞ'},
            {0x30, 'だ'},
            {0x31, 'ぢ'},
            {0x32, 'づ'},
            {0x33, 'で'},
            {0x34, 'ど'},
            {0x3A, 'ば'},
            {0x3B, 'び'},
            {0x3C, 'ぶ'},
            {0x3D, 'ベ'},
            {0x3E, 'ぼ'},
            {0x40, 'パ'},
            {0x41, 'ピ'},
            {0x42, 'プ'},
            {0x43, 'ポ'},
            {0x44, 'ぱ'},
            {0x45, 'ぴ'},
            {0x46, 'ぷ'},
            {0x47, 'ペ'},
            {0x48, 'ぽ'},
            {0x50, G1Terminator},
            {0x5D, G1TradeOT},
            {0x7F, ' '},
            {0x80, 'ア'},
            {0x81, 'イ'},
            {0x82, 'ウ'},
            {0x83, 'エ'},
            {0x84, 'オ'},
            {0x85, 'カ'},
            {0x86, 'キ'},
            {0x87, 'ク'},
            {0x88, 'ケ'},
            {0x89, 'コ'},
            {0x8A, 'サ'},
            {0x8B, 'シ'},
            {0x8C, 'ス'},
            {0x8D, 'セ'},
            {0x8E, 'ソ'},
            {0x8F, 'タ'},
            {0x90, 'チ'},
            {0x91, 'ツ'},
            {0x92, 'テ'},
            {0x93, 'ト'},
            {0x94, 'ナ'},
            {0x95, 'ニ'},
            {0x96, 'ヌ'},
            {0x97, 'ネ'},
            {0x98, 'ノ'},
            {0x99, 'ハ'},
            {0x9A, 'ヒ'},
            {0x9B, 'フ'},
            {0x9C, 'ホ'},
            {0x9D, 'マ'},
            {0x9E, 'ミ'},
            {0x9F, 'ム'},
            {0xA0, 'メ'},
            {0xA1, 'モ'},
            {0xA2, 'ヤ'},
            {0xA3, 'ユ'},
            {0xA4, 'ヨ'},
            {0xA5, 'ラ'},
            {0xA6, 'ル'},
            {0xA7, 'レ'},
            {0xA8, 'ロ'},
            {0xA9, 'ワ'},
            {0xAA, 'ヲ'},
            {0xAB, 'ン'},
            {0xAC, 'ッ'},
            {0xAD, 'ャ'},
            {0xAE, 'ュ'},
            {0xAF, 'ョ'},
            {0xB0, 'ィ'},
            {0xB1, 'あ'},
            {0xB2, 'い'},
            {0xB3, 'う'},
            {0xB4, 'え'},
            {0xB5, 'お'},
            {0xB6, 'か'},
            {0xB7, 'き'},
            {0xB8, 'く'},
            {0xB9, 'け'},
            {0xBA, 'こ'},
            {0xBB, 'さ'},
            {0xBC, 'し'},
            {0xBD, 'す'},
            {0xBE, 'せ'},
            {0xBF, 'そ'},
            {0xC0, 'た'},
            {0xC1, 'ち'},
            {0xC2, 'つ'},
            {0xC3, 'て'},
            {0xC4, 'と'},
            {0xC5, 'な'},
            {0xC6, 'に'},
            {0xC7, 'ぬ'},
            {0xC8, 'ね'},
            {0xC9, 'の'},
            {0xCA, 'は'},
            {0xCB, 'ひ'},
            {0xCC, 'ふ'},
            {0xCD, 'へ'},
            {0xCE, 'ほ'},
            {0xCF, 'ま'},
            {0xD0, 'み'},
            {0xD1, 'む'},
            {0xD2, 'め'},
            {0xD3, 'も'},
            {0xD4, 'や'},
            {0xD5, 'ゆ'},
            {0xD6, 'よ'},
            {0xD7, 'ら'},
            {0xD8, 'リ'},
            {0xD9, 'る'},
            {0xDA, 'れ'},
            {0xDB, 'ろ'},
            {0xDC, 'わ'},
            {0xDD, 'を'},
            {0xDE, 'ん'},
            {0xDF, 'っ'},
            {0xE0, 'ゃ'},
            {0xE1, 'ゅ'},
            {0xE2, 'ょ'},
            {0xE3, 'ー'},
            {0xE6, '?'},
            {0xE7, '!'},
            {0xE9, 'ァ'},
            {0xEA, 'ゥ'},
            {0xEB, 'ェ'},
            {0xEF, '♂'},
            {0xF4, 'ォ'},
            {0xF5, '♀'},
            {0xF6, '0'},
            {0xF7, '1'},
            {0xF8, '2'},
            {0xF9, '3'},
            {0xFA, '4'},
            {0xFB, '5'},
            {0xFC, '6'},
            {0xFD, '7'},
            {0xFE, '8'},
            {0xFF, '9'}
        };

        internal static readonly Dictionary<char, byte> U2RBY_U = new Dictionary<char, byte> {
            {G1Terminator, 0x50},
            {G1TradeOT, 0x5D}, // TRAINER (Localized per ROM)
            {' ', 0x7F},
            {'A', 0x80},
            {'B', 0x81},
            {'C', 0x82},
            {'D', 0x83},
            {'E', 0x84},
            {'F', 0x85},
            {'G', 0x86},
            {'H', 0x87},
            {'I', 0x88},
            {'J', 0x89},
            {'K', 0x8A},
            {'L', 0x8B},
            {'M', 0x8C},
            {'N', 0x8D},
            {'O', 0x8E},
            {'P', 0x8F},
            {'Q', 0x90},
            {'R', 0x91},
            {'S', 0x92},
            {'T', 0x93},
            {'U', 0x94},
            {'V', 0x95},
            {'W', 0x96},
            {'X', 0x97},
            {'Y', 0x98},
            {'Z', 0x99},
            {'(', 0x9A},
            {')', 0x9B},
            {':', 0x9C},
            {';', 0x9D},
            {'[', 0x9E},
            {']', 0x9F},
            {'a', 0xA0},
            {'b', 0xA1},
            {'c', 0xA2},
            {'d', 0xA3},
            {'e', 0xA4},
            {'f', 0xA5},
            {'g', 0xA6},
            {'h', 0xA7},
            {'i', 0xA8},
            {'j', 0xA9},
            {'k', 0xAA},
            {'l', 0xAB},
            {'m', 0xAC},
            {'n', 0xAD},
            {'o', 0xAE},
            {'p', 0xAF},
            {'q', 0xB0},
            {'r', 0xB1},
            {'s', 0xB2},
            {'t', 0xB3},
            {'u', 0xB4},
            {'v', 0xB5},
            {'w', 0xB6},
            {'x', 0xB7},
            {'y', 0xB8},
            {'z', 0xB9},

            // unused characters
            {'à', 0xBA},
            {'è', 0xBB},
            {'é', 0xBC},
            {'ù', 0xBD},
            {'À', 0xBE},
            {'Á', 0xBF}, // Used in Spanish FALCÁN in-game trade, inaccessible from keyboard

            {'Ä', 0xC0},
            {'Ö', 0xC1},
            {'Ü', 0xC2},
            {'ä', 0xC3},
            {'ö', 0xC4},
            {'ü', 0xC5},

            // unused characters
            {'È', 0xC6},
            {'É', 0xC7},
            {'Ì', 0xC8},
            {'Í', 0xC9}, // Used in Spanish MANÍA in-game trade, inaccessible from keyboard
            {'Ñ', 0xCA},
            {'Ò', 0xCB},
            {'Ó', 0xCC},
            {'Ù', 0xCD},
            {'Ú', 0xCE},
            {'á', 0xCF},
            {'ì', 0xD0},
            {'í', 0xD1},
            {'ñ', 0xD2},
            {'ò', 0xD3},
            {'ó', 0xD4},
            {'ú', 0xD5},

            {'\'', 0xE0}, // Alias ' to ’ for Farfetch'd
            {'’', 0xE0},
            {'{', 0xE1}, /* Pk */
            {'}', 0xE2}, /* Mn */
            {'-', 0xE3},
            {'?', 0xE6},
            {'!', 0xE7},
            {'♂', 0xEF},
            {'×', 0xF1},
            {'.', 0xF2},
            {'/', 0xF3},
            {',', 0xF4},
            {'♀', 0xF5},
            {'0', 0xF6},
            {'1', 0xF7},
            {'2', 0xF8},
            {'3', 0xF9},
            {'4', 0xFA},
            {'5', 0xFB},
            {'6', 0xFC},
            {'7', 0xFD},
            {'8', 0xFE},
            {'9', 0xFF}
        };

        private static readonly Dictionary<char, byte> U2RBY_J = new Dictionary<char, byte> {
            {'ガ', 0x05},
            {'ギ', 0x06},
            {'グ', 0x07},
            {'ゲ', 0x08},
            {'ゴ', 0x09},
            {'ザ', 0x0A},
            {'ジ', 0x0B},
            {'ズ', 0x0C},
            {'ゼ', 0x0D},
            {'ゾ', 0x0E},
            {'ダ', 0x0F},
            {'ヂ', 0x10},
            {'ヅ', 0x11},
            {'デ', 0x12},
            {'ド', 0x13},
            {'バ', 0x19},
            {'ビ', 0x1A},
            {'ブ', 0x1B},
            {'ボ', 0x1C},
            {'が', 0x26},
            {'ぎ', 0x27},
            {'ぐ', 0x28},
            {'げ', 0x29},
            {'ご', 0x2A},
            {'ざ', 0x2B},
            {'じ', 0x2C},
            {'ず', 0x2D},
            {'ぜ', 0x2E},
            {'ぞ', 0x2F},
            {'だ', 0x30},
            {'ぢ', 0x31},
            {'づ', 0x32},
            {'で', 0x33},
            {'ど', 0x34},
            {'ば', 0x3A},
            {'び', 0x3B},
            {'ぶ', 0x3C},
            {'ベ', 0x3D},
            {'べ', 0x3D},
            {'ぼ', 0x3E},
            {'パ', 0x40},
            {'ピ', 0x41},
            {'プ', 0x42},
            {'ポ', 0x43},
            {'ぱ', 0x44},
            {'ぴ', 0x45},
            {'ぷ', 0x46},
            {'ぺ', 0x47},
            {'ペ', 0x47},
            {'ぽ', 0x48},
            {G1Terminator, 0x50},
            {G1TradeOT, 0x5D}, // トレーナー, (Localized per ROM)
            {' ', 0x7F},
            {'ア', 0x80},
            {'イ', 0x81},
            {'ウ', 0x82},
            {'エ', 0x83},
            {'オ', 0x84},
            {'カ', 0x85},
            {'キ', 0x86},
            {'ク', 0x87},
            {'ケ', 0x88},
            {'コ', 0x89},
            {'サ', 0x8A},
            {'シ', 0x8B},
            {'ス', 0x8C},
            {'セ', 0x8D},
            {'ソ', 0x8E},
            {'タ', 0x8F},
            {'チ', 0x90},
            {'ツ', 0x91},
            {'テ', 0x92},
            {'ト', 0x93},
            {'ナ', 0x94},
            {'ニ', 0x95},
            {'ヌ', 0x96},
            {'ネ', 0x97},
            {'ノ', 0x98},
            {'ハ', 0x99},
            {'ヒ', 0x9A},
            {'フ', 0x9B},
            {'ホ', 0x9C},
            {'マ', 0x9D},
            {'ミ', 0x9E},
            {'ム', 0x9F},
            {'メ', 0xA0},
            {'モ', 0xA1},
            {'ヤ', 0xA2},
            {'ユ', 0xA3},
            {'ヨ', 0xA4},
            {'ラ', 0xA5},
            {'ル', 0xA6},
            {'レ', 0xA7},
            {'ロ', 0xA8},
            {'ワ', 0xA9},
            {'ヲ', 0xAA},
            {'ン', 0xAB},
            {'ッ', 0xAC},
            {'ャ', 0xAD},
            {'ュ', 0xAE},
            {'ョ', 0xAF},
            {'ィ', 0xB0},
            {'あ', 0xB1},
            {'い', 0xB2},
            {'う', 0xB3},
            {'え', 0xB4},
            {'お', 0xB5},
            {'か', 0xB6},
            {'き', 0xB7},
            {'く', 0xB8},
            {'け', 0xB9},
            {'こ', 0xBA},
            {'さ', 0xBB},
            {'し', 0xBC},
            {'す', 0xBD},
            {'せ', 0xBE},
            {'そ', 0xBF},
            {'た', 0xC0},
            {'ち', 0xC1},
            {'つ', 0xC2},
            {'て', 0xC3},
            {'と', 0xC4},
            {'な', 0xC5},
            {'に', 0xC6},
            {'ぬ', 0xC7},
            {'ね', 0xC8},
            {'の', 0xC9},
            {'は', 0xCA},
            {'ひ', 0xCB},
            {'ふ', 0xCC},
            {'へ', 0xCD},
            {'ヘ', 0xCD}, // Katakana ヘ => Hiragana へ
            {'ほ', 0xCE},
            {'ま', 0xCF},
            {'み', 0xD0},
            {'む', 0xD1},
            {'め', 0xD2},
            {'も', 0xD3},
            {'や', 0xD4},
            {'ゆ', 0xD5},
            {'よ', 0xD6},
            {'ら', 0xD7},
            {'リ', 0xD8},
            {'り', 0xD8},
            {'る', 0xD9},
            {'れ', 0xDA},
            {'ろ', 0xDB},
            {'わ', 0xDC},
            {'を', 0xDD},
            {'ん', 0xDE},
            {'っ', 0xDF},
            {'ゃ', 0xE0},
            {'ゅ', 0xE1},
            {'ょ', 0xE2},
            {'ー', 0xE3},
            {'?', 0xE6},
            {'!', 0xE7},
            {'ァ', 0xE9},
            {'ゥ', 0xEA},
            {'ェ', 0xEB},
            {'♂', 0xEF},
            {'ォ', 0xF4},
            {'♀', 0xF5},
            {'0', 0xF6},
            {'1', 0xF7},
            {'2', 0xF8},
            {'3', 0xF9},
            {'4', 0xFA},
            {'5', 0xFB},
            {'6', 0xFC},
            {'7', 0xFD},
            {'8', 0xFE},
            {'9', 0xFF}
        };

        #endregion
    }
}
