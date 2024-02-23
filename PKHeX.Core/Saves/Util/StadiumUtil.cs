using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.StadiumSaveType;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic pertaining to Pokémon Stadium Save Files.
/// </summary>
public static class StadiumUtil
{
    /// <summary>
    /// Checks if the <see cref="magic"/> value is present either with or without byte-swapping.
    /// </summary>
    public static StadiumSaveType IsMagicPresentEither(ReadOnlySpan<byte> data, [ConstantExpected] int size, [ConstantExpected] uint magic, [ConstantExpected] int count)
    {
        if (IsMagicPresent(data, size, magic, count))
            return Regular;

        if (IsMagicPresentSwap(data, size, magic, count))
            return Swapped;

        return None;
    }

    /// <summary>
    /// Checks if the <see cref="magic"/> value is present without byte-swapping.
    /// </summary>
    public static bool IsMagicPresent(ReadOnlySpan<byte> data, [ConstantExpected] int size, [ConstantExpected] uint magic, [ConstantExpected] int count)
    {
        // Check footers of first few chunks to see if the magic value is there.
        for (int i = 0; i < count; i++)
        {
            var footer = data[(size - 6 + (i * size))..];
            if (ReadUInt32LittleEndian(footer) != magic)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the <see cref="magic"/> value is present either with byte-swapping.
    /// </summary>
    public static bool IsMagicPresentSwap(ReadOnlySpan<byte> data, [ConstantExpected] int size, [ConstantExpected] uint magic, [ConstantExpected] int count)
    {
        // Check footers of first few chunks to see if the magic value is there.
        var right = ReverseEndianness((ushort)(magic >> 16));
        var left = ReverseEndianness((ushort)magic);

        for (int i = 0; i < count; i++)
        {
            var offset = size - 6 + (i * size);

            if (ReadUInt16LittleEndian(data[(offset + 4)..]) != right) // EK
                return false;
            if (ReadUInt16LittleEndian(data[(offset - 2)..]) != left) // OP
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the <see cref="magic"/> value is present either with or without byte-swapping.
    /// </summary>
    /// <remarks>Only checks for a single instance of the magic value.</remarks>
    public static StadiumSaveType IsMagicPresentAbsolute(ReadOnlySpan<byte> data, [ConstantExpected] int offset, [ConstantExpected] uint magic)
    {
        var actual = ReadUInt32LittleEndian(data[offset..]);
        if (actual == magic) // POKE
            return Regular;

        var right = ReverseEndianness((ushort)(magic >> 16));
        if (ReadUInt16LittleEndian(data[(offset + 4)..]) != right) // EK
            return None;
        var left = ReverseEndianness((ushort)magic);
        if (ReadUInt16LittleEndian(data[(offset - 2)..]) != left) // OP
            return None;

        return Swapped;
    }
}

public enum StadiumSaveType
{
    None,
    Regular,
    Swapped,
}
