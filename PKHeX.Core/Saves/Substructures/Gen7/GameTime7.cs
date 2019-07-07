using System;

namespace PKHeX.Core
{
    public sealed class GameTime7 : SaveBlock
    {
        public GameTime7(SaveFile sav, int offset) : base(sav) => Offset = offset;
        public int ResumeYear { get => BitConverter.ToInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }
        public int ResumeMonth { get => Data[Offset + 0x8]; set => Data[Offset + 0x8] = (byte)value; }
        public int ResumeDay { get => Data[Offset + 0x9]; set => Data[Offset + 0x9] = (byte)value; }
        public int ResumeHour { get => Data[Offset + 0xB]; set => Data[Offset + 0xB] = (byte)value; }
        public int ResumeMinute { get => Data[Offset + 0xC]; set => Data[Offset + 0xC] = (byte)value; }
        public int ResumeSeconds { get => Data[Offset + 0xD]; set => Data[Offset + 0xD] = (byte)value; }
        public uint SecondsToStart { get => BitConverter.ToUInt32(Data, Offset + 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x28); }
        public uint SecondsToFame { get => BitConverter.ToUInt32(Data, Offset + 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x30); }

        public ulong AlolaTime { get => BitConverter.ToUInt64(Data, Offset + 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x48); }
    }
}