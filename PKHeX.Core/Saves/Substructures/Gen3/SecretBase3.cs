using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase3(Memory<byte> raw)
{
    public const int SIZE = 160;

    private Span<byte> Data => raw.Span;

    private bool Japanese => Language == (int) LanguageID.Japanese;

    public int SecretBaseLocation { get => Data[0]; set => Data[0] = (byte) value; }

    public byte OriginalTrainerGender
    {
        get => (byte)((Data[1] >> 4) & 1);
        set => Data[1] = (byte) ((Data[1] & 0xEF) | ((value & 1) << 4));
    }

    public bool BattledToday
    {
        get => ((Data[1] >> 5) & 1) == 1;
        set => Data[1] = (byte)((Data[1] & 0xDF) | ((value ? 1 : 0) << 5));
    }

    public int RegistryStatus
    {
        get => (Data[1] >> 6) & 3;
        set => Data[1] = (byte)((Data[1] & 0x3F) | ((value & 3) << 6));
    }

    public string OriginalTrainerName
    {
        get => StringConverter3.GetString(Data.Slice(2, 7), Language);
        set => StringConverter3.SetString(Data.Slice(2, 7), value, 7, Language, StringConverterOption.ClearFF);
    }

    public uint OT_ID
    {
        get => ReadUInt32LittleEndian(Data[9..]);
        set => WriteUInt32LittleEndian(Data[9..], value);
    }

    public int OT_Class => Data[9] % 5;
    public int Language { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }

    public ushort SecretBasesReceived
    {
        get => ReadUInt16LittleEndian(Data[0x0E..]);
        set => WriteUInt16LittleEndian(Data[0x0E..], value);
    }

    public byte TimesEntered { get => Data[0x10]; set => Data[0x10] = value; }
    public int Unused11  { get => Data[0x11]; set => Data[0x11] = (byte)value; } // alignment padding

    public Span<byte> GetDecorations() => Data.Slice(0x12, 0x10);
    public void SetDecorations(Span<byte> value) => value.CopyTo(GetDecorations());

    public Span<byte> GetDecorationCoordinates() => Data.Slice(0x22, 0x10);
    public void SetDecorationCoordinates(Span<byte> value) => value.CopyTo(GetDecorationCoordinates());

    private Span<byte> TeamData => Data.Slice(0x34, 108);
    public SecretBase3Team Team
    {
        get => new(TeamData.ToArray());
        set => value.Write().CopyTo(TeamData);
    }

    public ushort TID16
    {
        get => (ushort)OT_ID;
        set => OT_ID = (ushort)(SID16 | value);
    }

    public ushort SID16
    {
        get => (ushort)(OT_ID >> 16);
        set => OT_ID = (ushort)((value << 16) | TID16);
    }
}
