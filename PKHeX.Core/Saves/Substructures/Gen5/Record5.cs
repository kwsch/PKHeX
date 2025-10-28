using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Record5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    public const int Max32 = 999_999_999;
    public const ushort Max16 = 65535;

    public const byte Record32 = 68; // int32
    public const byte Record16 = 100; // int16
    private const byte Count = Record32 + Record16;

    private Span<byte> DataRegion => Data[4..^4]; // 0..0x1DC

    private uint CryptoSeed // 0x1DC
    {
        get => ReadUInt32LittleEndian(Data[^4..]);
        set => WriteUInt32LittleEndian(Data[^4..], value);
    }

    private bool IsDecrypted;
    public void EndAccess() => EnsureDecrypted(false);
    private void EnsureDecrypted(bool state = true)
    {
        if (IsDecrypted == state)
            return;
        PokeCrypto.CryptArray(DataRegion, CryptoSeed);
        IsDecrypted = state;
    }

    public uint Revision // 0x00
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public static ushort GetMax16(int recordID) => Max16;
    public static uint GetMax32(int recordID) => Max32;

    public static int GetOffset(int recordID) => recordID switch
    {
        < Record32 => (recordID * sizeof(int)),
        < Count => (Record32 * sizeof(int)) + ((recordID - Record32) * sizeof(ushort)),
        _ => -1,
    };

    private const int Partition2 = Record32 * sizeof(uint);
    private Span<byte> Record32Data => DataRegion[..Partition2];
    private Span<byte> Record16Data => DataRegion[Partition2..];

    public uint GetRecord32(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, Record32);
        EnsureDecrypted();
        return ReadUInt32LittleEndian(Record32Data[(index * 4)..]);
    }

    public void SetRecord32(int index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, Record32);
        EnsureDecrypted();
        WriteUInt32LittleEndian(Record32Data[(index * 4)..], Math.Min(GetMax32(index), value));
    }

    public ushort GetRecord16(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, Record16);
        EnsureDecrypted();
        return ReadUInt16LittleEndian(Record16Data[(index * 2)..]);
    }

    public void SetRecord16(int index, ushort value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, Record16);
        EnsureDecrypted();
        WriteUInt16LittleEndian(Record16Data[(index * 2)..], Math.Min(GetMax16(index), value));
    }
}
