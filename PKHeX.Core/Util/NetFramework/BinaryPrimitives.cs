#if !NET6_0_OR_GREATER
global using static PKHeX.Core.Buffers.Binary.Extra.BinaryPrimitives;
using System;
using System.Runtime.InteropServices;

namespace PKHeX.Core.Buffers.Binary.Extra;

internal static class BinaryPrimitives
{
    public static float ReadSingleLittleEndian(ReadOnlySpan<byte> data)
    {
        var value = MemoryMarshal.Read<float>(data);
        if (BitConverter.IsLittleEndian)
            return value;
        return ReverseEndianness(value);
    }

    public static double ReadDoubleLittleEndian(ReadOnlySpan<byte> data)
    {
        var value = MemoryMarshal.Read<double>(data);
        if (BitConverter.IsLittleEndian)
            return value;
        return ReverseEndianness(value);
    }

    public static float ReadSingleBigEndian(Span<byte> data)
    {
        var value = MemoryMarshal.Read<float>(data);
        if (!BitConverter.IsLittleEndian)
            return value;
        return ReverseEndianness(value);
    }

    public static double ReadDoubleBigEndian(Span<byte> data)
    {
        var value = MemoryMarshal.Read<double>(data);
        if (!BitConverter.IsLittleEndian)
            return value;
        return ReverseEndianness(value);
    }

    public static void WriteSingleLittleEndian(Span<byte> data, float value)
    {
        if (!BitConverter.IsLittleEndian)
            value = ReverseEndianness(value);
        MemoryMarshal.Write(data, ref value);
    }

    public static void WriteDoubleLittleEndian(Span<byte> data, double value)
    {
        if (!BitConverter.IsLittleEndian)
            value = ReverseEndianness(value);
        MemoryMarshal.Write(data, ref value);
    }

    public static void WriteSingleBigEndian(Span<byte> data, float value)
    {
        if (BitConverter.IsLittleEndian)
            value = ReverseEndianness(value);
        MemoryMarshal.Write(data, ref value);
    }

    public static void WriteDoubleBigEndian(Span<byte> data, double value)
    {
        if (BitConverter.IsLittleEndian)
            value = ReverseEndianness(value);
        MemoryMarshal.Write(data, ref value);
    }

    public static float ReverseEndianness(float input)
    {
        Span<byte> span = stackalloc byte[4];
        MemoryMarshal.Write(span, ref input);
        span.Reverse();
        return MemoryMarshal.Read<float>(span);
    }

    public static double ReverseEndianness(double input)
    {
        Span<byte> span = stackalloc byte[8];
        MemoryMarshal.Write(span, ref input);
        span.Reverse();
        return MemoryMarshal.Read<double>(span);
    }
}
#endif
