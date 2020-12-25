using System;
using System.Diagnostics;

namespace PKHeX.Core
{
    public static class BVRequestUtil
    {
        public static string GetSMBattleVideoURL(string code)
        {
            code = code.Replace("-", string.Empty);
            Debug.Assert(code.Length == 16);
            var video_id = StrToU64(code, out bool valid);
            if (!valid)
                return string.Empty;
            return $"https://ctr-bnda-live.s3.amazonaws.com/10.CTR_BNDA_datastore/ds/1/data/{video_id:D11}-00001"; // Sun datastore
        }

        public static ulong StrToU64(string input, out bool valid)
        {
            var chk = Pull(0, 4) >> 4; // first four chars are checksum bits
            var result = Pull(4, input.Length); // next 12 chars are the 70 value bits
            var actual = Checksums.CRC16_CCITT(BitConverter.GetBytes(result));
            valid = chk == actual;
            return result;

            ulong Pull(int start, int count)
            {
                ulong val = 0;
                for (int i = start; i < count; i++)
                {
                    var c = input[i];
                    if (c == '-')
                        continue;

                    val <<= 5;
                    val |= Get5BitFromChar(c) & 0b11111;
                }
                return val;
            }
        }

        public static string U64ToStr(ulong input, bool insertDash)
        {
            uint chk = Checksums.CRC16_CCITT(BitConverter.GetBytes(input));
            var buff = new char[16];
            int ctr = 15;
            Push(input, 12); // store value bits
            Push(chk << 4, 4); // store checksum bits
            return !insertDash ? string.Concat(buff) : GetStringWithDashesEvery(buff, 4);

            void Push(ulong v, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    buff[ctr--] = Set5BitToChar((char)(v & 0b11111));
                    v >>= 5;
                }
            }
        }

        private static string GetStringWithDashesEvery(char[] buff, int spacer)
        {
            var buff2 = new char[buff.Length + ((buff.Length / spacer) - 1)];
            for (int i = 0, ctr = 0; i < buff.Length; i++)
            {
                buff2[ctr++] = buff[i];
                if (i % spacer == 3 && ctr < buff2.Length)
                    buff2[ctr++] = '-'; // add dash between every chunk of size {spacer}
            }
            return string.Concat(buff2);
        }

        private static char Set5BitToChar(char c)
        {
            var shift = c > 9 ? '7' : '0';
            c += shift;
            return MapToChar(c);
        }

        private static uint Get5BitFromChar(char c)
        {
            c = MapFromChar(c);
            var shift = c >= 'A' ? '7' : '0';
            return (uint)(c - shift);
        }

        private static char MapToChar(char c) => c switch
        {
            '0' => 'W',
            '1' => 'X',
            'I' => 'Y',
            'O' => 'Z',
            _ => c
        };

        private static char MapFromChar(char c) => c switch
        {
            'W' => '0',
            'X' => '1',
            'Y' => 'I',
            'Z' => 'O',
            _ => c
        };
    }
}
