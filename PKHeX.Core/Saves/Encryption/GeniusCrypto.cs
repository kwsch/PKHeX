using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Genius Sonority's logic for <see cref="GameVersion.XD"/> and <see cref="GameVersion.BATREV"/> encryption.
/// </summary>
public static class GeniusCrypto
{
    public static void ReadKeys(ReadOnlySpan<byte> input, Span<ushort> keys)
    {
        for (int i = 0; i < keys.Length; i++)
            keys[i] = ReadUInt16BigEndian(input[(i * 2)..]);
    }

    public static void Decrypt(ReadOnlySpan<byte> input, Span<byte> output, Span<ushort> keys)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(keys.Length, 4);

        var in16 = MemoryMarshal.Cast<byte, ushort>(input);
        var out16 = MemoryMarshal.Cast<byte, ushort>(output);
        int i = 0;
        do
        {
            foreach (var key in keys)
            {
                var value = in16[i];
                if (BitConverter.IsLittleEndian)
                    value = ReverseEndianness(value);
                value -= key;
                if (BitConverter.IsLittleEndian)
                    value = ReverseEndianness(value);
                out16[i] = value;
                i++;
            }
            AdvanceKeys(keys);
        } while (i != in16.Length);
    }

    public static void Encrypt(ReadOnlySpan<byte> input, Span<byte> output, Span<ushort> keys)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(keys.Length, 4);

        var in16 = MemoryMarshal.Cast<byte, ushort>(input);
        var out16 = MemoryMarshal.Cast<byte, ushort>(output);
        int i = 0;
        do
        {
            foreach (var key in keys)
            {
                var value = in16[i];
                if (BitConverter.IsLittleEndian)
                    value = ReverseEndianness(value);
                value += key;
                if (BitConverter.IsLittleEndian)
                    value = ReverseEndianness(value);
                out16[i] = value;
                i++;
            }
            AdvanceKeys(keys);
        } while (i != in16.Length);
    }

    private static void AdvanceKeys(Span<ushort> keys)
    {
        var k3 = keys[3] + 0x13;
        var k2 = keys[2] + 0x17;
        var k1 = keys[1] + 0x29;
        var k0 = keys[0] + 0x43;

        // Rotate 4bit groups across the diagonal [ / ] after biasing each u16 (no overflow):
        // 0123           FB73 
        // 4567           EA62
        // 89AB  becomes  D951
        // CDEF           C840
        // We can leave our intermediary types as int as the bit-masks remove any overflow.

        keys[3] = (ushort)(((k0 >> 12) & 0xf) | ((k1 >> 8) & 0xf0) | ((k2 >> 4) & 0xf00) | ( k3        & 0xf000));
        keys[2] = (ushort)(((k0 >> 08) & 0xf) | ((k1 >> 4) & 0xf0) | ( k2       & 0xf00) | ((k3 << 04) & 0xf000));
        keys[1] = (ushort)(((k0 >> 04) & 0xf) | ( k1       & 0xf0) | ((k2 << 4) & 0xf00) | ((k3 << 08) & 0xf000));
        keys[0] = (ushort)(( k0        & 0xf) | ((k1 << 4) & 0xf0) | ((k2 << 8) & 0xf00) | ((k3 << 12) & 0xf000));
    }
}
