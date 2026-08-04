using System;
using System.Text;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// Hex encoding/decoding helpers for the sys-botbase wire protocol. Poke payloads are sent as
/// upper-case hex; peek responses arrive as an ASCII hex string terminated by a newline.
/// </summary>
internal static class HexCodec
{
    /// <summary>Encodes <paramref name="data"/> as an upper-case hex string (no separators).</summary>
    public static string ToHex(ReadOnlySpan<byte> data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (var b in data)
            sb.Append(b.ToString("X2"));
        return sb.ToString();
    }

    /// <summary>
    /// Decodes an ASCII hex response (<paramref name="asciiHex"/>) into <paramref name="dest"/>.
    /// The response length must be exactly twice the destination length.
    /// </summary>
    public static void DecodeInto(ReadOnlySpan<byte> asciiHex, Span<byte> dest)
    {
        if (asciiHex.Length != dest.Length * 2)
            throw new FormatException($"Console returned {asciiHex.Length} hex chars; expected {dest.Length * 2}.");
        Convert.FromHexString(asciiHex, dest, out _, out _);
    }
}
