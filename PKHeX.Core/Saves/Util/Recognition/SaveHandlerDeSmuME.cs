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

    private const string SignatureDSV = "|-DESMUME SAVE-|";

    private static bool GetHasFooter(ReadOnlySpan<byte> input)
    {
        var start = input.Length - SignatureDSV.Length;
        var footer = input[start..];
        for (int i = SignatureDSV.Length - 1; i >= 0; i--)
        {
            byte c = (byte)SignatureDSV[i];
            if (footer[i] != c)
                return false;
        }
        return true;
    }

    public bool IsRecognized(int size) => size is ExpectedSize;

    public SaveHandlerSplitResult? TrySplit(ReadOnlySpan<byte> input)
    {
        if (!GetHasFooter(input))
            return null;

        var footer = input[^RealSize..].ToArray();
        var data = input[..RealSize].ToArray();

        return new SaveHandlerSplitResult(data, Array.Empty<byte>(), footer);
    }
}
