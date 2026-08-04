using System;
using System.Buffers.Binary;
using PKHeX.Application.Abstractions.LiveHex;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// Reads and writes a single box of raw (encrypted, party-size) Pokémon bytes to/from console RAM
/// according to a game's <see cref="LiveHexGameProfile"/>. The byte layout of a box in console RAM
/// is identical to <see cref="PKHeX.Core.SaveFile.GetBoxBinary"/>/<c>SetBoxBinary</c> for the
/// supported games, so callers can bridge directly through the loaded save.
/// </summary>
internal static class LiveHexBoxAddressing
{
    public static byte[] ReadBox(IConsoleConnection connection, LiveHexGameProfile profile, int box, int slotSize, int slotCount)
    {
        int boxLength = slotSize * slotCount;
        switch (profile.Mode)
        {
            case BoxAddressingMode.HeapFlat:
                return connection.ReadHeap(profile.HeapBoxStart + (ulong)(box * boxLength), boxLength);

            case BoxAddressingMode.PointerContiguous:
            {
                var boxStart = ResolveBase(connection, profile) + (ulong)(box * boxLength);
                return connection.ReadHeap(boxStart, boxLength);
            }

            case BoxAddressingMode.PointerScatter:
            {
                var pointers = GetMonPointers(connection, profile, box, slotCount);
                var result = new byte[boxLength];
                for (int i = 0; i < slotCount; i++)
                {
                    var mon = connection.ReadAbsolute(pointers[i] + 0x20, slotSize);
                    mon.CopyTo(result.AsSpan(i * slotSize));
                }
                return result;
            }

            default:
                throw new LiveHexConnectionException("Unsupported box addressing mode.");
        }
    }

    public static void WriteBox(IConsoleConnection connection, LiveHexGameProfile profile, int box, ReadOnlySpan<byte> boxData, int slotSize, int slotCount)
    {
        int boxLength = slotSize * slotCount;
        switch (profile.Mode)
        {
            case BoxAddressingMode.HeapFlat:
                connection.WriteHeap(boxData, profile.HeapBoxStart + (ulong)(box * boxLength));
                break;

            case BoxAddressingMode.PointerContiguous:
            {
                var boxStart = ResolveBase(connection, profile) + (ulong)(box * boxLength);
                connection.WriteHeap(boxData, boxStart);
                break;
            }

            case BoxAddressingMode.PointerScatter:
            {
                var pointers = GetMonPointers(connection, profile, box, slotCount);
                for (int i = 0; i < slotCount; i++)
                    connection.WriteAbsolute(boxData.Slice(i * slotSize, slotSize), pointers[i] + 0x20);
                break;
            }

            default:
                throw new LiveHexConnectionException("Unsupported box addressing mode.");
        }
    }

    private static ulong ResolveBase(IConsoleConnection connection, LiveHexGameProfile profile)
    {
        var address = PointerResolver.Resolve(connection, profile.PointerExpr);
        if (address == PointerResolver.InvalidPointer)
            throw new LiveHexConnectionException(
                "Failed to resolve the box memory pointer. Make sure the game is running and you are past the title screen.");
        return address;
    }

    private static ulong[] GetMonPointers(IConsoleConnection connection, LiveHexGameProfile profile, int box, int slotCount)
    {
        var listAddress = ResolveBase(connection, profile);
        var table = connection.ReadHeap(listAddress, profile.ScatterPointerCount * 8);
        if (table.Length == 0 || table[0] == 0)
            throw new LiveHexConnectionException(
                "The console returned an empty box list. Make sure the game is running and you are past the title screen.");

        var boxObjectPointer = BinaryPrimitives.ReadUInt64LittleEndian(table.AsSpan(box * 8)) + 0x20;
        var monTable = connection.ReadAbsolute(boxObjectPointer, slotCount * 8);
        var pointers = new ulong[slotCount];
        for (int i = 0; i < slotCount; i++)
            pointers[i] = BinaryPrimitives.ReadUInt64LittleEndian(monTable.AsSpan(i * 8));
        return pointers;
    }
}
