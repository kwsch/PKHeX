using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for recognizing .dsv save files from BizHawk.
/// </summary>
public sealed class SaveHandlerBizHawk : ISaveHandler
{
    private const int sizeFooter = 0x16;

    private static bool GetHasFooter(ReadOnlySpan<byte> input)
    {
        var start = input.Length - sizeFooter;
        var footer = input[start..];
        var _0x0b = ReadUInt16LittleEndian(footer[0x0B..]);
        var _0x14 = ReadUInt16LittleEndian(footer[0x14..]);
        return _0x0b == _0x14;
    }

    public bool IsRecognized(long size) => SaveUtil.IsSizeValidNoHandler(size - sizeFooter);

    public SaveHandlerSplitResult? TrySplit(ReadOnlySpan<byte> input)
    {
        if (!GetHasFooter(input))
            return null;

        var realSize = input.Length - sizeFooter;
        var footer = input[realSize..].ToArray();
        var data = input[..realSize].ToArray();

        return new SaveHandlerSplitResult(data, [], footer);
    }
}
