using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for recognizing .duc save files dumped via an ARDS.
/// </summary>
public sealed class SaveHandlerARDS : ISaveHandler
{
    private const int sizeHeader = 0xA4;
    private const int ExpectedSize = SaveUtil.SIZE_G4RAW + sizeHeader; // 0x800A4

    public bool IsRecognized(long size) => size is ExpectedSize;

    public SaveHandlerSplitResult TrySplit(Memory<byte> input)
    {
        // No authentication to see if it actually is a header; no size collisions expected.
        var header = input[..sizeHeader];
        var data = input[sizeHeader..];
        return new SaveHandlerSplitResult(data, header, default, this);
    }

    public void Finalize(Span<byte> data) { }
}
