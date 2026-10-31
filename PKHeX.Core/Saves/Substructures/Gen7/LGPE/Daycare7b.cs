using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Daycare7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    private const int StoredSize = PokeCrypto.SIZE_6STORED;

    /// <summary> Whether a Pokémon is deposited. </summary>
    public bool IsOccupied
    {
        get => Data[0] != 0;
        set => Data[0] = value ? (byte)1 : (byte)0;
    }

    /// <summary> Step counter. </summary>
    public uint StepCounter
    {
        get => ReadUInt32LittleEndian(Data[4..]);
        set => WriteUInt32LittleEndian(Data[4..], value);
    }

    public Memory<byte> Stored => Raw.Slice(8, StoredSize);
}
