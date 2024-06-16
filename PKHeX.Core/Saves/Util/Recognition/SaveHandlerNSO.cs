using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.HeaderInfoNSO;

namespace PKHeX.Core;

/// <summary>
/// Handler for recognizing games saved by the Nintendo Switch Online service.
/// </summary>
public sealed class SaveHandlerNSO : ISaveHandler
{
    private static ReadOnlySpan<byte> Magic => "SRAM"u8;

    public bool IsRecognized(long size) => IsPlausible(size);

    public static ReadOnlySpan<byte> GetHashROM(ReadOnlySpan<byte> header) => new UnpackedHeaderNSOReadOnly(header).HashROM;

    public SaveHandlerSplitResult? TrySplit(ReadOnlySpan<byte> input)
    {
        if (input.Length < SmallestSize)
            return null;
        if (!input.StartsWith(Magic))
            return null;

        var obj = new UnpackedHeaderNSOReadOnly(input);
        var header = input[..obj.Length];
        var data = input[obj.Length..];
        return new SaveHandlerSplitResult([..data], [..header], [], this);
    }

    public void Finalize(Span<byte> input)
    {
        // Need to re-sign the save file.
        var obj = new UnpackedHeaderNSO(input);
        var hashSave = obj.SaveHash;
        var data = input[obj.Length..];
        WriteHash(data, hashSave);
    }

    public static void WriteHash(ReadOnlySpan<byte> input, Span<byte> result)
    {
        // Compute hash into the result span, then inflate in place.
        System.Security.Cryptography.SHA1.HashData(input, result);
        InflateAscii(result);
    }

    public static void InflateAscii(Span<byte> data)
    {
        // Since we're expanding in-place, our current hash is 20 bytes.
        // We can expand backwards; no byte will be overwritten until it is no longer needed.
        for (int i = 19, j = data.Length - 2; i >= 0; i--, j -= 2)
        {
            byte b = data[i];
            data[j + 1] = GetHexCharLower(b & 0x0F);
            data[j] = GetHexCharLower(b >> 4);
        }
    }

    private static byte GetHexCharLower(int value) => (byte)(value < 10 ? value + '0' : value - 10 + 'a');
}

public static class HeaderInfoNSO
{
    public const int SIZE_HASH = 40; // Inflated from bytes to ASCII chars
    public const int OffsetROM = 0x08;
    public const int SIZE_RTC = 0x20;

    // Anatomy:
    // 0x00 - 0x03: Magic
    // 0x04 - 0x07: (u32 ???) 3?
    // 0x08 - 0x2F: SHA1 hash of the ROM
    // 0x30 - 0x33: Length of following Ascii string
    // 0x34: u8[length] Ascii Build
    // .... bool8 HasRTC
    // .... {if true, 0x20 bytes of RTC}
    // SHA1 hash of the save data
    // remainder: save data

    public const int SmallestSize = SaveUtil.SIZE_G1RAW;

    public static ReadOnlySpan<int> AllowedBaseSizes =>
    [
        SaveUtil.SIZE_G1RAW, // smallest
        SaveUtil.SIZE_G2RAW_J,
    ];

    public static bool IsPlausible(long size)
    {
        foreach (var inner in AllowedBaseSizes)
        {
            if (IsPlausible(size, inner))
                return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPlausible(long size, int expected)
    {
        // allow different builds and RTC presence
        var delta = (uint)(size - expected);
        return delta is >= 0x68 and <= 0xA0;
    }
}

/// <summary>
/// Readonly version of <see cref="UnpackedHeaderNSO"/>
/// </summary>
public readonly ref struct UnpackedHeaderNSOReadOnly
{
    public readonly ReadOnlySpan<byte> HashROM;
    public readonly ReadOnlySpan<byte> Build;
    public readonly bool HasRTC;
    public readonly ReadOnlySpan<byte> RTC;
    public readonly ReadOnlySpan<byte> HashSaveFile;
    public readonly int Length; // of this header

    public UnpackedHeaderNSOReadOnly(ReadOnlySpan<byte> input)
    {
        HashROM = input.Slice(OffsetROM, SIZE_HASH);
        var length = input[0x30]; // really is a 32-bit number, but is never bigger than 255.
        Build = input.Slice(0x34, length);

        var ofs = 0x34 + length;
        HasRTC = input[ofs++] != 0;
        if (HasRTC)
        {
            RTC = input.Slice(ofs, SIZE_RTC);
            ofs += SIZE_RTC;
        }
        HashSaveFile = input.Slice(ofs, SIZE_HASH); ofs += SIZE_HASH;
        Length = ofs;
    }
}

/// <summary>
/// Unpacked Header of an NSO save file's variable-sized header.
/// </summary>
public readonly ref struct UnpackedHeaderNSO
{
    public readonly Span<byte> ROMHash;
    public readonly Span<byte> Build;
    public readonly bool HasRTC;
    public readonly Span<byte> RTC;
    public readonly Span<byte> SaveHash;
    public readonly int Length;

    public UnpackedHeaderNSO(Span<byte> input)
    {
        ROMHash = input.Slice(OffsetROM, SIZE_HASH);
        var length = input[0x30]; // really is a 32-bit number, but is never bigger than 255.
        Build = input.Slice(0x34, length);

        var ofs = 0x34 + length;
        HasRTC = input[ofs++] != 0;
        if (HasRTC)
        {
            RTC = input.Slice(ofs, SIZE_RTC);
            ofs += SIZE_RTC;
        }
        SaveHash = input.Slice(ofs, SIZE_HASH); ofs += SIZE_HASH;
        Length = ofs;
    }
}
