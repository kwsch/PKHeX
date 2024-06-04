using System;

namespace PKHeX.Core;

/// <summary>
/// Relaxed handler for Gen1-3 games that store RTC data in the footer.
/// </summary>
public sealed class SaveHandlerFooterRTC : ISaveHandler
{
    private const byte MinFooter = 0x0C;
    private const byte MaxFooter = 0x30;

    public bool IsRecognized(long size)
    {
        // Emulators store their RTC data in the footer differently across implementations.
        // Check for the 3 sizes, with a tolerance.
        var footerSize = size & 0x3F;
        if (!IsPlausibleFooterSize(footerSize))
            return false;
        var noFooter = size - footerSize;
        return noFooter is SaveUtil.SIZE_G3RAW or SaveUtil.SIZE_G2RAW_J or SaveUtil.SIZE_G1RAW;
    }

    private static bool IsPlausibleFooterSize(long size)
    {
        if ((size & 1) != 0) // must be even
            return false;
        return size is not (> MaxFooter or < MinFooter);
    }

    public SaveHandlerSplitResult TrySplit(ReadOnlySpan<byte> input)
    {
        var footerSize = input.Length & 0x3F;
        var RealSize = input.Length - footerSize;
        var footer = input[RealSize..].ToArray();
        var data = input[..RealSize].ToArray();

        return new SaveHandlerSplitResult(data, [], footer, this);
    }

    public void Finalize(Span<byte> data) { }
}
