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
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[0x02..]);
        set => WriteUInt16LittleEndian(Data[0x02..], value);
    }

    public byte Game
    {
        get => Data[0x04];
        set => Data[0x04] = value;
    }

    public byte Gender
    {
        get => Data[0x05];
        set => Data[0x05] = value;
    }

    // A6
    public int Language
    {
        get => Data[0x07];
        set
        {
            if (value == Language)
                return;
            Data[0x07] = (byte)value;

            // For runtime language, the game has different indexes (not even shifted like previous games, just different)
            RuntimeLanguageId = RuntimeLanguageExtensions.GetRuntimeLanguage((LanguageID)value);
        }
    }

    public RuntimeLanguage RuntimeLanguageId
    {
        get => (RuntimeLanguage)SAV.GetValue<int>(SaveBlockAccessor9SV.KGameLanguage);
        set => SAV.SetValue(SaveBlockAccessor9SV.KGameLanguage, (int)value);
    }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0x10, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public byte BirthMonth { get => Data[0x5A]; set => Data[0x5A] = value; }
    public byte BirthDay { get => Data[0x5B]; set => Data[0x5B] = value; }
}
