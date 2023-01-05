using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public class MyStatus6 : SaveBlock<SAV6>, IRegionOrigin
{
    public MyStatus6(SAV6 sav, int offset) : base(sav) => Offset = offset;

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0), value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0), value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 2));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 2), value);
    }

    public int Game
    {
        get => Data[Offset + 4];
        set => Data[Offset + 4] = (byte)value;
    }

    public int Gender
    {
        get => Data[Offset + 5];
        set => Data[Offset + 5] = (byte)value;
    }

    public int MultiplayerSpriteID_1
    {
        get => Data[Offset + 6];
        set => Data[Offset + 6] = (byte)value;
    }

    public int MultiplayerSpriteID_2
    {
        get => Data[Offset + 7];
        set => Data[Offset + 7] = (byte)value;
    }

    public const int GameSyncIDSize = 16; // 64 bits

    public string GameSyncID
    {
        get => Util.GetHexStringFromBytes(Data.AsSpan(Offset + 0x08, GameSyncIDSize / 2));
        set
        {
            if (value.Length != GameSyncIDSize)
                throw new ArgumentException(nameof(value));

            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(data, Offset + 0x08);
        }
    }

    public byte Region
    {
        get => Data[Offset + 0x26];
        set => Data[Offset + 0x26] = value;
    }

    public byte Country
    {
        get => Data[Offset + 0x27];
        set => Data[Offset + 0x27] = value;
    }

    public decimal Latitude // don't use the setters
    {
        get => (ReadInt16LittleEndian(Data.AsSpan(Offset + 0x28)) * 180m) / 0x8000;
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0x28), (short)((value * 0x8000) / 180m));
    }

    public decimal Longitude // don't use the setters
    {
        get => (ReadInt16LittleEndian(Data.AsSpan(Offset + 0x2A)) * 180m) / 0x8000;
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0x2A), (short)((value * 0x8000) / 180m));
    }

    public byte ConsoleRegion
    {
        get => Data[Offset + 0x2C];
        set => Data[Offset + 0x2C] = value;
    }

    public int Language
    {
        get => Data[Offset + 0x2D];
        set => Data[Offset + 0x2D] = (byte)value;
    }

    private Span<byte> OT_Trash => Data.AsSpan(Offset + 0x48, 0x1A);

    public string OT
    {
        get => SAV.GetString(OT_Trash);
        set => SAV.SetString(OT_Trash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    private Span<byte> GetSayingSpan(int say) => Data.AsSpan(GetSayingOffset(say), SAV6.LongStringLength);
    private int GetSayingOffset(int say) => Offset + 0x7C + (SAV6.LongStringLength * say);
    private string GetSaying(int say) => SAV.GetString(GetSayingSpan(say));
    private void SetSaying(int say, ReadOnlySpan<char> value) => SAV.SetString(GetSayingSpan(say), value, SAV6.LongStringLength / 2, StringConverterOption.ClearZero);

    public string Saying1 { get => GetSaying(0); set => SetSaying(0, value); }
    public string Saying2 { get => GetSaying(1); set => SetSaying(1, value); }
    public string Saying3 { get => GetSaying(2); set => SetSaying(2, value); }
    public string Saying4 { get => GetSaying(3); set => SetSaying(3, value); }
    public string Saying5 { get => GetSaying(4); set => SetSaying(4, value); }

    public bool IsMegaEvolutionUnlocked
    {
        get => (Data[Offset + 0x14A] & 0x01) != 0;
        set => Data[Offset + 0x14A] = (byte)((Data[Offset + 0x14A] & 0xFE) | (value ? 1 : 0)); // in battle
    }

    public bool IsMegaRayquazaUnlocked
    {
        get => (Data[Offset + 0x14A] & 0x02) != 0;
        set => Data[Offset + 0x14A] = (byte)((Data[Offset + 0x14A] & ~2) | (value ? 2 : 0)); // in battle
    }
}
