using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public uint ID32 { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    public ushort TID16 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }

    public byte Game { get => Data[0x04]; set => Data[0x04] = value; }
    public byte Gender { get => Data[0x05]; set => Data[0x05] = value; }

    public int Language { get => Data[0x07]; set => Data[0x07] = (byte)value; }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0x10, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public byte BirthMonth { get => Data[0x5A]; set => Data[0x5A] = value; }
    public byte BirthDay { get => Data[0x5B]; set => Data[0x5B] = value; }

    public float MegaGaugePercent { get => ReadSingleLittleEndian(Data[0x68..]); set => WriteSingleLittleEndian(Data[0x68..], value); }

    public uint HP { get => ReadUInt32LittleEndian(Data[0x70..]); set => WriteUInt32LittleEndian(Data[0x70..], value); }
    public bool WasBirthdayGiven { get => Data[0x74] != 0; set => Data[0x74] = (byte)(value ? 1 : 0); } // un-setting allows to provide birthday day/month again
    public bool ObservedBirthday { get => Data[0x75] != 0; set => Data[0x75] = (byte)(value ? 1 : 0); }
    public ushort ObservedBirthdayYear { get => ReadUInt16LittleEndian(Data[0x76..]); set => WriteUInt16LittleEndian(Data[0x76..], value); }
}
