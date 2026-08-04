using System;
using System.Buffers.Binary;
using System.Globalization;
using PKHeX.Application.Abstractions.LiveHex;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// Resolves a sys-botbase pointer-expression string (e.g. <c>"[[main+44C1C18]+130]+9B0"</c>) to a
/// concrete memory address by walking the pointer chain over the connection.
/// </summary>
/// <remarks>
/// Clean-room re-implementation of the pointer-walk used by LiveHeX tooling. The pointer-expression
/// grammar and the per-game offset constants are factual data mirrored from the upstream
/// PKHeX-Plugins project (see <c>NOTICE.LiveHeX.md</c>).
/// </remarks>
internal static class PointerResolver
{
    public const ulong InvalidPointer = 0;

    /// <summary>
    /// Walks <paramref name="pointer"/> and returns the final address. When
    /// <paramref name="heapRelative"/> is true (the default) the returned address is expressed
    /// relative to the heap base so it can be used with heap <c>peek</c>/<c>poke</c> commands.
    /// </summary>
    public static ulong Resolve(IConsoleConnection connection, string pointer, bool heapRelative = true)
    {
        if (string.IsNullOrWhiteSpace(pointer) || pointer.AsSpan().IndexOfAny('-', '/', '*') != -1)
            return InvalidPointer;

        // "]]" means a dereference with a +0 offset; normalise it so the split below is uniform.
        while (pointer.Contains("]]"))
            pointer = pointer.Replace("]]", "]+0]");

        uint finalAdd = 0;
        if (!pointer.EndsWith(']'))
        {
            var tail = pointer[(pointer.LastIndexOf('+') + 1)..];
            finalAdd = ParseHex(tail);
            int lastPlus = pointer.LastIndexOf('+');
            if (lastPlus != -1)
                pointer = pointer[..lastPlus];
        }

        var jumps = pointer
            .Replace("main", string.Empty)
            .Replace("[", string.Empty)
            .Replace("]", string.Empty)
            .Split('+', StringSplitOptions.RemoveEmptyEntries);
        if (jumps.Length == 0)
            return InvalidPointer;

        var initialAddress = ParseHex(jumps[0].Trim());
        ulong address = BinaryPrimitives.ReadUInt64LittleEndian(connection.ReadMain(initialAddress, 0x8));
        foreach (var jump in jumps)
        {
            var val = ParseHex(jump.Trim());
            if (val == initialAddress)
            {
                if (jumps.Length == 1 && finalAdd == 0)
                    return initialAddress;
                continue;
            }
            address = BinaryPrimitives.ReadUInt64LittleEndian(connection.ReadAbsolute(address + val, 0x8));
        }

        address += finalAdd;
        if (heapRelative)
            address -= connection.GetHeapBase();
        return address;
    }

    private static uint ParseHex(string value)
    {
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            value = value[2..];
        return uint.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
    }
}
