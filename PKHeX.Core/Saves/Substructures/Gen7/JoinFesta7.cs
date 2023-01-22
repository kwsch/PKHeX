using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class JoinFesta7 : SaveBlock<SAV7>
{
    public JoinFesta7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
    public JoinFesta7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

    public int FestaCoins
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x508));
        set
        {
            if (value > 9999999)
                value = 9999999;
            WriteInt32LittleEndian(Data.AsSpan(Offset + 0x508), value);

            TotalFestaCoins = SAV.GetRecord(038) + value; // UsedFestaCoins
        }
    }

    public int TotalFestaCoins
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x50C));
        set
        {
            if (value > 9999999)
                value = 9999999;
            WriteInt32LittleEndian(Data.AsSpan(Offset + 0x50C), value);
        }
    }

    private Span<byte> FestivalPlazaNameSpan => Data.AsSpan(Offset + 0x510, 0x2A);

    public string FestivalPlazaName
    {
        get => StringConverter7.GetString(FestivalPlazaNameSpan);
        set => StringConverter7.SetString(FestivalPlazaNameSpan, value, 20, 0, StringConverterOption.ClearZero);
    }

    public ushort FestaRank { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x53A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x53A), value); }
    public ushort GetFestaMessage(int index) => ReadUInt16LittleEndian(Data.AsSpan(Offset + (index * 2)));
    public void SetFestaMessage(int index, ushort value) => WriteUInt16LittleEndian(Data.AsSpan(Offset + (index * 2)), value);
    public bool GetFestaPhraseUnlocked(int index) => Data[Offset + 0x2A50 + index] != 0; //index: 0 to 105:commonPhrases, 106:Lv100!

    public void SetFestaPhraseUnlocked(int index, bool value)
    {
        if (GetFestaPhraseUnlocked(index) != value)
            Data[Offset + 0x2A50 + index] = value ? (byte)1 : (byte)0;
    }

    public byte GetFestPrizeReceived(int index) => Data[Offset + 0x53C + index];
    public void SetFestaPrizeReceived(int index, byte value) => Data[Offset + 0x53C + index] = value;
    private int FestaYear { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x2F0)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x2F0), value); }
    private int FestaMonth { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x2F4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x2F4), value); }
    private int FestaDay { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x2F8)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x2F8), value); }
    private int FestaHour { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x300)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x300), value); }
    private int FestaMinute { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x304)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x304), value); }
    private int FestaSecond { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x308)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x308), value); }

    public DateTime? FestaDate
    {
        get => FestaYear >= 0 && FestaMonth > 0 && FestaDay > 0 && FestaHour >= 0 && FestaMinute >= 0 && FestaSecond >= 0 && DateUtil.IsDateValid(FestaYear, FestaMonth, FestaDay)
            ? new DateTime(FestaYear, FestaMonth, FestaDay, FestaHour, FestaMinute, FestaSecond)
            : null;
        set
        {
            if (value.HasValue)
            {
                DateTime dt = value.Value;
                FestaYear = dt.Year;
                FestaMonth = dt.Month;
                FestaDay = dt.Day;
                FestaHour = dt.Hour;
                FestaMinute = dt.Minute;
                FestaSecond = dt.Second;
            }
        }
    }
}
