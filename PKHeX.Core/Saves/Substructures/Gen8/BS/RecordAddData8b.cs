using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores additional record data.
/// </summary>
/// <remarks>size: 0x3C0</remarks>
public sealed class RecordAddData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    // RECORD_ADD_DATA: 0x30-sized[12] (0x240 bytes), and 12*byte[32] (0x180), total 0x3C0

    private const int COUNT_RECORD_ADD = 12;
    private const int COUNT_RECORD_RANKING = 12;
    private const int COUNT_RECORD_RANKING_FLAG = 32;

    public RecordAdd8b GetRecord(int index)
    {
        if ((uint)index >= COUNT_RECORD_ADD)
            throw new ArgumentOutOfRangeException(nameof(index));
        return new RecordAdd8b(Raw.Slice(index * RecordAdd8b.SIZE, RecordAdd8b.SIZE));
    }

    public RecordAdd8b[] GetRecords()
    {
        var result = new RecordAdd8b[COUNT_RECORD_ADD];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRecord(i);
        return result;
    }

    public void ReplaceOT(ITrainerInfo oldTrainer, ITrainerInfo newTrainer)
    {
        foreach (var r in GetRecords())
        {
            if (string.IsNullOrWhiteSpace(r.OT))
                continue;

            if (oldTrainer.OT != r.OT || oldTrainer.ID32 != r.ID32)
                continue;

            r.OT = newTrainer.OT;
            r.ID32 = newTrainer.ID32;
        }
    }
}

public sealed class RecordAdd8b(Memory<byte> Raw)
{
    private Span<byte> Data => Raw.Span;

    public const int SIZE = 0x30;

    public string OT
    {
        get => StringConverter8.GetString(Data[..0x1A]);
        set => StringConverter8.SetString(Data[..0x1A], value, 12, StringConverterOption.ClearZero);
    }

    // 1A reserved byte
    // 1B reserved byte

    public int Language
    {
        get => ReadInt32LittleEndian(Data[0x1C..]);
        set => WriteInt32LittleEndian(Data[0x1C..], value);
    }

    public byte Gender { get => Data[0x20]; set => Data[0x20] = value; }
    // 21
    // 22
    // 23

    public int BodyType
    {
        get => ReadInt32LittleEndian(Data[0x24..]);
        set => WriteInt32LittleEndian(Data[0x24..], value);
    }

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data[0x28..]);
        set => WriteUInt32LittleEndian(Data[0x28..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[0x28..]);
        set => WriteUInt16LittleEndian(Data[0x28..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[0x2A..]);
        set => WriteUInt16LittleEndian(Data[0x2A..], value);
    }

    // 0x2C int32 reserved
}
