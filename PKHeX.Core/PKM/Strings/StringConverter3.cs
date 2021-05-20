using System;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for converting a <see cref="string"/> for Generation 3.
    /// </summary>
    public static class StringConverter3
    {
        private const byte TerminatorByte = 0xFF;
        private const char Terminator = (char)TerminatorByte;
        private const char TerminatorBigEndian = (char)0; // GC

        /// <summary>
        /// Converts a Generation 3 encoded value array to string.
        /// </summary>
        /// <param name="data">Byte array containing string data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString3(byte[] data, int offset, int count, bool jp)
        {
            var s = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                var val = data[offset + i];
                var c = GetG3Char(val, jp); // Convert to Unicode
                if (c == Terminator) // Stop if Terminator/Invalid
                    break;
                s.Append(c);
            }
            StringConverter.SanitizeString(s);
            return s.ToString();
        }

        /// <summary>
        /// Converts a string to a Generation 3 encoded value array.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="jp">String destination is Japanese font.</param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString3(string value, int maxLength, bool jp, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value[..maxLength]; // Hard cap
            var data = new byte[value.Length + 1]; // +1 for Terminator
            for (int i = 0; i < value.Length; i++)
            {
                var chr = value[i];
                var val = SetG3Char(chr, jp);
                if (val == Terminator) // end
                {
                    Array.Resize(ref data, i + 1);
                    break;
                }
                data[i] = val;
            }
            if (data.Length > 0)
                data[^1] = TerminatorByte;
            if (data.Length > maxLength && padTo <= maxLength)
            {
                // Truncate
                Array.Resize(ref data, maxLength);
                return data;
            }
            if (data.Length < padTo)
            {
                var start = data.Length;
                Array.Resize(ref data, padTo);
                for (int i = start; i < data.Length; i++)
                    data[i] = (byte)padWith;
            }
            return data;
        }

        /// <summary>Converts Big Endian encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetBEString3(byte[] data, int offset, int count)
        {
            var raw = Encoding.BigEndianUnicode.GetString(data, offset, count);
            var sb = new StringBuilder(raw);
            Util.TrimFromFirst(sb, TerminatorBigEndian);
            return sb.ToString();
        }

        /// <summary>Gets the bytes for a Big Endian string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetBEString3(string value, int maxLength, int padTo = 0, ushort padWith = TerminatorBigEndian)
        {
            if (value.Length > maxLength)
                value = value[..maxLength]; // Hard cap
            var sb = new StringBuilder(value);
            StringConverter.SanitizeString(sb);
            sb.Append(TerminatorBigEndian);
            var delta = padTo - value.Length;
            if (delta > 0)
                sb.Append((char)padWith, delta);
            var result = sb.ToString();
            return Encoding.BigEndianUnicode.GetBytes(result);
        }

        /// <summary>
        /// Decodes a character from a Generation 3 encoded value.
        /// </summary>
        /// <param name="chr">Generation 4 decoded character.</param>
        /// <param name="jp">Character destination is Japanese font.</param>
        /// <returns>Generation 3 encoded value.</returns>
        private static char GetG3Char(byte chr, bool jp)
        {
            var table = jp ? G3_JP : G3_EN;
            return table[chr];
        }

        /// <summary>
        /// Encodes a character to a Generation 3 encoded value.
        /// </summary>
        /// <param name="chr">Generation 4 decoded character.</param>
        /// <param name="jp">Character destination is Japanese font.</param>
        /// <returns>Generation 3 encoded value.</returns>
        private static byte SetG3Char(char chr, bool jp)
        {
            if (chr == '\'') // ’
                return 0xB4;
            var table = jp ? G3_JP : G3_EN;
            var index = Array.IndexOf(table, chr);
            if (index == -1)
                return TerminatorByte;
            return (byte)index;
        }

        private static readonly char[] G3_EN =
        {
            ' ',  'À',  'Á',  'Â', 'Ç',  'È',  'É',  'Ê',  'Ë',  'Ì', 'こ', 'Î',  'Ï',  'Ò',  'Ó',  'Ô',  // 0
            'Œ',  'Ù',  'Ú',  'Û', 'Ñ',  'ß',  'à',  'á',  'ね', 'Ç',  'È', 'é',  'ê',  'ë',  'ì',  'í',  // 1
            'î',  'ï',  'ò',  'ó', 'ô',  'œ',  'ù',  'ú',  'û',  'ñ',  'º', 'ª',  '⒅', '&',  '+',  'あ', // 2
            'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', '=',  'ょ', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', // 3
            'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',  // 4
            'っ', '¿',  '¡',  '⒆', '⒇', 'オ', 'カ', 'キ', 'ク', 'ケ', 'Í',  'コ', 'サ', 'ス', 'セ', 'ソ', // 5
            'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'â',  'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'í',  // 6
            'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ァ', // 7
            'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', // 8
            'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', // 9
            'ッ', '0',  '1',  '2', '3',  '4',  '5',  '6',  '7',  '8',  '9',  '!', '?',  '.',  '-',  '・',// A
            '⑬',  '“',  '”',  '‘', '’',  '♂',  '♀',  '$',  ',',  '⑧',  '/',  'A', 'B',  'C',  'D',  'E', // B
            'F',  'G',  'H',  'I', 'J',  'K',  'L',  'M',  'N',  'O',  'P',  'Q', 'R',  'S',  'T',  'U', // C
            'V',  'W',  'X',  'Y', 'Z',  'a',  'b',  'c',  'd',  'e',  'f',  'g', 'h',  'i',  'j',  'k', // D
            'l',  'm',  'n',  'o', 'p',  'q',  'r',  's',  't',  'u',  'v',  'w', 'x',  'y',  'z',  '0', // E
            ':',  'Ä',  'Ö',  'Ü', 'ä',  'ö',  'ü',                                                      // F

            // Make the total length 256 so that any byte access is always within the array
            Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator
        };

        private static readonly char[] G3_JP =
        {
            '　', 'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', // 0
            'た', 'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ', 'ふ', 'へ', 'ほ', 'ま', // 1
            'み', 'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'わ', 'を', 'ん', 'ぁ', // 2
            'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', 'ゅ', 'ょ', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', // 3
            'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ', // 4
            'っ', 'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', // 5
            'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', // 6
            'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ァ', // 7
            'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', // 8
            'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', // 9
            'ッ', '０', '１', '２', '３', '４', '５', '６', '７', '８', '９', '！', '？', '。', 'ー', '・', // A
            '⋯',  '『', '』', '「', '」', '♂',  '♀',  '$',  '.', '⑧',  '/',  'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', // B
            'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', // C
            'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ', 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', // D
            'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ', '0',  // E
            ':',  'Ä',  'Ö',  'Ü',  'ä',  'ö', 'ü',                                                      // F

            // Make the total length 256 so that any byte access is always within the array
            Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator
        };
    }
}
