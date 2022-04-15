using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class GameTime6 : SaveBlock<SAV6>
    {
        public GameTime6(SAV6 sav, int offset) : base(sav) => Offset = offset;

        public int ResumeYear { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x4), value); }
        public int ResumeMonth { get => Data[Offset + 0x8]; set => Data[Offset + 0x8] = (byte)value; }
        public int ResumeDay { get => Data[Offset + 0x9]; set => Data[Offset + 0x9] = (byte)value; }
        public int ResumeHour { get => Data[Offset + 0xB]; set => Data[Offset + 0xB] = (byte)value; }
        public int ResumeMinute { get => Data[Offset + 0xC]; set => Data[Offset + 0xC] = (byte)value; }
        public int ResumeSeconds { get => Data[Offset + 0xD]; set => Data[Offset + 0xD] = (byte)value; }
        public uint SecondsToStart { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x18)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x18), value); }
        public uint SecondsToFame { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x20)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x20), value); }
    }
}