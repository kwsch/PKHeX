using System;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public static class BVRequestUtil
    {
        public static string GetSMBattleVideoURL(string code)
        {
            code = code.Replace("-", string.Empty);
            Debug.Assert(code.Length == 16);
            var video_id = StringToUInt64(code);
            if (video_id == uint.MaxValue)
                return null;
            return $"https://ctr-bnda-live.s3.amazonaws.com/10.CTR_BNDA_datastore/ds/1/data/{video_id:D11}-00001"; // Sun datastore
        }

        private const ushort INVALID = ushort.MaxValue;
        private const string _01IO = "01IO";
        private const string _WXYZ = "WXYZ";
        private static ushort CharToU16(char c)
        {
            if (_01IO.Contains(c))
                return INVALID;
            int index = _WXYZ.IndexOf(c);
            if (index >= 0)
                c = _01IO[index];
            if (c >= '0' && c <= '9')
                c -= '0';
            else
                c -= '7';
            return c;
        }

        private static ulong StringToUInt64(string s)
        {
            // First 4 characters act as the checksum
            ulong v1 = 0;
            ulong v2 = 0;
            for (int i = 0; i < 4; i++)
            {
                var c = CharToU16(s[i]);
                if (c == INVALID)
                    return INVALID;
                v2 = (uint)(((v1 | c) >> 27) | (v2 << 5));
                v1 = (uint)((v1 | c) << 5);
            }
            var crc = ((v1 >> 5) | (uint)(v2 << 27)) >> 4;
            crc |= (v2 >> 5) << 28 | (v2 >> 9) << 32;

            // Last 12 characters act as the file ID (returned value)
            ulong val = 0;
            for (int i = 4; i < 16; i++)
            {
                var c = CharToU16(s[i]);
                if (c == INVALID)
                    return INVALID;
                var v14 = val | c;
                val = (val & 0xFFFFFFFF00000000) | (uint)(val | c);
                if (i == 0xF)
                    continue;
                val = (uint)((val >> 32) << 5) << 32 | (uint)val | (v14 >> 27);
                val = (val & 0xFFFFFFFF00000000) | (uint)(v14 << 5);
            }

            if (SaveUtil.CRC16_CCITT(BitConverter.GetBytes(val)) != crc)
                return uint.MaxValue;
            return val;
        }
    }
}
