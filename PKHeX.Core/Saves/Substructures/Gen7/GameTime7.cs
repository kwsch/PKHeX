using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class GameTime7 : SaveBlock<SAV7>
    {
        public GameTime7(SAV7 sav, int offset) : base(sav) => Offset = offset;
        public int ResumeYear { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x4), value); }
        public int ResumeMonth { get => Data[Offset + 0x8]; set => Data[Offset + 0x8] = (byte)value; }
        public int ResumeDay { get => Data[Offset + 0x9]; set => Data[Offset + 0x9] = (byte)value; }
        public int ResumeHour { get => Data[Offset + 0xB]; set => Data[Offset + 0xB] = (byte)value; }
        public int ResumeMinute { get => Data[Offset + 0xC]; set => Data[Offset + 0xC] = (byte)value; }
        public int ResumeSeconds { get => Data[Offset + 0xD]; set => Data[Offset + 0xD] = (byte)value; }
        public uint SecondsToStart { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x28)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x28), value); }
        public uint SecondsToFame { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x30)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x30), value); }

        public ulong AlolaTime { get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x48)); set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x48), value); }
    }
}