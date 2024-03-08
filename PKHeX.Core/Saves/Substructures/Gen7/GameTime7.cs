using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameTime7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    public int ResumeYear { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x04..], value); }
    public int ResumeMonth { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public int ResumeDay { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
    public int ResumeHour { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public int ResumeMinute { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public int ResumeSeconds { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public uint SecondsToStart { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], value); }
    public uint SecondsToFame { get => ReadUInt32LittleEndian(Data[0x30..]); set => WriteUInt32LittleEndian(Data[0x30..], value); }

    public ulong AlolaTime { get => ReadUInt64LittleEndian(Data[0x48..]); set => WriteUInt64LittleEndian(Data[0x48..], value); }
}
