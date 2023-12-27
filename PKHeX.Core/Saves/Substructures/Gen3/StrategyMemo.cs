using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class StrategyMemo
{
    private readonly bool XD;
    public const int SIZE_ENTRY = 12;
    private readonly List<StrategyMemoEntry> Entries;
    public const int MAX_COUNT = 500;
    private StrategyMemoEntry? this[ushort Species] => Entries.Find(e => e.Species == Species);
    private readonly ushort _unk;

    public StrategyMemo(bool xd = true) : this(stackalloc byte[4], xd) { }

    public StrategyMemo(Span<byte> block, bool xd)
    {
        XD = xd;
        int count = ReadUInt16BigEndian(block);
        if (count > MAX_COUNT)
            count = MAX_COUNT;
        _unk = ReadUInt16BigEndian(block[2..]);

        Entries = new List<StrategyMemoEntry>(count);
        for (int i = 0; i < count; i++)
        {
            var entry = Read(block, i);
            Entries.Add(entry);
        }
    }

    private StrategyMemoEntry Read(Span<byte> block, int index)
    {
        var ofs = 4 + (SIZE_ENTRY * index);
        var span = block.Slice(ofs, SIZE_ENTRY);
        var data = span.ToArray();
        return new StrategyMemoEntry(XD, data);
    }

    public byte[] Write()
    {
        var result = new byte[4 + (Entries.Count * SIZE_ENTRY)];
        WriteInt16BigEndian(result.AsSpan(0), (short)Entries.Count);
        WriteInt16BigEndian(result.AsSpan(2), (short)_unk);

        var count = Math.Min(MAX_COUNT, Entries.Count);
        for (int i = 0; i < count; i++)
            Entries[i].Data.CopyTo(result, 4 + (i * SIZE_ENTRY));
        return result;
    }

    public StrategyMemoEntry GetEntry(ushort Species)
    {
        return this[Species] ?? new StrategyMemoEntry(XD);
    }

    public void SetEntry(StrategyMemoEntry entry)
    {
        int index = Entries.FindIndex(ent => ent.Species == entry.Species);
        if (index >= 0)
            Entries[index] = entry;
        else
            Entries.Add(entry);
    }
}

public sealed class StrategyMemoEntry(bool XD, byte[] Data)
{
    public readonly byte[] Data = Data;

    public StrategyMemoEntry(bool XD) : this(XD, new byte[StrategyMemo.SIZE_ENTRY]) { }

    public ushort Species
    {
        get
        {
            var val = (ushort)(ReadUInt16BigEndian(Data.AsSpan(0)) & 0x1FF);
            return SpeciesConverter.GetNational3(val);
        }
        set
        {
            var val = SpeciesConverter.GetInternal3(value);
            var cval = ReadUInt16BigEndian(Data.AsSpan(0));
            cval &= 0xE00; val &= 0x1FF; cval |= val;
            WriteUInt16BigEndian(Data.AsSpan(0x00), cval);
        }
    }

    private bool Flag0 { get => Data[0] >> 6 == 1; set { Data[0] &= 0xBF; if (value) Data[0] |= 0x40; } } // Unused
    private bool Flag1 { get => Data[0] >> 7 == 1; set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } } // Complete Entry

    public uint ID32
    {
        get => ReadUInt32BigEndian(Data.AsSpan(0x04));
        set => WriteUInt32BigEndian(Data.AsSpan(0x04), value);
    }
    public ushort SID16 { get => ReadUInt16BigEndian(Data.AsSpan(4)); set => WriteUInt16BigEndian(Data.AsSpan(4), value); }
    public ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), value); }
    public uint PID { get => ReadUInt32BigEndian(Data.AsSpan(8)); set => WriteUInt32BigEndian(Data.AsSpan(8), value); }

    public bool Seen
    {
        get
        {
            if (XD)
                return !Flag1;
            return Species != 0;
        }
        set
        {
            if (XD)
                Flag1 = !value;
            else if (!value)
                Data.AsSpan(0, StrategyMemo.SIZE_ENTRY).Clear();
        }
    }

    public bool Owned
    {
        get
        {
            if (XD)
                return false;
            return Flag0 || !Flag1;
        }
        set
        {
            if (XD)
                return;
            if (!value)
                Flag1 = true;
        }
    }

    public bool IsEmpty => Species == 0;
    public bool Matches(ushort species, uint pid, uint id32) => Species == species && PID == pid && ID32 == id32;
}
