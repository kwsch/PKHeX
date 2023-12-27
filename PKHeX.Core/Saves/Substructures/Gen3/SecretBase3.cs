using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase3(byte[] Data, int Offset)
{
    private bool Japanese => Language == (int) LanguageID.Japanese;

    public int SecretBaseLocation { get => Data[Offset + 0]; set => Data[Offset + 0] = (byte) value; }

    public int OT_Gender
    {
        get => (Data[Offset + 1] >> 4) & 1;
        set => Data[Offset + 1] = (byte) ((Data[Offset + 1] & 0xEF) | ((value & 1) << 4));
    }

    public bool BattledToday
    {
        get => ((Data[Offset + 1] >> 5) & 1) == 1;
        set => Data[Offset + 1] = (byte)((Data[Offset + 1] & 0xDF) | ((value ? 1 : 0) << 5));
    }

    public int RegistryStatus
    {
        get => (Data[Offset + 1] >> 6) & 3;
        set => Data[Offset + 1] = (byte)((Data[Offset + 1] & 0x3F) | ((value & 3) << 6));
    }

    public string OT_Name
    {
        get => StringConverter3.GetString(Data.AsSpan(Offset + 2, 7), Japanese);
        set => StringConverter3.SetString(Data.AsSpan(Offset + 2, 7), value, 7, Japanese, StringConverterOption.ClearFF);
    }

    public uint OT_ID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 9));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 9), value);
    }

    public int OT_Class => Data[Offset + 9] % 5;
    public int Language { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }

    public ushort SecretBasesReceived
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0E));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0E), value);
    }

    public byte TimesEntered { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = value; }
    public int Unused11  { get => Data[Offset + 0x11]; set => Data[Offset + 0x11] = (byte)value; } // alignment padding

    public Span<byte> GetDecorations() => Data.AsSpan(Offset + 0x12, 0x10);
    public void SetDecorations(Span<byte> value) => value.CopyTo(GetDecorations());

    public Span<byte> GetDecorationCoordinates() => Data.AsSpan(Offset + 0x22, 0x10);
    public void SetDecorationCoordinates(Span<byte> value) => value.CopyTo(GetDecorationCoordinates());

    private Span<byte> TeamData => Data.AsSpan(Offset + 50, 72);
    public SecretBase3Team Team
    {
        get => new(TeamData.ToArray());
        set => value.Write().CopyTo(TeamData);
    }

    public int TID16
    {
        get => (ushort)OT_ID;
        set => OT_ID = (ushort)(SID16 | (ushort)value);
    }

    public int SID16
    {
        get => (ushort)OT_ID >> 8;
        set => OT_ID = (ushort)(((ushort)value << 16) | TID16);
    }
}
