using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for recognizing .dsv save files from DeSmuME.
/// </summary>
public sealed class SaveHandlerDeSmuME : ISaveHandler
{
    private const int sizeFooter = 0x7A;
    private const int RealSize = SaveUtil.SIZE_G4RAW;
    private const int ExpectedSize = RealSize + sizeFooter;

    private static bool GetHasFooter(ReadOnlySpan<byte> input) => input.EndsWith("|-DESMUME SAVE-|"u8);

    public bool IsRecognized(long size) => size is ExpectedSize;

    public SaveHandlerSplitResult? TrySplit(Memory<byte> input)
    {
        if (!GetHasFooter(input.Span))
            return null;

        var footer = input[RealSize..];
        var data = input[..RealSize];

        return new SaveHandlerSplitResult(data, default, footer, this);
    }

    public void Finalize(Span<byte> data) { }
}
