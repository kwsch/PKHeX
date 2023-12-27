using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FestaFacility
{
    private const int SIZE = 0x48;
    private readonly byte[] Data;
    private readonly int Language;
    private readonly int ofs;

    public FestaFacility(SAV7 sav, int index)
    {
        ofs = (index * SIZE) + sav.Festa.Offset + 0x310;
        Data = sav.Data.AsSpan(ofs, SIZE).ToArray();
        Language = sav.Language;
    }

    public void CopyTo(SAV7 sav) => sav.SetData(Data, ofs);

    public int Type { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public int Color { get => Data[0x01]; set => Data[0x01] = (byte)value; }
    public bool IsIntroduced { get => Data[0x02] != 0; set => Data[0x02] = value ? (byte)1 : (byte)0; }
    public int Gender { get => Data[0x03]; set => Data[0x03] = (byte)value; }
    public string OT_Name { get => StringConverter7.GetString(Data.AsSpan(0x04, 0x1A)); set => StringConverter7.SetString(Data.AsSpan(0x04, 0x1A), value, 12, Language, StringConverterOption.ClearZero); }

    private int MessageMeet { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), (ushort)value); }
    private int MessagePart { get => ReadUInt16LittleEndian(Data.AsSpan(0x20)); set => WriteUInt16LittleEndian(Data.AsSpan(0x20), (ushort)value); }
    private int MessageMoved { get => ReadUInt16LittleEndian(Data.AsSpan(0x22)); set => WriteUInt16LittleEndian(Data.AsSpan(0x22), (ushort)value); }
    private int MessageDisappointed { get => ReadUInt16LittleEndian(Data.AsSpan(0x24)); set => WriteUInt16LittleEndian(Data.AsSpan(0x24), (ushort)value); }
    public int UsedLuckyRank { get => Data[0x26]; set => Data[0x26] = (byte)value; } // 1:a bit, 2:a whole lot, 3:a whole ton
    public int UsedLuckyPlace { get => Data[0x27]; set => Data[0x27] = (byte)value; } // 1:GTS, ... 7:haunted house
    public uint UsedFlags { get => ReadUInt32LittleEndian(Data.AsSpan(0x28)); set => WriteUInt32LittleEndian(Data.AsSpan(0x28), value); }
    public uint UsedRandStat { get => ReadUInt32LittleEndian(Data.AsSpan(0x2C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x2C), value); }

    public int NPC { get => Math.Max(0, ReadInt32LittleEndian(Data.AsSpan(0x30))); set => WriteInt32LittleEndian(Data.AsSpan(0x30), Math.Max(0, value)); }
    public Span<byte> TrainerFesID => Data.AsSpan(0x34, 0xC);
    public void ClearTrainerFesID() => TrainerFesID.Clear();
    public int ExchangeLeftCount { get => Data[0x40]; set => Data[0x40] = (byte)value; } // used when Type=Exchange

    public int GetMessage(int index) => index switch
    {
        0 => MessageMeet,
        1 => MessagePart,
        2 => MessageMoved,
        3 => MessageDisappointed,
        _ => 0,
    };

    public int SetMessage(int index, ushort value) => index switch
    {
        0 => MessageMeet = value,
        1 => MessagePart = value,
        2 => MessageMoved = value,
        3 => MessageDisappointed = value,
        _ => -1,
    };
}
