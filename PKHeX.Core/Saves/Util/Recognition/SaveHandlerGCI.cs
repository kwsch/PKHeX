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

    private static ReadOnlySpan<byte> COLO_JP => "GC6J"u8; // NTSC-J
    private static ReadOnlySpan<byte> COLO_US => "GC6E"u8; // NTSC-U
    private static ReadOnlySpan<byte> COLO_EU => "GC6P"u8; // PAL

    private static ReadOnlySpan<byte> XD_JP => "GXXJ"u8; // NTSC-J
    private static ReadOnlySpan<byte> XD_US => "GXXE"u8; // NTSC-U
    private static ReadOnlySpan<byte> XD_EU => "GXXP"u8; // PAL

    private static ReadOnlySpan<byte> RSBOX_JP => "GPXJ"u8; // NTSC-J
    private static ReadOnlySpan<byte> RSBOX_US => "GPXE"u8; // NTSC-U
    private static ReadOnlySpan<byte> RSBOX_EU => "GPXP"u8; // PAL

    public bool IsRecognized(long size) => size is SIZE_G3BOXGCI or SIZE_G3COLOGCI or SIZE_G3XDGCI;

    private static bool Is(ReadOnlySpan<byte> intro, ReadOnlySpan<byte> expect) => intro.SequenceEqual(expect);

    public SaveHandlerSplitResult? TrySplit(Memory<byte> input)
    {
        if (input.Length < 4)
            return null;
        var gameCode = input[..4].Span;
        if (gameCode[0] != 'G' && (char)gameCode[1] is not ('C' or 'X' or 'P')) // eager check for G*
            return null;

        switch (input.Length)
        {
            case SIZE_G3COLOGCI when IsColo(gameCode):
            case SIZE_G3XDGCI   when IsXD(gameCode):
            case SIZE_G3BOXGCI  when IsRSBox(gameCode):
                break;
            default:
                return null;
        }

        var header = input[..headerSize];
        var data = input[headerSize..];

        return new SaveHandlerSplitResult(data, header, default, this);
    }

    private static bool IsRSBox(ReadOnlySpan<byte> gameCode) => Is(gameCode, RSBOX_JP) || Is(gameCode, RSBOX_US) || Is(gameCode, RSBOX_EU);
    private static bool IsXD(ReadOnlySpan<byte> gameCode) => Is(gameCode, XD_JP) || Is(gameCode, XD_US) || Is(gameCode, XD_EU);
    private static bool IsColo(ReadOnlySpan<byte> gameCode) => Is(gameCode, COLO_JP) || Is(gameCode, COLO_US) || Is(gameCode, COLO_EU);

    public void Finalize(Span<byte> data) { }

    /// <summary>
    /// Checks if the game code is one of the recognizable versions.
    /// </summary>
    /// <param name="gameCode">4 character game code string</param>
    /// <returns>Magic version ID enumeration; <see cref="GameVersion.Invalid"/> if no match.</returns>
    public static GameVersion GetGameCode(ReadOnlySpan<byte> gameCode)
    {
        if (gameCode.Length < 4)
            return GameVersion.Invalid;
        if (IsColo(gameCode))
            return GameVersion.COLO;
        if (IsXD(gameCode))
            return GameVersion.XD;
        if (IsRSBox(gameCode))
            return GameVersion.RSBOX;

        return GameVersion.Invalid;
    }
}
