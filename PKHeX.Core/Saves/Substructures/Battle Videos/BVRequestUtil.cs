using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

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

    public static ulong StrToU64(ReadOnlySpan<char> input, out bool valid)
    {
        var chk = Pull(input[..4]) >> 4; // first four chars are checksum bits
        var result = Pull(input[4..]); // next 12 chars are the 70 value bits

        Span<byte> temp = stackalloc byte[8];
        WriteUInt64LittleEndian(temp, result);
        var actual = Checksums.CRC16_CCITT(temp);
        valid = chk == actual;
        return result;

        static ulong Pull(ReadOnlySpan<char> input)
        {
            ulong val = 0;
            foreach (char c in input)
            {
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
        Span<byte> temp = stackalloc byte[8];
        WriteUInt64LittleEndian(temp, input);
        uint chk = Checksums.CRC16_CCITT(temp);
        Span<char> buff = stackalloc char[16];
        int ctr = 15;
        Push(buff, ref ctr, 12, input); // store value bits
        Push(buff, ref ctr, 04, chk << 4); // store checksum bits
        return !insertDash ? new string(buff) : GetStringWithDashesEvery(buff, 4);

        static void Push(Span<char> buff, ref int ctr, int bit5Chunks, ulong value)
        {
            for (int i = 0; i < bit5Chunks; i++)
            {
                buff[ctr--] = Set5BitToChar((char)(value & 0x1F));
                value >>= 5;
            }
        }
    }

    private static string GetStringWithDashesEvery(ReadOnlySpan<char> buff, int spacer)
    {
        Span<char> buff2 = stackalloc char[buff.Length + ((buff.Length / spacer) - 1)];
        for (int i = 0, ctr = 0; i < buff.Length; i++)
        {
            buff2[ctr++] = buff[i];
            if (i % spacer == 3 && ctr < buff2.Length)
                buff2[ctr++] = '-'; // add dash between every chunk of size {spacer}
        }
        return new string(buff2);
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
        _ => c,
    };

    private static char MapFromChar(char c) => c switch
    {
        'W' => '0',
        'X' => '1',
        'Y' => 'I',
        'Z' => 'O',
        _ => c,
    };
}
