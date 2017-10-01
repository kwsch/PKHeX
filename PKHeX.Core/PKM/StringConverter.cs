using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public static class StringConverter
    {
        /// <summary>
        /// Converts bytes to a string according to the input parameters.
        /// </summary>
        /// <param name="data">Encoded data</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString(byte[] data, int generation, bool jp, bool bigendian, int count, int offset = 0)
        {
            if (bigendian)
                return generation == 3 ? GetBEString3(data, offset, count) : GetBEString4(data, offset, count);

            switch (generation)
            {
                case 1:
                case 2: return GetString1(data, offset, count, jp);
                case 3: return GetString3(data, offset, count, jp);
                case 4: return GetString4(data, offset, count);
                case 5: return GetString5(data, offset, count);
                case 6: return GetString6(data, offset, count);
                default: return GetString7(data, offset, count);
            }
        }

        /// <summary>
        /// Gets the bytes for a Generation specific string according to the input parameters.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="maxLength"></param>
        /// <param name="language"></param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString(string value, int generation, bool jp, bool bigendian, int maxLength, int language = 0, int padTo = 0, ushort padWith = 0)
        {
            if (bigendian)
                return generation == 3 ? SetBEString3(value, maxLength, padTo, padWith) : SetBEString4(value, maxLength, padTo, padWith);

            switch (generation)
            {
                case 1:
                case 2: return SetString1(value, maxLength, jp, padTo, padWith);
                case 3: return SetString3(value, maxLength, jp, padTo, padWith);
                case 4: return SetString4(value, maxLength, padTo, padWith);
                case 5: return SetString5(value, maxLength, padTo, padWith);
                case 6: return SetString6(value, maxLength, padTo, padWith);
                default: return SetString7(value, maxLength, language, padTo, padWith);
            }
        }

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
            Dictionary<byte, string> dict = jp ? RBY2U_J : RBY2U_U;

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                byte val = strdata[offset + i];
                if (!dict.TryGetValue(val, out string c)) // Take valid values
                    break;
                if (c == "\0") // Stop if Terminator
                    break;
                s.Append(c);
            }
            return SanitizeString(s.ToString());
        }

        /// <summary>
        /// Converts Generation 2 Korean encoded data into a string.
        /// </summary>
        /// <param name="strdata">Encoded data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count"></param>
        /// <returns>Decoded string.</returns>
        public static string GetString2KOR(byte[] strdata, int offset, int count)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                byte val = strdata[offset + i];
                var dict = val <= 0xB ? GSC2U_KOR[val] : RBY2U_U;
                if (val <= 0xB && val != 0)
                    val = strdata[offset + ++i];
                if (!dict.TryGetValue(val, out string c)) // Take valid values
                    break;
                if (c == "\0") // Stop if Terminator
                    break;
                s.Append(c);
            }
            return SanitizeString(s.ToString());
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
            return Util.TrimFromZero(new string(strdata.TakeWhile(b => b != 0).Select(b => (char)table[b]).ToArray()));
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

            Dictionary<string, byte> dict = jp ? U2RBY_J : U2RBY_U;
            if (dict.ContainsKey(value)) // Handle "[TRAINER]"
                return new[] { dict[value], dict["\0"] };

            List<byte> arr = new List<byte>();
            foreach (char c in value)
            {
                if (!dict.TryGetValue(c.ToString(), out byte val))
                    break;
                arr.Add(val);
            }
            var term = dict["\0"]; // terminator
            arr.Add(term);
            while (arr.Count < padTo)
                arr.Add((byte)padWith);
            return arr.ToArray();
        }

        /// <summary>
        /// Converts a string to Generation 1 encoded data.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString2KOR(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap

            Dictionary<string, byte> dict = U2RBY_U;
            if (value == "트레이너") // Handle "[TRAINER]"
                return new[] { (byte)0x55, (byte)0x50 };

            List<byte> arr = new List<byte>();
            foreach (char c in value)
            {
                bool koreanChar = false;
                // while byte pairs starting with 0x00 and 0x0B have identical outputs, the game only naturally uses 0x0B pairs.
                for (byte i = 1; i < U2GSC_KOR.Length; i++)
                {
                    var table = U2GSC_KOR[i];
                    if (!table.TryGetValue(c.ToString(), out byte val))
                        continue;
                    koreanChar = true;
                    if (arr.Count + 1 >= maxLength)
                        break; // adding 2 characters will overflow requested buffer cap
                    arr.Add(i);
                    arr.Add(val);
                    break;
                }
                if (!koreanChar)
                {
                    if (!dict.TryGetValue(c.ToString(), out byte val))
                        break;
                    arr.Add(val);
                }
            }
            while (arr.Count >= maxLength)
                arr.RemoveAt(arr.Count - 1);
            arr.Add(0x50); // terminator
            while (arr.Count < padTo)
                arr.Add((byte)padWith);
            return arr.ToArray();
        }

        /// <summary>Converts Big Endian encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetBEString3(byte[] data, int offset, int count)
        {
            return Util.TrimFromZero(Encoding.BigEndianUnicode.GetString(data, offset, count));
        }

        /// <summary>Gets the bytes for a BigEndian string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetBEString3(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = SanitizeString(value)
                .PadRight(value.Length + 1, (char)0) // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.BigEndianUnicode.GetBytes(temp);
        }

        /// <summary>
        /// Converts a Generation 3 encoded value array to string.
        /// </summary>
        /// <param name="strdata">Byte array containing string data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString3(byte[] strdata, int offset, int count, bool jp)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                byte val = strdata[offset + i];
                if (val >= 247) // Take valid values
                    break;
                var c = GetG3Char(val, jp); // Convert to Unicode
                if (c == 0xFF) // Stop if Terminator
                    break;
                s.Append((char)c);
            }
            return SanitizeString(s.ToString());
        }

        /// <summary>
        /// Converts a string to a Generation 3 encoded value array.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="jp">String destination is Japanese font.</param>
        /// <param name="maxLength">Maximum length of string</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns></returns>
        public static byte[] SetString3(string value, int maxLength, bool jp, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            byte[] strdata = new byte[value.Length + 1]; // +1 for 0xFF
            for (int i = 0; i < value.Length; i++)
            {
                ushort chr = value[i];
                byte val = SetG3Char(chr, jp);
                if (val == 0xFF || chr == 0xFF)
                { Array.Resize(ref strdata, i); break; }
                strdata[i] = val;
            }
            if (strdata.Length > 0)
                strdata[strdata.Length - 1] = 0xFF;
            if (strdata.Length > maxLength && padTo <= maxLength)
                Array.Resize(ref strdata, maxLength);
            if (strdata.Length < padTo)
            {
                int start = strdata.Length;
                Array.Resize(ref strdata, padTo);
                for (int i = start; i < strdata.Length; i++)
                    strdata[i] = (byte)padWith;
            }
            return strdata;
        }

        /// <summary>
        /// Converts Generation 4 Big Endian encoded character data to string.
        /// </summary>
        /// <param name="strdata">Byte array containing encoded character data.</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Converted string.</returns>
        public static string GetBEString4(byte[] strdata, int offset, int count)
        {
            string s = "";
            for (int i = 0; i < count; i += 2)
            {
                ushort val = BigEndian.ToUInt16(strdata, offset + i);
                if (val == 0xFFFF) break;
                ushort chr = ConvertValue2CharG4(val);
                if (chr == 0xFFFF) break;
                s += (char)chr;
            }
            return SanitizeString(s);
        }

        /// <summary>
        /// Converts a string to Generation 4 Big Endian encoded character data.
        /// </summary>
        /// <param name="value">String to be converted.</param>
        /// <param name="maxLength">Maximum length of string</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Byte array containing encoded character data</returns>
        public static byte[] SetBEString4(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap

            string temp = UnSanitizeString(value, 4) // Replace Special Characters and add Terminator
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding

            byte[] strdata = new byte[temp.Length * 2];
            for (int i = 0; i < temp.Length; i++)
            {
                ushort chr = temp[i];
                ushort val = ConvertChar2ValueG4(chr);
                BigEndian.GetBytes(val).CopyTo(strdata, i * 2);
            }
            return strdata;
        }

        /// <summary>Converts Generation 4 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString4(byte[] data, int offset, int count)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < count; i += 2)
            {
                ushort val = BitConverter.ToUInt16(data, offset + i);
                if (val == 0xFFFF) break;
                ushort chr = ConvertValue2CharG4(val);
                if (chr == 0xFFFF) break;
                s.Append((char)chr);
            }
            return SanitizeString(s.ToString());
        }

        /// <summary>Gets the bytes for a 4th Generation String</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString4(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value, 4) // Replace Special Characters and add Terminator
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding

            byte[] strdata = new byte[temp.Length * 2];
            for (int i = 0; i < temp.Length; i++)
            {
                ushort chr = temp[i];
                ushort val = ConvertChar2ValueG4(chr);
                BitConverter.GetBytes(val).CopyTo(strdata, i * 2);
            }
            return strdata;
        }

        /// <summary>Converts Generation 5 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString5(byte[] data, int offset, int count)
        {
            return SanitizeString(TrimFromFFFF(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 5 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString5(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value, 5)
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 6 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns> 
        public static string GetString6(byte[] data, int offset, int count)
        {
            return SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 6 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns> 
        public static byte[] SetString6(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value, 6)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 7 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString7(byte[] data, int offset, int count)
        {
            return ConvertBin2StringG7_zh(SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count))));
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString7(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, 12); // Hard cap
            string temp = ConvertString2BinG7_zh(UnSanitizeString(value, 7), language == 10)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>
        /// Converts Generation 1 encoded character to string.
        /// </summary>
        /// <param name="key">Encoded character.</param>
        /// <param name="jp">Data source is Japanese.</param>
        /// <returns>Decoded string.</returns>
        public static string GetG1Char(byte key, bool jp)
        {
            Dictionary<byte, string> dict = jp ? RBY2U_J : RBY2U_U;
            return dict.ContainsKey(key) ? dict[key] : "";
        }

        public static bool GetIsG1Japanese(string str) => str.All(z => U2RBY_J.ContainsKey(z.ToString()));
        public static bool GetIsG1English(string str) => str.All(z => U2RBY_U.ContainsKey(z.ToString()));
        public static bool GetIsG2Korean(string str) => str.All(z => U2GSC_KOR.Any(x => x.ContainsKey(z.ToString())));

        /// <summary>
        /// Converts a Unicode string to Generation 7 in-game chinese string.
        /// </summary>
        /// <param name="inputstr">Unicode string.</param>
        /// <param name="cht">Pkm language is Traditional Chinese.</param>
        /// <returns>In-game chinese string.</returns>
        private static string ConvertString2BinG7_zh(string inputstr, bool cht = false)
        {
            var str = new StringBuilder();

            // A string cannot contain a mix of CHS and CHT characters.
            bool IsCHT = inputstr.Any(chr => Gen7_CHT.Contains(chr) && !Gen7_CHS.Contains(chr));
            IsCHT |= cht && !inputstr.Any(chr => Gen7_CHT.Contains(chr) ^ Gen7_CHS.Contains(chr)); // CHS and CHT have the same display name
            var table = IsCHT ? Gen7_CHT : Gen7_CHS;
            ushort ofs = IsCHT ? Gen7_CHT_Ofs : Gen7_CHS_Ofs;

            foreach (char chr in inputstr)
            {
                var index = Array.IndexOf(table, chr);
                var val = index > -1 ? (char) (ofs + index) : chr;
                str.Append(val);
            }
            return str.ToString();
        }

        /// <summary>
        /// Converts a Generation 7 in-game chinese string to Unicode string.
        /// </summary>
        /// <param name="inputstr">In-game chinese string.</param>
        /// <returns>Unicode string.</returns>
        private static string ConvertBin2StringG7_zh(string inputstr)
        {
            var str = new StringBuilder();
            foreach (var val in inputstr)
                str.Append((char)Getg7zhChar(val));
            return str.ToString();
        }

        /// <summary>
        /// Shifts a character from the CHS/CHT character tables
        /// </summary>
        /// <param name="val">Input value to shift</param>
        /// <returns>Shifted character</returns>
        private static ushort Getg7zhChar(ushort val)
        {
            if (Gen7_CHS_Ofs <= val && val < Gen7_CHS_Ofs + Gen7_CHS.Length) // within CHS char table
                return Gen7_CHS[val - Gen7_CHS_Ofs];
            if (Gen7_CHT_Ofs <= val && val < Gen7_CHT_Ofs + Gen7_CHT.Length) // within CHT char table
                return Gen7_CHT[val - Gen7_CHT_Ofs];
            return val; // regular character
        }

        /// <summary>
        /// Converts a Generation 4 value to Unicode character.
        /// </summary>
        /// <param name="val">Encoded value.</param>
        /// <returns>Decoded value (unicode).</returns>
        private static ushort ConvertValue2CharG4(ushort val)
        {
            int index = Array.IndexOf(G4Values, val);
            return index > -1 ? G4Chars[index] : (ushort)0xFFFF;
        }

        /// <summary>
        /// Converts a Unicode character to Generation 4 value.
        /// </summary>
        /// <param name="chr">Decoded value (unicode).</param>
        /// <returns>Encoded value.</returns>
        private static ushort ConvertChar2ValueG4(ushort chr)
        {
            int index = Array.IndexOf(G4Chars, chr);
            return index > -1 ? G4Values[index] : (ushort)0xFFFF;
        }

        /// <summary>
        /// Converts a Generation 3 encoded value to corresponding Generation 4 encoded value.
        /// </summary>
        /// <param name="val">Generation 3 encoded value.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Generation 4 encoded value.</returns>
        private static ushort GetG4Val(byte val, bool jp) => jp ? G34_4J[val] : G34_4E[val];

        /// <summary>
        /// Converts a Generation 3 encoded value to corresponding Generation 4 decoded character.
        /// </summary>
        /// <param name="val">Generation 3 encoded value.</param>
        /// <param name="jp">Value source is Japanese font.</param>
        /// <returns>Decoded value.</returns>
        private static ushort GetG3Char(byte val, bool jp) => ConvertValue2CharG4(GetG4Val(val, jp));

        /// <summary>
        /// Converts a Generation 4 decoded character to Generation 3 encoded value.
        /// </summary>
        /// <param name="chr">Generation 4 decoded character.</param>
        /// <param name="jp">Character destination is Japanese font.</param>
        /// <returns>Generation 3 encoded value.</returns>
        private static byte SetG3Char(ushort chr, bool jp)
        {
            int index = Array.IndexOf(jp ? G34_4J : G34_4E, ConvertChar2ValueG4(chr));
            return (byte)(index > -1 ? index : 0xFF);
        }

        #region Gen 1/2 Character Tables
        /// <summary>
        /// International 1->7 character translation table
        /// </summary>
        private static readonly ushort[] us_table = { 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0000, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 0x0058, 0x0059, 0x005A, 0x0028, 0x0029, 0x003A, 0x003B, 0x0028, 0x0029, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 0x0078, 0x0079, 0x007A, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x00C4, 0x00D6, 0x00DC, 0x00E4, 0x00F6, 0x00FC, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0050, 0x004D, 0x002D, 0x0020, 0x0020, 0x003F, 0x0021, 0x002D, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0xE08E, 0x0020, 0x0078, 0x002E, 0x002F, 0x002C, 0xE08F, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020, 0x0020 };
        /// <summary>
        /// Japanese 1->7 character translation table
        /// </summary>
        private static readonly ushort[] jp_table = { 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30AC, 0x30AE, 0x30B0, 0x30B2, 0x30B4, 0x30B6, 0x30B8, 0x30BA, 0x30BC, 0x30BE, 0x30C0, 0x30C2, 0x30C5, 0x30C7, 0x30C9, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30D0, 0x30D3, 0x30D6, 0x30DC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x304C, 0x304E, 0x3050, 0x3052, 0x3054, 0x3056, 0x3058, 0x305A, 0x305C, 0x305E, 0x3060, 0x3062, 0x3065, 0x3067, 0x3069, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3070, 0x3073, 0x3076, 0x30D9, 0x307C, 0x3000, 0x30D1, 0x30D4, 0x30D7, 0x30DD, 0x3071, 0x3074, 0x3077, 0x30DA, 0x307D, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x0000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A2, 0x30A4, 0x30A6, 0x30A8, 0x30AA, 0x30AB, 0x30AD, 0x30AF, 0x30B1, 0x30B3, 0x30B5, 0x30B7, 0x30B9, 0x30BB, 0x30BD, 0x30BF, 0x30C1, 0x30C4, 0x30C6, 0x30C8, 0x30CA, 0x30CB, 0x30CC, 0x30CD, 0x30CE, 0x30CF, 0x30D2, 0x30D5, 0x30DB, 0x30DE, 0x30DF, 0x30E0, 0x30E1, 0x30E2, 0x30E4, 0x30E6, 0x30E8, 0x30E9, 0x30EB, 0x30EC, 0x30ED, 0x30EF, 0x30F2, 0x30F3, 0x30C3, 0x30E3, 0x30E5, 0x30E7, 0x30A3, 0x3042, 0x3044, 0x3046, 0x3048, 0x304A, 0x304B, 0x304D, 0x304F, 0x3051, 0x3053, 0x3055, 0x3057, 0x3059, 0x305B, 0x305D, 0x305F, 0x3061, 0x3064, 0x3066, 0x3068, 0x306A, 0x306B, 0x306C, 0x306D, 0x306E, 0x306F, 0x3072, 0x3075, 0x30D8, 0x307B, 0x307E, 0x307F, 0x3080, 0x3081, 0x3082, 0x3084, 0x3086, 0x3088, 0x3089, 0x30EA, 0x308B, 0x308C, 0x308D, 0x308F, 0x3092, 0x3093, 0x3063, 0x3083, 0x3085, 0x3087, 0x30FC, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A1, 0x30A5, 0x30A7, 0x3000, 0x3000, 0x3000, 0x2642, 0x3000, 0x3000, 0x3000, 0x3000, 0x30A9, 0x2640, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000, 0x3000 };

        private static readonly Dictionary<byte, string> RBY2U_U = new Dictionary<byte, string>{
            {0x50, "\0"},
            {0x5D, "TRAINER"},
            {0x7F, " "},
            {0x80, "A"},
            {0x81, "B"},
            {0x82, "C"},
            {0x83, "D"},
            {0x84, "E"},
            {0x85, "F"},
            {0x86, "G"},
            {0x87, "H"},
            {0x88, "I"},
            {0x89, "J"},
            {0x8A, "K"},
            {0x8B, "L"},
            {0x8C, "M"},
            {0x8D, "N"},
            {0x8E, "O"},
            {0x8F, "P"},
            {0x90, "Q"},
            {0x91, "R"},
            {0x92, "S"},
            {0x93, "T"},
            {0x94, "U"},
            {0x95, "V"},
            {0x96, "W"},
            {0x97, "X"},
            {0x98, "Y"},
            {0x99, "Z"},
            {0x9A, "("},
            {0x9B, ")"},
            {0x9C, ":"},
            {0x9D, ";"},
            {0x9E, "["},
            {0x9F, "]"},
            {0xA0, "a"},
            {0xA1, "b"},
            {0xA2, "c"},
            {0xA3, "d"},
            {0xA4, "e"},
            {0xA5, "f"},
            {0xA6, "g"},
            {0xA7, "h"},
            {0xA8, "i"},
            {0xA9, "j"},
            {0xAA, "k"},
            {0xAB, "l"},
            {0xAC, "m"},
            {0xAD, "n"},
            {0xAE, "o"},
            {0xAF, "p"},
            {0xB0, "q"},
            {0xB1, "r"},
            {0xB2, "s"},
            {0xB3, "t"},
            {0xB4, "u"},
            {0xB5, "v"},
            {0xB6, "w"},
            {0xB7, "x"},
            {0xB8, "y"},
            {0xB9, "z"},
            {0xC0, "Ä"},
            {0xC1, "Ö"},
            {0xC2, "Ü"},
            {0xC3, "ä"},
            {0xC4, "ö"},
            {0xC5, "ü"},
            {0xE0, "’"},
            {0xE1, "{"}, /* Pk */
            {0xE2, "}"}, /* Mn */
            {0xE3, "-"},
            {0xE6, "?"},
            {0xE7, "!"},
            {0xE8, "."}, // Alias decimal point to .
            {0xEF, "♂"},
            {0xF1, "×"},
            {0xF2, "."},
            {0xF3, "/"},
            {0xF4, ","},
            {0xF5, "♀"},
            {0xF6, "0"},
            {0xF7, "1"},
            {0xF8, "2"},
            {0xF9, "3"},
            {0xFA, "4"},
            {0xFB, "5"},
            {0xFC, "6"},
            {0xFD, "7"},
            {0xFE, "8"},
            {0xFF, "9"}
        };
        private static readonly Dictionary<byte, string> RBY2U_J = new Dictionary<byte, string> {
            {0x05, "ガ"},
            {0x06, "ギ"},
            {0x07, "グ"},
            {0x08, "ゲ"},
            {0x09, "ゴ"},
            {0x0A, "ザ"},
            {0x0B, "ジ"},
            {0x0C, "ズ"},
            {0x0D, "ゼ"},
            {0x0E, "ゾ"},
            {0x0F, "ダ"},
            {0x10, "ヂ"},
            {0x11, "ヅ"},
            {0x12, "デ"},
            {0x13, "ド"},
            {0x19, "バ"},
            {0x1A, "ビ"},
            {0x1B, "ブ"},
            {0x1C, "ボ"},
            {0x26, "が"},
            {0x27, "ぎ"},
            {0x28, "ぐ"},
            {0x29, "げ"},
            {0x2A, "ご"},
            {0x2B, "ざ"},
            {0x2C, "じ"},
            {0x2D, "ず"},
            {0x2E, "ぜ"},
            {0x2F, "ぞ"},
            {0x30, "だ"},
            {0x31, "ぢ"},
            {0x32, "づ"},
            {0x33, "で"},
            {0x34, "ど"},
            {0x3A, "ば"},
            {0x3B, "び"},
            {0x3C, "ぶ"},
            {0x3D, "ベ"},
            {0x3E, "ぼ"},
            {0x40, "パ"},
            {0x41, "ピ"},
            {0x42, "プ"},
            {0x43, "ポ"},
            {0x44, "ぱ"},
            {0x45, "ぴ"},
            {0x46, "ぷ"},
            {0x47, "ペ"},
            {0x48, "ぽ"},
            {0x50, "\0"},
            {0x5D, "トレーナー"},
            {0x7F, " "},
            {0x80, "ア"},
            {0x81, "イ"},
            {0x82, "ウ"},
            {0x83, "エ"},
            {0x84, "オ"},
            {0x85, "カ"},
            {0x86, "キ"},
            {0x87, "ク"},
            {0x88, "ケ"},
            {0x89, "コ"},
            {0x8A, "サ"},
            {0x8B, "シ"},
            {0x8C, "ス"},
            {0x8D, "セ"},
            {0x8E, "ソ"},
            {0x8F, "タ"},
            {0x90, "チ"},
            {0x91, "ツ"},
            {0x92, "テ"},
            {0x93, "ト"},
            {0x94, "ナ"},
            {0x95, "ニ"},
            {0x96, "ヌ"},
            {0x97, "ネ"},
            {0x98, "ノ"},
            {0x99, "ハ"},
            {0x9A, "ヒ"},
            {0x9B, "フ"},
            {0x9C, "ホ"},
            {0x9D, "マ"},
            {0x9E, "ミ"},
            {0x9F, "ム"},
            {0xA0, "メ"},
            {0xA1, "モ"},
            {0xA2, "ヤ"},
            {0xA3, "ユ"},
            {0xA4, "ヨ"},
            {0xA5, "ラ"},
            {0xA6, "ル"},
            {0xA7, "レ"},
            {0xA8, "ロ"},
            {0xA9, "ワ"},
            {0xAA, "ヲ"},
            {0xAB, "ン"},
            {0xAC, "ッ"},
            {0xAD, "ャ"},
            {0xAE, "ュ"},
            {0xAF, "ョ"},
            {0xB0, "ィ"},
            {0xB1, "あ"},
            {0xB2, "い"},
            {0xB3, "う"},
            {0xB4, "え"},
            {0xB5, "お"},
            {0xB6, "か"},
            {0xB7, "き"},
            {0xB8, "く"},
            {0xB9, "け"},
            {0xBA, "こ"},
            {0xBB, "さ"},
            {0xBC, "し"},
            {0xBD, "す"},
            {0xBE, "せ"},
            {0xBF, "そ"},
            {0xC0, "た"},
            {0xC1, "ち"},
            {0xC2, "つ"},
            {0xC3, "て"},
            {0xC4, "と"},
            {0xC5, "な"},
            {0xC6, "に"},
            {0xC7, "ぬ"},
            {0xC8, "ね"},
            {0xC9, "の"},
            {0xCA, "は"},
            {0xCB, "ひ"},
            {0xCC, "ふ"},
            {0xCD, "へ"},
            {0xCE, "ほ"},
            {0xCF, "ま"},
            {0xD0, "み"},
            {0xD1, "む"},
            {0xD2, "め"},
            {0xD3, "も"},
            {0xD4, "や"},
            {0xD5, "ゆ"},
            {0xD6, "よ"},
            {0xD7, "ら"},
            {0xD8, "リ"},
            {0xD9, "る"},
            {0xDA, "れ"},
            {0xDB, "ろ"},
            {0xDC, "わ"},
            {0xDD, "を"},
            {0xDE, "ん"},
            {0xDF, "っ"},
            {0xE0, "ゃ"},
            {0xE1, "ゅ"},
            {0xE2, "ょ"},
            {0xE3, "ー"},
            {0xE6, "?"},
            {0xE7, "!"},
            {0xE9, "ァ"},
            {0xEA, "ゥ"},
            {0xEB, "ェ"},
            {0xEF, "♂"},
            {0xF4, "ォ"},
            {0xF5, "♀"},
            {0xF6, "0"},
            {0xF7, "1"},
            {0xF8, "2"},
            {0xF9, "3"},
            {0xFA, "4"},
            {0xFB, "5"},
            {0xFC, "6"},
            {0xFD, "7"},
            {0xFE, "8"},
            {0xFF, "9"}
        };

        private static readonly Dictionary<string, byte> U2RBY_U = new Dictionary<string, byte> {
            {"\0", 0x50},
            {"TRAINER", 0x5D},
            {" ", 0x7F},
            {"A", 0x80},
            {"B", 0x81},
            {"C", 0x82},
            {"D", 0x83},
            {"E", 0x84},
            {"F", 0x85},
            {"G", 0x86},
            {"H", 0x87},
            {"I", 0x88},
            {"J", 0x89},
            {"K", 0x8A},
            {"L", 0x8B},
            {"M", 0x8C},
            {"N", 0x8D},
            {"O", 0x8E},
            {"P", 0x8F},
            {"Q", 0x90},
            {"R", 0x91},
            {"S", 0x92},
            {"T", 0x93},
            {"U", 0x94},
            {"V", 0x95},
            {"W", 0x96},
            {"X", 0x97},
            {"Y", 0x98},
            {"Z", 0x99},
            {"(", 0x9A},
            {")", 0x9B},
            {":", 0x9C},
            {";", 0x9D},
            {"[", 0x9E},
            {"]", 0x9F},
            {"a", 0xA0},
            {"b", 0xA1},
            {"c", 0xA2},
            {"d", 0xA3},
            {"e", 0xA4},
            {"f", 0xA5},
            {"g", 0xA6},
            {"h", 0xA7},
            {"i", 0xA8},
            {"j", 0xA9},
            {"k", 0xAA},
            {"l", 0xAB},
            {"m", 0xAC},
            {"n", 0xAD},
            {"o", 0xAE},
            {"p", 0xAF},
            {"q", 0xB0},
            {"r", 0xB1},
            {"s", 0xB2},
            {"t", 0xB3},
            {"u", 0xB4},
            {"v", 0xB5},
            {"w", 0xB6},
            {"x", 0xB7},
            {"y", 0xB8},
            {"z", 0xB9},
            {"Ä", 0xC0},
            {"Ö", 0xC1},
            {"Ü", 0xC2},
            {"ä", 0xC3},
            {"ö", 0xC4},
            {"ü", 0xC5},
            {"'", 0xE0}, // Alias ' to ’ for Farfetch'd
            {"’", 0xE0},
            {"{", 0xE1}, /* Pk */
            {"}", 0xE2}, /* Mn */
            {"-", 0xE3},
            {"?", 0xE6},
            {"!", 0xE7},
            {"♂", 0xEF},
            {"×", 0xF1},
            {".", 0xF2},
            {"/", 0xF3},
            {",", 0xF4},
            {"♀", 0xF5},
            {"0", 0xF6},
            {"1", 0xF7},
            {"2", 0xF8},
            {"3", 0xF9},
            {"4", 0xFA},
            {"5", 0xFB},
            {"6", 0xFC},
            {"7", 0xFD},
            {"8", 0xFE},
            {"9", 0xFF}
        };
        private static readonly Dictionary<string, byte> U2RBY_J = new Dictionary<string, byte> {
            {"ガ", 0x05},
            {"ギ", 0x06},
            {"グ", 0x07},
            {"ゲ", 0x08},
            {"ゴ", 0x09},
            {"ザ", 0x0A},
            {"ジ", 0x0B},
            {"ズ", 0x0C},
            {"ゼ", 0x0D},
            {"ゾ", 0x0E},
            {"ダ", 0x0F},
            {"ヂ", 0x10},
            {"ヅ", 0x11},
            {"デ", 0x12},
            {"ド", 0x13},
            {"バ", 0x19},
            {"ビ", 0x1A},
            {"ブ", 0x1B},
            {"ボ", 0x1C},
            {"が", 0x26},
            {"ぎ", 0x27},
            {"ぐ", 0x28},
            {"げ", 0x29},
            {"ご", 0x2A},
            {"ざ", 0x2B},
            {"じ", 0x2C},
            {"ず", 0x2D},
            {"ぜ", 0x2E},
            {"ぞ", 0x2F},
            {"だ", 0x30},
            {"ぢ", 0x31},
            {"づ", 0x32},
            {"で", 0x33},
            {"ど", 0x34},
            {"ば", 0x3A},
            {"び", 0x3B},
            {"ぶ", 0x3C},
            {"ベ", 0x3D},
            {"べ", 0x3D},
            {"ぼ", 0x3E},
            {"パ", 0x40},
            {"ピ", 0x41},
            {"プ", 0x42},
            {"ポ", 0x43},
            {"ぱ", 0x44},
            {"ぴ", 0x45},
            {"ぷ", 0x46},
            {"ぺ", 0x47},
            {"ペ", 0x47},
            {"ぽ", 0x48},
            {"\0", 0x50},
            {"トレーナー", 0x5D},
            {" ", 0x7F},
            {"ア", 0x80},
            {"イ", 0x81},
            {"ウ", 0x82},
            {"エ", 0x83},
            {"オ", 0x84},
            {"カ", 0x85},
            {"キ", 0x86},
            {"ク", 0x87},
            {"ケ", 0x88},
            {"コ", 0x89},
            {"サ", 0x8A},
            {"シ", 0x8B},
            {"ス", 0x8C},
            {"セ", 0x8D},
            {"ソ", 0x8E},
            {"タ", 0x8F},
            {"チ", 0x90},
            {"ツ", 0x91},
            {"テ", 0x92},
            {"ト", 0x93},
            {"ナ", 0x94},
            {"ニ", 0x95},
            {"ヌ", 0x96},
            {"ネ", 0x97},
            {"ノ", 0x98},
            {"ハ", 0x99},
            {"ヒ", 0x9A},
            {"フ", 0x9B},
            {"ホ", 0x9C},
            {"マ", 0x9D},
            {"ミ", 0x9E},
            {"ム", 0x9F},
            {"メ", 0xA0},
            {"モ", 0xA1},
            {"ヤ", 0xA2},
            {"ユ", 0xA3},
            {"ヨ", 0xA4},
            {"ラ", 0xA5},
            {"ル", 0xA6},
            {"レ", 0xA7},
            {"ロ", 0xA8},
            {"ワ", 0xA9},
            {"ヲ", 0xAA},
            {"ン", 0xAB},
            {"ッ", 0xAC},
            {"ャ", 0xAD},
            {"ュ", 0xAE},
            {"ョ", 0xAF},
            {"ィ", 0xB0},
            {"あ", 0xB1},
            {"い", 0xB2},
            {"う", 0xB3},
            {"え", 0xB4},
            {"お", 0xB5},
            {"か", 0xB6},
            {"き", 0xB7},
            {"く", 0xB8},
            {"け", 0xB9},
            {"こ", 0xBA},
            {"さ", 0xBB},
            {"し", 0xBC},
            {"す", 0xBD},
            {"せ", 0xBE},
            {"そ", 0xBF},
            {"た", 0xC0},
            {"ち", 0xC1},
            {"つ", 0xC2},
            {"て", 0xC3},
            {"と", 0xC4},
            {"な", 0xC5},
            {"に", 0xC6},
            {"ぬ", 0xC7},
            {"ね", 0xC8},
            {"の", 0xC9},
            {"は", 0xCA},
            {"ひ", 0xCB},
            {"ふ", 0xCC},
            {"へ", 0xCD},
            {"ほ", 0xCE},
            {"ま", 0xCF},
            {"み", 0xD0},
            {"む", 0xD1},
            {"め", 0xD2},
            {"も", 0xD3},
            {"や", 0xD4},
            {"ゆ", 0xD5},
            {"よ", 0xD6},
            {"ら", 0xD7},
            {"リ", 0xD8},
            {"り", 0xD8},
            {"る", 0xD9},
            {"れ", 0xDA},
            {"ろ", 0xDB},
            {"わ", 0xDC},
            {"を", 0xDD},
            {"ん", 0xDE},
            {"っ", 0xDF},
            {"ゃ", 0xE0},
            {"ゅ", 0xE1},
            {"ょ", 0xE2},
            {"ー", 0xE3},
            {"?", 0xE6},
            {"!", 0xE7},
            {"ァ", 0xE9},
            {"ゥ", 0xEA},
            {"ェ", 0xEB},
            {"♂", 0xEF},
            {"ォ", 0xF4},
            {"♀", 0xF5},
            {"0", 0xF6},
            {"1", 0xF7},
            {"2", 0xF8},
            {"3", 0xF9},
            {"4", 0xFA},
            {"5", 0xFB},
            {"6", 0xFC},
            {"7", 0xFD},
            {"8", 0xFE},
            {"9", 0xFF}
        };
        #endregion

        #region Gen 2 Korean Character Tables
        private static readonly Dictionary<string, byte> U2GSC_KOR_0 = new Dictionary<string, byte> {
            {"ㄱ", 0x00}, {"ㄴ", 0x01}, {"ㄷ", 0x02}, {"ㄹ", 0x03}, {"ㅁ", 0x04}, {"ㅂ", 0x05}, {"ㅅ", 0x06}, {"ㅇ", 0x07}, {"ㅈ", 0x08}, {"ㅊ", 0x09}, {"ㅋ", 0x0A}, {"ㅌ", 0x0B}, {"ㅍ", 0x0C}, {"ㅎ", 0x0D}, {"ㄲ", 0x0E}, {"ㄸ", 0x0F},
            {"ㅃ", 0x10}, {"ㅆ", 0x11}, {"ㅉ", 0x12},
            {"ㅏ", 0x20}, {"ㅑ", 0x21}, {"ㅓ", 0x22}, {"ㅕ", 0x23}, {"ㅗ", 0x24}, {"ㅛ", 0x25}, {"ㅜ", 0x26}, {"ㅠ", 0x27}, {"ㅡ", 0x28}, {"ㅣ", 0x29}, {"ㅐ", 0x2A}, {"ㅒ", 0x2B}, {"ㅔ", 0x2C}, {"ㅖ", 0x2D}, {"ㅘ", 0x2E}, {"ㅙ", 0x2F},
            {"ㅚ", 0x30}, {"ㅝ", 0x31}, {"ㅞ", 0x32}, {"ㅟ", 0x33}, {"ㅢ", 0x34}, {"_", 0x35}, {"—", 0x36},
            {"「", 0x60}, {"」", 0x61}, {"『", 0x62}, {"』", 0x63}, {"(", 0x64}, {")", 0x65}, {"!", 0x66}, {"?", 0x67}, {"-", 0x68}, {"~", 0x69}, {"⋯", 0x6A}, {",", 0x6B}, {".", 0x6C},
            {"0", 0xF0}, {"1", 0xF1}, {"2", 0xF2}, {"3", 0xF3}, {"4", 0xF4}, {"5", 0xF5}, {"6", 0xF6}, {"7", 0xF7}, {"8", 0xF8}, {"9", 0xF9},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_1 = new Dictionary<string, byte> {
            {"가", 0x00}, {"각", 0x01}, {"간", 0x02}, {"갇", 0x03}, {"갈", 0x04}, {"갉", 0x05}, {"갊", 0x06}, {"감", 0x07}, {"갑", 0x08}, {"값", 0x09}, {"갓", 0x0A}, {"갔", 0x0B}, {"강", 0x0C}, {"갖", 0x0D}, {"갗", 0x0E},
            {"같", 0x10}, {"갚", 0x11}, {"갛", 0x12}, {"개", 0x13}, {"객", 0x14}, {"갠", 0x15}, {"갤", 0x16}, {"갬", 0x17}, {"갭", 0x18}, {"갯", 0x19}, {"갰", 0x1A}, {"갱", 0x1B}, {"갸", 0x1C}, {"갹", 0x1D}, {"갼", 0x1E}, {"걀", 0x1F},
            {"걋", 0x20}, {"걍", 0x21}, {"걔", 0x22}, {"걘", 0x23}, {"걜", 0x24}, {"거", 0x25}, {"걱", 0x26}, {"건", 0x27}, {"걷", 0x28}, {"걸", 0x29}, {"걺", 0x2A}, {"검", 0x2B}, {"겁", 0x2C}, {"것", 0x2D}, {"겄", 0x2E}, {"겅", 0x2F},
            {"겆", 0x30}, {"겉", 0x31}, {"겊", 0x32}, {"겋", 0x33}, {"게", 0x34}, {"겐", 0x35}, {"겔", 0x36}, {"겜", 0x37}, {"겝", 0x38}, {"겟", 0x39}, {"겠", 0x3A}, {"겡", 0x3B}, {"겨", 0x3C}, {"격", 0x3D}, {"겪", 0x3E}, {"견", 0x3F},
            {"겯", 0x40}, {"결", 0x41}, {"겹", 0x42}, {"겸", 0x43}, {"겻", 0x44}, {"겼", 0x45}, {"경", 0x46}, {"곁", 0x47}, {"계", 0x48}, {"곈", 0x49}, {"곌", 0x4A}, {"곕", 0x4B}, {"곗", 0x4C}, {"고", 0x4D}, {"곡", 0x4E}, {"곤", 0x4F},
            {"곧", 0x60}, {"골", 0x61}, {"곪", 0x62}, {"곬", 0x63}, {"곯", 0x64}, {"곰", 0x65}, {"곱", 0x66}, {"곳", 0x67}, {"공", 0x68}, {"곶", 0x69}, {"과", 0x6A}, {"곽", 0x6B}, {"관", 0x6C}, {"괄", 0x6D}, {"괆", 0x6E},
            {"괌", 0x70}, {"괍", 0x71}, {"괏", 0x72}, {"광", 0x73}, {"괘", 0x74}, {"괜", 0x75}, {"괠", 0x76}, {"괩", 0x77}, {"괬", 0x78}, {"괭", 0x79}, {"괴", 0x7A}, {"괵", 0x7B}, {"괸", 0x7C}, {"괼", 0x7D}, {"괻", 0x7E},
            {"굅", 0x80}, {"굇", 0x81}, {"굉", 0x82}, {"교", 0x83}, {"굔", 0x84}, {"굘", 0x85}, {"굡", 0x86}, {"굣", 0x87}, {"구", 0x88}, {"국", 0x89}, {"군", 0x8A}, {"굳", 0x8B}, {"굴", 0x8C}, {"굵", 0x8D}, {"굶", 0x8E}, {"굻", 0x8F},
            {"굼", 0x90}, {"굽", 0x91}, {"굿", 0x92}, {"궁", 0x93}, {"궂", 0x94}, {"궈", 0x95}, {"궉", 0x96}, {"권", 0x97}, {"궐", 0x98}, {"궜", 0x99}, {"궝", 0x9A}, {"궤", 0x9B}, {"궷", 0x9C}, {"귀", 0x9D}, {"귁", 0x9E}, {"귄", 0x9F},
            {"귈", 0xA0}, {"귐", 0xA1}, {"귑", 0xA2}, {"귓", 0xA3}, {"규", 0xA4}, {"균", 0xA5}, {"귤", 0xA6}, {"그", 0xA7}, {"극", 0xA8}, {"근", 0xA9}, {"귿", 0xAA}, {"글", 0xAB}, {"긁", 0xAC}, {"금", 0xAD}, {"급", 0xAE}, {"긋", 0xAF},
            {"긍", 0xB0}, {"긔", 0xB1}, {"기", 0xB2}, {"긱", 0xB3}, {"긴", 0xB4}, {"긷", 0xB5}, {"길", 0xB6}, {"긺", 0xB7}, {"김", 0xB8}, {"깁", 0xB9}, {"깃", 0xBA}, {"깅", 0xBB}, {"깆", 0xBC}, {"깊", 0xBD}, {"까", 0xBE}, {"깍", 0xBF},
            {"깎", 0xC0}, {"깐", 0xC1}, {"깔", 0xC2}, {"깖", 0xC3}, {"깜", 0xC4}, {"깝", 0xC5}, {"깟", 0xC6}, {"깠", 0xC7}, {"깡", 0xC8}, {"깥", 0xC9}, {"깨", 0xCA}, {"깩", 0xCB}, {"깬", 0xCC}, {"깰", 0xCD}, {"깸", 0xCE},
            {"깹", 0xD0}, {"깻", 0xD1}, {"깼", 0xD2}, {"깽", 0xD3}, {"꺄", 0xD4}, {"꺅", 0xD5}, {"꺌", 0xD6}, {"꺼", 0xD7}, {"꺽", 0xD8}, {"꺾", 0xD9}, {"껀", 0xDA}, {"껄", 0xDB}, {"껌", 0xDC}, {"껍", 0xDD}, {"껏", 0xDE},
            {"껐", 0xE0}, {"껑", 0xE1}, {"께", 0xE2}, {"껙", 0xE3}, {"껜", 0xE4}, {"껨", 0xE5}, {"껫", 0xE6}, {"껭", 0xE7}, {"껴", 0xE8}, {"껸", 0xE9}, {"껼", 0xEA}, {"꼇", 0xEB}, {"꼈", 0xEC}, {"꼍", 0xED}, {"꼐", 0xEE}, {"꼬", 0xEF},
            {"꼭", 0xF0}, {"꼰", 0xF1}, {"꼲", 0xF2}, {"꼴", 0xF3}, {"꼼", 0xF4}, {"꼽", 0xF5}, {"꼿", 0xF6}, {"꽁", 0xF7}, {"꽂", 0xF8}, {"꽃", 0xF9}, {"꽈", 0xFA}, {"꽉", 0xFB}, {"꽐", 0xFC}, {"꽜", 0xFD}, {"꽝", 0xFE}, {"꽤", 0xFF},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_2 = new Dictionary<string, byte> {
            {"꽥", 0x00}, {"꽹", 0x01}, {"꾀", 0x02}, {"꾄", 0x03}, {"꾈", 0x04}, {"꾐", 0x05}, {"꾑", 0x06}, {"꾕", 0x07}, {"꾜", 0x08}, {"꾸", 0x09}, {"꾹", 0x0A}, {"꾼", 0x0B}, {"꿀", 0x0C}, {"꿇", 0x0D}, {"꿈", 0x0E}, {"꿉", 0x0F},
            {"꿋", 0x10}, {"꿍", 0x11}, {"꿎", 0x12}, {"꿔", 0x13}, {"꿜", 0x14}, {"꿨", 0x15}, {"꿩", 0x16}, {"꿰", 0x17}, {"꿱", 0x18}, {"꿴", 0x19}, {"꿸", 0x1A}, {"뀀", 0x1B}, {"뀁", 0x1C}, {"뀄", 0x1D}, {"뀌", 0x1E}, {"뀐", 0x1F},
            {"뀔", 0x20}, {"뀜", 0x21}, {"뀝", 0x22}, {"뀨", 0x23}, {"끄", 0x24}, {"끅", 0x25}, {"끈", 0x26}, {"끊", 0x27}, {"끌", 0x28}, {"끎", 0x29}, {"끓", 0x2A}, {"끔", 0x2B}, {"끕", 0x2C}, {"끗", 0x2D}, {"끙", 0x2E},
            {"끝", 0x30}, {"끼", 0x31}, {"끽", 0x32}, {"낀", 0x33}, {"낄", 0x34}, {"낌", 0x35}, {"낍", 0x36}, {"낏", 0x37}, {"낑", 0x38}, {"나", 0x39}, {"낙", 0x3A}, {"낚", 0x3B}, {"난", 0x3C}, {"낟", 0x3D}, {"날", 0x3E},
            {"낡", 0x40}, {"낢", 0x41}, {"남", 0x42}, {"납", 0x43}, {"낫", 0x44}, {"났", 0x45}, {"낭", 0x46}, {"낮", 0x47}, {"낯", 0x48}, {"낱", 0x49}, {"낳", 0x4A}, {"내", 0x4B}, {"낵", 0x4C}, {"낸", 0x4D}, {"낼", 0x4E}, {"냄", 0x4F},
            {"냅", 0x60}, {"냇", 0x61}, {"냈", 0x62}, {"냉", 0x63}, {"냐", 0x64}, {"냑", 0x65}, {"냔", 0x66}, {"냘", 0x67}, {"냠", 0x68}, {"냥", 0x69}, {"너", 0x6A}, {"넉", 0x6B}, {"넋", 0x6C}, {"넌", 0x6D}, {"널", 0x6E}, {"넒", 0x6F},
            {"넓", 0x70}, {"넘", 0x71}, {"넙", 0x72}, {"넛", 0x73}, {"넜", 0x74}, {"넝", 0x75}, {"넣", 0x76}, {"네", 0x77}, {"넥", 0x78}, {"넨", 0x79}, {"넬", 0x7A}, {"넴", 0x7B}, {"넵", 0x7C}, {"넷", 0x7D}, {"넸", 0x7E}, {"넹", 0x7F},
            {"녀", 0x80}, {"녁", 0x81}, {"년", 0x82}, {"녈", 0x83}, {"념", 0x84}, {"녑", 0x85}, {"녔", 0x86}, {"녕", 0x87}, {"녘", 0x88}, {"녜", 0x89}, {"녠", 0x8A}, {"노", 0x8B}, {"녹", 0x8C}, {"논", 0x8D}, {"놀", 0x8E}, {"놂", 0x8F},
            {"놈", 0x90}, {"놉", 0x91}, {"놋", 0x92}, {"농", 0x93}, {"높", 0x94}, {"놓", 0x95}, {"놔", 0x96}, {"놘", 0x97}, {"놜", 0x98}, {"놨", 0x99}, {"뇌", 0x9A}, {"뇐", 0x9B}, {"뇔", 0x9C}, {"뇜", 0x9D}, {"뇝", 0x9E},
            {"뇟", 0xA0}, {"뇨", 0xA1}, {"뇩", 0xA2}, {"뇬", 0xA3}, {"뇰", 0xA4}, {"뇹", 0xA5}, {"뇻", 0xA6}, {"뇽", 0xA7}, {"누", 0xA8}, {"눅", 0xA9}, {"눈", 0xAA}, {"눋", 0xAB}, {"눌", 0xAC}, {"눔", 0xAD}, {"눕", 0xAE},
            {"눗", 0xB0}, {"눙", 0xB1}, {"눠", 0xB2}, {"눴", 0xB3}, {"눼", 0xB4}, {"뉘", 0xB5}, {"뉜", 0xB6}, {"뉠", 0xB7}, {"뉨", 0xB8}, {"뉩", 0xB9}, {"뉴", 0xBA}, {"뉵", 0xBB}, {"뉼", 0xBC}, {"늄", 0xBD}, {"늅", 0xBE}, {"늉", 0xBF},
            {"느", 0xC0}, {"늑", 0xC1}, {"는", 0xC2}, {"늘", 0xC3}, {"늙", 0xC4}, {"늚", 0xC5}, {"늠", 0xC6}, {"늡", 0xC7}, {"늣", 0xC8}, {"능", 0xC9}, {"늦", 0xCA}, {"늪", 0xCB}, {"늬", 0xCC}, {"늰", 0xCD}, {"늴", 0xCE}, {"니", 0xCF},
            {"닉", 0xD0}, {"닌", 0xD1}, {"닐", 0xD2}, {"닒", 0xD3}, {"님", 0xD4}, {"닙", 0xD5}, {"닛", 0xD6}, {"닝", 0xD7}, {"닢", 0xD8}, {"다", 0xD9}, {"닥", 0xDA}, {"닦", 0xDB}, {"단", 0xDC}, {"닫", 0xDD}, {"달", 0xDE}, {"닭", 0xDF},
            {"닮", 0xE0}, {"닯", 0xE1}, {"닳", 0xE2}, {"담", 0xE3}, {"답", 0xE4}, {"닷", 0xE5}, {"닸", 0xE6}, {"당", 0xE7}, {"닺", 0xE8}, {"닻", 0xE9}, {"닿", 0xEA}, {"대", 0xEB}, {"댁", 0xEC}, {"댄", 0xED}, {"댈", 0xEE}, {"댐", 0xEF},
            {"댑", 0xF0}, {"댓", 0xF1}, {"댔", 0xF2}, {"댕", 0xF3}, {"더", 0xF4}, {"덕", 0xF5}, {"덖", 0xF6}, {"던", 0xF7}, {"덛", 0xF8}, {"덜", 0xF9}, {"덞", 0xFA}, {"덟", 0xFB}, {"덤", 0xFC}, {"덥", 0xFD},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_3 = new Dictionary<string, byte> {
            {"덧", 0x00}, {"덩", 0x01}, {"덫", 0x02}, {"덮", 0x03}, {"데", 0x04}, {"덱", 0x05}, {"덴", 0x06}, {"델", 0x07}, {"뎀", 0x08}, {"뎁", 0x09}, {"뎃", 0x0A}, {"뎄", 0x0B}, {"뎅", 0x0C}, {"뎌", 0x0D}, {"뎐", 0x0E},
            {"뎔", 0x10}, {"뎠", 0x11}, {"뎡", 0x12}, {"뎨", 0x13}, {"뎬", 0x14}, {"도", 0x15}, {"독", 0x16}, {"돈", 0x17}, {"돋", 0x18}, {"돌", 0x19}, {"돎", 0x1A}, {"돔", 0x1B}, {"돕", 0x1C}, {"돗", 0x1D}, {"동", 0x1E},
            {"돛", 0x20}, {"돝", 0x21}, {"돠", 0x22}, {"돤", 0x23}, {"돨", 0x24}, {"돼", 0x25}, {"됐", 0x26}, {"되", 0x27}, {"된", 0x28}, {"될", 0x29}, {"됨", 0x2A}, {"됩", 0x2B}, {"됫", 0x2C}, {"됴", 0x2D}, {"두", 0x2E}, {"둑", 0x2F},
            {"둔", 0x30}, {"둘", 0x31}, {"둠", 0x32}, {"둡", 0x33}, {"둣", 0x34}, {"둥", 0x35}, {"둬", 0x36}, {"뒀", 0x37}, {"뒈", 0x38}, {"뒝", 0x39}, {"뒤", 0x3A}, {"뒨", 0x3B}, {"뒬", 0x3C}, {"뒵", 0x3D}, {"뒷", 0x3E}, {"뒹", 0x3F},
            {"듀", 0x40}, {"듄", 0x41}, {"듈", 0x42}, {"듐", 0x43}, {"듕", 0x44}, {"드", 0x45}, {"득", 0x46}, {"든", 0x47}, {"듣", 0x48}, {"들", 0x49}, {"듦", 0x4A}, {"듬", 0x4B}, {"듭", 0x4C}, {"듯", 0x4D}, {"등", 0x4E}, {"듸", 0x4F},
            {"디", 0x60}, {"딕", 0x61}, {"딘", 0x62}, {"딛", 0x63}, {"딜", 0x64}, {"딤", 0x65}, {"딥", 0x66}, {"딧", 0x67}, {"딨", 0x68}, {"딩", 0x69}, {"딪", 0x6A}, {"따", 0x6B}, {"딱", 0x6C}, {"딴", 0x6D}, {"딸", 0x6E},
            {"땀", 0x70}, {"땁", 0x71}, {"땃", 0x72}, {"땄", 0x73}, {"땅", 0x74}, {"땋", 0x75}, {"때", 0x76}, {"땍", 0x77}, {"땐", 0x78}, {"땔", 0x79}, {"땜", 0x7A}, {"땝", 0x7B}, {"땟", 0x7C}, {"땠", 0x7D}, {"땡", 0x7E},
            {"떠", 0x80}, {"떡", 0x81}, {"떤", 0x82}, {"떨", 0x83}, {"떪", 0x84}, {"떫", 0x85}, {"떰", 0x86}, {"떱", 0x87}, {"떳", 0x88}, {"떴", 0x89}, {"떵", 0x8A}, {"떻", 0x8B}, {"떼", 0x8C}, {"떽", 0x8D}, {"뗀", 0x8E}, {"뗄", 0x8F},
            {"뗌", 0x90}, {"뗍", 0x91}, {"뗏", 0x92}, {"뗐", 0x93}, {"뗑", 0x94}, {"뗘", 0x95}, {"뗬", 0x96}, {"또", 0x97}, {"똑", 0x98}, {"똔", 0x99}, {"똘", 0x9A}, {"똥", 0x9B}, {"똬", 0x9C}, {"똴", 0x9D}, {"뙈", 0x9E}, {"뙤", 0x9F},
            {"뙨", 0xA0}, {"뚜", 0xA1}, {"뚝", 0xA2}, {"뚠", 0xA3}, {"뚤", 0xA4}, {"뚫", 0xA5}, {"뚬", 0xA6}, {"뚱", 0xA7}, {"뛔", 0xA8}, {"뛰", 0xA9}, {"뛴", 0xAA}, {"뛸", 0xAB}, {"뜀", 0xAC}, {"뜁", 0xAD}, {"뜅", 0xAE}, {"뜨", 0xAF},
            {"뜩", 0xB0}, {"뜬", 0xB1}, {"뜯", 0xB2}, {"뜰", 0xB3}, {"뜸", 0xB4}, {"뜹", 0xB5}, {"뜻", 0xB6}, {"띄", 0xB7}, {"띈", 0xB8}, {"띌", 0xB9}, {"띔", 0xBA}, {"띕", 0xBB}, {"띠", 0xBC}, {"띤", 0xBD}, {"띨", 0xBE}, {"띰", 0xBF},
            {"띱", 0xC0}, {"띳", 0xC1}, {"띵", 0xC2}, {"라", 0xC3}, {"락", 0xC4}, {"란", 0xC5}, {"랄", 0xC6}, {"람", 0xC7}, {"랍", 0xC8}, {"랏", 0xC9}, {"랐", 0xCA}, {"랑", 0xCB}, {"랒", 0xCC}, {"랖", 0xCD}, {"랗", 0xCE},
            {"뢔", 0xD0}, {"래", 0xD1}, {"랙", 0xD2}, {"랜", 0xD3}, {"랠", 0xD4}, {"램", 0xD5}, {"랩", 0xD6}, {"랫", 0xD7}, {"랬", 0xD8}, {"랭", 0xD9}, {"랴", 0xDA}, {"략", 0xDB}, {"랸", 0xDC}, {"럇", 0xDD}, {"량", 0xDE}, {"러", 0xDF},
            {"럭", 0xE0}, {"런", 0xE1}, {"럴", 0xE2}, {"럼", 0xE3}, {"럽", 0xE4}, {"럿", 0xE5}, {"렀", 0xE6}, {"렁", 0xE7}, {"렇", 0xE8}, {"레", 0xE9}, {"렉", 0xEA}, {"렌", 0xEB}, {"렐", 0xEC}, {"렘", 0xED}, {"렙", 0xEE}, {"렛", 0xEF},
            {"렝", 0xF0}, {"려", 0xF1}, {"력", 0xF2}, {"련", 0xF3}, {"렬", 0xF4}, {"렴", 0xF5}, {"렵", 0xF6}, {"렷", 0xF7}, {"렸", 0xF8}, {"령", 0xF9}, {"례", 0xFA}, {"롄", 0xFB}, {"롑", 0xFC}, {"롓", 0xFD}, {"로", 0xFE}, {"록", 0xFF},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_4 = new Dictionary<string, byte> {
            {"론", 0x00}, {"롤", 0x01}, {"롬", 0x02}, {"롭", 0x03}, {"롯", 0x04}, {"롱", 0x05}, {"롸", 0x06}, {"롼", 0x07}, {"뢍", 0x08}, {"뢨", 0x09}, {"뢰", 0x0A}, {"뢴", 0x0B}, {"뢸", 0x0C}, {"룀", 0x0D}, {"룁", 0x0E}, {"룃", 0x0F},
            {"룅", 0x10}, {"료", 0x11}, {"룐", 0x12}, {"룔", 0x13}, {"룝", 0x14}, {"룟", 0x15}, {"룡", 0x16}, {"루", 0x17}, {"룩", 0x18}, {"룬", 0x19}, {"룰", 0x1A}, {"룸", 0x1B}, {"룹", 0x1C}, {"룻", 0x1D}, {"룽", 0x1E}, {"뤄", 0x1F},
            {"뤘", 0x20}, {"뤠", 0x21}, {"뤼", 0x22}, {"뤽", 0x23}, {"륀", 0x24}, {"륄", 0x25}, {"륌", 0x26}, {"륏", 0x27}, {"륑", 0x28}, {"류", 0x29}, {"륙", 0x2A}, {"륜", 0x2B}, {"률", 0x2C}, {"륨", 0x2D}, {"륩", 0x2E},
            {"륫", 0x30}, {"륭", 0x31}, {"르", 0x32}, {"륵", 0x33}, {"른", 0x34}, {"를", 0x35}, {"름", 0x36}, {"릅", 0x37}, {"릇", 0x38}, {"릉", 0x39}, {"릊", 0x3A}, {"릍", 0x3B}, {"릎", 0x3C}, {"리", 0x3D}, {"릭", 0x3E},
            {"린", 0x40}, {"릴", 0x41}, {"림", 0x42}, {"립", 0x43}, {"릿", 0x44}, {"링", 0x45}, {"마", 0x46}, {"막", 0x47}, {"만", 0x48}, {"많", 0x49}, {"맏", 0x4A}, {"말", 0x4B}, {"맑", 0x4C}, {"맒", 0x4D}, {"맘", 0x4E}, {"맙", 0x4F},
            {"맛", 0x60}, {"망", 0x61}, {"맞", 0x62}, {"맡", 0x63}, {"맣", 0x64}, {"매", 0x65}, {"맥", 0x66}, {"맨", 0x67}, {"맬", 0x68}, {"맴", 0x69}, {"맵", 0x6A}, {"맷", 0x6B}, {"맸", 0x6C}, {"맹", 0x6D}, {"맺", 0x6E}, {"먀", 0x6F},
            {"먁", 0x70}, {"먈", 0x71}, {"먕", 0x72}, {"머", 0x73}, {"먹", 0x74}, {"먼", 0x75}, {"멀", 0x76}, {"멂", 0x77}, {"멈", 0x78}, {"멉", 0x79}, {"멋", 0x7A}, {"멍", 0x7B}, {"멎", 0x7C}, {"멓", 0x7D}, {"메", 0x7E}, {"멕", 0x7F},
            {"멘", 0x80}, {"멜", 0x81}, {"멤", 0x82}, {"멥", 0x83}, {"멧", 0x84}, {"멨", 0x85}, {"멩", 0x86}, {"며", 0x87}, {"멱", 0x88}, {"면", 0x89}, {"멸", 0x8A}, {"몃", 0x8B}, {"몄", 0x8C}, {"명", 0x8D}, {"몇", 0x8E}, {"몌", 0x8F},
            {"모", 0x90}, {"목", 0x91}, {"몫", 0x92}, {"몬", 0x93}, {"몰", 0x94}, {"몲", 0x95}, {"몸", 0x96}, {"몹", 0x97}, {"못", 0x98}, {"몽", 0x99}, {"뫄", 0x9A}, {"뫈", 0x9B}, {"뫘", 0x9C}, {"뫙", 0x9D}, {"뫼", 0x9E},
            {"묀", 0xA0}, {"묄", 0xA1}, {"묍", 0xA2}, {"묏", 0xA3}, {"묑", 0xA4}, {"묘", 0xA5}, {"묜", 0xA6}, {"묠", 0xA7}, {"묩", 0xA8}, {"묫", 0xA9}, {"무", 0xAA}, {"묵", 0xAB}, {"묶", 0xAC}, {"문", 0xAD}, {"묻", 0xAE},
            {"물", 0xB0}, {"묽", 0xB1}, {"묾", 0xB2}, {"뭄", 0xB3}, {"뭅", 0xB4}, {"뭇", 0xB5}, {"뭉", 0xB6}, {"뭍", 0xB7}, {"뭏", 0xB8}, {"뭐", 0xB9}, {"뭔", 0xBA}, {"뭘", 0xBB}, {"뭡", 0xBC}, {"뭣", 0xBD}, {"뭬", 0xBE}, {"뮈", 0xBF},
            {"뮌", 0xC0}, {"뮐", 0xC1}, {"뮤", 0xC2}, {"뮨", 0xC3}, {"뮬", 0xC4}, {"뮴", 0xC5}, {"뮷", 0xC6}, {"므", 0xC7}, {"믄", 0xC8}, {"믈", 0xC9}, {"믐", 0xCA}, {"믓", 0xCB}, {"미", 0xCC}, {"믹", 0xCD}, {"민", 0xCE}, {"믿", 0xCF},
            {"밀", 0xD0}, {"밂", 0xD1}, {"밈", 0xD2}, {"밉", 0xD3}, {"밋", 0xD4}, {"밌", 0xD5}, {"밍", 0xD6}, {"및", 0xD7}, {"밑", 0xD8}, {"바", 0xD9}, {"박", 0xDA}, {"밖", 0xDB}, {"밗", 0xDC}, {"반", 0xDD}, {"받", 0xDE}, {"발", 0xDF},
            {"밝", 0xE0}, {"밞", 0xE1}, {"밟", 0xE2}, {"밤", 0xE3}, {"밥", 0xE4}, {"밧", 0xE5}, {"방", 0xE6}, {"밭", 0xE7}, {"배", 0xE8}, {"백", 0xE9}, {"밴", 0xEA}, {"밸", 0xEB}, {"뱀", 0xEC}, {"뱁", 0xED}, {"뱃", 0xEE}, {"뱄", 0xEF},
            {"뱅", 0xF0}, {"뱉", 0xF1}, {"뱌", 0xF2}, {"뱍", 0xF3}, {"뱐", 0xF4}, {"뱝", 0xF5}, {"버", 0xF6}, {"벅", 0xF7}, {"번", 0xF8}, {"벋", 0xF9}, {"벌", 0xFA}, {"벎", 0xFB}, {"범", 0xFC}, {"법", 0xFD}, {"벗", 0xFE},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_5 = new Dictionary<string, byte> {
            {"벙", 0x00}, {"벚", 0x01}, {"베", 0x02}, {"벡", 0x03}, {"벤", 0x04}, {"벧", 0x05}, {"벨", 0x06}, {"벰", 0x07}, {"벱", 0x08}, {"벳", 0x09}, {"벵", 0x0A}, {"벴", 0x0B}, {"벼", 0x0C}, {"벽", 0x0D}, {"변", 0x0E},
            {"별", 0x10}, {"볍", 0x11}, {"볏", 0x12}, {"병", 0x13}, {"볕", 0x14}, {"볐", 0x15}, {"볘", 0x16}, {"볜", 0x17}, {"보", 0x18}, {"복", 0x19}, {"본", 0x1A}, {"볼", 0x1B}, {"봄", 0x1C}, {"봅", 0x1D}, {"봇", 0x1E}, {"봉", 0x1F},
            {"볶", 0x20}, {"봐", 0x21}, {"봔", 0x22}, {"봤", 0x23}, {"봬", 0x24}, {"뵀", 0x25}, {"뵈", 0x26}, {"뵉", 0x27}, {"뵌", 0x28}, {"뵐", 0x29}, {"뵘", 0x2A}, {"뵙", 0x2B}, {"뵤", 0x2C}, {"뵨", 0x2D}, {"부", 0x2E}, {"북", 0x2F},
            {"분", 0x30}, {"붇", 0x31}, {"불", 0x32}, {"붐", 0x33}, {"붑", 0x34}, {"붓", 0x35}, {"붕", 0x36}, {"붙", 0x37}, {"붚", 0x38}, {"붉", 0x39}, {"붊", 0x3A}, {"붜", 0x3B}, {"붤", 0x3C}, {"붰", 0x3D}, {"붸", 0x3E}, {"뷔", 0x3F},
            {"뷕", 0x40}, {"뷘", 0x41}, {"뷜", 0x42}, {"뷩", 0x43}, {"뷰", 0x44}, {"뷴", 0x45}, {"뷸", 0x46}, {"븀", 0x47}, {"븃", 0x48}, {"븅", 0x49}, {"브", 0x4A}, {"븍", 0x4B}, {"븐", 0x4C}, {"블", 0x4D}, {"븜", 0x4E}, {"븝", 0x4F},
            {"븟", 0x60}, {"비", 0x61}, {"빅", 0x62}, {"빈", 0x63}, {"빌", 0x64}, {"빎", 0x65}, {"빔", 0x66}, {"빕", 0x67}, {"빗", 0x68}, {"빙", 0x69}, {"빚", 0x6A}, {"빛", 0x6B}, {"빠", 0x6C}, {"빡", 0x6D}, {"빤", 0x6E},
            {"빨", 0x70}, {"빪", 0x71}, {"빰", 0x72}, {"빱", 0x73}, {"빳", 0x74}, {"빴", 0x75}, {"빵", 0x76}, {"빻", 0x77}, {"빼", 0x78}, {"빽", 0x79}, {"뺀", 0x7A}, {"뺄", 0x7B}, {"뺌", 0x7C}, {"뺍", 0x7D}, {"뺏", 0x7E},
            {"뺐", 0x80}, {"뺑", 0x81}, {"뺘", 0x82}, {"뺙", 0x83}, {"뺨", 0x84}, {"뻐", 0x85}, {"뻑", 0x86}, {"뻔", 0x87}, {"뻗", 0x88}, {"뻘", 0x89}, {"뻠", 0x8A}, {"뻣", 0x8B}, {"뻤", 0x8C}, {"뻥", 0x8D}, {"뻬", 0x8E}, {"뼁", 0x8F},
            {"뼈", 0x90}, {"뼉", 0x91}, {"뼘", 0x92}, {"뼙", 0x93}, {"뼛", 0x94}, {"뼜", 0x95}, {"뼝", 0x96}, {"뽀", 0x97}, {"뽁", 0x98}, {"뽄", 0x99}, {"뽈", 0x9A}, {"뽐", 0x9B}, {"뽑", 0x9C}, {"뽕", 0x9D}, {"뾔", 0x9E}, {"뾰", 0x9F},
            {"뿅", 0xA0}, {"뿌", 0xA1}, {"뿍", 0xA2}, {"뿐", 0xA3}, {"뿔", 0xA4}, {"뿜", 0xA5}, {"뿟", 0xA6}, {"뿡", 0xA7}, {"쀼", 0xA8}, {"쁑", 0xA9}, {"쁘", 0xAA}, {"쁜", 0xAB}, {"쁠", 0xAC}, {"쁨", 0xAD}, {"쁩", 0xAE}, {"삐", 0xAF},
            {"삑", 0xB0}, {"삔", 0xB1}, {"삘", 0xB2}, {"삠", 0xB3}, {"삡", 0xB4}, {"삣", 0xB5}, {"삥", 0xB6}, {"사", 0xB7}, {"삭", 0xB8}, {"삯", 0xB9}, {"산", 0xBA}, {"삳", 0xBB}, {"살", 0xBC}, {"삵", 0xBD}, {"삶", 0xBE}, {"삼", 0xBF},
            {"삽", 0xC0}, {"삿", 0xC1}, {"샀", 0xC2}, {"상", 0xC3}, {"샅", 0xC4}, {"새", 0xC5}, {"색", 0xC6}, {"샌", 0xC7}, {"샐", 0xC8}, {"샘", 0xC9}, {"샙", 0xCA}, {"샛", 0xCB}, {"샜", 0xCC}, {"생", 0xCD}, {"샤", 0xCE},
            {"샥", 0xD0}, {"샨", 0xD1}, {"샬", 0xD2}, {"샴", 0xD3}, {"샵", 0xD4}, {"샷", 0xD5}, {"샹", 0xD6}, {"섀", 0xD7}, {"섄", 0xD8}, {"섈", 0xD9}, {"섐", 0xDA}, {"섕", 0xDB}, {"서", 0xDC}, {"석", 0xDD}, {"섞", 0xDE},
            {"섟", 0xE0}, {"선", 0xE1}, {"섣", 0xE2}, {"설", 0xE3}, {"섦", 0xE4}, {"섧", 0xE5}, {"섬", 0xE6}, {"섭", 0xE7}, {"섯", 0xE8}, {"섰", 0xE9}, {"성", 0xEA}, {"섶", 0xEB}, {"세", 0xEC}, {"섹", 0xED}, {"센", 0xEE}, {"셀", 0xEF},
            {"셈", 0xF0}, {"셉", 0xF1}, {"셋", 0xF2}, {"셌", 0xF3}, {"셍", 0xF4}, {"셔", 0xF5}, {"셕", 0xF6}, {"션", 0xF7}, {"셜", 0xF8}, {"셤", 0xF9}, {"셥", 0xFA}, {"셧", 0xFB}, {"셨", 0xFC}, {"셩", 0xFD}, {"셰", 0xFE}, {"셴", 0xFF},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_6 = new Dictionary<string, byte> {
            {"셸", 0x00}, {"솅", 0x01}, {"소", 0x02}, {"속", 0x03}, {"솎", 0x04}, {"손", 0x05}, {"솔", 0x06}, {"솖", 0x07}, {"솜", 0x08}, {"솝", 0x09}, {"솟", 0x0A}, {"송", 0x0B}, {"솥", 0x0C}, {"솨", 0x0D}, {"솩", 0x0E}, {"솬", 0x0F},
            {"솰", 0x10}, {"솽", 0x11}, {"쇄", 0x12}, {"쇈", 0x13}, {"쇌", 0x14}, {"쇔", 0x15}, {"쇗", 0x16}, {"쇘", 0x17}, {"쇠", 0x18}, {"쇤", 0x19}, {"쇨", 0x1A}, {"쇰", 0x1B}, {"쇱", 0x1C}, {"쇳", 0x1D}, {"쇼", 0x1E}, {"쇽", 0x1F},
            {"숀", 0x20}, {"숄", 0x21}, {"숌", 0x22}, {"숍", 0x23}, {"숏", 0x24}, {"숑", 0x25}, {"수", 0x26}, {"숙", 0x27}, {"순", 0x28}, {"숟", 0x29}, {"술", 0x2A}, {"숨", 0x2B}, {"숩", 0x2C}, {"숫", 0x2D}, {"숭", 0x2E}, {"쌰", 0x2F},
            {"쎼", 0x30}, {"숯", 0x31}, {"숱", 0x32}, {"숲", 0x33}, {"숴", 0x34}, {"쉈", 0x35}, {"쉐", 0x36}, {"쉑", 0x37}, {"쉔", 0x38}, {"쉘", 0x39}, {"쉠", 0x3A}, {"쉥", 0x3B}, {"쉬", 0x3C}, {"쉭", 0x3D}, {"쉰", 0x3E}, {"쉴", 0x3F},
            {"쉼", 0x40}, {"쉽", 0x41}, {"쉿", 0x42}, {"슁", 0x43}, {"슈", 0x44}, {"슉", 0x45}, {"슐", 0x46}, {"슘", 0x47}, {"슛", 0x48}, {"슝", 0x49}, {"스", 0x4A}, {"슥", 0x4B}, {"슨", 0x4C}, {"슬", 0x4D}, {"슭", 0x4E}, {"슴", 0x4F},
            {"습", 0x60}, {"슷", 0x61}, {"승", 0x62}, {"시", 0x63}, {"식", 0x64}, {"신", 0x65}, {"싣", 0x66}, {"실", 0x67}, {"싫", 0x68}, {"심", 0x69}, {"십", 0x6A}, {"싯", 0x6B}, {"싱", 0x6C}, {"싶", 0x6D}, {"싸", 0x6E}, {"싹", 0x6F},
            {"싻", 0x70}, {"싼", 0x71}, {"쌀", 0x72}, {"쌈", 0x73}, {"쌉", 0x74}, {"쌌", 0x75}, {"쌍", 0x76}, {"쌓", 0x77}, {"쌔", 0x78}, {"쌕", 0x79}, {"쌘", 0x7A}, {"쌜", 0x7B}, {"쌤", 0x7C}, {"쌥", 0x7D}, {"쌨", 0x7E}, {"쌩", 0x7F},
            {"썅", 0x80}, {"써", 0x81}, {"썩", 0x82}, {"썬", 0x83}, {"썰", 0x84}, {"썲", 0x85}, {"썸", 0x86}, {"썹", 0x87}, {"썼", 0x88}, {"썽", 0x89}, {"쎄", 0x8A}, {"쎈", 0x8B}, {"쎌", 0x8C}, {"쏀", 0x8D}, {"쏘", 0x8E}, {"쏙", 0x8F},
            {"쏜", 0x90}, {"쏟", 0x91}, {"쏠", 0x92}, {"쏢", 0x93}, {"쏨", 0x94}, {"쏩", 0x95}, {"쏭", 0x96}, {"쏴", 0x97}, {"쏵", 0x98}, {"쏸", 0x99}, {"쐈", 0x9A}, {"쐐", 0x9B}, {"쐤", 0x9C}, {"쐬", 0x9D}, {"쐰", 0x9E},
            {"쓔", 0xA0}, {"쐴", 0xA1}, {"쐼", 0xA2}, {"쐽", 0xA3}, {"쑈", 0xA4}, {"쑤", 0xA5}, {"쑥", 0xA6}, {"쑨", 0xA7}, {"쑬", 0xA8}, {"쑴", 0xA9}, {"쑵", 0xAA}, {"쑹", 0xAB}, {"쒀", 0xAC}, {"쒔", 0xAD}, {"쒜", 0xAE}, {"쒸", 0xAF},
            {"쒼", 0xB0}, {"쓩", 0xB1}, {"쓰", 0xB2}, {"쓱", 0xB3}, {"쓴", 0xB4}, {"쓸", 0xB5}, {"쓺", 0xB6}, {"쓿", 0xB7}, {"씀", 0xB8}, {"씁", 0xB9}, {"씌", 0xBA}, {"씐", 0xBB}, {"씔", 0xBC}, {"씜", 0xBD}, {"씨", 0xBE}, {"씩", 0xBF},
            {"씬", 0xC0}, {"씰", 0xC1}, {"씸", 0xC2}, {"씹", 0xC3}, {"씻", 0xC4}, {"씽", 0xC5}, {"아", 0xC6}, {"악", 0xC7}, {"안", 0xC8}, {"앉", 0xC9}, {"않", 0xCA}, {"알", 0xCB}, {"앍", 0xCC}, {"앎", 0xCD}, {"앓", 0xCE}, {"암", 0xCF},
            {"압", 0xD0}, {"앗", 0xD1}, {"았", 0xD2}, {"앙", 0xD3}, {"앝", 0xD4}, {"앞", 0xD5}, {"애", 0xD6}, {"액", 0xD7}, {"앤", 0xD8}, {"앨", 0xD9}, {"앰", 0xDA}, {"앱", 0xDB}, {"앳", 0xDC}, {"앴", 0xDD}, {"앵", 0xDE}, {"야", 0xDF},
            {"약", 0xE0}, {"얀", 0xE1}, {"얄", 0xE2}, {"얇", 0xE3}, {"얌", 0xE4}, {"얍", 0xE5}, {"얏", 0xE6}, {"양", 0xE7}, {"얕", 0xE8}, {"얗", 0xE9}, {"얘", 0xEA}, {"얜", 0xEB}, {"얠", 0xEC}, {"얩", 0xED}, {"어", 0xEE}, {"억", 0xEF},
            {"언", 0xF0}, {"얹", 0xF1}, {"얻", 0xF2}, {"얼", 0xF3}, {"얽", 0xF4}, {"얾", 0xF5}, {"엄", 0xF6}, {"업", 0xF7}, {"없", 0xF8}, {"엇", 0xF9}, {"었", 0xFA}, {"엉", 0xFB}, {"엊", 0xFC}, {"엌", 0xFD}, {"엎", 0xFE},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_7 = new Dictionary<string, byte> {
            {"에", 0x00}, {"엑", 0x01}, {"엔", 0x02}, {"엘", 0x03}, {"엠", 0x04}, {"엡", 0x05}, {"엣", 0x06}, {"엥", 0x07}, {"여", 0x08}, {"역", 0x09}, {"엮", 0x0A}, {"연", 0x0B}, {"열", 0x0C}, {"엶", 0x0D}, {"엷", 0x0E},
            {"염", 0x10}, {"엽", 0x11}, {"엾", 0x12}, {"엿", 0x13}, {"였", 0x14}, {"영", 0x15}, {"옅", 0x16}, {"옆", 0x17}, {"옇", 0x18}, {"예", 0x19}, {"옌", 0x1A}, {"옐", 0x1B}, {"옘", 0x1C}, {"옙", 0x1D}, {"옛", 0x1E}, {"옜", 0x1F},
            {"오", 0x20}, {"옥", 0x21}, {"온", 0x22}, {"올", 0x23}, {"옭", 0x24}, {"옮", 0x25}, {"옰", 0x26}, {"옳", 0x27}, {"옴", 0x28}, {"옵", 0x29}, {"옷", 0x2A}, {"옹", 0x2B}, {"옻", 0x2C}, {"와", 0x2D}, {"왁", 0x2E}, {"완", 0x2F},
            {"왈", 0x30}, {"왐", 0x31}, {"왑", 0x32}, {"왓", 0x33}, {"왔", 0x34}, {"왕", 0x35}, {"왜", 0x36}, {"왝", 0x37}, {"왠", 0x38}, {"왬", 0x39}, {"왯", 0x3A}, {"왱", 0x3B}, {"외", 0x3C}, {"왹", 0x3D}, {"왼", 0x3E}, {"욀", 0x3F},
            {"욈", 0x40}, {"욉", 0x41}, {"욋", 0x42}, {"욍", 0x43}, {"요", 0x44}, {"욕", 0x45}, {"욘", 0x46}, {"욜", 0x47}, {"욤", 0x48}, {"욥", 0x49}, {"욧", 0x4A}, {"용", 0x4B}, {"우", 0x4C}, {"욱", 0x4D}, {"운", 0x4E}, {"울", 0x4F},
            {"욹", 0x60}, {"욺", 0x61}, {"움", 0x62}, {"웁", 0x63}, {"웃", 0x64}, {"웅", 0x65}, {"워", 0x66}, {"웍", 0x67}, {"원", 0x68}, {"월", 0x69}, {"웜", 0x6A}, {"웝", 0x6B}, {"웠", 0x6C}, {"웡", 0x6D}, {"웨", 0x6E},
            {"웩", 0x70}, {"웬", 0x71}, {"웰", 0x72}, {"웸", 0x73}, {"웹", 0x74}, {"웽", 0x75}, {"위", 0x76}, {"윅", 0x77}, {"윈", 0x78}, {"윌", 0x79}, {"윔", 0x7A}, {"윕", 0x7B}, {"윗", 0x7C}, {"윙", 0x7D}, {"유", 0x7E},
            {"육", 0x80}, {"윤", 0x81}, {"율", 0x82}, {"윰", 0x83}, {"윱", 0x84}, {"윳", 0x85}, {"융", 0x86}, {"윷", 0x87}, {"으", 0x88}, {"윽", 0x89}, {"은", 0x8A}, {"을", 0x8B}, {"읆", 0x8C}, {"음", 0x8D}, {"읍", 0x8E}, {"읏", 0x8F},
            {"응", 0x90}, {"읒", 0x91}, {"읓", 0x92}, {"읔", 0x93}, {"읕", 0x94}, {"읖", 0x95}, {"읗", 0x96}, {"의", 0x97}, {"읜", 0x98}, {"읠", 0x99}, {"읨", 0x9A}, {"읫", 0x9B}, {"이", 0x9C}, {"익", 0x9D}, {"인", 0x9E}, {"일", 0x9F},
            {"읽", 0xA0}, {"읾", 0xA1}, {"잃", 0xA2}, {"임", 0xA3}, {"입", 0xA4}, {"잇", 0xA5}, {"있", 0xA6}, {"잉", 0xA7}, {"잊", 0xA8}, {"잎", 0xA9}, {"자", 0xAA}, {"작", 0xAB}, {"잔", 0xAC}, {"잖", 0xAD}, {"잗", 0xAE}, {"잘", 0xAF},
            {"잚", 0xB0}, {"잠", 0xB1}, {"잡", 0xB2}, {"잣", 0xB3}, {"잤", 0xB4}, {"장", 0xB5}, {"잦", 0xB6}, {"재", 0xB7}, {"잭", 0xB8}, {"잰", 0xB9}, {"잴", 0xBA}, {"잼", 0xBB}, {"잽", 0xBC}, {"잿", 0xBD}, {"쟀", 0xBE}, {"쟁", 0xBF},
            {"쟈", 0xC0}, {"쟉", 0xC1}, {"쟌", 0xC2}, {"쟎", 0xC3}, {"쟐", 0xC4}, {"쟘", 0xC5}, {"쟝", 0xC6}, {"쟤", 0xC7}, {"쟨", 0xC8}, {"쟬", 0xC9}, {"저", 0xCA}, {"적", 0xCB}, {"전", 0xCC}, {"절", 0xCD}, {"젊", 0xCE},
            {"점", 0xD0}, {"접", 0xD1}, {"젓", 0xD2}, {"정", 0xD3}, {"젖", 0xD4}, {"제", 0xD5}, {"젝", 0xD6}, {"젠", 0xD7}, {"젤", 0xD8}, {"젬", 0xD9}, {"젭", 0xDA}, {"젯", 0xDB}, {"젱", 0xDC}, {"져", 0xDD}, {"젼", 0xDE},
            {"졀", 0xE0}, {"졈", 0xE1}, {"졉", 0xE2}, {"졌", 0xE3}, {"졍", 0xE4}, {"졔", 0xE5}, {"조", 0xE6}, {"족", 0xE7}, {"존", 0xE8}, {"졸", 0xE9}, {"졺", 0xEA}, {"좀", 0xEB}, {"좁", 0xEC}, {"좃", 0xED}, {"종", 0xEE}, {"좆", 0xEF},
            {"좇", 0xF0}, {"좋", 0xF1}, {"좌", 0xF2}, {"좍", 0xF3}, {"좔", 0xF4}, {"좝", 0xF5}, {"좟", 0xF6}, {"좡", 0xF7}, {"좨", 0xF8}, {"좼", 0xF9}, {"좽", 0xFA}, {"죄", 0xFB}, {"죈", 0xFC}, {"죌", 0xFD}, {"죔", 0xFE}, {"죕", 0xFF},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_8 = new Dictionary<string, byte> {
            {"죗", 0x00}, {"죙", 0x01}, {"죠", 0x02}, {"죡", 0x03}, {"죤", 0x04}, {"죵", 0x05}, {"주", 0x06}, {"죽", 0x07}, {"준", 0x08}, {"줄", 0x09}, {"줅", 0x0A}, {"줆", 0x0B}, {"줌", 0x0C}, {"줍", 0x0D}, {"줏", 0x0E}, {"중", 0x0F},
            {"줘", 0x10}, {"줬", 0x11}, {"줴", 0x12}, {"쥐", 0x13}, {"쥑", 0x14}, {"쥔", 0x15}, {"쥘", 0x16}, {"쥠", 0x17}, {"쥡", 0x18}, {"쥣", 0x19}, {"쥬", 0x1A}, {"쥰", 0x1B}, {"쥴", 0x1C}, {"쥼", 0x1D}, {"즈", 0x1E}, {"즉", 0x1F},
            {"즌", 0x20}, {"즐", 0x21}, {"즘", 0x22}, {"즙", 0x23}, {"즛", 0x24}, {"증", 0x25}, {"지", 0x26}, {"직", 0x27}, {"진", 0x28}, {"짇", 0x29}, {"질", 0x2A}, {"짊", 0x2B}, {"짐", 0x2C}, {"집", 0x2D}, {"짓", 0x2E},
            {"쬬", 0x30}, {"징", 0x31}, {"짖", 0x32}, {"짙", 0x33}, {"짚", 0x34}, {"짜", 0x35}, {"짝", 0x36}, {"짠", 0x37}, {"짢", 0x38}, {"짤", 0x39}, {"짧", 0x3A}, {"짬", 0x3B}, {"짭", 0x3C}, {"짯", 0x3D}, {"짰", 0x3E}, {"짱", 0x3F},
            {"째", 0x40}, {"짹", 0x41}, {"짼", 0x42}, {"쨀", 0x43}, {"쨈", 0x44}, {"쨉", 0x45}, {"쨋", 0x46}, {"쨌", 0x47}, {"쨍", 0x48}, {"쨔", 0x49}, {"쨘", 0x4A}, {"쨩", 0x4B}, {"쩌", 0x4C}, {"쩍", 0x4D}, {"쩐", 0x4E}, {"쩔", 0x4F},
            {"쩜", 0x60}, {"쩝", 0x61}, {"쩟", 0x62}, {"쩠", 0x63}, {"쩡", 0x64}, {"쩨", 0x65}, {"쩽", 0x66}, {"쪄", 0x67}, {"쪘", 0x68}, {"쪼", 0x69}, {"쪽", 0x6A}, {"쫀", 0x6B}, {"쫄", 0x6C}, {"쫌", 0x6D}, {"쫍", 0x6E}, {"쫏", 0x6F},
            {"쫑", 0x70}, {"쫓", 0x71}, {"쫘", 0x72}, {"쫙", 0x73}, {"쫠", 0x74}, {"쫬", 0x75}, {"쫴", 0x76}, {"쬈", 0x77}, {"쬐", 0x78}, {"쬔", 0x79}, {"쬘", 0x7A}, {"쬠", 0x7B}, {"쬡", 0x7C}, {"쭁", 0x7D}, {"쭈", 0x7E}, {"쭉", 0x7F},
            {"쭌", 0x80}, {"쭐", 0x81}, {"쭘", 0x82}, {"쭙", 0x83}, {"쭝", 0x84}, {"쭤", 0x85}, {"쭸", 0x86}, {"쭹", 0x87}, {"쮜", 0x88}, {"쮸", 0x89}, {"쯔", 0x8A}, {"쯤", 0x8B}, {"쯧", 0x8C}, {"쯩", 0x8D}, {"찌", 0x8E}, {"찍", 0x8F},
            {"찐", 0x90}, {"찔", 0x91}, {"찜", 0x92}, {"찝", 0x93}, {"찡", 0x94}, {"찢", 0x95}, {"찧", 0x96}, {"차", 0x97}, {"착", 0x98}, {"찬", 0x99}, {"찮", 0x9A}, {"찰", 0x9B}, {"참", 0x9C}, {"찹", 0x9D}, {"찻", 0x9E},
            {"찼", 0xA0}, {"창", 0xA1}, {"찾", 0xA2}, {"채", 0xA3}, {"책", 0xA4}, {"챈", 0xA5}, {"챌", 0xA6}, {"챔", 0xA7}, {"챕", 0xA8}, {"챗", 0xA9}, {"챘", 0xAA}, {"챙", 0xAB}, {"챠", 0xAC}, {"챤", 0xAD}, {"챦", 0xAE},
            {"챨", 0xB0}, {"챰", 0xB1}, {"챵", 0xB2}, {"처", 0xB3}, {"척", 0xB4}, {"천", 0xB5}, {"철", 0xB6}, {"첨", 0xB7}, {"첩", 0xB8}, {"첫", 0xB9}, {"첬", 0xBA}, {"청", 0xBB}, {"체", 0xBC}, {"첵", 0xBD}, {"첸", 0xBE}, {"첼", 0xBF},
            {"쳄", 0xC0}, {"쳅", 0xC1}, {"쳇", 0xC2}, {"쳉", 0xC3}, {"쳐", 0xC4}, {"쳔", 0xC5}, {"쳤", 0xC6}, {"쳬", 0xC7}, {"쳰", 0xC8}, {"촁", 0xC9}, {"초", 0xCA}, {"촉", 0xCB}, {"촌", 0xCC}, {"촐", 0xCD}, {"촘", 0xCE}, {"촙", 0xCF},
            {"촛", 0xD0}, {"총", 0xD1}, {"촤", 0xD2}, {"촨", 0xD3}, {"촬", 0xD4}, {"촹", 0xD5}, {"최", 0xD6}, {"쵠", 0xD7}, {"쵤", 0xD8}, {"쵬", 0xD9}, {"쵭", 0xDA}, {"쵯", 0xDB}, {"쵱", 0xDC}, {"쵸", 0xDD}, {"춈", 0xDE}, {"추", 0xDF},
            {"축", 0xE0}, {"춘", 0xE1}, {"출", 0xE2}, {"춤", 0xE3}, {"춥", 0xE4}, {"춧", 0xE5}, {"충", 0xE6}, {"춰", 0xE7}, {"췄", 0xE8}, {"췌", 0xE9}, {"췐", 0xEA}, {"취", 0xEB}, {"췬", 0xEC}, {"췰", 0xED}, {"췸", 0xEE}, {"췹", 0xEF},
            {"췻", 0xF0}, {"췽", 0xF1}, {"츄", 0xF2}, {"츈", 0xF3}, {"츌", 0xF4}, {"츔", 0xF5}, {"츙", 0xF6}, {"츠", 0xF7}, {"측", 0xF8}, {"츤", 0xF9}, {"츨", 0xFA}, {"츰", 0xFB}, {"츱", 0xFC}, {"츳", 0xFD}, {"층", 0xFE},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_9 = new Dictionary<string, byte> {
            {"치", 0x00}, {"칙", 0x01}, {"친", 0x02}, {"칟", 0x03}, {"칠", 0x04}, {"칡", 0x05}, {"침", 0x06}, {"칩", 0x07}, {"칫", 0x08}, {"칭", 0x09}, {"카", 0x0A}, {"칵", 0x0B}, {"칸", 0x0C}, {"칼", 0x0D}, {"캄", 0x0E},
            {"캅", 0x10}, {"캇", 0x11}, {"캉", 0x12}, {"캐", 0x13}, {"캑", 0x14}, {"캔", 0x15}, {"캘", 0x16}, {"캠", 0x17}, {"캡", 0x18}, {"캣", 0x19}, {"캤", 0x1A}, {"캥", 0x1B}, {"캬", 0x1C}, {"캭", 0x1D}, {"컁", 0x1E}, {"커", 0x1F},
            {"컥", 0x20}, {"컨", 0x21}, {"컫", 0x22}, {"컬", 0x23}, {"컴", 0x24}, {"컵", 0x25}, {"컷", 0x26}, {"컸", 0x27}, {"컹", 0x28}, {"케", 0x29}, {"켁", 0x2A}, {"켄", 0x2B}, {"켈", 0x2C}, {"켐", 0x2D}, {"켑", 0x2E}, {"켓", 0x2F},
            {"켕", 0x30}, {"켜", 0x31}, {"켠", 0x32}, {"켤", 0x33}, {"켬", 0x34}, {"켭", 0x35}, {"켯", 0x36}, {"켰", 0x37}, {"켱", 0x38}, {"켸", 0x39}, {"코", 0x3A}, {"콕", 0x3B}, {"콘", 0x3C}, {"콜", 0x3D}, {"콤", 0x3E}, {"콥", 0x3F},
            {"콧", 0x40}, {"콩", 0x41}, {"콰", 0x42}, {"콱", 0x43}, {"콴", 0x44}, {"콸", 0x45}, {"쾀", 0x46}, {"쾅", 0x47}, {"쾌", 0x48}, {"쾡", 0x49}, {"쾨", 0x4A}, {"쾰", 0x4B}, {"쿄", 0x4C}, {"쿠", 0x4D}, {"쿡", 0x4E}, {"쿤", 0x4F},
            {"쿨", 0x60}, {"쿰", 0x61}, {"쿱", 0x62}, {"쿳", 0x63}, {"쿵", 0x64}, {"쿼", 0x65}, {"퀀", 0x66}, {"퀄", 0x67}, {"퀑", 0x68}, {"퀘", 0x69}, {"퀭", 0x6A}, {"퀴", 0x6B}, {"퀵", 0x6C}, {"퀸", 0x6D}, {"퀼", 0x6E},
            {"큄", 0x70}, {"큅", 0x71}, {"큇", 0x72}, {"큉", 0x73}, {"큐", 0x74}, {"큔", 0x75}, {"큘", 0x76}, {"큠", 0x77}, {"크", 0x78}, {"큭", 0x79}, {"큰", 0x7A}, {"클", 0x7B}, {"큼", 0x7C}, {"큽", 0x7D}, {"킁", 0x7E},
            {"키", 0x80}, {"킥", 0x81}, {"킨", 0x82}, {"킬", 0x83}, {"킴", 0x84}, {"킵", 0x85}, {"킷", 0x86}, {"킹", 0x87}, {"타", 0x88}, {"탁", 0x89}, {"탄", 0x8A}, {"탈", 0x8B}, {"탉", 0x8C}, {"탐", 0x8D}, {"탑", 0x8E}, {"탓", 0x8F},
            {"탔", 0x90}, {"탕", 0x91}, {"태", 0x92}, {"택", 0x93}, {"탠", 0x94}, {"탤", 0x95}, {"탬", 0x96}, {"탭", 0x97}, {"탯", 0x98}, {"탰", 0x99}, {"탱", 0x9A}, {"탸", 0x9B}, {"턍", 0x9C}, {"터", 0x9D}, {"턱", 0x9E}, {"턴", 0x9F},
            {"털", 0xA0}, {"턺", 0xA1}, {"텀", 0xA2}, {"텁", 0xA3}, {"텃", 0xA4}, {"텄", 0xA5}, {"텅", 0xA6}, {"테", 0xA7}, {"텍", 0xA8}, {"텐", 0xA9}, {"텔", 0xAA}, {"템", 0xAB}, {"텝", 0xAC}, {"텟", 0xAD}, {"텡", 0xAE}, {"텨", 0xAF},
            {"텬", 0xB0}, {"텼", 0xB1}, {"톄", 0xB2}, {"톈", 0xB3}, {"토", 0xB4}, {"톡", 0xB5}, {"톤", 0xB6}, {"톨", 0xB7}, {"톰", 0xB8}, {"톱", 0xB9}, {"톳", 0xBA}, {"통", 0xBB}, {"톺", 0xBC}, {"톼", 0xBD}, {"퇀", 0xBE}, {"퇘", 0xBF},
            {"퇴", 0xC0}, {"퇸", 0xC1}, {"툇", 0xC2}, {"툉", 0xC3}, {"툐", 0xC4}, {"투", 0xC5}, {"툭", 0xC6}, {"툰", 0xC7}, {"툴", 0xC8}, {"툼", 0xC9}, {"툽", 0xCA}, {"툿", 0xCB}, {"퉁", 0xCC}, {"퉈", 0xCD}, {"퉜", 0xCE},
            {"퉤", 0xD0}, {"튀", 0xD1}, {"튁", 0xD2}, {"튄", 0xD3}, {"튈", 0xD4}, {"튐", 0xD5}, {"튑", 0xD6}, {"튕", 0xD7}, {"튜", 0xD8}, {"튠", 0xD9}, {"튤", 0xDA}, {"튬", 0xDB}, {"튱", 0xDC}, {"트", 0xDD}, {"특", 0xDE},
            {"튼", 0xE0}, {"튿", 0xE1}, {"틀", 0xE2}, {"틂", 0xE3}, {"틈", 0xE4}, {"틉", 0xE5}, {"틋", 0xE6}, {"틔", 0xE7}, {"틘", 0xE8}, {"틜", 0xE9}, {"틤", 0xEA}, {"틥", 0xEB}, {"티", 0xEC}, {"틱", 0xED}, {"틴", 0xEE}, {"틸", 0xEF},
            {"팀", 0xF0}, {"팁", 0xF1}, {"팃", 0xF2}, {"팅", 0xF3}, {"파", 0xF4}, {"팍", 0xF5}, {"팎", 0xF6}, {"판", 0xF7}, {"팔", 0xF8}, {"팖", 0xF9}, {"팜", 0xFA}, {"팝", 0xFB}, {"팟", 0xFC}, {"팠", 0xFD}, {"팡", 0xFE}, {"팥", 0xFF},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_A = new Dictionary<string, byte> {
            {"패", 0x00}, {"팩", 0x01}, {"팬", 0x02}, {"팰", 0x03}, {"팸", 0x04}, {"팹", 0x05}, {"팻", 0x06}, {"팼", 0x07}, {"팽", 0x08}, {"퍄", 0x09}, {"퍅", 0x0A}, {"퍼", 0x0B}, {"퍽", 0x0C}, {"펀", 0x0D}, {"펄", 0x0E}, {"펌", 0x0F},
            {"펍", 0x10}, {"펏", 0x11}, {"펐", 0x12}, {"펑", 0x13}, {"페", 0x14}, {"펙", 0x15}, {"펜", 0x16}, {"펠", 0x17}, {"펨", 0x18}, {"펩", 0x19}, {"펫", 0x1A}, {"펭", 0x1B}, {"펴", 0x1C}, {"편", 0x1D}, {"펼", 0x1E}, {"폄", 0x1F},
            {"폅", 0x20}, {"폈", 0x21}, {"평", 0x22}, {"폐", 0x23}, {"폘", 0x24}, {"폡", 0x25}, {"폣", 0x26}, {"포", 0x27}, {"폭", 0x28}, {"폰", 0x29}, {"폴", 0x2A}, {"폼", 0x2B}, {"폽", 0x2C}, {"폿", 0x2D}, {"퐁", 0x2E},
            {"퐈", 0x30}, {"퐝", 0x31}, {"푀", 0x32}, {"푄", 0x33}, {"표", 0x34}, {"푠", 0x35}, {"푤", 0x36}, {"푭", 0x37}, {"푯", 0x38}, {"푸", 0x39}, {"푹", 0x3A}, {"푼", 0x3B}, {"푿", 0x3C}, {"풀", 0x3D}, {"풂", 0x3E},
            {"품", 0x40}, {"풉", 0x41}, {"풋", 0x42}, {"풍", 0x43}, {"풔", 0x44}, {"풩", 0x45}, {"퓌", 0x46}, {"퓐", 0x47}, {"퓔", 0x48}, {"퓜", 0x49}, {"퓟", 0x4A}, {"퓨", 0x4B}, {"퓬", 0x4C}, {"퓰", 0x4D}, {"퓸", 0x4E}, {"퓻", 0x4F},
            {"퓽", 0x60}, {"프", 0x61}, {"픈", 0x62}, {"플", 0x63}, {"픔", 0x64}, {"픕", 0x65}, {"픗", 0x66}, {"피", 0x67}, {"픽", 0x68}, {"핀", 0x69}, {"필", 0x6A}, {"핌", 0x6B}, {"핍", 0x6C}, {"핏", 0x6D}, {"핑", 0x6E}, {"하", 0x6F},
            {"학", 0x70}, {"한", 0x71}, {"할", 0x72}, {"핥", 0x73}, {"함", 0x74}, {"합", 0x75}, {"핫", 0x76}, {"항", 0x77}, {"해", 0x78}, {"핵", 0x79}, {"핸", 0x7A}, {"핼", 0x7B}, {"햄", 0x7C}, {"햅", 0x7D}, {"햇", 0x7E}, {"했", 0x7F},
            {"행", 0x80}, {"햐", 0x81}, {"향", 0x82}, {"허", 0x83}, {"헉", 0x84}, {"헌", 0x85}, {"헐", 0x86}, {"헒", 0x87}, {"험", 0x88}, {"헙", 0x89}, {"헛", 0x8A}, {"헝", 0x8B}, {"헤", 0x8C}, {"헥", 0x8D}, {"헨", 0x8E}, {"헬", 0x8F},
            {"헴", 0x90}, {"헵", 0x91}, {"헷", 0x92}, {"헹", 0x93}, {"혀", 0x94}, {"혁", 0x95}, {"현", 0x96}, {"혈", 0x97}, {"혐", 0x98}, {"협", 0x99}, {"혓", 0x9A}, {"혔", 0x9B}, {"형", 0x9C}, {"혜", 0x9D}, {"혠", 0x9E},
            {"혤", 0xA0}, {"혭", 0xA1}, {"호", 0xA2}, {"혹", 0xA3}, {"혼", 0xA4}, {"홀", 0xA5}, {"홅", 0xA6}, {"홈", 0xA7}, {"홉", 0xA8}, {"홋", 0xA9}, {"홍", 0xAA}, {"홑", 0xAB}, {"화", 0xAC}, {"확", 0xAD}, {"환", 0xAE},
            {"활", 0xB0}, {"홧", 0xB1}, {"황", 0xB2}, {"홰", 0xB3}, {"홱", 0xB4}, {"홴", 0xB5}, {"횃", 0xB6}, {"횅", 0xB7}, {"회", 0xB8}, {"획", 0xB9}, {"횐", 0xBA}, {"횔", 0xBB}, {"횝", 0xBC}, {"횟", 0xBD}, {"횡", 0xBE}, {"효", 0xBF},
            {"횬", 0xC0}, {"횰", 0xC1}, {"횹", 0xC2}, {"횻", 0xC3}, {"후", 0xC4}, {"훅", 0xC5}, {"훈", 0xC6}, {"훌", 0xC7}, {"훑", 0xC8}, {"훔", 0xC9}, {"훗", 0xCA}, {"훙", 0xCB}, {"훠", 0xCC}, {"훤", 0xCD}, {"훨", 0xCE}, {"훰", 0xCF},
            {"훵", 0xD0}, {"훼", 0xD1}, {"훽", 0xD2}, {"휀", 0xD3}, {"휄", 0xD4}, {"휑", 0xD5}, {"휘", 0xD6}, {"휙", 0xD7}, {"휜", 0xD8}, {"휠", 0xD9}, {"휨", 0xDA}, {"휩", 0xDB}, {"휫", 0xDC}, {"휭", 0xDD}, {"휴", 0xDE}, {"휵", 0xDF},
            {"휸", 0xE0}, {"휼", 0xE1}, {"흄", 0xE2}, {"흇", 0xE3}, {"흉", 0xE4}, {"흐", 0xE5}, {"흑", 0xE6}, {"흔", 0xE7}, {"흖", 0xE8}, {"흗", 0xE9}, {"흘", 0xEA}, {"흙", 0xEB}, {"흠", 0xEC}, {"흡", 0xED}, {"흣", 0xEE}, {"흥", 0xEF},
            {"흩", 0xF0}, {"희", 0xF1}, {"흰", 0xF2}, {"흴", 0xF3}, {"흼", 0xF4}, {"흽", 0xF5}, {"힁", 0xF6}, {"히", 0xF7}, {"힉", 0xF8}, {"힌", 0xF9}, {"힐", 0xFA}, {"힘", 0xFB}, {"힙", 0xFC}, {"힛", 0xFD}, {"힝", 0xFE},
        };
        private static readonly Dictionary<string, byte> U2GSC_KOR_B = new Dictionary<string, byte> {
            {"ㄱ", 0x00}, {"ㄴ", 0x01}, {"ㄷ", 0x02}, {"ㄹ", 0x03}, {"ㅁ", 0x04}, {"ㅂ", 0x05}, {"ㅅ", 0x06}, {"ㅇ", 0x07}, {"ㅈ", 0x08}, {"ㅊ", 0x09}, {"ㅋ", 0x0A}, {"ㅌ", 0x0B}, {"ㅍ", 0x0C}, {"ㅎ", 0x0D}, {"ㄲ", 0x0E}, {"ㄸ", 0x0F},
            {"ㅃ", 0x10}, {"ㅆ", 0x11}, {"ㅉ", 0x12},
            {"ㅏ", 0x20}, {"ㅑ", 0x21}, {"ㅓ", 0x22}, {"ㅕ", 0x23}, {"ㅗ", 0x24}, {"ㅛ", 0x25}, {"ㅜ", 0x26}, {"ㅠ", 0x27}, {"ㅡ", 0x28}, {"ㅣ", 0x29}, {"ㅐ", 0x2A}, {"ㅒ", 0x2B}, {"ㅔ", 0x2C}, {"ㅖ", 0x2D}, {"ㅘ", 0x2E}, {"ㅙ", 0x2F},
            {"ㅚ", 0x30}, {"ㅝ", 0x31}, {"ㅞ", 0x32}, {"ㅟ", 0x33}, {"ㅢ", 0x34}, {"_", 0x35}, {"—", 0x36},
            {"「", 0x60}, {"」", 0x61}, {"『", 0x62}, {"』", 0x63}, {"(", 0x64}, {")", 0x65}, {"!", 0x66}, {"?", 0x67}, {"-", 0x68}, {"~", 0x69}, {"⋯", 0x6A}, {",", 0x6B}, {".", 0x6C},
            {"0", 0xF0}, {"1", 0xF1}, {"2", 0xF2}, {"3", 0xF3}, {"4", 0xF4}, {"5", 0xF5}, {"6", 0xF6}, {"7", 0xF7}, {"8", 0xF8}, {"9", 0xF9},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_0 = new Dictionary<byte, string> {
            {0x00, "ㄱ"}, {0x01, "ㄴ"}, {0x02, "ㄷ"}, {0x03, "ㄹ"}, {0x04, "ㅁ"}, {0x05, "ㅂ"}, {0x06, "ㅅ"}, {0x07, "ㅇ"}, {0x08, "ㅈ"}, {0x09, "ㅊ"}, {0x0A, "ㅋ"}, {0x0B, "ㅌ"}, {0x0C, "ㅍ"}, {0x0D, "ㅎ"}, {0x0E, "ㄲ"}, {0x0F, "ㄸ"},
            {0x10, "ㅃ"}, {0x11, "ㅆ"}, {0x12, "ㅉ"},
            {0x20, "ㅏ"}, {0x21, "ㅑ"}, {0x22, "ㅓ"}, {0x23, "ㅕ"}, {0x24, "ㅗ"}, {0x25, "ㅛ"}, {0x26, "ㅜ"}, {0x27, "ㅠ"}, {0x28, "ㅡ"}, {0x29, "ㅣ"}, {0x2A, "ㅐ"}, {0x2B, "ㅒ"}, {0x2C, "ㅔ"}, {0x2D, "ㅖ"}, {0x2E, "ㅘ"}, {0x2F, "ㅙ"},
            {0x30, "ㅚ"}, {0x31, "ㅝ"}, {0x32, "ㅞ"}, {0x33, "ㅟ"}, {0x34, "ㅢ"}, {0x35, "_"}, {0x36, "—"},
            {0x60, "「"}, {0x61, "」"}, {0x62, "『"}, {0x63, "』"}, {0x64, "("}, {0x65, ")"}, {0x66, "!"}, {0x67, "?"}, {0x68, "-"}, {0x69, "~"}, {0x6A, "⋯"}, {0x6B, ","}, {0x6C, "."},
            {0xF0, "0"}, {0xF1, "1"}, {0xF2, "2"}, {0xF3, "3"}, {0xF4, "4"}, {0xF5, "5"}, {0xF6, "6"}, {0xF7, "7"}, {0xF8, "8"}, {0xF9, "9"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_1 = new Dictionary<byte, string> {
            {0x00, "가"}, {0x01, "각"}, {0x02, "간"}, {0x03, "갇"}, {0x04, "갈"}, {0x05, "갉"}, {0x06, "갊"}, {0x07, "감"}, {0x08, "갑"}, {0x09, "값"}, {0x0A, "갓"}, {0x0B, "갔"}, {0x0C, "강"}, {0x0D, "갖"}, {0x0E, "갗"},
            {0x10, "같"}, {0x11, "갚"}, {0x12, "갛"}, {0x13, "개"}, {0x14, "객"}, {0x15, "갠"}, {0x16, "갤"}, {0x17, "갬"}, {0x18, "갭"}, {0x19, "갯"}, {0x1A, "갰"}, {0x1B, "갱"}, {0x1C, "갸"}, {0x1D, "갹"}, {0x1E, "갼"}, {0x1F, "걀"},
            {0x20, "걋"}, {0x21, "걍"}, {0x22, "걔"}, {0x23, "걘"}, {0x24, "걜"}, {0x25, "거"}, {0x26, "걱"}, {0x27, "건"}, {0x28, "걷"}, {0x29, "걸"}, {0x2A, "걺"}, {0x2B, "검"}, {0x2C, "겁"}, {0x2D, "것"}, {0x2E, "겄"}, {0x2F, "겅"},
            {0x30, "겆"}, {0x31, "겉"}, {0x32, "겊"}, {0x33, "겋"}, {0x34, "게"}, {0x35, "겐"}, {0x36, "겔"}, {0x37, "겜"}, {0x38, "겝"}, {0x39, "겟"}, {0x3A, "겠"}, {0x3B, "겡"}, {0x3C, "겨"}, {0x3D, "격"}, {0x3E, "겪"}, {0x3F, "견"},
            {0x40, "겯"}, {0x41, "결"}, {0x42, "겹"}, {0x43, "겸"}, {0x44, "겻"}, {0x45, "겼"}, {0x46, "경"}, {0x47, "곁"}, {0x48, "계"}, {0x49, "곈"}, {0x4A, "곌"}, {0x4B, "곕"}, {0x4C, "곗"}, {0x4D, "고"}, {0x4E, "곡"}, {0x4F, "곤"},
            {0x60, "곧"}, {0x61, "골"}, {0x62, "곪"}, {0x63, "곬"}, {0x64, "곯"}, {0x65, "곰"}, {0x66, "곱"}, {0x67, "곳"}, {0x68, "공"}, {0x69, "곶"}, {0x6A, "과"}, {0x6B, "곽"}, {0x6C, "관"}, {0x6D, "괄"}, {0x6E, "괆"},
            {0x70, "괌"}, {0x71, "괍"}, {0x72, "괏"}, {0x73, "광"}, {0x74, "괘"}, {0x75, "괜"}, {0x76, "괠"}, {0x77, "괩"}, {0x78, "괬"}, {0x79, "괭"}, {0x7A, "괴"}, {0x7B, "괵"}, {0x7C, "괸"}, {0x7D, "괼"}, {0x7E, "괻"},
            {0x80, "굅"}, {0x81, "굇"}, {0x82, "굉"}, {0x83, "교"}, {0x84, "굔"}, {0x85, "굘"}, {0x86, "굡"}, {0x87, "굣"}, {0x88, "구"}, {0x89, "국"}, {0x8A, "군"}, {0x8B, "굳"}, {0x8C, "굴"}, {0x8D, "굵"}, {0x8E, "굶"}, {0x8F, "굻"},
            {0x90, "굼"}, {0x91, "굽"}, {0x92, "굿"}, {0x93, "궁"}, {0x94, "궂"}, {0x95, "궈"}, {0x96, "궉"}, {0x97, "권"}, {0x98, "궐"}, {0x99, "궜"}, {0x9A, "궝"}, {0x9B, "궤"}, {0x9C, "궷"}, {0x9D, "귀"}, {0x9E, "귁"}, {0x9F, "귄"},
            {0xA0, "귈"}, {0xA1, "귐"}, {0xA2, "귑"}, {0xA3, "귓"}, {0xA4, "규"}, {0xA5, "균"}, {0xA6, "귤"}, {0xA7, "그"}, {0xA8, "극"}, {0xA9, "근"}, {0xAA, "귿"}, {0xAB, "글"}, {0xAC, "긁"}, {0xAD, "금"}, {0xAE, "급"}, {0xAF, "긋"},
            {0xB0, "긍"}, {0xB1, "긔"}, {0xB2, "기"}, {0xB3, "긱"}, {0xB4, "긴"}, {0xB5, "긷"}, {0xB6, "길"}, {0xB7, "긺"}, {0xB8, "김"}, {0xB9, "깁"}, {0xBA, "깃"}, {0xBB, "깅"}, {0xBC, "깆"}, {0xBD, "깊"}, {0xBE, "까"}, {0xBF, "깍"},
            {0xC0, "깎"}, {0xC1, "깐"}, {0xC2, "깔"}, {0xC3, "깖"}, {0xC4, "깜"}, {0xC5, "깝"}, {0xC6, "깟"}, {0xC7, "깠"}, {0xC8, "깡"}, {0xC9, "깥"}, {0xCA, "깨"}, {0xCB, "깩"}, {0xCC, "깬"}, {0xCD, "깰"}, {0xCE, "깸"},
            {0xD0, "깹"}, {0xD1, "깻"}, {0xD2, "깼"}, {0xD3, "깽"}, {0xD4, "꺄"}, {0xD5, "꺅"}, {0xD6, "꺌"}, {0xD7, "꺼"}, {0xD8, "꺽"}, {0xD9, "꺾"}, {0xDA, "껀"}, {0xDB, "껄"}, {0xDC, "껌"}, {0xDD, "껍"}, {0xDE, "껏"},
            {0xE0, "껐"}, {0xE1, "껑"}, {0xE2, "께"}, {0xE3, "껙"}, {0xE4, "껜"}, {0xE5, "껨"}, {0xE6, "껫"}, {0xE7, "껭"}, {0xE8, "껴"}, {0xE9, "껸"}, {0xEA, "껼"}, {0xEB, "꼇"}, {0xEC, "꼈"}, {0xED, "꼍"}, {0xEE, "꼐"}, {0xEF, "꼬"},
            {0xF0, "꼭"}, {0xF1, "꼰"}, {0xF2, "꼲"}, {0xF3, "꼴"}, {0xF4, "꼼"}, {0xF5, "꼽"}, {0xF6, "꼿"}, {0xF7, "꽁"}, {0xF8, "꽂"}, {0xF9, "꽃"}, {0xFA, "꽈"}, {0xFB, "꽉"}, {0xFC, "꽐"}, {0xFD, "꽜"}, {0xFE, "꽝"}, {0xFF, "꽤"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_2 = new Dictionary<byte, string> {
            {0x00, "꽥"}, {0x01, "꽹"}, {0x02, "꾀"}, {0x03, "꾄"}, {0x04, "꾈"}, {0x05, "꾐"}, {0x06, "꾑"}, {0x07, "꾕"}, {0x08, "꾜"}, {0x09, "꾸"}, {0x0A, "꾹"}, {0x0B, "꾼"}, {0x0C, "꿀"}, {0x0D, "꿇"}, {0x0E, "꿈"}, {0x0F, "꿉"},
            {0x10, "꿋"}, {0x11, "꿍"}, {0x12, "꿎"}, {0x13, "꿔"}, {0x14, "꿜"}, {0x15, "꿨"}, {0x16, "꿩"}, {0x17, "꿰"}, {0x18, "꿱"}, {0x19, "꿴"}, {0x1A, "꿸"}, {0x1B, "뀀"}, {0x1C, "뀁"}, {0x1D, "뀄"}, {0x1E, "뀌"}, {0x1F, "뀐"},
            {0x20, "뀔"}, {0x21, "뀜"}, {0x22, "뀝"}, {0x23, "뀨"}, {0x24, "끄"}, {0x25, "끅"}, {0x26, "끈"}, {0x27, "끊"}, {0x28, "끌"}, {0x29, "끎"}, {0x2A, "끓"}, {0x2B, "끔"}, {0x2C, "끕"}, {0x2D, "끗"}, {0x2E, "끙"},
            {0x30, "끝"}, {0x31, "끼"}, {0x32, "끽"}, {0x33, "낀"}, {0x34, "낄"}, {0x35, "낌"}, {0x36, "낍"}, {0x37, "낏"}, {0x38, "낑"}, {0x39, "나"}, {0x3A, "낙"}, {0x3B, "낚"}, {0x3C, "난"}, {0x3D, "낟"}, {0x3E, "날"},
            {0x40, "낡"}, {0x41, "낢"}, {0x42, "남"}, {0x43, "납"}, {0x44, "낫"}, {0x45, "났"}, {0x46, "낭"}, {0x47, "낮"}, {0x48, "낯"}, {0x49, "낱"}, {0x4A, "낳"}, {0x4B, "내"}, {0x4C, "낵"}, {0x4D, "낸"}, {0x4E, "낼"}, {0x4F, "냄"},
            {0x60, "냅"}, {0x61, "냇"}, {0x62, "냈"}, {0x63, "냉"}, {0x64, "냐"}, {0x65, "냑"}, {0x66, "냔"}, {0x67, "냘"}, {0x68, "냠"}, {0x69, "냥"}, {0x6A, "너"}, {0x6B, "넉"}, {0x6C, "넋"}, {0x6D, "넌"}, {0x6E, "널"}, {0x6F, "넒"},
            {0x70, "넓"}, {0x71, "넘"}, {0x72, "넙"}, {0x73, "넛"}, {0x74, "넜"}, {0x75, "넝"}, {0x76, "넣"}, {0x77, "네"}, {0x78, "넥"}, {0x79, "넨"}, {0x7A, "넬"}, {0x7B, "넴"}, {0x7C, "넵"}, {0x7D, "넷"}, {0x7E, "넸"}, {0x7F, "넹"},
            {0x80, "녀"}, {0x81, "녁"}, {0x82, "년"}, {0x83, "녈"}, {0x84, "념"}, {0x85, "녑"}, {0x86, "녔"}, {0x87, "녕"}, {0x88, "녘"}, {0x89, "녜"}, {0x8A, "녠"}, {0x8B, "노"}, {0x8C, "녹"}, {0x8D, "논"}, {0x8E, "놀"}, {0x8F, "놂"},
            {0x90, "놈"}, {0x91, "놉"}, {0x92, "놋"}, {0x93, "농"}, {0x94, "높"}, {0x95, "놓"}, {0x96, "놔"}, {0x97, "놘"}, {0x98, "놜"}, {0x99, "놨"}, {0x9A, "뇌"}, {0x9B, "뇐"}, {0x9C, "뇔"}, {0x9D, "뇜"}, {0x9E, "뇝"},
            {0xA0, "뇟"}, {0xA1, "뇨"}, {0xA2, "뇩"}, {0xA3, "뇬"}, {0xA4, "뇰"}, {0xA5, "뇹"}, {0xA6, "뇻"}, {0xA7, "뇽"}, {0xA8, "누"}, {0xA9, "눅"}, {0xAA, "눈"}, {0xAB, "눋"}, {0xAC, "눌"}, {0xAD, "눔"}, {0xAE, "눕"},
            {0xB0, "눗"}, {0xB1, "눙"}, {0xB2, "눠"}, {0xB3, "눴"}, {0xB4, "눼"}, {0xB5, "뉘"}, {0xB6, "뉜"}, {0xB7, "뉠"}, {0xB8, "뉨"}, {0xB9, "뉩"}, {0xBA, "뉴"}, {0xBB, "뉵"}, {0xBC, "뉼"}, {0xBD, "늄"}, {0xBE, "늅"}, {0xBF, "늉"},
            {0xC0, "느"}, {0xC1, "늑"}, {0xC2, "는"}, {0xC3, "늘"}, {0xC4, "늙"}, {0xC5, "늚"}, {0xC6, "늠"}, {0xC7, "늡"}, {0xC8, "늣"}, {0xC9, "능"}, {0xCA, "늦"}, {0xCB, "늪"}, {0xCC, "늬"}, {0xCD, "늰"}, {0xCE, "늴"}, {0xCF, "니"},
            {0xD0, "닉"}, {0xD1, "닌"}, {0xD2, "닐"}, {0xD3, "닒"}, {0xD4, "님"}, {0xD5, "닙"}, {0xD6, "닛"}, {0xD7, "닝"}, {0xD8, "닢"}, {0xD9, "다"}, {0xDA, "닥"}, {0xDB, "닦"}, {0xDC, "단"}, {0xDD, "닫"}, {0xDE, "달"}, {0xDF, "닭"},
            {0xE0, "닮"}, {0xE1, "닯"}, {0xE2, "닳"}, {0xE3, "담"}, {0xE4, "답"}, {0xE5, "닷"}, {0xE6, "닸"}, {0xE7, "당"}, {0xE8, "닺"}, {0xE9, "닻"}, {0xEA, "닿"}, {0xEB, "대"}, {0xEC, "댁"}, {0xED, "댄"}, {0xEE, "댈"}, {0xEF, "댐"},
            {0xF0, "댑"}, {0xF1, "댓"}, {0xF2, "댔"}, {0xF3, "댕"}, {0xF4, "더"}, {0xF5, "덕"}, {0xF6, "덖"}, {0xF7, "던"}, {0xF8, "덛"}, {0xF9, "덜"}, {0xFA, "덞"}, {0xFB, "덟"}, {0xFC, "덤"}, {0xFD, "덥"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_3 = new Dictionary<byte, string> {
            {0x00, "덧"}, {0x01, "덩"}, {0x02, "덫"}, {0x03, "덮"}, {0x04, "데"}, {0x05, "덱"}, {0x06, "덴"}, {0x07, "델"}, {0x08, "뎀"}, {0x09, "뎁"}, {0x0A, "뎃"}, {0x0B, "뎄"}, {0x0C, "뎅"}, {0x0D, "뎌"}, {0x0E, "뎐"},
            {0x10, "뎔"}, {0x11, "뎠"}, {0x12, "뎡"}, {0x13, "뎨"}, {0x14, "뎬"}, {0x15, "도"}, {0x16, "독"}, {0x17, "돈"}, {0x18, "돋"}, {0x19, "돌"}, {0x1A, "돎"}, {0x1B, "돔"}, {0x1C, "돕"}, {0x1D, "돗"}, {0x1E, "동"},
            {0x20, "돛"}, {0x21, "돝"}, {0x22, "돠"}, {0x23, "돤"}, {0x24, "돨"}, {0x25, "돼"}, {0x26, "됐"}, {0x27, "되"}, {0x28, "된"}, {0x29, "될"}, {0x2A, "됨"}, {0x2B, "됩"}, {0x2C, "됫"}, {0x2D, "됴"}, {0x2E, "두"}, {0x2F, "둑"},
            {0x30, "둔"}, {0x31, "둘"}, {0x32, "둠"}, {0x33, "둡"}, {0x34, "둣"}, {0x35, "둥"}, {0x36, "둬"}, {0x37, "뒀"}, {0x38, "뒈"}, {0x39, "뒝"}, {0x3A, "뒤"}, {0x3B, "뒨"}, {0x3C, "뒬"}, {0x3D, "뒵"}, {0x3E, "뒷"}, {0x3F, "뒹"},
            {0x40, "듀"}, {0x41, "듄"}, {0x42, "듈"}, {0x43, "듐"}, {0x44, "듕"}, {0x45, "드"}, {0x46, "득"}, {0x47, "든"}, {0x48, "듣"}, {0x49, "들"}, {0x4A, "듦"}, {0x4B, "듬"}, {0x4C, "듭"}, {0x4D, "듯"}, {0x4E, "등"}, {0x4F, "듸"},
            {0x60, "디"}, {0x61, "딕"}, {0x62, "딘"}, {0x63, "딛"}, {0x64, "딜"}, {0x65, "딤"}, {0x66, "딥"}, {0x67, "딧"}, {0x68, "딨"}, {0x69, "딩"}, {0x6A, "딪"}, {0x6B, "따"}, {0x6C, "딱"}, {0x6D, "딴"}, {0x6E, "딸"},
            {0x70, "땀"}, {0x71, "땁"}, {0x72, "땃"}, {0x73, "땄"}, {0x74, "땅"}, {0x75, "땋"}, {0x76, "때"}, {0x77, "땍"}, {0x78, "땐"}, {0x79, "땔"}, {0x7A, "땜"}, {0x7B, "땝"}, {0x7C, "땟"}, {0x7D, "땠"}, {0x7E, "땡"},
            {0x80, "떠"}, {0x81, "떡"}, {0x82, "떤"}, {0x83, "떨"}, {0x84, "떪"}, {0x85, "떫"}, {0x86, "떰"}, {0x87, "떱"}, {0x88, "떳"}, {0x89, "떴"}, {0x8A, "떵"}, {0x8B, "떻"}, {0x8C, "떼"}, {0x8D, "떽"}, {0x8E, "뗀"}, {0x8F, "뗄"},
            {0x90, "뗌"}, {0x91, "뗍"}, {0x92, "뗏"}, {0x93, "뗐"}, {0x94, "뗑"}, {0x95, "뗘"}, {0x96, "뗬"}, {0x97, "또"}, {0x98, "똑"}, {0x99, "똔"}, {0x9A, "똘"}, {0x9B, "똥"}, {0x9C, "똬"}, {0x9D, "똴"}, {0x9E, "뙈"}, {0x9F, "뙤"},
            {0xA0, "뙨"}, {0xA1, "뚜"}, {0xA2, "뚝"}, {0xA3, "뚠"}, {0xA4, "뚤"}, {0xA5, "뚫"}, {0xA6, "뚬"}, {0xA7, "뚱"}, {0xA8, "뛔"}, {0xA9, "뛰"}, {0xAA, "뛴"}, {0xAB, "뛸"}, {0xAC, "뜀"}, {0xAD, "뜁"}, {0xAE, "뜅"}, {0xAF, "뜨"},
            {0xB0, "뜩"}, {0xB1, "뜬"}, {0xB2, "뜯"}, {0xB3, "뜰"}, {0xB4, "뜸"}, {0xB5, "뜹"}, {0xB6, "뜻"}, {0xB7, "띄"}, {0xB8, "띈"}, {0xB9, "띌"}, {0xBA, "띔"}, {0xBB, "띕"}, {0xBC, "띠"}, {0xBD, "띤"}, {0xBE, "띨"}, {0xBF, "띰"},
            {0xC0, "띱"}, {0xC1, "띳"}, {0xC2, "띵"}, {0xC3, "라"}, {0xC4, "락"}, {0xC5, "란"}, {0xC6, "랄"}, {0xC7, "람"}, {0xC8, "랍"}, {0xC9, "랏"}, {0xCA, "랐"}, {0xCB, "랑"}, {0xCC, "랒"}, {0xCD, "랖"}, {0xCE, "랗"},
            {0xD0, "뢔"}, {0xD1, "래"}, {0xD2, "랙"}, {0xD3, "랜"}, {0xD4, "랠"}, {0xD5, "램"}, {0xD6, "랩"}, {0xD7, "랫"}, {0xD8, "랬"}, {0xD9, "랭"}, {0xDA, "랴"}, {0xDB, "략"}, {0xDC, "랸"}, {0xDD, "럇"}, {0xDE, "량"}, {0xDF, "러"},
            {0xE0, "럭"}, {0xE1, "런"}, {0xE2, "럴"}, {0xE3, "럼"}, {0xE4, "럽"}, {0xE5, "럿"}, {0xE6, "렀"}, {0xE7, "렁"}, {0xE8, "렇"}, {0xE9, "레"}, {0xEA, "렉"}, {0xEB, "렌"}, {0xEC, "렐"}, {0xED, "렘"}, {0xEE, "렙"}, {0xEF, "렛"},
            {0xF0, "렝"}, {0xF1, "려"}, {0xF2, "력"}, {0xF3, "련"}, {0xF4, "렬"}, {0xF5, "렴"}, {0xF6, "렵"}, {0xF7, "렷"}, {0xF8, "렸"}, {0xF9, "령"}, {0xFA, "례"}, {0xFB, "롄"}, {0xFC, "롑"}, {0xFD, "롓"}, {0xFE, "로"}, {0xFF, "록"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_4 = new Dictionary<byte, string> {
            {0x00, "론"}, {0x01, "롤"}, {0x02, "롬"}, {0x03, "롭"}, {0x04, "롯"}, {0x05, "롱"}, {0x06, "롸"}, {0x07, "롼"}, {0x08, "뢍"}, {0x09, "뢨"}, {0x0A, "뢰"}, {0x0B, "뢴"}, {0x0C, "뢸"}, {0x0D, "룀"}, {0x0E, "룁"}, {0x0F, "룃"},
            {0x10, "룅"}, {0x11, "료"}, {0x12, "룐"}, {0x13, "룔"}, {0x14, "룝"}, {0x15, "룟"}, {0x16, "룡"}, {0x17, "루"}, {0x18, "룩"}, {0x19, "룬"}, {0x1A, "룰"}, {0x1B, "룸"}, {0x1C, "룹"}, {0x1D, "룻"}, {0x1E, "룽"}, {0x1F, "뤄"},
            {0x20, "뤘"}, {0x21, "뤠"}, {0x22, "뤼"}, {0x23, "뤽"}, {0x24, "륀"}, {0x25, "륄"}, {0x26, "륌"}, {0x27, "륏"}, {0x28, "륑"}, {0x29, "류"}, {0x2A, "륙"}, {0x2B, "륜"}, {0x2C, "률"}, {0x2D, "륨"}, {0x2E, "륩"},
            {0x30, "륫"}, {0x31, "륭"}, {0x32, "르"}, {0x33, "륵"}, {0x34, "른"}, {0x35, "를"}, {0x36, "름"}, {0x37, "릅"}, {0x38, "릇"}, {0x39, "릉"}, {0x3A, "릊"}, {0x3B, "릍"}, {0x3C, "릎"}, {0x3D, "리"}, {0x3E, "릭"},
            {0x40, "린"}, {0x41, "릴"}, {0x42, "림"}, {0x43, "립"}, {0x44, "릿"}, {0x45, "링"}, {0x46, "마"}, {0x47, "막"}, {0x48, "만"}, {0x49, "많"}, {0x4A, "맏"}, {0x4B, "말"}, {0x4C, "맑"}, {0x4D, "맒"}, {0x4E, "맘"}, {0x4F, "맙"},
            {0x60, "맛"}, {0x61, "망"}, {0x62, "맞"}, {0x63, "맡"}, {0x64, "맣"}, {0x65, "매"}, {0x66, "맥"}, {0x67, "맨"}, {0x68, "맬"}, {0x69, "맴"}, {0x6A, "맵"}, {0x6B, "맷"}, {0x6C, "맸"}, {0x6D, "맹"}, {0x6E, "맺"}, {0x6F, "먀"},
            {0x70, "먁"}, {0x71, "먈"}, {0x72, "먕"}, {0x73, "머"}, {0x74, "먹"}, {0x75, "먼"}, {0x76, "멀"}, {0x77, "멂"}, {0x78, "멈"}, {0x79, "멉"}, {0x7A, "멋"}, {0x7B, "멍"}, {0x7C, "멎"}, {0x7D, "멓"}, {0x7E, "메"}, {0x7F, "멕"},
            {0x80, "멘"}, {0x81, "멜"}, {0x82, "멤"}, {0x83, "멥"}, {0x84, "멧"}, {0x85, "멨"}, {0x86, "멩"}, {0x87, "며"}, {0x88, "멱"}, {0x89, "면"}, {0x8A, "멸"}, {0x8B, "몃"}, {0x8C, "몄"}, {0x8D, "명"}, {0x8E, "몇"}, {0x8F, "몌"},
            {0x90, "모"}, {0x91, "목"}, {0x92, "몫"}, {0x93, "몬"}, {0x94, "몰"}, {0x95, "몲"}, {0x96, "몸"}, {0x97, "몹"}, {0x98, "못"}, {0x99, "몽"}, {0x9A, "뫄"}, {0x9B, "뫈"}, {0x9C, "뫘"}, {0x9D, "뫙"}, {0x9E, "뫼"},
            {0xA0, "묀"}, {0xA1, "묄"}, {0xA2, "묍"}, {0xA3, "묏"}, {0xA4, "묑"}, {0xA5, "묘"}, {0xA6, "묜"}, {0xA7, "묠"}, {0xA8, "묩"}, {0xA9, "묫"}, {0xAA, "무"}, {0xAB, "묵"}, {0xAC, "묶"}, {0xAD, "문"}, {0xAE, "묻"},
            {0xB0, "물"}, {0xB1, "묽"}, {0xB2, "묾"}, {0xB3, "뭄"}, {0xB4, "뭅"}, {0xB5, "뭇"}, {0xB6, "뭉"}, {0xB7, "뭍"}, {0xB8, "뭏"}, {0xB9, "뭐"}, {0xBA, "뭔"}, {0xBB, "뭘"}, {0xBC, "뭡"}, {0xBD, "뭣"}, {0xBE, "뭬"}, {0xBF, "뮈"},
            {0xC0, "뮌"}, {0xC1, "뮐"}, {0xC2, "뮤"}, {0xC3, "뮨"}, {0xC4, "뮬"}, {0xC5, "뮴"}, {0xC6, "뮷"}, {0xC7, "므"}, {0xC8, "믄"}, {0xC9, "믈"}, {0xCA, "믐"}, {0xCB, "믓"}, {0xCC, "미"}, {0xCD, "믹"}, {0xCE, "민"}, {0xCF, "믿"},
            {0xD0, "밀"}, {0xD1, "밂"}, {0xD2, "밈"}, {0xD3, "밉"}, {0xD4, "밋"}, {0xD5, "밌"}, {0xD6, "밍"}, {0xD7, "및"}, {0xD8, "밑"}, {0xD9, "바"}, {0xDA, "박"}, {0xDB, "밖"}, {0xDC, "밗"}, {0xDD, "반"}, {0xDE, "받"}, {0xDF, "발"},
            {0xE0, "밝"}, {0xE1, "밞"}, {0xE2, "밟"}, {0xE3, "밤"}, {0xE4, "밥"}, {0xE5, "밧"}, {0xE6, "방"}, {0xE7, "밭"}, {0xE8, "배"}, {0xE9, "백"}, {0xEA, "밴"}, {0xEB, "밸"}, {0xEC, "뱀"}, {0xED, "뱁"}, {0xEE, "뱃"}, {0xEF, "뱄"},
            {0xF0, "뱅"}, {0xF1, "뱉"}, {0xF2, "뱌"}, {0xF3, "뱍"}, {0xF4, "뱐"}, {0xF5, "뱝"}, {0xF6, "버"}, {0xF7, "벅"}, {0xF8, "번"}, {0xF9, "벋"}, {0xFA, "벌"}, {0xFB, "벎"}, {0xFC, "범"}, {0xFD, "법"}, {0xFE, "벗"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_5 = new Dictionary<byte, string> {
            {0x00, "벙"}, {0x01, "벚"}, {0x02, "베"}, {0x03, "벡"}, {0x04, "벤"}, {0x05, "벧"}, {0x06, "벨"}, {0x07, "벰"}, {0x08, "벱"}, {0x09, "벳"}, {0x0A, "벵"}, {0x0B, "벴"}, {0x0C, "벼"}, {0x0D, "벽"}, {0x0E, "변"},
            {0x10, "별"}, {0x11, "볍"}, {0x12, "볏"}, {0x13, "병"}, {0x14, "볕"}, {0x15, "볐"}, {0x16, "볘"}, {0x17, "볜"}, {0x18, "보"}, {0x19, "복"}, {0x1A, "본"}, {0x1B, "볼"}, {0x1C, "봄"}, {0x1D, "봅"}, {0x1E, "봇"}, {0x1F, "봉"},
            {0x20, "볶"}, {0x21, "봐"}, {0x22, "봔"}, {0x23, "봤"}, {0x24, "봬"}, {0x25, "뵀"}, {0x26, "뵈"}, {0x27, "뵉"}, {0x28, "뵌"}, {0x29, "뵐"}, {0x2A, "뵘"}, {0x2B, "뵙"}, {0x2C, "뵤"}, {0x2D, "뵨"}, {0x2E, "부"}, {0x2F, "북"},
            {0x30, "분"}, {0x31, "붇"}, {0x32, "불"}, {0x33, "붐"}, {0x34, "붑"}, {0x35, "붓"}, {0x36, "붕"}, {0x37, "붙"}, {0x38, "붚"}, {0x39, "붉"}, {0x3A, "붊"}, {0x3B, "붜"}, {0x3C, "붤"}, {0x3D, "붰"}, {0x3E, "붸"}, {0x3F, "뷔"},
            {0x40, "뷕"}, {0x41, "뷘"}, {0x42, "뷜"}, {0x43, "뷩"}, {0x44, "뷰"}, {0x45, "뷴"}, {0x46, "뷸"}, {0x47, "븀"}, {0x48, "븃"}, {0x49, "븅"}, {0x4A, "브"}, {0x4B, "븍"}, {0x4C, "븐"}, {0x4D, "블"}, {0x4E, "븜"}, {0x4F, "븝"},
            {0x60, "븟"}, {0x61, "비"}, {0x62, "빅"}, {0x63, "빈"}, {0x64, "빌"}, {0x65, "빎"}, {0x66, "빔"}, {0x67, "빕"}, {0x68, "빗"}, {0x69, "빙"}, {0x6A, "빚"}, {0x6B, "빛"}, {0x6C, "빠"}, {0x6D, "빡"}, {0x6E, "빤"},
            {0x70, "빨"}, {0x71, "빪"}, {0x72, "빰"}, {0x73, "빱"}, {0x74, "빳"}, {0x75, "빴"}, {0x76, "빵"}, {0x77, "빻"}, {0x78, "빼"}, {0x79, "빽"}, {0x7A, "뺀"}, {0x7B, "뺄"}, {0x7C, "뺌"}, {0x7D, "뺍"}, {0x7E, "뺏"},
            {0x80, "뺐"}, {0x81, "뺑"}, {0x82, "뺘"}, {0x83, "뺙"}, {0x84, "뺨"}, {0x85, "뻐"}, {0x86, "뻑"}, {0x87, "뻔"}, {0x88, "뻗"}, {0x89, "뻘"}, {0x8A, "뻠"}, {0x8B, "뻣"}, {0x8C, "뻤"}, {0x8D, "뻥"}, {0x8E, "뻬"}, {0x8F, "뼁"},
            {0x90, "뼈"}, {0x91, "뼉"}, {0x92, "뼘"}, {0x93, "뼙"}, {0x94, "뼛"}, {0x95, "뼜"}, {0x96, "뼝"}, {0x97, "뽀"}, {0x98, "뽁"}, {0x99, "뽄"}, {0x9A, "뽈"}, {0x9B, "뽐"}, {0x9C, "뽑"}, {0x9D, "뽕"}, {0x9E, "뾔"}, {0x9F, "뾰"},
            {0xA0, "뿅"}, {0xA1, "뿌"}, {0xA2, "뿍"}, {0xA3, "뿐"}, {0xA4, "뿔"}, {0xA5, "뿜"}, {0xA6, "뿟"}, {0xA7, "뿡"}, {0xA8, "쀼"}, {0xA9, "쁑"}, {0xAA, "쁘"}, {0xAB, "쁜"}, {0xAC, "쁠"}, {0xAD, "쁨"}, {0xAE, "쁩"}, {0xAF, "삐"},
            {0xB0, "삑"}, {0xB1, "삔"}, {0xB2, "삘"}, {0xB3, "삠"}, {0xB4, "삡"}, {0xB5, "삣"}, {0xB6, "삥"}, {0xB7, "사"}, {0xB8, "삭"}, {0xB9, "삯"}, {0xBA, "산"}, {0xBB, "삳"}, {0xBC, "살"}, {0xBD, "삵"}, {0xBE, "삶"}, {0xBF, "삼"},
            {0xC0, "삽"}, {0xC1, "삿"}, {0xC2, "샀"}, {0xC3, "상"}, {0xC4, "샅"}, {0xC5, "새"}, {0xC6, "색"}, {0xC7, "샌"}, {0xC8, "샐"}, {0xC9, "샘"}, {0xCA, "샙"}, {0xCB, "샛"}, {0xCC, "샜"}, {0xCD, "생"}, {0xCE, "샤"},
            {0xD0, "샥"}, {0xD1, "샨"}, {0xD2, "샬"}, {0xD3, "샴"}, {0xD4, "샵"}, {0xD5, "샷"}, {0xD6, "샹"}, {0xD7, "섀"}, {0xD8, "섄"}, {0xD9, "섈"}, {0xDA, "섐"}, {0xDB, "섕"}, {0xDC, "서"}, {0xDD, "석"}, {0xDE, "섞"},
            {0xE0, "섟"}, {0xE1, "선"}, {0xE2, "섣"}, {0xE3, "설"}, {0xE4, "섦"}, {0xE5, "섧"}, {0xE6, "섬"}, {0xE7, "섭"}, {0xE8, "섯"}, {0xE9, "섰"}, {0xEA, "성"}, {0xEB, "섶"}, {0xEC, "세"}, {0xED, "섹"}, {0xEE, "센"}, {0xEF, "셀"},
            {0xF0, "셈"}, {0xF1, "셉"}, {0xF2, "셋"}, {0xF3, "셌"}, {0xF4, "셍"}, {0xF5, "셔"}, {0xF6, "셕"}, {0xF7, "션"}, {0xF8, "셜"}, {0xF9, "셤"}, {0xFA, "셥"}, {0xFB, "셧"}, {0xFC, "셨"}, {0xFD, "셩"}, {0xFE, "셰"}, {0xFF, "셴"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_6 = new Dictionary<byte, string> {
            {0x00, "셸"}, {0x01, "솅"}, {0x02, "소"}, {0x03, "속"}, {0x04, "솎"}, {0x05, "손"}, {0x06, "솔"}, {0x07, "솖"}, {0x08, "솜"}, {0x09, "솝"}, {0x0A, "솟"}, {0x0B, "송"}, {0x0C, "솥"}, {0x0D, "솨"}, {0x0E, "솩"}, {0x0F, "솬"},
            {0x10, "솰"}, {0x11, "솽"}, {0x12, "쇄"}, {0x13, "쇈"}, {0x14, "쇌"}, {0x15, "쇔"}, {0x16, "쇗"}, {0x17, "쇘"}, {0x18, "쇠"}, {0x19, "쇤"}, {0x1A, "쇨"}, {0x1B, "쇰"}, {0x1C, "쇱"}, {0x1D, "쇳"}, {0x1E, "쇼"}, {0x1F, "쇽"},
            {0x20, "숀"}, {0x21, "숄"}, {0x22, "숌"}, {0x23, "숍"}, {0x24, "숏"}, {0x25, "숑"}, {0x26, "수"}, {0x27, "숙"}, {0x28, "순"}, {0x29, "숟"}, {0x2A, "술"}, {0x2B, "숨"}, {0x2C, "숩"}, {0x2D, "숫"}, {0x2E, "숭"}, {0x2F, "쌰"},
            {0x30, "쎼"}, {0x31, "숯"}, {0x32, "숱"}, {0x33, "숲"}, {0x34, "숴"}, {0x35, "쉈"}, {0x36, "쉐"}, {0x37, "쉑"}, {0x38, "쉔"}, {0x39, "쉘"}, {0x3A, "쉠"}, {0x3B, "쉥"}, {0x3C, "쉬"}, {0x3D, "쉭"}, {0x3E, "쉰"}, {0x3F, "쉴"},
            {0x40, "쉼"}, {0x41, "쉽"}, {0x42, "쉿"}, {0x43, "슁"}, {0x44, "슈"}, {0x45, "슉"}, {0x46, "슐"}, {0x47, "슘"}, {0x48, "슛"}, {0x49, "슝"}, {0x4A, "스"}, {0x4B, "슥"}, {0x4C, "슨"}, {0x4D, "슬"}, {0x4E, "슭"}, {0x4F, "슴"},
            {0x60, "습"}, {0x61, "슷"}, {0x62, "승"}, {0x63, "시"}, {0x64, "식"}, {0x65, "신"}, {0x66, "싣"}, {0x67, "실"}, {0x68, "싫"}, {0x69, "심"}, {0x6A, "십"}, {0x6B, "싯"}, {0x6C, "싱"}, {0x6D, "싶"}, {0x6E, "싸"}, {0x6F, "싹"},
            {0x70, "싻"}, {0x71, "싼"}, {0x72, "쌀"}, {0x73, "쌈"}, {0x74, "쌉"}, {0x75, "쌌"}, {0x76, "쌍"}, {0x77, "쌓"}, {0x78, "쌔"}, {0x79, "쌕"}, {0x7A, "쌘"}, {0x7B, "쌜"}, {0x7C, "쌤"}, {0x7D, "쌥"}, {0x7E, "쌨"}, {0x7F, "쌩"},
            {0x80, "썅"}, {0x81, "써"}, {0x82, "썩"}, {0x83, "썬"}, {0x84, "썰"}, {0x85, "썲"}, {0x86, "썸"}, {0x87, "썹"}, {0x88, "썼"}, {0x89, "썽"}, {0x8A, "쎄"}, {0x8B, "쎈"}, {0x8C, "쎌"}, {0x8D, "쏀"}, {0x8E, "쏘"}, {0x8F, "쏙"},
            {0x90, "쏜"}, {0x91, "쏟"}, {0x92, "쏠"}, {0x93, "쏢"}, {0x94, "쏨"}, {0x95, "쏩"}, {0x96, "쏭"}, {0x97, "쏴"}, {0x98, "쏵"}, {0x99, "쏸"}, {0x9A, "쐈"}, {0x9B, "쐐"}, {0x9C, "쐤"}, {0x9D, "쐬"}, {0x9E, "쐰"},
            {0xA0, "쓔"}, {0xA1, "쐴"}, {0xA2, "쐼"}, {0xA3, "쐽"}, {0xA4, "쑈"}, {0xA5, "쑤"}, {0xA6, "쑥"}, {0xA7, "쑨"}, {0xA8, "쑬"}, {0xA9, "쑴"}, {0xAA, "쑵"}, {0xAB, "쑹"}, {0xAC, "쒀"}, {0xAD, "쒔"}, {0xAE, "쒜"}, {0xAF, "쒸"},
            {0xB0, "쒼"}, {0xB1, "쓩"}, {0xB2, "쓰"}, {0xB3, "쓱"}, {0xB4, "쓴"}, {0xB5, "쓸"}, {0xB6, "쓺"}, {0xB7, "쓿"}, {0xB8, "씀"}, {0xB9, "씁"}, {0xBA, "씌"}, {0xBB, "씐"}, {0xBC, "씔"}, {0xBD, "씜"}, {0xBE, "씨"}, {0xBF, "씩"},
            {0xC0, "씬"}, {0xC1, "씰"}, {0xC2, "씸"}, {0xC3, "씹"}, {0xC4, "씻"}, {0xC5, "씽"}, {0xC6, "아"}, {0xC7, "악"}, {0xC8, "안"}, {0xC9, "앉"}, {0xCA, "않"}, {0xCB, "알"}, {0xCC, "앍"}, {0xCD, "앎"}, {0xCE, "앓"}, {0xCF, "암"},
            {0xD0, "압"}, {0xD1, "앗"}, {0xD2, "았"}, {0xD3, "앙"}, {0xD4, "앝"}, {0xD5, "앞"}, {0xD6, "애"}, {0xD7, "액"}, {0xD8, "앤"}, {0xD9, "앨"}, {0xDA, "앰"}, {0xDB, "앱"}, {0xDC, "앳"}, {0xDD, "앴"}, {0xDE, "앵"}, {0xDF, "야"},
            {0xE0, "약"}, {0xE1, "얀"}, {0xE2, "얄"}, {0xE3, "얇"}, {0xE4, "얌"}, {0xE5, "얍"}, {0xE6, "얏"}, {0xE7, "양"}, {0xE8, "얕"}, {0xE9, "얗"}, {0xEA, "얘"}, {0xEB, "얜"}, {0xEC, "얠"}, {0xED, "얩"}, {0xEE, "어"}, {0xEF, "억"},
            {0xF0, "언"}, {0xF1, "얹"}, {0xF2, "얻"}, {0xF3, "얼"}, {0xF4, "얽"}, {0xF5, "얾"}, {0xF6, "엄"}, {0xF7, "업"}, {0xF8, "없"}, {0xF9, "엇"}, {0xFA, "었"}, {0xFB, "엉"}, {0xFC, "엊"}, {0xFD, "엌"}, {0xFE, "엎"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_7 = new Dictionary<byte, string> {
            {0x00, "에"}, {0x01, "엑"}, {0x02, "엔"}, {0x03, "엘"}, {0x04, "엠"}, {0x05, "엡"}, {0x06, "엣"}, {0x07, "엥"}, {0x08, "여"}, {0x09, "역"}, {0x0A, "엮"}, {0x0B, "연"}, {0x0C, "열"}, {0x0D, "엶"}, {0x0E, "엷"},
            {0x10, "염"}, {0x11, "엽"}, {0x12, "엾"}, {0x13, "엿"}, {0x14, "였"}, {0x15, "영"}, {0x16, "옅"}, {0x17, "옆"}, {0x18, "옇"}, {0x19, "예"}, {0x1A, "옌"}, {0x1B, "옐"}, {0x1C, "옘"}, {0x1D, "옙"}, {0x1E, "옛"}, {0x1F, "옜"},
            {0x20, "오"}, {0x21, "옥"}, {0x22, "온"}, {0x23, "올"}, {0x24, "옭"}, {0x25, "옮"}, {0x26, "옰"}, {0x27, "옳"}, {0x28, "옴"}, {0x29, "옵"}, {0x2A, "옷"}, {0x2B, "옹"}, {0x2C, "옻"}, {0x2D, "와"}, {0x2E, "왁"}, {0x2F, "완"},
            {0x30, "왈"}, {0x31, "왐"}, {0x32, "왑"}, {0x33, "왓"}, {0x34, "왔"}, {0x35, "왕"}, {0x36, "왜"}, {0x37, "왝"}, {0x38, "왠"}, {0x39, "왬"}, {0x3A, "왯"}, {0x3B, "왱"}, {0x3C, "외"}, {0x3D, "왹"}, {0x3E, "왼"}, {0x3F, "욀"},
            {0x40, "욈"}, {0x41, "욉"}, {0x42, "욋"}, {0x43, "욍"}, {0x44, "요"}, {0x45, "욕"}, {0x46, "욘"}, {0x47, "욜"}, {0x48, "욤"}, {0x49, "욥"}, {0x4A, "욧"}, {0x4B, "용"}, {0x4C, "우"}, {0x4D, "욱"}, {0x4E, "운"}, {0x4F, "울"},
            {0x60, "욹"}, {0x61, "욺"}, {0x62, "움"}, {0x63, "웁"}, {0x64, "웃"}, {0x65, "웅"}, {0x66, "워"}, {0x67, "웍"}, {0x68, "원"}, {0x69, "월"}, {0x6A, "웜"}, {0x6B, "웝"}, {0x6C, "웠"}, {0x6D, "웡"}, {0x6E, "웨"},
            {0x70, "웩"}, {0x71, "웬"}, {0x72, "웰"}, {0x73, "웸"}, {0x74, "웹"}, {0x75, "웽"}, {0x76, "위"}, {0x77, "윅"}, {0x78, "윈"}, {0x79, "윌"}, {0x7A, "윔"}, {0x7B, "윕"}, {0x7C, "윗"}, {0x7D, "윙"}, {0x7E, "유"},
            {0x80, "육"}, {0x81, "윤"}, {0x82, "율"}, {0x83, "윰"}, {0x84, "윱"}, {0x85, "윳"}, {0x86, "융"}, {0x87, "윷"}, {0x88, "으"}, {0x89, "윽"}, {0x8A, "은"}, {0x8B, "을"}, {0x8C, "읆"}, {0x8D, "음"}, {0x8E, "읍"}, {0x8F, "읏"},
            {0x90, "응"}, {0x91, "읒"}, {0x92, "읓"}, {0x93, "읔"}, {0x94, "읕"}, {0x95, "읖"}, {0x96, "읗"}, {0x97, "의"}, {0x98, "읜"}, {0x99, "읠"}, {0x9A, "읨"}, {0x9B, "읫"}, {0x9C, "이"}, {0x9D, "익"}, {0x9E, "인"}, {0x9F, "일"},
            {0xA0, "읽"}, {0xA1, "읾"}, {0xA2, "잃"}, {0xA3, "임"}, {0xA4, "입"}, {0xA5, "잇"}, {0xA6, "있"}, {0xA7, "잉"}, {0xA8, "잊"}, {0xA9, "잎"}, {0xAA, "자"}, {0xAB, "작"}, {0xAC, "잔"}, {0xAD, "잖"}, {0xAE, "잗"}, {0xAF, "잘"},
            {0xB0, "잚"}, {0xB1, "잠"}, {0xB2, "잡"}, {0xB3, "잣"}, {0xB4, "잤"}, {0xB5, "장"}, {0xB6, "잦"}, {0xB7, "재"}, {0xB8, "잭"}, {0xB9, "잰"}, {0xBA, "잴"}, {0xBB, "잼"}, {0xBC, "잽"}, {0xBD, "잿"}, {0xBE, "쟀"}, {0xBF, "쟁"},
            {0xC0, "쟈"}, {0xC1, "쟉"}, {0xC2, "쟌"}, {0xC3, "쟎"}, {0xC4, "쟐"}, {0xC5, "쟘"}, {0xC6, "쟝"}, {0xC7, "쟤"}, {0xC8, "쟨"}, {0xC9, "쟬"}, {0xCA, "저"}, {0xCB, "적"}, {0xCC, "전"}, {0xCD, "절"}, {0xCE, "젊"},
            {0xD0, "점"}, {0xD1, "접"}, {0xD2, "젓"}, {0xD3, "정"}, {0xD4, "젖"}, {0xD5, "제"}, {0xD6, "젝"}, {0xD7, "젠"}, {0xD8, "젤"}, {0xD9, "젬"}, {0xDA, "젭"}, {0xDB, "젯"}, {0xDC, "젱"}, {0xDD, "져"}, {0xDE, "젼"},
            {0xE0, "졀"}, {0xE1, "졈"}, {0xE2, "졉"}, {0xE3, "졌"}, {0xE4, "졍"}, {0xE5, "졔"}, {0xE6, "조"}, {0xE7, "족"}, {0xE8, "존"}, {0xE9, "졸"}, {0xEA, "졺"}, {0xEB, "좀"}, {0xEC, "좁"}, {0xED, "좃"}, {0xEE, "종"}, {0xEF, "좆"},
            {0xF0, "좇"}, {0xF1, "좋"}, {0xF2, "좌"}, {0xF3, "좍"}, {0xF4, "좔"}, {0xF5, "좝"}, {0xF6, "좟"}, {0xF7, "좡"}, {0xF8, "좨"}, {0xF9, "좼"}, {0xFA, "좽"}, {0xFB, "죄"}, {0xFC, "죈"}, {0xFD, "죌"}, {0xFE, "죔"}, {0xFF, "죕"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_8 = new Dictionary<byte, string> {
            {0x00, "죗"}, {0x01, "죙"}, {0x02, "죠"}, {0x03, "죡"}, {0x04, "죤"}, {0x05, "죵"}, {0x06, "주"}, {0x07, "죽"}, {0x08, "준"}, {0x09, "줄"}, {0x0A, "줅"}, {0x0B, "줆"}, {0x0C, "줌"}, {0x0D, "줍"}, {0x0E, "줏"}, {0x0F, "중"},
            {0x10, "줘"}, {0x11, "줬"}, {0x12, "줴"}, {0x13, "쥐"}, {0x14, "쥑"}, {0x15, "쥔"}, {0x16, "쥘"}, {0x17, "쥠"}, {0x18, "쥡"}, {0x19, "쥣"}, {0x1A, "쥬"}, {0x1B, "쥰"}, {0x1C, "쥴"}, {0x1D, "쥼"}, {0x1E, "즈"}, {0x1F, "즉"},
            {0x20, "즌"}, {0x21, "즐"}, {0x22, "즘"}, {0x23, "즙"}, {0x24, "즛"}, {0x25, "증"}, {0x26, "지"}, {0x27, "직"}, {0x28, "진"}, {0x29, "짇"}, {0x2A, "질"}, {0x2B, "짊"}, {0x2C, "짐"}, {0x2D, "집"}, {0x2E, "짓"},
            {0x30, "쬬"}, {0x31, "징"}, {0x32, "짖"}, {0x33, "짙"}, {0x34, "짚"}, {0x35, "짜"}, {0x36, "짝"}, {0x37, "짠"}, {0x38, "짢"}, {0x39, "짤"}, {0x3A, "짧"}, {0x3B, "짬"}, {0x3C, "짭"}, {0x3D, "짯"}, {0x3E, "짰"}, {0x3F, "짱"},
            {0x40, "째"}, {0x41, "짹"}, {0x42, "짼"}, {0x43, "쨀"}, {0x44, "쨈"}, {0x45, "쨉"}, {0x46, "쨋"}, {0x47, "쨌"}, {0x48, "쨍"}, {0x49, "쨔"}, {0x4A, "쨘"}, {0x4B, "쨩"}, {0x4C, "쩌"}, {0x4D, "쩍"}, {0x4E, "쩐"}, {0x4F, "쩔"},
            {0x60, "쩜"}, {0x61, "쩝"}, {0x62, "쩟"}, {0x63, "쩠"}, {0x64, "쩡"}, {0x65, "쩨"}, {0x66, "쩽"}, {0x67, "쪄"}, {0x68, "쪘"}, {0x69, "쪼"}, {0x6A, "쪽"}, {0x6B, "쫀"}, {0x6C, "쫄"}, {0x6D, "쫌"}, {0x6E, "쫍"}, {0x6F, "쫏"},
            {0x70, "쫑"}, {0x71, "쫓"}, {0x72, "쫘"}, {0x73, "쫙"}, {0x74, "쫠"}, {0x75, "쫬"}, {0x76, "쫴"}, {0x77, "쬈"}, {0x78, "쬐"}, {0x79, "쬔"}, {0x7A, "쬘"}, {0x7B, "쬠"}, {0x7C, "쬡"}, {0x7D, "쭁"}, {0x7E, "쭈"}, {0x7F, "쭉"},
            {0x80, "쭌"}, {0x81, "쭐"}, {0x82, "쭘"}, {0x83, "쭙"}, {0x84, "쭝"}, {0x85, "쭤"}, {0x86, "쭸"}, {0x87, "쭹"}, {0x88, "쮜"}, {0x89, "쮸"}, {0x8A, "쯔"}, {0x8B, "쯤"}, {0x8C, "쯧"}, {0x8D, "쯩"}, {0x8E, "찌"}, {0x8F, "찍"},
            {0x90, "찐"}, {0x91, "찔"}, {0x92, "찜"}, {0x93, "찝"}, {0x94, "찡"}, {0x95, "찢"}, {0x96, "찧"}, {0x97, "차"}, {0x98, "착"}, {0x99, "찬"}, {0x9A, "찮"}, {0x9B, "찰"}, {0x9C, "참"}, {0x9D, "찹"}, {0x9E, "찻"},
            {0xA0, "찼"}, {0xA1, "창"}, {0xA2, "찾"}, {0xA3, "채"}, {0xA4, "책"}, {0xA5, "챈"}, {0xA6, "챌"}, {0xA7, "챔"}, {0xA8, "챕"}, {0xA9, "챗"}, {0xAA, "챘"}, {0xAB, "챙"}, {0xAC, "챠"}, {0xAD, "챤"}, {0xAE, "챦"},
            {0xB0, "챨"}, {0xB1, "챰"}, {0xB2, "챵"}, {0xB3, "처"}, {0xB4, "척"}, {0xB5, "천"}, {0xB6, "철"}, {0xB7, "첨"}, {0xB8, "첩"}, {0xB9, "첫"}, {0xBA, "첬"}, {0xBB, "청"}, {0xBC, "체"}, {0xBD, "첵"}, {0xBE, "첸"}, {0xBF, "첼"},
            {0xC0, "쳄"}, {0xC1, "쳅"}, {0xC2, "쳇"}, {0xC3, "쳉"}, {0xC4, "쳐"}, {0xC5, "쳔"}, {0xC6, "쳤"}, {0xC7, "쳬"}, {0xC8, "쳰"}, {0xC9, "촁"}, {0xCA, "초"}, {0xCB, "촉"}, {0xCC, "촌"}, {0xCD, "촐"}, {0xCE, "촘"}, {0xCF, "촙"},
            {0xD0, "촛"}, {0xD1, "총"}, {0xD2, "촤"}, {0xD3, "촨"}, {0xD4, "촬"}, {0xD5, "촹"}, {0xD6, "최"}, {0xD7, "쵠"}, {0xD8, "쵤"}, {0xD9, "쵬"}, {0xDA, "쵭"}, {0xDB, "쵯"}, {0xDC, "쵱"}, {0xDD, "쵸"}, {0xDE, "춈"}, {0xDF, "추"},
            {0xE0, "축"}, {0xE1, "춘"}, {0xE2, "출"}, {0xE3, "춤"}, {0xE4, "춥"}, {0xE5, "춧"}, {0xE6, "충"}, {0xE7, "춰"}, {0xE8, "췄"}, {0xE9, "췌"}, {0xEA, "췐"}, {0xEB, "취"}, {0xEC, "췬"}, {0xED, "췰"}, {0xEE, "췸"}, {0xEF, "췹"},
            {0xF0, "췻"}, {0xF1, "췽"}, {0xF2, "츄"}, {0xF3, "츈"}, {0xF4, "츌"}, {0xF5, "츔"}, {0xF6, "츙"}, {0xF7, "츠"}, {0xF8, "측"}, {0xF9, "츤"}, {0xFA, "츨"}, {0xFB, "츰"}, {0xFC, "츱"}, {0xFD, "츳"}, {0xFE, "층"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_9 = new Dictionary<byte, string> {
            {0x00, "치"}, {0x01, "칙"}, {0x02, "친"}, {0x03, "칟"}, {0x04, "칠"}, {0x05, "칡"}, {0x06, "침"}, {0x07, "칩"}, {0x08, "칫"}, {0x09, "칭"}, {0x0A, "카"}, {0x0B, "칵"}, {0x0C, "칸"}, {0x0D, "칼"}, {0x0E, "캄"},
            {0x10, "캅"}, {0x11, "캇"}, {0x12, "캉"}, {0x13, "캐"}, {0x14, "캑"}, {0x15, "캔"}, {0x16, "캘"}, {0x17, "캠"}, {0x18, "캡"}, {0x19, "캣"}, {0x1A, "캤"}, {0x1B, "캥"}, {0x1C, "캬"}, {0x1D, "캭"}, {0x1E, "컁"}, {0x1F, "커"},
            {0x20, "컥"}, {0x21, "컨"}, {0x22, "컫"}, {0x23, "컬"}, {0x24, "컴"}, {0x25, "컵"}, {0x26, "컷"}, {0x27, "컸"}, {0x28, "컹"}, {0x29, "케"}, {0x2A, "켁"}, {0x2B, "켄"}, {0x2C, "켈"}, {0x2D, "켐"}, {0x2E, "켑"}, {0x2F, "켓"},
            {0x30, "켕"}, {0x31, "켜"}, {0x32, "켠"}, {0x33, "켤"}, {0x34, "켬"}, {0x35, "켭"}, {0x36, "켯"}, {0x37, "켰"}, {0x38, "켱"}, {0x39, "켸"}, {0x3A, "코"}, {0x3B, "콕"}, {0x3C, "콘"}, {0x3D, "콜"}, {0x3E, "콤"}, {0x3F, "콥"},
            {0x40, "콧"}, {0x41, "콩"}, {0x42, "콰"}, {0x43, "콱"}, {0x44, "콴"}, {0x45, "콸"}, {0x46, "쾀"}, {0x47, "쾅"}, {0x48, "쾌"}, {0x49, "쾡"}, {0x4A, "쾨"}, {0x4B, "쾰"}, {0x4C, "쿄"}, {0x4D, "쿠"}, {0x4E, "쿡"}, {0x4F, "쿤"},
            {0x60, "쿨"}, {0x61, "쿰"}, {0x62, "쿱"}, {0x63, "쿳"}, {0x64, "쿵"}, {0x65, "쿼"}, {0x66, "퀀"}, {0x67, "퀄"}, {0x68, "퀑"}, {0x69, "퀘"}, {0x6A, "퀭"}, {0x6B, "퀴"}, {0x6C, "퀵"}, {0x6D, "퀸"}, {0x6E, "퀼"},
            {0x70, "큄"}, {0x71, "큅"}, {0x72, "큇"}, {0x73, "큉"}, {0x74, "큐"}, {0x75, "큔"}, {0x76, "큘"}, {0x77, "큠"}, {0x78, "크"}, {0x79, "큭"}, {0x7A, "큰"}, {0x7B, "클"}, {0x7C, "큼"}, {0x7D, "큽"}, {0x7E, "킁"},
            {0x80, "키"}, {0x81, "킥"}, {0x82, "킨"}, {0x83, "킬"}, {0x84, "킴"}, {0x85, "킵"}, {0x86, "킷"}, {0x87, "킹"}, {0x88, "타"}, {0x89, "탁"}, {0x8A, "탄"}, {0x8B, "탈"}, {0x8C, "탉"}, {0x8D, "탐"}, {0x8E, "탑"}, {0x8F, "탓"},
            {0x90, "탔"}, {0x91, "탕"}, {0x92, "태"}, {0x93, "택"}, {0x94, "탠"}, {0x95, "탤"}, {0x96, "탬"}, {0x97, "탭"}, {0x98, "탯"}, {0x99, "탰"}, {0x9A, "탱"}, {0x9B, "탸"}, {0x9C, "턍"}, {0x9D, "터"}, {0x9E, "턱"}, {0x9F, "턴"},
            {0xA0, "털"}, {0xA1, "턺"}, {0xA2, "텀"}, {0xA3, "텁"}, {0xA4, "텃"}, {0xA5, "텄"}, {0xA6, "텅"}, {0xA7, "테"}, {0xA8, "텍"}, {0xA9, "텐"}, {0xAA, "텔"}, {0xAB, "템"}, {0xAC, "텝"}, {0xAD, "텟"}, {0xAE, "텡"}, {0xAF, "텨"},
            {0xB0, "텬"}, {0xB1, "텼"}, {0xB2, "톄"}, {0xB3, "톈"}, {0xB4, "토"}, {0xB5, "톡"}, {0xB6, "톤"}, {0xB7, "톨"}, {0xB8, "톰"}, {0xB9, "톱"}, {0xBA, "톳"}, {0xBB, "통"}, {0xBC, "톺"}, {0xBD, "톼"}, {0xBE, "퇀"}, {0xBF, "퇘"},
            {0xC0, "퇴"}, {0xC1, "퇸"}, {0xC2, "툇"}, {0xC3, "툉"}, {0xC4, "툐"}, {0xC5, "투"}, {0xC6, "툭"}, {0xC7, "툰"}, {0xC8, "툴"}, {0xC9, "툼"}, {0xCA, "툽"}, {0xCB, "툿"}, {0xCC, "퉁"}, {0xCD, "퉈"}, {0xCE, "퉜"},
            {0xD0, "퉤"}, {0xD1, "튀"}, {0xD2, "튁"}, {0xD3, "튄"}, {0xD4, "튈"}, {0xD5, "튐"}, {0xD6, "튑"}, {0xD7, "튕"}, {0xD8, "튜"}, {0xD9, "튠"}, {0xDA, "튤"}, {0xDB, "튬"}, {0xDC, "튱"}, {0xDD, "트"}, {0xDE, "특"},
            {0xE0, "튼"}, {0xE1, "튿"}, {0xE2, "틀"}, {0xE3, "틂"}, {0xE4, "틈"}, {0xE5, "틉"}, {0xE6, "틋"}, {0xE7, "틔"}, {0xE8, "틘"}, {0xE9, "틜"}, {0xEA, "틤"}, {0xEB, "틥"}, {0xEC, "티"}, {0xED, "틱"}, {0xEE, "틴"}, {0xEF, "틸"},
            {0xF0, "팀"}, {0xF1, "팁"}, {0xF2, "팃"}, {0xF3, "팅"}, {0xF4, "파"}, {0xF5, "팍"}, {0xF6, "팎"}, {0xF7, "판"}, {0xF8, "팔"}, {0xF9, "팖"}, {0xFA, "팜"}, {0xFB, "팝"}, {0xFC, "팟"}, {0xFD, "팠"}, {0xFE, "팡"}, {0xFF, "팥"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_A = new Dictionary<byte, string> {
            {0x00, "패"}, {0x01, "팩"}, {0x02, "팬"}, {0x03, "팰"}, {0x04, "팸"}, {0x05, "팹"}, {0x06, "팻"}, {0x07, "팼"}, {0x08, "팽"}, {0x09, "퍄"}, {0x0A, "퍅"}, {0x0B, "퍼"}, {0x0C, "퍽"}, {0x0D, "펀"}, {0x0E, "펄"}, {0x0F, "펌"},
            {0x10, "펍"}, {0x11, "펏"}, {0x12, "펐"}, {0x13, "펑"}, {0x14, "페"}, {0x15, "펙"}, {0x16, "펜"}, {0x17, "펠"}, {0x18, "펨"}, {0x19, "펩"}, {0x1A, "펫"}, {0x1B, "펭"}, {0x1C, "펴"}, {0x1D, "편"}, {0x1E, "펼"}, {0x1F, "폄"},
            {0x20, "폅"}, {0x21, "폈"}, {0x22, "평"}, {0x23, "폐"}, {0x24, "폘"}, {0x25, "폡"}, {0x26, "폣"}, {0x27, "포"}, {0x28, "폭"}, {0x29, "폰"}, {0x2A, "폴"}, {0x2B, "폼"}, {0x2C, "폽"}, {0x2D, "폿"}, {0x2E, "퐁"},
            {0x30, "퐈"}, {0x31, "퐝"}, {0x32, "푀"}, {0x33, "푄"}, {0x34, "표"}, {0x35, "푠"}, {0x36, "푤"}, {0x37, "푭"}, {0x38, "푯"}, {0x39, "푸"}, {0x3A, "푹"}, {0x3B, "푼"}, {0x3C, "푿"}, {0x3D, "풀"}, {0x3E, "풂"},
            {0x40, "품"}, {0x41, "풉"}, {0x42, "풋"}, {0x43, "풍"}, {0x44, "풔"}, {0x45, "풩"}, {0x46, "퓌"}, {0x47, "퓐"}, {0x48, "퓔"}, {0x49, "퓜"}, {0x4A, "퓟"}, {0x4B, "퓨"}, {0x4C, "퓬"}, {0x4D, "퓰"}, {0x4E, "퓸"}, {0x4F, "퓻"},
            {0x60, "퓽"}, {0x61, "프"}, {0x62, "픈"}, {0x63, "플"}, {0x64, "픔"}, {0x65, "픕"}, {0x66, "픗"}, {0x67, "피"}, {0x68, "픽"}, {0x69, "핀"}, {0x6A, "필"}, {0x6B, "핌"}, {0x6C, "핍"}, {0x6D, "핏"}, {0x6E, "핑"}, {0x6F, "하"},
            {0x70, "학"}, {0x71, "한"}, {0x72, "할"}, {0x73, "핥"}, {0x74, "함"}, {0x75, "합"}, {0x76, "핫"}, {0x77, "항"}, {0x78, "해"}, {0x79, "핵"}, {0x7A, "핸"}, {0x7B, "핼"}, {0x7C, "햄"}, {0x7D, "햅"}, {0x7E, "햇"}, {0x7F, "했"},
            {0x80, "행"}, {0x81, "햐"}, {0x82, "향"}, {0x83, "허"}, {0x84, "헉"}, {0x85, "헌"}, {0x86, "헐"}, {0x87, "헒"}, {0x88, "험"}, {0x89, "헙"}, {0x8A, "헛"}, {0x8B, "헝"}, {0x8C, "헤"}, {0x8D, "헥"}, {0x8E, "헨"}, {0x8F, "헬"},
            {0x90, "헴"}, {0x91, "헵"}, {0x92, "헷"}, {0x93, "헹"}, {0x94, "혀"}, {0x95, "혁"}, {0x96, "현"}, {0x97, "혈"}, {0x98, "혐"}, {0x99, "협"}, {0x9A, "혓"}, {0x9B, "혔"}, {0x9C, "형"}, {0x9D, "혜"}, {0x9E, "혠"},
            {0xA0, "혤"}, {0xA1, "혭"}, {0xA2, "호"}, {0xA3, "혹"}, {0xA4, "혼"}, {0xA5, "홀"}, {0xA6, "홅"}, {0xA7, "홈"}, {0xA8, "홉"}, {0xA9, "홋"}, {0xAA, "홍"}, {0xAB, "홑"}, {0xAC, "화"}, {0xAD, "확"}, {0xAE, "환"},
            {0xB0, "활"}, {0xB1, "홧"}, {0xB2, "황"}, {0xB3, "홰"}, {0xB4, "홱"}, {0xB5, "홴"}, {0xB6, "횃"}, {0xB7, "횅"}, {0xB8, "회"}, {0xB9, "획"}, {0xBA, "횐"}, {0xBB, "횔"}, {0xBC, "횝"}, {0xBD, "횟"}, {0xBE, "횡"}, {0xBF, "효"},
            {0xC0, "횬"}, {0xC1, "횰"}, {0xC2, "횹"}, {0xC3, "횻"}, {0xC4, "후"}, {0xC5, "훅"}, {0xC6, "훈"}, {0xC7, "훌"}, {0xC8, "훑"}, {0xC9, "훔"}, {0xCA, "훗"}, {0xCB, "훙"}, {0xCC, "훠"}, {0xCD, "훤"}, {0xCE, "훨"}, {0xCF, "훰"},
            {0xD0, "훵"}, {0xD1, "훼"}, {0xD2, "훽"}, {0xD3, "휀"}, {0xD4, "휄"}, {0xD5, "휑"}, {0xD6, "휘"}, {0xD7, "휙"}, {0xD8, "휜"}, {0xD9, "휠"}, {0xDA, "휨"}, {0xDB, "휩"}, {0xDC, "휫"}, {0xDD, "휭"}, {0xDE, "휴"}, {0xDF, "휵"},
            {0xE0, "휸"}, {0xE1, "휼"}, {0xE2, "흄"}, {0xE3, "흇"}, {0xE4, "흉"}, {0xE5, "흐"}, {0xE6, "흑"}, {0xE7, "흔"}, {0xE8, "흖"}, {0xE9, "흗"}, {0xEA, "흘"}, {0xEB, "흙"}, {0xEC, "흠"}, {0xED, "흡"}, {0xEE, "흣"}, {0xEF, "흥"},
            {0xF0, "흩"}, {0xF1, "희"}, {0xF2, "흰"}, {0xF3, "흴"}, {0xF4, "흼"}, {0xF5, "흽"}, {0xF6, "힁"}, {0xF7, "히"}, {0xF8, "힉"}, {0xF9, "힌"}, {0xFA, "힐"}, {0xFB, "힘"}, {0xFC, "힙"}, {0xFD, "힛"}, {0xFE, "힝"},
        };
        private static readonly Dictionary<byte, string> GSC2U_KOR_B = new Dictionary<byte, string> {
            {0x00, "ㄱ"}, {0x01, "ㄴ"}, {0x02, "ㄷ"}, {0x03, "ㄹ"}, {0x04, "ㅁ"}, {0x05, "ㅂ"}, {0x06, "ㅅ"}, {0x07, "ㅇ"}, {0x08, "ㅈ"}, {0x09, "ㅊ"}, {0x0A, "ㅋ"}, {0x0B, "ㅌ"}, {0x0C, "ㅍ"}, {0x0D, "ㅎ"}, {0x0E, "ㄲ"}, {0x0F, "ㄸ"},
            {0x10, "ㅃ"}, {0x11, "ㅆ"}, {0x12, "ㅉ"},
            {0x20, "ㅏ"}, {0x21, "ㅑ"}, {0x22, "ㅓ"}, {0x23, "ㅕ"}, {0x24, "ㅗ"}, {0x25, "ㅛ"}, {0x26, "ㅜ"}, {0x27, "ㅠ"}, {0x28, "ㅡ"}, {0x29, "ㅣ"}, {0x2A, "ㅐ"}, {0x2B, "ㅒ"}, {0x2C, "ㅔ"}, {0x2D, "ㅖ"}, {0x2E, "ㅘ"}, {0x2F, "ㅙ"},
            {0x30, "ㅚ"}, {0x31, "ㅝ"}, {0x32, "ㅞ"}, {0x33, "ㅟ"}, {0x34, "ㅢ"}, {0x35, "_"}, {0x36, "—"},
            {0x60, "「"}, {0x61, "」"}, {0x62, "『"}, {0x63, "』"}, {0x64, "("}, {0x65, ")"}, {0x66, "!"}, {0x67, "?"}, {0x68, "-"}, {0x69, "~"}, {0x6A, "⋯"}, {0x6B, ","}, {0x6C, "."},
            {0xF0, "0"}, {0xF1, "1"}, {0xF2, "2"}, {0xF3, "3"}, {0xF4, "4"}, {0xF5, "5"}, {0xF6, "6"}, {0xF7, "7"}, {0xF8, "8"}, {0xF9, "9"},
        };

        private static readonly Dictionary<string, byte>[] U2GSC_KOR =
        {
            U2GSC_KOR_0, U2GSC_KOR_1, U2GSC_KOR_2, U2GSC_KOR_3,
            U2GSC_KOR_4, U2GSC_KOR_5, U2GSC_KOR_6, U2GSC_KOR_7,
            U2GSC_KOR_8, U2GSC_KOR_9, U2GSC_KOR_A, U2GSC_KOR_B,
        };
        private static readonly Dictionary<byte, string>[] GSC2U_KOR =
        {
            GSC2U_KOR_0, GSC2U_KOR_1, GSC2U_KOR_2, GSC2U_KOR_3,
            GSC2U_KOR_4, GSC2U_KOR_5, GSC2U_KOR_6, GSC2U_KOR_7,
            GSC2U_KOR_8, GSC2U_KOR_9, GSC2U_KOR_A, GSC2U_KOR_B,
        };

        #endregion

        #region Gen 3/4 Character Tables (Val->Unicode)
        private static readonly ushort[] G4Values =
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75,
            76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123,
            124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145,
            146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167,
            168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
            190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211,
            212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234,
            235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256,
            257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278,
            279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
            301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322,
            323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344,
            345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366,
            367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388,
            389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410,
            411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432,
            433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454,
            455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476,
            477, 478, 479, 480, 481, 488, 489, 490,

            1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 1037, 1038, 1039,
            1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053,
            1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071,
            1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089,
            1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1104, 1105, 1106, 1107,
            1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125,
            1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143,
            1144, 1145, 1146, 1147, 1148, 1149, 1150, 1151, 1152, 1153, 1154, 1155, 1156, 1157, 1158, 1159, 1160, 1161,
            1162, 1163, 1164, 1165, 1166, 1167, 1168, 1169, 1170, 1171, 1172, 1173, 1174, 1175, 1176, 1177, 1178, 1179,
            1180, 1181, 1182, 1183, 1184, 1185, 1186, 1187, 1188, 1189, 1190, 1191, 1192, 1193, 1194, 1195, 1196, 1197,
            1198, 1199, 1200, 1201, 1202, 1203, 1204, 1205, 1206, 1207, 1208, 1209, 1210, 1211, 1212, 1213, 1214, 1215,
            1216, 1217, 1218, 1219, 1220, 1221, 1222, 1223, 1224, 1225, 1226, 1227, 1228, 1229, 1230, 1231, 1232, 1233,
            1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251,
            1252, 1253, 1254, 1255, 1256, 1257, 1258, 1259, 1260, 1261, 1262, 1263, 1264, 1265, 1266, 1267, 1268, 1269,
            1270, 1271, 1272, 1273, 1274, 1275, 1276, 1277, 1278, 1279, 1280, 1281, 1282, 1283, 1284, 1285, 1286, 1287,
            1288, 1289, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297, 1298, 1299, 1300, 1301, 1302, 1303, 1304, 1305,
            1306, 1307, 1308, 1309, 1310, 1311, 1312, 1313, 1314, 1315, 1316, 1317, 1318, 1319, 1320, 1321, 1322, 1323,
            1324, 1325, 1326, 1327, 1328, 1329, 1330, 1331, 1332, 1333, 1334, 1335, 1336, 1337, 1338, 1339, 1340, 1341,
            1342, 1343, 1344, 1345, 1346, 1347, 1348, 1349, 1350, 1351, 1352, 1353, 1354, 1355, 1356, 1357, 1358, 1359,
            1360, 1361, 1362, 1363, 1364, 1365, 1366, 1367, 1368, 1369, 1370, 1371, 1372, 1373, 1374, 1375, 1376, 1377,
            1378, 1379, 1380, 1381, 1382, 1383, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395,
            1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413,
            1414, 1415, 1416, 1417, 1418, 1419, 1420, 1421, 1422, 1423, 1424, 1425, 1426, 1427, 1428, 1429, 1430, 1431,
            1432, 1433, 1434, 1435, 1436, 1437, 1438, 1439, 1440, 1441, 1442, 1443, 1444, 1445, 1446, 1447, 1448, 1449,
            1450, 1451, 1452, 1453, 1454, 1455, 1456, 1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, 1466, 1467,
            1468, 1469, 1470, 1471, 1472, 1473, 1474, 1475, 1476, 1477, 1478, 1479, 1480, 1481, 1482, 1483, 1484, 1485,
            1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503,
            1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 1515, 1516, 1517, 1518, 1519, 1520, 1521,
            1522, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530, 1531, 1532, 1533, 1534, 1535, 1536, 1537, 1538, 1539,
            1540, 1541, 1542, 1543, 1544, 1545, 1546, 1547, 1548, 1549, 1550, 1551, 1552, 1553, 1554, 1555, 1556, 1557,
            1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575,
            1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 1591, 1592, 1593,
            1594, 1595, 1596, 1597, 1598, 1599, 1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608, 1609, 1610, 1611,
            1612, 1613, 1614, 1615, 1616, 1617, 1618, 1619, 1620, 1621, 1622, 1623, 1624, 1625, 1626, 1627, 1628, 1629,
            1630, 1631, 1632, 1633, 1634, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1642, 1643, 1644, 1645, 1646, 1647,
            1648, 1649, 1650, 1651, 1652, 1653, 1654, 1655, 1656, 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664, 1665,
            1666, 1667, 1668, 1669, 1670, 1671, 1672, 1673, 1674, 1675, 1676, 1677, 1678, 1679, 1680, 1681, 1682, 1683,
            1684, 1685, 1686, 1687, 1688, 1689, 1690, 1691, 1692, 1693, 1694, 1695, 1696, 1697, 1698, 1699, 1700, 1701,
            1702, 1703, 1704, 1705, 1706, 1707, 1708, 1709, 1710, 1711, 1712, 1713, 1714, 1715, 1716, 1717, 1718, 1719,
            1720, 1721, 1722, 1723, 1724, 1725, 1726, 1727, 1728, 1729, 1730, 1731, 1732, 1733, 1734, 1735, 1736, 1737,
            1738, 1739, 1740, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749, 1750, 1751, 1752, 1753, 1754, 1755,
            1756, 1757, 1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765, 1766, 1767, 1768, 1769, 1770, 1771, 1772, 1773,
            1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1791,
            1792, 1793, 1794, 1795, 1796, 1797, 1798, 1799, 1800, 1801, 1802, 1803, 1804, 1805, 1806, 1807, 1808, 1809,
            1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823, 1824, 1825, 1826, 1827,
            1828, 1829, 1830, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839, 1840, 1841, 1842, 1843, 1844, 1845,
            1846, 1847, 1848, 1849, 1850, 1851, 1852, 1853, 1854, 1855, 1856, 1857, 1858, 1859, 1860, 1861, 1862, 1863,
            1864, 1865, 1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875, 1876, 1877, 1878, 1879, 1880, 1881,
            1882, 1883, 1884, 1885, 1886, 1887, 1888, 1889, 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897, 1898, 1899,
            1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907, 1908, 1909, 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917,
            1918, 1919, 1920, 1921, 1922, 1923, 1924, 1925, 1926, 1927, 1928, 1929, 1930, 1931, 1932, 1933, 1934, 1935,
            1936, 1937, 1938, 1939, 1940, 1941, 1942, 1943, 1944, 1945, 1946, 1947, 1948, 1949, 1950, 1951, 1952, 1953,
            1954, 1955, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971,
            1972, 1973, 1974, 1975, 1976, 1977, 1978, 1979, 1980, 1981, 1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989,
            1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007,
            2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025,
            2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035, 2036, 2037, 2038, 2039, 2040, 2041, 2042, 2043,
            2044, 2045, 2046, 2047, 2048, 2049, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057, 2058, 2059, 2060, 2061,
            2062, 2063, 2064, 2065, 2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073, 2074, 2075, 2076, 2077, 2078, 2079,
            2080, 2081, 2082, 2083, 2084, 2085, 2086, 2087, 2088, 2089, 2090, 2091, 2092, 2093, 2094, 2095, 2096, 2097,
            2098, 2099, 2100, 2101, 2102, 2103, 2104, 2105, 2106, 2107, 2108, 2109, 2110, 2111, 2112, 2113, 2114, 2115,
            2116, 2117, 2118, 2119, 2120, 2121, 2122, 2123, 2124, 2125, 2126, 2127, 2128, 2129, 2130, 2131, 2132, 2133,
            2134, 2135, 2136, 2137, 2138, 2139, 2140, 2141, 2142, 2143, 2144, 2145, 2146, 2147, 2148, 2149, 2150, 2151,
            2152, 2153, 2154, 2155, 2156, 2157, 2158, 2159, 2160, 2161, 2162, 2163, 2164, 2165, 2166, 2167, 2168, 2169,
            2170, 2171, 2172, 2173, 2174, 2175, 2176, 2177, 2178, 2179, 2180, 2181, 2182, 2183, 2184, 2185, 2186, 2187,
            2188, 2189, 2190, 2191, 2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199, 2200, 2201, 2202, 2203, 2204, 2205,
            2206, 2207, 2208, 2209, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220, 2221, 2222, 2223,
            2224, 2225, 2226, 2227, 2228, 2229, 2230, 2231, 2232, 2233, 2234, 2235, 2236, 2237, 2238, 2239, 2240, 2241,
            2242, 2243, 2244, 2245, 2246, 2247, 2248, 2249, 2250, 2251, 2252, 2253, 2254, 2255, 2256, 2257, 2258, 2259,
            2260, 2261, 2262, 2263, 2264, 2265, 2266, 2267, 2268, 2269, 2270, 2271, 2272, 2273, 2274, 2275, 2276, 2277,
            2278, 2279, 2280, 2281, 2282, 2283, 2284, 2285, 2286, 2287, 2288, 2289, 2290, 2291, 2292, 2293, 2294, 2295,
            2296, 2297, 2298, 2299, 2300, 2301, 2302, 2303, 2304, 2305, 2306, 2307, 2308, 2309, 2310, 2311, 2312, 2313,
            2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2326, 2327, 2328, 2329, 2330, 2331,
            2332, 2333, 2334, 2335, 2336, 2337, 2338, 2339, 2340, 2341, 2342, 2343, 2344, 2345, 2346, 2347, 2348, 2349,
            2350, 2351, 2352, 2353, 2354, 2355, 2356, 2357, 2358, 2359, 2360, 2361, 2362, 2363, 2364, 2365, 2366, 2367,
            2368, 2369, 2370, 2371, 2372, 2373, 2374, 2375, 2376, 2377, 2378, 2379, 2380, 2381, 2382, 2383, 2384, 2385,
            2386, 2387, 2388, 2389, 2390, 2391, 2392, 2393, 2394, 2395, 2396, 2397, 2398, 2399, 2400, 2401, 2402, 2403,
            2404, 2405, 2406, 2407, 2408, 2409, 2410, 2411, 2412, 2413, 2414, 2415, 2416, 2417, 2418, 2419, 2420, 2421,
            2422, 2423, 2424, 2425, 2426, 2427, 2428, 2429, 2430, 2431, 2432, 2433, 2434, 2435, 2436, 2437, 2438, 2439,
            2440, 2441, 2442, 2443, 2444, 2445, 2446, 2447, 2448, 2449, 2450, 2451, 2452, 2453, 2454, 2455, 2456, 2457,
            2458, 2459, 2460, 2461, 2462, 2463, 2464, 2465, 2466, 2467, 2468, 2469, 2470, 2471, 2472, 2473, 2474, 2475,
            2476, 2477, 2478, 2479, 2480, 2481, 2482, 2483, 2484, 2485, 2486, 2487, 2488, 2489, 2490, 2491, 2492, 2493,
            2494, 2495, 2496, 2497, 2498, 2499, 2500, 2501, 2502, 2503, 2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511,
            2512, 2513, 2514, 2515, 2516, 2517, 2518, 2519, 2520, 2521, 2522, 2523, 2524, 2525, 2526, 2527, 2528, 2529,
            2530, 2531, 2532, 2533, 2534, 2535, 2536, 2537, 2538, 2539, 2540, 2541, 2542, 2543, 2544, 2545, 2546, 2547,
            2548, 2549, 2550, 2551, 2552, 2553, 2554, 2555, 2556, 2557, 2558, 2559, 2560, 2561, 2562, 2563, 2564, 2565,
            2566, 2567, 2568, 2569, 2570, 2571, 2572, 2573, 2574, 2575, 2576, 2577, 2578, 2579, 2580, 2581, 2582, 2583,
            2584, 2585, 2586, 2587, 2588, 2589, 2590, 2591, 2592, 2593, 2594, 2595, 2596, 2597, 2598, 2599, 2600, 2601,
            2602, 2603, 2604, 2605, 2606, 2607, 2608, 2609, 2610, 2611, 2612, 2613, 2614, 2615, 2616, 2617, 2618, 2619,
            2620, 2621, 2622, 2623, 2624, 2625, 2626, 2627, 2628, 2629, 2630, 2631, 2632, 2633, 2634, 2635, 2636, 2637,
            2638, 2639, 2640, 2641, 2642, 2643, 2644, 2645, 2646, 2647, 2648, 2649, 2650, 2651, 2652, 2653, 2654, 2655,
            2656, 2657, 2658, 2659, 2660, 2661, 2662, 2663, 2664, 2665, 2666, 2667, 2668, 2669, 2670, 2671, 2672, 2673,
            2674, 2675, 2676, 2677, 2678, 2679, 2680, 2681, 2682, 2683, 2684, 2685, 2686, 2687, 2688, 2689, 2690, 2691,
            2692, 2693, 2694, 2695, 2696, 2697, 2698, 2699, 2700, 2701, 2702, 2703, 2704, 2705, 2706, 2707, 2708, 2709,
            2710, 2711, 2712, 2713, 2714, 2715, 2716, 2717, 2718, 2719, 2720, 2721, 2722, 2723, 2724, 2725, 2726, 2727,
            2728, 2729, 2730, 2731, 2732, 2733, 2734, 2735, 2736, 2737, 2738, 2739, 2740, 2741, 2742, 2743, 2744, 2745,
            2746, 2747, 2748, 2749, 2750, 2751, 2752, 2753, 2754, 2755, 2756, 2757, 2758, 2759, 2760, 2761, 2762, 2763,
            2764, 2765, 2766, 2767, 2768, 2769, 2770, 2771, 2772, 2773, 2774, 2775, 2776, 2777, 2778, 2779, 2780, 2781,
            2782, 2783, 2784, 2785, 2786, 2787, 2788, 2789, 2790, 2791, 2792, 2793, 2794, 2795, 2796, 2797, 2798, 2799,
            2800, 2801, 2802, 2803, 2804, 2805, 2806, 2807, 2808, 2809, 2810, 2811, 2812, 2813, 2814, 2815, 2816, 2817,
            2818, 2819, 2820, 2821, 2822, 2823, 2824, 2825, 2826, 2827, 2828, 2829, 2830, 2831, 2832, 2833, 2834, 2835,
            2836, 2837, 2838, 2839, 2840, 2841, 2842, 2843, 2844, 2845, 2846, 2847, 2848, 2849, 2850, 2851, 2852, 2853,
            2854, 2855, 2856, 2857, 2858, 2859, 2860, 2861, 2862, 2863, 2864, 2865, 2866, 2867, 2868, 2869, 2870, 2871,
            2872, 2873, 2874, 2875, 2876, 2877, 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 2886, 2887, 2888, 2889,
            2890, 2891, 2892, 2893, 2894, 2895, 2896, 2897, 2898, 2899, 2900, 2901, 2902, 2903, 2904, 2905, 2906, 2907,
            2908, 2909, 2910, 2911, 2912, 2913, 2914, 2915, 2916, 2917, 2918, 2919, 2920, 2921, 2922, 2923, 2924, 2925,
            2926, 2927, 2928, 2929, 2930, 2931, 2932, 2933, 2934, 2935, 2936, 2937, 2938, 2939, 2940, 2941, 2942, 2943,
            2944, 2945, 2946, 2947, 2948, 2949, 2950, 2951, 2952, 2953, 2954, 2955, 2956, 2957, 2958, 2959, 2960, 2961,
            2962, 2963, 2964, 2965, 2966, 2967, 2968, 2969, 2970, 2971, 2972, 2973, 2974, 2975, 2976, 2977, 2978, 2979,
            2980, 2981, 2982, 2983, 2984, 2985, 2986, 2987, 2988, 2989, 2990, 2991, 2992, 2993, 2994, 2995, 2996, 2997,
            2998, 2999, 3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015,
            3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030, 3031, 3032, 3033,
            3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046, 3047, 3048, 3049, 3050, 3051,
            3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064, 3065, 3066, 3067, 3068, 3069,
            3070, 3071, 3072, 3073, 3074, 3075, 3076, 3077, 3078, 3079, 3080, 3081, 3082, 3083, 3084, 3085, 3086, 3087,
            3088, 3089, 3090, 3091, 3092, 3093, 3094, 3095, 3096, 3097, 3098, 3099, 3100, 3101, 3102, 3103, 3104, 3105,
            3106, 3107, 3108, 3109, 3110, 3111, 3112, 3113, 3114, 3115, 3116, 3117, 3118, 3119, 3120, 3121, 3122, 3123,
            3124, 3125, 3126, 3127, 3128, 3129, 3130, 3131, 3132, 3133, 3134, 3135, 3136, 3137, 3138, 3139, 3140, 3141,
            3142, 3143, 3144, 3145, 3146, 3147, 3148, 3149, 3150, 3151, 3152, 3153, 3154, 3155, 3156, 3157, 3158, 3159,
            3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3169, 3170, 3171, 3172, 3173, 3174, 3175, 3176, 3177,
            3178, 3179, 3180, 3181, 3182, 3183, 3184, 3185, 3186, 3187, 3188, 3189, 3190, 3191, 3192, 3193, 3194, 3195,
            3196, 3197, 3198, 3199, 3200, 3201, 3202, 3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213,
            3214, 3215, 3216, 3217, 3218, 3219, 3220, 3221, 3222, 3223, 3224, 3225, 3226, 3227, 3228, 3229, 3230, 3231,
            3232, 3233, 3234, 3235, 3236, 3237, 3238, 3239, 3240, 3241, 3242, 3243, 3244, 3245, 3246, 3247, 3248, 3249,
            3250, 3251, 3252, 3253, 3254, 3255, 3256, 3257, 3258, 3259, 3260, 3261, 3262, 3263, 3264, 3265, 3266, 3267,
            3268, 3269, 3270, 3271, 3272, 3273, 3274, 3275, 3276, 3277, 3278, 3279, 3280, 3281, 3282, 3283, 3284, 3285,
            3286, 3287, 3288, 3289, 3290, 3291, 3292, 3293, 3294, 3295, 3296, 3297, 3298, 3299, 3300, 3301, 3302, 3303,
            3304, 3305, 3306, 3307, 3308, 3309, 3310, 3311, 3312, 3313, 3314, 3315, 3316, 3317, 3318, 3319, 3320, 3321,
            3322, 3323, 3324, 3325, 3326, 3327, 3328, 3329, 3330, 3331, 3332, 3333, 3334, 3335, 3336, 3337, 3338, 3339,
            3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353, 3354, 3355, 3356, 3357,
            3358, 3359, 3360, 3361, 3362, 3363, 3364, 3365, 3366, 3367, 3368, 3369, 3370, 3371, 3372, 3373, 3374, 3377,
            3378, 3379, 3380, 3381, 3382, 3383, 3384, 3385, 3386, 3387, 3388, 3389, 3390, 3391, 3392, 3393, 3394, 3395,
            3396, 3397, 3398, 3399, 3400, 3401, 3402, 3403, 3404, 3405, 3406, 3407, 3408, 3409, 3425, 3426, 3427, 3428,
            3429, 65535
        };

        private static readonly ushort[] G4Chars =
        {
            12288, 12353, 12354, 12355, 12356, 12357, 12358, 12359, 12360, 12361, 12362, 12363,
            12364, 12365, 12366, 12367, 12368, 12369, 12370, 12371, 12372, 12373, 12374, 12375, 12376, 12377, 12378,
            12379, 12380, 12381, 12382, 12383, 12384, 12385, 12386, 12387, 12388, 12389, 12390, 12391, 12392, 12393,
            12394, 12395, 12396, 12397, 12398, 12399, 12400, 12401, 12402, 12403, 12404, 12405, 12406, 12407, 12408,
            12409, 12410, 12411, 12412, 12413, 12414, 12415, 12416, 12417, 12418, 12419, 12420, 12421, 12422, 12423,
            12424, 12425, 12426, 12427, 12428, 12429, 12431, 12434, 12435, 12449, 12450, 12451, 12452, 12453, 12454,
            12455, 12456, 12457, 12458, 12459, 12460, 12461, 12462, 12463, 12464, 12465, 12466, 12467, 12468, 12469,
            12470, 12471, 12472, 12473, 12474, 12475, 12476, 12477, 12478, 12479, 12480, 12481, 12482, 12483, 12484,
            12485, 12486, 12487, 12488, 12489, 12490, 12491, 12492, 12493, 12494, 12495, 12496, 12497, 12498, 12499,
            12500, 12501, 12502, 12503, 12504, 12505, 12506, 12507, 12508, 12509, 12510, 12511, 12512, 12513, 12514,
            12515, 12516, 12517, 12518, 12519, 12520, 12521, 12522, 12523, 12524, 12525, 12527, 12530, 12531, 65296,
            65297, 65298, 65299, 65300, 65301, 65302, 65303, 65304, 65305, 65313, 65314, 65315, 65316, 65317, 65318,
            65319, 65320, 65321, 65322, 65323, 65324, 65325, 65326, 65327, 65328, 65329, 65330, 65331, 65332, 65333,
            65334, 65335, 65336, 65337, 65338, 65345, 65346, 65347, 65348, 65349, 65350, 65351, 65352, 65353, 65354,
            65355, 65356, 65357, 65358, 65359, 65360, 65361, 65362, 65363, 65364, 65365, 65366, 65367, 65368, 65369,
            65370, 65281, 65311, 12289, 12290, 8943, 12539, 65295, 12300, 12301, 12302, 12303, 65288, 65289, 9325, 9326,
            65291, 12540, 9319, 9320, 65309, 65370, 65306, 65307, 65294, 65292, 9327, 9328, 9329, 9330, 9331, 9332, 9333,
            9334, 9335, 9336, 65312, 9337, 65285, 9338, 9339, 9341, 10052, 9739, 9812, 9813, 9738, 8663, 8664, 9790, 165,
            9800, 9801, 9802, 9803, 9804, 9805, 9806, 9807, 8592, 8593, 8595, 8594, 8227, 65286, 48, 49, 50, 51, 52, 53,
            54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87,
            88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116,
            117, 118, 119, 120, 121, 122, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
            208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
            230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 254, 255, 338, 339, 350, 351, 170, 186, 185, 178, 179, 36, 161, 191, 33, 63, 44, 46, 9324, 65381,
            47, 8216, 8217, 8220, 8221, 8222, 12298, 12299, 40, 41, 9794, 9792, 43, 45, 42, 35, 61, 38, 126, 58, 59,
            9327, 9328, 9329, 9330, 9331, 9332, 9333, 9334, 9335, 9336, 64, 9337, 37, 9338, 9339, 9340, 9341, 9342, 9343,
            9344, 9345, 9346, 9347, 9348, 32, 9349, 9350, 9351, 176, 95, 65343, 44032, 44033, 44036, 44039, 44040, 44041,
            44042, 44048, 44049, 44050, 44051, 44052, 44053, 44054, 44055, 44057, 44058, 44059, 44060, 44061, 44064,
            44068, 44076, 44077, 44079, 44080, 44081, 44088, 44089, 44092, 44096, 44107, 44109, 44116, 44120, 44124,
            44144, 44145, 44148, 44151, 44152, 44154, 44160, 44161, 44163, 44164, 44165, 44166, 44169, 44170, 44171,
            44172, 44176, 44180, 44188, 44189, 44191, 44192, 44193, 44200, 44201, 44202, 44204, 44207, 44208, 44216,
            44217, 44219, 44220, 44221, 44225, 44228, 44232, 44236, 44245, 44247, 44256, 44257, 44260, 44263, 44264,
            44266, 44268, 44271, 44272, 44273, 44275, 44277, 44278, 44284, 44285, 44288, 44292, 44294, 44300, 44301,
            44303, 44305, 44312, 44316, 44320, 44329, 44332, 44333, 44340, 44341, 44344, 44348, 44356, 44357, 44359,
            44361, 44368, 44372, 44376, 44385, 44387, 44396, 44397, 44400, 44403, 44404, 44405, 44406, 44411, 44412,
            44413, 44415, 44417, 44418, 44424, 44425, 44428, 44432, 44444, 44445, 44452, 44471, 44480, 44481, 44484,
            44488, 44496, 44497, 44499, 44508, 44512, 44516, 44536, 44537, 44540, 44543, 44544, 44545, 44552, 44553,
            44555, 44557, 44564, 44592, 44593, 44596, 44599, 44600, 44602, 44608, 44609, 44611, 44613, 44614, 44618,
            44620, 44621, 44622, 44624, 44628, 44630, 44636, 44637, 44639, 44640, 44641, 44645, 44648, 44649, 44652,
            44656, 44664, 44665, 44667, 44668, 44669, 44676, 44677, 44684, 44732, 44733, 44734, 44736, 44740, 44748,
            44749, 44751, 44752, 44753, 44760, 44761, 44764, 44776, 44779, 44781, 44788, 44792, 44796, 44807, 44808,
            44813, 44816, 44844, 44845, 44848, 44850, 44852, 44860, 44861, 44863, 44865, 44866, 44867, 44872, 44873,
            44880, 44892, 44893, 44900, 44901, 44921, 44928, 44932, 44936, 44944, 44945, 44949, 44956, 44984, 44985,
            44988, 44992, 44999, 45000, 45001, 45003, 45005, 45006, 45012, 45020, 45032, 45033, 45040, 45041, 45044,
            45048, 45056, 45057, 45060, 45068, 45072, 45076, 45084, 45085, 45096, 45124, 45125, 45128, 45130, 45132,
            45134, 45139, 45140, 45141, 45143, 45145, 45149, 45180, 45181, 45184, 45188, 45196, 45197, 45199, 45201,
            45208, 45209, 45210, 45212, 45215, 45216, 45217, 45218, 45224, 45225, 45227, 45228, 45229, 45230, 45231,
            45233, 45235, 45236, 45237, 45240, 45244, 45252, 45253, 45255, 45256, 45257, 45264, 45265, 45268, 45272,
            45280, 45285, 45320, 45321, 45323, 45324, 45328, 45330, 45331, 45336, 45337, 45339, 45340, 45341, 45347,
            45348, 45349, 45352, 45356, 45364, 45365, 45367, 45368, 45369, 45376, 45377, 45380, 45384, 45392, 45393,
            45396, 45397, 45400, 45404, 45408, 45432, 45433, 45436, 45440, 45442, 45448, 45449, 45451, 45453, 45458,
            45459, 45460, 45464, 45468, 45480, 45516, 45520, 45524, 45532, 45533, 45535, 45544, 45545, 45548, 45552,
            45561, 45563, 45565, 45572, 45573, 45576, 45579, 45580, 45588, 45589, 45591, 45593, 45600, 45620, 45628,
            45656, 45660, 45664, 45672, 45673, 45684, 45685, 45692, 45700, 45701, 45705, 45712, 45713, 45716, 45720,
            45721, 45722, 45728, 45729, 45731, 45733, 45734, 45738, 45740, 45744, 45748, 45768, 45769, 45772, 45776,
            45778, 45784, 45785, 45787, 45789, 45794, 45796, 45797, 45798, 45800, 45803, 45804, 45805, 45806, 45807,
            45811, 45812, 45813, 45815, 45816, 45817, 45818, 45819, 45823, 45824, 45825, 45828, 45832, 45840, 45841,
            45843, 45844, 45845, 45852, 45908, 45909, 45910, 45912, 45915, 45916, 45918, 45919, 45924, 45925, 45927,
            45929, 45931, 45934, 45936, 45937, 45940, 45944, 45952, 45953, 45955, 45956, 45957, 45964, 45968, 45972,
            45984, 45985, 45992, 45996, 46020, 46021, 46024, 46027, 46028, 46030, 46032, 46036, 46037, 46039, 46041,
            46043, 46045, 46048, 46052, 46056, 46076, 46096, 46104, 46108, 46112, 46120, 46121, 46123, 46132, 46160,
            46161, 46164, 46168, 46176, 46177, 46179, 46181, 46188, 46208, 46216, 46237, 46244, 46248, 46252, 46261,
            46263, 46265, 46272, 46276, 46280, 46288, 46293, 46300, 46301, 46304, 46307, 46308, 46310, 46316, 46317,
            46319, 46321, 46328, 46356, 46357, 46360, 46363, 46364, 46372, 46373, 46375, 46376, 46377, 46378, 46384,
            46385, 46388, 46392, 46400, 46401, 46403, 46404, 46405, 46411, 46412, 46413, 46416, 46420, 46428, 46429,
            46431, 46432, 46433, 46496, 46497, 46500, 46504, 46506, 46507, 46512, 46513, 46515, 46516, 46517, 46523,
            46524, 46525, 46528, 46532, 46540, 46541, 46543, 46544, 46545, 46552, 46572, 46608, 46609, 46612, 46616,
            46629, 46636, 46644, 46664, 46692, 46696, 46748, 46749, 46752, 46756, 46763, 46764, 46769, 46804, 46832,
            46836, 46840, 46848, 46849, 46853, 46888, 46889, 46892, 46895, 46896, 46904, 46905, 46907, 46916, 46920,
            46924, 46932, 46933, 46944, 46948, 46952, 46960, 46961, 46963, 46965, 46972, 46973, 46976, 46980, 46988,
            46989, 46991, 46992, 46993, 46994, 46998, 46999, 47000, 47001, 47004, 47008, 47016, 47017, 47019, 47020,
            47021, 47028, 47029, 47032, 47047, 47049, 47084, 47085, 47088, 47092, 47100, 47101, 47103, 47104, 47105,
            47111, 47112, 47113, 47116, 47120, 47128, 47129, 47131, 47133, 47140, 47141, 47144, 47148, 47156, 47157,
            47159, 47160, 47161, 47168, 47172, 47185, 47187, 47196, 47197, 47200, 47204, 47212, 47213, 47215, 47217,
            47224, 47228, 47245, 47272, 47280, 47284, 47288, 47296, 47297, 47299, 47301, 47308, 47312, 47316, 47325,
            47327, 47329, 47336, 47337, 47340, 47344, 47352, 47353, 47355, 47357, 47364, 47384, 47392, 47420, 47421,
            47424, 47428, 47436, 47439, 47441, 47448, 47449, 47452, 47456, 47464, 47465, 47467, 47469, 47476, 47477,
            47480, 47484, 47492, 47493, 47495, 47497, 47498, 47501, 47502, 47532, 47533, 47536, 47540, 47548, 47549,
            47551, 47553, 47560, 47561, 47564, 47566, 47567, 47568, 47569, 47570, 47576, 47577, 47579, 47581, 47582,
            47585, 47587, 47588, 47589, 47592, 47596, 47604, 47605, 47607, 47608, 47609, 47610, 47616, 47617, 47624,
            47637, 47672, 47673, 47676, 47680, 47682, 47688, 47689, 47691, 47693, 47694, 47699, 47700, 47701, 47704,
            47708, 47716, 47717, 47719, 47720, 47721, 47728, 47729, 47732, 47736, 47747, 47748, 47749, 47751, 47756,
            47784, 47785, 47787, 47788, 47792, 47794, 47800, 47801, 47803, 47805, 47812, 47816, 47832, 47833, 47868,
            47872, 47876, 47885, 47887, 47889, 47896, 47900, 47904, 47913, 47915, 47924, 47925, 47926, 47928, 47931,
            47932, 47933, 47934, 47940, 47941, 47943, 47945, 47949, 47951, 47952, 47956, 47960, 47969, 47971, 47980,
            48008, 48012, 48016, 48036, 48040, 48044, 48052, 48055, 48064, 48068, 48072, 48080, 48083, 48120, 48121,
            48124, 48127, 48128, 48130, 48136, 48137, 48139, 48140, 48141, 48143, 48145, 48148, 48149, 48150, 48151,
            48152, 48155, 48156, 48157, 48158, 48159, 48164, 48165, 48167, 48169, 48173, 48176, 48177, 48180, 48184,
            48192, 48193, 48195, 48196, 48197, 48201, 48204, 48205, 48208, 48221, 48260, 48261, 48264, 48267, 48268,
            48270, 48276, 48277, 48279, 48281, 48282, 48288, 48289, 48292, 48295, 48296, 48304, 48305, 48307, 48308,
            48309, 48316, 48317, 48320, 48324, 48333, 48335, 48336, 48337, 48341, 48344, 48348, 48372, 48373, 48374,
            48376, 48380, 48388, 48389, 48391, 48393, 48400, 48404, 48420, 48428, 48448, 48456, 48457, 48460, 48464,
            48472, 48473, 48484, 48488, 48512, 48513, 48516, 48519, 48520, 48521, 48522, 48528, 48529, 48531, 48533,
            48537, 48538, 48540, 48548, 48560, 48568, 48596, 48597, 48600, 48604, 48617, 48624, 48628, 48632, 48640,
            48643, 48645, 48652, 48653, 48656, 48660, 48668, 48669, 48671, 48708, 48709, 48712, 48716, 48718, 48724,
            48725, 48727, 48729, 48730, 48731, 48736, 48737, 48740, 48744, 48746, 48752, 48753, 48755, 48756, 48757,
            48763, 48764, 48765, 48768, 48772, 48780, 48781, 48783, 48784, 48785, 48792, 48793, 48808, 48848, 48849,
            48852, 48855, 48856, 48864, 48867, 48868, 48869, 48876, 48897, 48904, 48905, 48920, 48921, 48923, 48924,
            48925, 48960, 48961, 48964, 48968, 48976, 48977, 48981, 49044, 49072, 49093, 49100, 49101, 49104, 49108,
            49116, 49119, 49121, 49212, 49233, 49240, 49244, 49248, 49256, 49257, 49296, 49297, 49300, 49304, 49312,
            49313, 49315, 49317, 49324, 49325, 49327, 49328, 49331, 49332, 49333, 49334, 49340, 49341, 49343, 49344,
            49345, 49349, 49352, 49353, 49356, 49360, 49368, 49369, 49371, 49372, 49373, 49380, 49381, 49384, 49388,
            49396, 49397, 49399, 49401, 49408, 49412, 49416, 49424, 49429, 49436, 49437, 49438, 49439, 49440, 49443,
            49444, 49446, 49447, 49452, 49453, 49455, 49456, 49457, 49462, 49464, 49465, 49468, 49472, 49480, 49481,
            49483, 49484, 49485, 49492, 49493, 49496, 49500, 49508, 49509, 49511, 49512, 49513, 49520, 49524, 49528,
            49541, 49548, 49549, 49550, 49552, 49556, 49558, 49564, 49565, 49567, 49569, 49573, 49576, 49577, 49580,
            49584, 49597, 49604, 49608, 49612, 49620, 49623, 49624, 49632, 49636, 49640, 49648, 49649, 49651, 49660,
            49661, 49664, 49668, 49676, 49677, 49679, 49681, 49688, 49689, 49692, 49695, 49696, 49704, 49705, 49707,
            49709, 49711, 49713, 49714, 49716, 49736, 49744, 49745, 49748, 49752, 49760, 49765, 49772, 49773, 49776,
            49780, 49788, 49789, 49791, 49793, 49800, 49801, 49808, 49816, 49819, 49821, 49828, 49829, 49832, 49836,
            49837, 49844, 49845, 49847, 49849, 49884, 49885, 49888, 49891, 49892, 49899, 49900, 49901, 49903, 49905,
            49910, 49912, 49913, 49915, 49916, 49920, 49928, 49929, 49932, 49933, 49939, 49940, 49941, 49944, 49948,
            49956, 49957, 49960, 49961, 49989, 50024, 50025, 50028, 50032, 50034, 50040, 50041, 50044, 50045, 50052,
            50056, 50060, 50112, 50136, 50137, 50140, 50143, 50144, 50146, 50152, 50153, 50157, 50164, 50165, 50168,
            50184, 50192, 50212, 50220, 50224, 50228, 50236, 50237, 50248, 50276, 50277, 50280, 50284, 50292, 50293,
            50297, 50304, 50324, 50332, 50360, 50364, 50409, 50416, 50417, 50420, 50424, 50426, 50431, 50432, 50433,
            50444, 50448, 50452, 50460, 50472, 50473, 50476, 50480, 50488, 50489, 50491, 50493, 50500, 50501, 50504,
            50505, 50506, 50508, 50509, 50510, 50515, 50516, 50517, 50519, 50520, 50521, 50525, 50526, 50528, 50529,
            50532, 50536, 50544, 50545, 50547, 50548, 50549, 50556, 50557, 50560, 50564, 50567, 50572, 50573, 50575,
            50577, 50581, 50583, 50584, 50588, 50592, 50601, 50612, 50613, 50616, 50617, 50619, 50620, 50621, 50622,
            50628, 50629, 50630, 50631, 50632, 50633, 50634, 50636, 50638, 50640, 50641, 50644, 50648, 50656, 50657,
            50659, 50661, 50668, 50669, 50670, 50672, 50676, 50678, 50679, 50684, 50685, 50686, 50687, 50688, 50689,
            50693, 50694, 50695, 50696, 50700, 50704, 50712, 50713, 50715, 50716, 50724, 50725, 50728, 50732, 50733,
            50734, 50736, 50739, 50740, 50741, 50743, 50745, 50747, 50752, 50753, 50756, 50760, 50768, 50769, 50771,
            50772, 50773, 50780, 50781, 50784, 50796, 50799, 50801, 50808, 50809, 50812, 50816, 50824, 50825, 50827,
            50829, 50836, 50837, 50840, 50844, 50852, 50853, 50855, 50857, 50864, 50865, 50868, 50872, 50873, 50874,
            50880, 50881, 50883, 50885, 50892, 50893, 50896, 50900, 50908, 50909, 50912, 50913, 50920, 50921, 50924,
            50928, 50936, 50937, 50941, 50948, 50949, 50952, 50956, 50964, 50965, 50967, 50969, 50976, 50977, 50980,
            50984, 50992, 50993, 50995, 50997, 50999, 51004, 51005, 51008, 51012, 51018, 51020, 51021, 51023, 51025,
            51026, 51027, 51028, 51029, 51030, 51031, 51032, 51036, 51040, 51048, 51051, 51060, 51061, 51064, 51068,
            51069, 51070, 51075, 51076, 51077, 51079, 51080, 51081, 51082, 51086, 51088, 51089, 51092, 51094, 51095,
            51096, 51098, 51104, 51105, 51107, 51108, 51109, 51110, 51116, 51117, 51120, 51124, 51132, 51133, 51135,
            51136, 51137, 51144, 51145, 51148, 51150, 51152, 51160, 51165, 51172, 51176, 51180, 51200, 51201, 51204,
            51208, 51210, 51216, 51217, 51219, 51221, 51222, 51228, 51229, 51232, 51236, 51244, 51245, 51247, 51249,
            51256, 51260, 51264, 51272, 51273, 51276, 51277, 51284, 51312, 51313, 51316, 51320, 51322, 51328, 51329,
            51331, 51333, 51334, 51335, 51339, 51340, 51341, 51348, 51357, 51359, 51361, 51368, 51388, 51389, 51396,
            51400, 51404, 51412, 51413, 51415, 51417, 51424, 51425, 51428, 51445, 51452, 51453, 51456, 51460, 51461,
            51462, 51468, 51469, 51471, 51473, 51480, 51500, 51508, 51536, 51537, 51540, 51544, 51552, 51553, 51555,
            51564, 51568, 51572, 51580, 51592, 51593, 51596, 51600, 51608, 51609, 51611, 51613, 51648, 51649, 51652,
            51655, 51656, 51658, 51664, 51665, 51667, 51669, 51670, 51673, 51674, 51676, 51677, 51680, 51682, 51684,
            51687, 51692, 51693, 51695, 51696, 51697, 51704, 51705, 51708, 51712, 51720, 51721, 51723, 51724, 51725,
            51732, 51736, 51753, 51788, 51789, 51792, 51796, 51804, 51805, 51807, 51808, 51809, 51816, 51837, 51844,
            51864, 51900, 51901, 51904, 51908, 51916, 51917, 51919, 51921, 51923, 51928, 51929, 51936, 51948, 51956,
            51976, 51984, 51988, 51992, 52000, 52001, 52033, 52040, 52041, 52044, 52048, 52056, 52057, 52061, 52068,
            52088, 52089, 52124, 52152, 52180, 52196, 52199, 52201, 52236, 52237, 52240, 52244, 52252, 52253, 52257,
            52258, 52263, 52264, 52265, 52268, 52270, 52272, 52280, 52281, 52283, 52284, 52285, 52286, 52292, 52293,
            52296, 52300, 52308, 52309, 52311, 52312, 52313, 52320, 52324, 52326, 52328, 52336, 52341, 52376, 52377,
            52380, 52384, 52392, 52393, 52395, 52396, 52397, 52404, 52405, 52408, 52412, 52420, 52421, 52423, 52425,
            52432, 52436, 52452, 52460, 52464, 52481, 52488, 52489, 52492, 52496, 52504, 52505, 52507, 52509, 52516,
            52520, 52524, 52537, 52572, 52576, 52580, 52588, 52589, 52591, 52593, 52600, 52616, 52628, 52629, 52632,
            52636, 52644, 52645, 52647, 52649, 52656, 52676, 52684, 52688, 52712, 52716, 52720, 52728, 52729, 52731,
            52733, 52740, 52744, 52748, 52756, 52761, 52768, 52769, 52772, 52776, 52784, 52785, 52787, 52789, 52824,
            52825, 52828, 52831, 52832, 52833, 52840, 52841, 52843, 52845, 52852, 52853, 52856, 52860, 52868, 52869,
            52871, 52873, 52880, 52881, 52884, 52888, 52896, 52897, 52899, 52900, 52901, 52908, 52909, 52929, 52964,
            52965, 52968, 52971, 52972, 52980, 52981, 52983, 52984, 52985, 52992, 52993, 52996, 53000, 53008, 53009,
            53011, 53013, 53020, 53024, 53028, 53036, 53037, 53039, 53040, 53041, 53048, 53076, 53077, 53080, 53084,
            53092, 53093, 53095, 53097, 53104, 53105, 53108, 53112, 53120, 53125, 53132, 53153, 53160, 53168, 53188,
            53216, 53217, 53220, 53224, 53232, 53233, 53235, 53237, 53244, 53248, 53252, 53265, 53272, 53293, 53300,
            53301, 53304, 53308, 53316, 53317, 53319, 53321, 53328, 53332, 53336, 53344, 53356, 53357, 53360, 53364,
            53372, 53373, 53377, 53412, 53413, 53416, 53420, 53428, 53429, 53431, 53433, 53440, 53441, 53444, 53448,
            53449, 53456, 53457, 53459, 53460, 53461, 53468, 53469, 53472, 53476, 53484, 53485, 53487, 53488, 53489,
            53496, 53517, 53552, 53553, 53556, 53560, 53562, 53568, 53569, 53571, 53572, 53573, 53580, 53581, 53584,
            53588, 53596, 53597, 53599, 53601, 53608, 53612, 53628, 53636, 53640, 53664, 53665, 53668, 53672, 53680,
            53681, 53683, 53685, 53690, 53692, 53696, 53720, 53748, 53752, 53767, 53769, 53776, 53804, 53805, 53808,
            53812, 53820, 53821, 53823, 53825, 53832, 53852, 53860, 53888, 53889, 53892, 53896, 53904, 53905, 53909,
            53916, 53920, 53924, 53932, 53937, 53944, 53945, 53948, 53951, 53952, 53954, 53960, 53961, 53963, 53972,
            53976, 53980, 53988, 53989, 54000, 54001, 54004, 54008, 54016, 54017, 54019, 54021, 54028, 54029, 54030,
            54032, 54036, 54038, 54044, 54045, 54047, 54048, 54049, 54053, 54056, 54057, 54060, 54064, 54072, 54073,
            54075, 54076, 54077, 54084, 54085, 54140, 54141, 54144, 54148, 54156, 54157, 54159, 54160, 54161, 54168,
            54169, 54172, 54176, 54184, 54185, 54187, 54189, 54196, 54200, 54204, 54212, 54213, 54216, 54217, 54224,
            54232, 54241, 54243, 54252, 54253, 54256, 54260, 54268, 54269, 54271, 54273, 54280, 54301, 54336, 54340,
            54364, 54368, 54372, 54381, 54383, 54392, 54393, 54396, 54399, 54400, 54402, 54408, 54409, 54411, 54413,
            54420, 54441, 54476, 54480, 54484, 54492, 54495, 54504, 54508, 54512, 54520, 54523, 54525, 54532, 54536,
            54540, 54548, 54549, 54551, 54588, 54589, 54592, 54596, 54604, 54605, 54607, 54609, 54616, 54617, 54620,
            54624, 54629, 54632, 54633, 54635, 54637, 54644, 54645, 54648, 54652, 54660, 54661, 54663, 54664, 54665,
            54672, 54693, 54728, 54729, 54732, 54736, 54738, 54744, 54745, 54747, 54749, 54756, 54757, 54760, 54764,
            54772, 54773, 54775, 54777, 54784, 54785, 54788, 54792, 54800, 54801, 54803, 54804, 54805, 54812, 54816,
            54820, 54829, 54840, 54841, 54844, 54848, 54853, 54856, 54857, 54859, 54861, 54865, 54868, 54869, 54872,
            54876, 54887, 54889, 54896, 54897, 54900, 54915, 54917, 54924, 54925, 54928, 54932, 54941, 54943, 54945,
            54952, 54956, 54960, 54969, 54971, 54980, 54981, 54984, 54988, 54993, 54996, 54999, 55001, 55008, 55012,
            55016, 55024, 55029, 55036, 55037, 55040, 55044, 55057, 55064, 55065, 55068, 55072, 55080, 55081, 55083,
            55085, 55092, 55093, 55096, 55100, 55108, 55111, 55113, 55120, 55121, 55124, 55126, 55127, 55128, 55129,
            55136, 55137, 55139, 55141, 55145, 55148, 55152, 55156, 55164, 55165, 55169, 55176, 55177, 55180, 55184,
            55192, 55193, 55195, 55197, 4352, 4353, 4354, 4355, 4356, 4357, 4358, 4359, 4360, 4361, 4362, 4363, 4364,
            4365, 4366, 4367, 4368, 4369, 4370, 4449, 4450, 4451, 4452, 4453, 4454, 4455, 4456, 4457, 4461, 4462, 4466,
            4467, 4469, 47252, 49968, 50108, 50388, 52012, 65535
        };

        private static readonly ushort[] G34_4E =
        {
            478, 351, 352, 353, 358, 359, 360, 361, 362, 363, 020, 365, 366, 369, 370, 371, // 0
            415, 376, 377, 378, 368, 382, 383, 384, 046, 358, 359, 392, 393, 394, 395, 396, // 1
            397, 398, 401, 402, 403, 416, 408, 409, 410, 400, 420, 419, 479, 450, 445, 003, // 2
            004, 006, 008, 010, 068, 449, 072, 013, 015, 017, 019, 021, 023, 025, 027, 029, // 3
            031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061, 050, 053, 056, 059, 062, // 4
            036, 426, 425, 480, 481, 091, 092, 094, 096, 098, 364, 100, 102, 106, 108, 110, // 5
            112, 114, 117, 119, 121, 123, 124, 125, 385, 127, 128, 131, 134, 137, 140, 396, // 6
            144, 145, 146, 147, 149, 151, 153, 154, 155, 156, 157, 158, 159, 160, 161, 082, // 7
            084, 086, 088, 090, 148, 150, 152, 093, 095, 097, 099, 101, 103, 105, 107, 109, // 8
            111, 113, 115, 118, 120, 122, 129, 132, 135, 138, 141, 130, 133, 136, 139, 142, // 9
            116, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 427, 428, 430, 446, 230, // A
            431, 436, 437, 434, 435, 443, 444, 424, 429, 242, 433, 299, 300, 301, 302, 303, // B
            304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, // C
            320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, // D
            336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 289, // E
            452, 355, 373, 379, 387, 405, 411                                               // F
        };

        private static readonly ushort[] G34_4J =
        {
            001, 003, 005, 007, 009, 011, 012, 014, 016, 018, 020, 022, 024, 026, 028, 030, // 0
            032, 034, 037, 039, 041, 043, 044, 045, 046, 047, 048, 051, 054, 057, 060, 063, // 1
            064, 065, 066, 067, 069, 071, 073, 074, 075, 076, 077, 078, 079, 080, 081, 002, // 2
            004, 006, 008, 010, 068, 070, 072, 013, 015, 017, 019, 021, 023, 025, 027, 029, // 3
            031, 033, 035, 038, 040, 042, 049, 052, 055, 058, 061, 050, 053, 056, 059, 062, // 4
            036, 083, 085, 087, 089, 091, 092, 094, 096, 098, 100, 102, 104, 106, 108, 110, // 5
            112, 114, 117, 119, 121, 123, 124, 125, 126, 127, 128, 131, 134, 137, 140, 143, // 6
            144, 145, 146, 147, 149, 151, 153, 154, 155, 156, 157, 158, 159, 160, 161, 082, // 7
            084, 086, 088, 090, 148, 150, 152, 093, 095, 097, 099, 101, 103, 105, 107, 109, // 8
            111, 113, 115, 118, 120, 122, 129, 132, 135, 138, 141, 130, 133, 136, 139, 142, // 9
            116, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 225, 226, 228, 241, 230, // A
            229, 234, 235, 232, 233, 443, 444, 424, 430, 242, 433, 172, 173, 174, 175, 176, // B
            177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, // C
            193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, // D
            209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 289, // E
            452, 355, 373, 379, 387, 405, 411                                               // F
        };
        #endregion

        #region Gen 7 Chinese Character Tables
        private static readonly char[] Gen7_CHS = Util.GetStringList("Char", "zh")[0].ToCharArray();
        private const ushort Gen7_CHS_Ofs = 0xE800;
        private static readonly char[] Gen7_CHT = Util.GetStringList("Char", "zh2")[0].ToCharArray();
        private const ushort Gen7_CHT_Ofs = 0xEB0F;
        #endregion

        /// <summary>
        /// Trash Bytes for Generation 3->4
        /// </summary>
        /// <remarks>String buffers are reused, data is not cleared which yields the trash bytes.</remarks>
        public static readonly byte[][] G4TransferTrashBytes = {
            new byte[] { }, // Unused
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { }, // Unused
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
        };

        /// <summary>
        /// Converts full width to single width
        /// </summary>
        /// <param name="str">Input string to sanitize.</param>
        /// <returns></returns>
        private static string SanitizeString(string str)
        {
            if (str.Length == 0)
                return str;
            var s = str.Replace("\u2019", "\u0027"); // farfetch'd
            s = s.Replace("\uE08F", "\u2640"); // ♀ (gen6+)
            s = s.Replace("\uE08E", "\u2642"); // ♂ (gen6+)
            s = s.Replace("\u246E", "\u2640"); // ♀ (gen5)
            s = s.Replace("\u246D", "\u2642"); // ♂ (gen5)
            return s;
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <param name="generation"></param>
        /// <param name="species"></param>
        /// <param name="nicknamed"></param>
        /// <returns></returns>
        private static string UnSanitizeString(string str, int generation, int species = -1, bool nicknamed = true)
        {
            var s = str;
            if (generation >= 6)
                s = str.Replace("\u0027", "\u2019"); // farfetch'd

            if (generation == 5 || generation == 4)
            {
                s = s.Replace("\u2640", "\u246E"); // ♀
                s = s.Replace("\u2642", "\u246D"); // ♂
                return s;
            }

            bool foreign = true;
            if ((species == 029 || species == 032) && !nicknamed)
                foreign = str[0] != 'N'; // idoran
            else if (nicknamed)
                foreign = str.Select(c => c >> 12).Any(c => c != 0 && c != 0xE);

            // Convert back to half width
            if (foreign)
                return s;
            s = s.Replace("\u2640", "\uE08F"); // ♀
            s = s.Replace("\u2642", "\uE08E"); // ♂
            return s;
        }

        /// <summary>
        /// Trims a string at the first instance of a 0xFFFF terminator.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <returns>Trimmed string.</returns>
        public static string TrimFromFFFF(string input)
        {
            int index = input.IndexOf((char)0xFFFF);
            return index < 0 ? input : input.Substring(0, index);
        }

        /// <summary>
        /// Strips diacritics on gen1-4 french pkm names
        /// </summary>
        /// <param name="input">String to clean</param>
        /// <returns>Cleaned string</returns>
        /// <remarks>Only 4 characters are accented in gen1-4</remarks>
        public static string StripDiacriticsFR4(string input)
        {
            var result = new StringBuilder(input.Length);
            foreach (var c in input)
                result.Append(FrDiacritic.TryGetValue(c, out char o) ? o : c);
            return result.ToString();
        }
        private static readonly Dictionary<char, char> FrDiacritic = new Dictionary<char, char>
        {
            { 'È', 'E' },
            { 'É', 'E' },
            { 'Ê', 'E' },
            { 'Ï', 'I' },
        };
    }
}
