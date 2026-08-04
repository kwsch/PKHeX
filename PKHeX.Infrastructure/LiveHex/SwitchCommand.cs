using System;
using System.Text;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// Encodes sys-botbase text commands as ASCII byte arrays for transmission to a Nintendo Switch
/// running the sys-botbase sys-module.
/// </summary>
/// <remarks>
/// Clean-room re-implementation of the small, well-documented sys-botbase wire protocol
/// (peek/poke/peekMain/peekAbsolute/pokeAbsolute/getHeapBase/getTitleID/getVersion/game). The
/// command grammar is a public protocol; see <c>NOTICE.LiveHeX.md</c> for attribution to the
/// upstream PKHeX-Plugins / sys-botbase projects.
/// </remarks>
internal static class SwitchCommand
{
    private static byte[] Encode(string command) => Encoding.ASCII.GetBytes(command + "\r\n");

    /// <summary>peek: read <paramref name="count"/> bytes from a heap-relative <paramref name="offset"/>.</summary>
    public static byte[] Peek(ulong offset, int count) => Encode($"peek 0x{offset:X16} {count}");

    /// <summary>poke: write <paramref name="data"/> to a heap-relative <paramref name="offset"/>.</summary>
    public static byte[] Poke(ulong offset, ReadOnlySpan<byte> data) => Encode($"poke 0x{offset:X16} 0x{HexCodec.ToHex(data)}");

    /// <summary>peekMain: read <paramref name="count"/> bytes from a main-NSO-relative <paramref name="offset"/>.</summary>
    public static byte[] PeekMain(ulong offset, int count) => Encode($"peekMain 0x{offset:X16} {count}");

    /// <summary>peekAbsolute: read <paramref name="count"/> bytes from an absolute <paramref name="offset"/>.</summary>
    public static byte[] PeekAbsolute(ulong offset, int count) => Encode($"peekAbsolute 0x{offset:X16} {count}");

    /// <summary>pokeAbsolute: write <paramref name="data"/> to an absolute <paramref name="offset"/>.</summary>
    public static byte[] PokeAbsolute(ulong offset, ReadOnlySpan<byte> data) => Encode($"pokeAbsolute 0x{offset:X16} 0x{HexCodec.ToHex(data)}");

    /// <summary>getHeapBase: request the heap base address of the attached process.</summary>
    public static byte[] GetHeapBase() => Encode("getHeapBase");

    /// <summary>getTitleID: request the title id of the attached process.</summary>
    public static byte[] GetTitleId() => Encode("getTitleID");

    /// <summary>getVersion: request the sys-botbase version.</summary>
    public static byte[] GetBotbaseVersion() => Encode("getVersion");

    /// <summary>game: request running-game information (e.g. "version").</summary>
    public static byte[] GetGameInfo(string info) => Encode($"game {info}");
}
