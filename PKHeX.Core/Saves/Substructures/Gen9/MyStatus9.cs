using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x00));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x00), value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x02));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x02), value);
    }

    public int Game
    {
        get => Data[0x04];
        set => Data[0x04] = (byte)value;
    }

    public int Gender
    {
        get => Data[0x05];
        set => Data[0x05] = (byte)value;
    }

    // A6
    public int Language
    {
        get => Data[Offset + 0x07];
        set
        {
            if (value == Language)
                return;
            Data[Offset + 0x07] = (byte)value;

            // For runtime language, the game has different indexes (not even shifted like previous games, just different)
            var runtimeLanguage = GetRuntimeLanguage((LanguageID)value);
            SAV.SetValue(SaveBlockAccessor9SV.KGameLanguage, (int)runtimeLanguage); // Int32
        }
    }

    private static RuntimeLanguage GetRuntimeLanguage(LanguageID value) => value switch
    {
        LanguageID.Japanese => RuntimeLanguage.Japanese,
        LanguageID.English => RuntimeLanguage.English,
        LanguageID.Spanish => RuntimeLanguage.Spanish,
        LanguageID.German => RuntimeLanguage.German,
        LanguageID.French => RuntimeLanguage.French,
        LanguageID.Italian => RuntimeLanguage.Italian,
        LanguageID.Korean => RuntimeLanguage.Korean,
        LanguageID.ChineseS => RuntimeLanguage.ChineseS,
        LanguageID.ChineseT => RuntimeLanguage.ChineseT,
        _ => RuntimeLanguage.English, // Default to English
    };

    private enum RuntimeLanguage
    {
        Japanese = 0,
        English = 1,
        Spanish = 2,
        German = 3,
        French = 4,
        Italian = 5,
        Korean = 6,
        ChineseS = 7,
        ChineseT = 8,
    }

    private Span<byte> OT_Trash => Data.AsSpan(0x10, 0x1A);

    public string OT
    {
        get => SAV.GetString(OT_Trash);
        set => SAV.SetString(OT_Trash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public byte BirthMonth { get => Data[0x5A]; set => Data[0x5A] = value; }
    public byte BirthDay { get => Data[0x5B]; set => Data[0x5B] = value; }
}
