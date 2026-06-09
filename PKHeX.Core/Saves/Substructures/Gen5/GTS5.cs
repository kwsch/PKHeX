using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GTS5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    // 0x08: Stored Upload
    private const int SizeStored = PokeCrypto.SIZE_5PARTY;

    public Memory<byte> Upload => Raw[..SizeStored];

    // 16 bytes unused (maybe they forgot to update from Gen4 size const?)

    public ushort UnknownEC { get => ReadUInt16LittleEndian(Data[0xEC..]); set => WriteUInt16LittleEndian(Data[0xEC..], value); }
    public ushort UnknownEE { get => ReadUInt16LittleEndian(Data[0xEE..]); set => WriteUInt16LittleEndian(Data[0xEE..], value); }

    // Timestamps for interaction tracking:
    // Example 1:
    // 02 07 08 0C
    // 05 02 09 0B
    // Example 2:
    // 00 00 00 00
    // 04 0E 04 0B
    public DateQuad5 DateUpload => new(Raw[0xF0..0xF4]);
    public DateQuad5 DateSearch => new(Raw[0xF4..0xF8]); // don't have to upload to search
}
