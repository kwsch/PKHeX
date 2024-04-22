using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameTime6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw)
{
    public int ResumeYear { get => ReadInt32LittleEndian(Data[0x4..]); set => WriteInt32LittleEndian(Data[0x4..], value); }
    public int ResumeMonth { get => Data[0x8]; set => Data[0x8] = (byte)value; }
    public int ResumeDay { get => Data[0x9]; set => Data[0x9] = (byte)value; }
    public int ResumeHour { get => Data[0xB]; set => Data[0xB] = (byte)value; }
    public int ResumeMinute { get => Data[0xC]; set => Data[0xC] = (byte)value; }
    public int ResumeSeconds { get => Data[0xD]; set => Data[0xD] = (byte)value; }
    public uint SecondsToStart { get => ReadUInt32LittleEndian(Data[0x18..]); set => WriteUInt32LittleEndian(Data[0x18..], value); }
    public uint SecondsToFame { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], value); }
}
