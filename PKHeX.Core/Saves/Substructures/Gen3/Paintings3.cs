using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Paintings3(Memory<byte> raw, bool Japanese)
{
    public const int CountCaption = 3;
    public const int FlagIndexContestStat = 160;
    public const int SIZE = 0x20;

    public Span<byte> Data => raw.Span;

    public uint PID { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort TID { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort SID { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }
    public ushort Species
    {
        get => SpeciesConverter.GetNational3(ReadUInt16LittleEndian(Data[0x08..]));
        set => WriteUInt16LittleEndian(Data[0x08..], SpeciesConverter.GetInternal3(value));
    }

    public byte Caption { get => Data[0x0A]; set => Data[0x0A] = value; }

    public Span<byte> NicknameTrash => Data.Slice(0x0B, 10);
    public Span<byte> OriginalTrainerTrash => Data.Slice(0x16, 7);

    public string Nickname
    {
        get => GetString(NicknameTrash);
        set => SetString(NicknameTrash, value, 10, StringConverterOption.None);
    }

    public string OT
    {
        get => GetString(OriginalTrainerTrash);
        set => SetString(OriginalTrainerTrash, value, 7, StringConverterOption.None);
    }

    public string GetString(ReadOnlySpan<byte> data)
        => StringConverter3.GetString(data, Japanese);
    public int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3.SetString(destBuffer, value, maxLength, Japanese, option);

    public bool IsShiny => (uint)(TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF)) < 8;
    public int Category => Caption / CountCaption;

    public void Clear()
    {
        Data.Clear();
        Data[0x0B] = 0xFF;
        Data[0x16] = 0xFF;
    }

    public static byte ConvertoToAbsolute(int index, int caption) => (byte)((index * CountCaption) + caption);
    public static byte ConvertToRelative(int index, int caption) => (byte)Math.Clamp(caption - (index * CountCaption), 0, CountCaption - 1);
    public int GetCaptionRelative(int index) => ConvertToRelative(index, Caption);
    public void SetCaptionRelative(int index, int value) => Caption = ConvertoToAbsolute(index, value);
    public bool IsValidCaption(int index) => Category == index;

    public static int GetFlagIndexContestStat(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, 4);
        return FlagIndexContestStat + index;
    }
}
