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
    private StrategyMemoEntry? this[int Species] => Entries.Find(e => e.Species == Species);
    private readonly ushort _unk;

    public StrategyMemo(bool xd = true) : this(new byte[4], 0, xd) { }

    public StrategyMemo(byte[] input, int offset, bool xd)
    {
        XD = xd;
        int count = ReadUInt16BigEndian(input.AsSpan(offset));
        if (count > MAX_COUNT)
            count = MAX_COUNT;
        _unk = ReadUInt16BigEndian(input.AsSpan(offset + 2));

        Entries = new List<StrategyMemoEntry>(count);
        for (int i = 0; i < count; i++)
        {
            var entry = Read(input, offset, i);
            Entries.Add(entry);
        }
    }

    private StrategyMemoEntry Read(byte[] input, int offset, int index)
    {
        var ofs = 4 + offset + (SIZE_ENTRY * index);
        var span = input.AsSpan(ofs, SIZE_ENTRY);
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

    public StrategyMemoEntry GetEntry(int Species)
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

public sealed class StrategyMemoEntry
{
    public readonly byte[] Data;
    private readonly bool XD;

    public StrategyMemoEntry(bool xd) : this(xd, new byte[StrategyMemo.SIZE_ENTRY]) { }

    public StrategyMemoEntry(bool xd, byte[] data)
    {
        Data = data;
        XD = xd;
    }

    public int Species
    {
        get
        {
            var val = ReadUInt16BigEndian(Data.AsSpan(0)) & 0x1FF;
            return SpeciesConverter.GetG4Species(val);
        }
        set
        {
            var val = SpeciesConverter.GetG3Species(value);
            var cval = ReadUInt16BigEndian(Data.AsSpan(0));
            cval &= 0xE00; val &= 0x1FF; cval |= val;
            WriteUInt16BigEndian(Data.AsSpan(0x00), cval);
        }
    }

    private bool Flag0 { get => Data[0] >> 6 == 1; set { Data[0] &= 0xBF; if (value) Data[0] |= 0x40; } } // Unused
    private bool Flag1 { get => Data[0] >> 7 == 1; set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } } // Complete Entry
    public int SID { get => ReadUInt16BigEndian(Data.AsSpan(4)); set => WriteUInt16BigEndian(Data.AsSpan(4), (ushort)value); }
    public int TID { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), (ushort)value); }
    public uint PID { get => ReadUInt32BigEndian(Data.AsSpan(8)); set => WriteUInt32BigEndian(Data.AsSpan(8), value); }

    public bool Seen
    {
        get
        {
            if (XD) return !Flag1;
            return Species != 0;
        }
        set
        {
            if (XD)
                Flag1 = !value;
            else if (!value)
                new byte[StrategyMemo.SIZE_ENTRY].CopyTo(Data, 0);
        }
    }

    public bool Owned
    {
        get
        {
            if (XD) return false;
            return Flag0 || !Flag1;
        }
        set
        {
            if (XD) return;
            if (!value)
                Flag1 = true;
        }
    }

    public bool IsEmpty => Species == 0;
    public bool Matches(int species, uint pid, int tid, int sid) => Species == species && PID == pid && TID == tid && SID == sid;
}
