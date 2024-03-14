using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for recognizing .gci save files.
/// </summary>
public sealed class SaveHandlerGCI : ISaveHandler
{
    private const int headerSize = 0x40;
    private const int SIZE_G3BOXGCI  = headerSize + SaveUtil.SIZE_G3BOX; // GCI data
    private const int SIZE_G3COLOGCI = headerSize + SaveUtil.SIZE_G3COLO; // GCI data
    private const int SIZE_G3XDGCI   = headerSize + SaveUtil.SIZE_G3XD; // GCI data

    private static readonly string[] HEADER_COLO  = ["GC6J", "GC6E", "GC6P"]; // NTSC-J, NTSC-U, PAL
    private static readonly string[] HEADER_XD    = ["GXXJ", "GXXE", "GXXP"]; // NTSC-J, NTSC-U, PAL
    private static readonly string[] HEADER_RSBOX = ["GPXJ", "GPXE", "GPXP"]; // NTSC-J, NTSC-U, PAL

    private static bool IsGameMatchHeader(ReadOnlySpan<string> headers, ReadOnlySpan<byte> data)
    {
        foreach (var header in headers)
        {
            if (IsGameMatchHeader(data, header))
                return true;
        }
        return false;
    }

    private static bool IsGameMatchHeader(ReadOnlySpan<byte> data, ReadOnlySpan<char> header)
    {
        for (int i = 0; i < header.Length; i++)
        {
            var c = (byte)header[i];
            if (data[i] != c)
                return false;
        }

        return true;
    }

    public bool IsRecognized(long size) => size is SIZE_G3BOXGCI or SIZE_G3COLOGCI or SIZE_G3XDGCI;

    public SaveHandlerSplitResult? TrySplit(ReadOnlySpan<byte> input)
    {
        switch (input.Length)
        {
            case SIZE_G3COLOGCI when IsGameMatchHeader(HEADER_COLO , input):
            case SIZE_G3XDGCI   when IsGameMatchHeader(HEADER_XD   , input):
            case SIZE_G3BOXGCI  when IsGameMatchHeader(HEADER_RSBOX, input):
                break;
            default:
                return null;
        }

        var header = input[..headerSize].ToArray();
        var data = input[headerSize..].ToArray();

        return new SaveHandlerSplitResult(data, header, [], this);
    }

    public void Finalize(Span<byte> data) { }

    /// <summary>
    /// Checks if the game code is one of the recognizable versions.
    /// </summary>
    /// <param name="gameCode">4 character game code string</param>
    /// <returns>Magic version ID enumeration; <see cref="GameVersion.Invalid"/> if no match.</returns>
    public static GameVersion GetGameCode(ReadOnlySpan<byte> gameCode)
    {
        if (IsGameMatchHeader(HEADER_COLO, gameCode))
            return GameVersion.COLO;
        if (IsGameMatchHeader(HEADER_XD, gameCode))
            return GameVersion.XD;
        if (IsGameMatchHeader(HEADER_RSBOX, gameCode))
            return GameVersion.RSBOX;

        return GameVersion.Invalid;
    }
}
