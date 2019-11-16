using System;

namespace PKHeX.Core
{
    public sealed class GameTime8 : SaveBlock
    {
        public GameTime8(SaveFile sav, int offset) : base(sav) => Offset = offset;
        public uint SecondsToStart { get => BitConverter.ToUInt32(Data, 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, 0x28); }
        public uint SecondsToFame { get => BitConverter.ToUInt32(Data, 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, 0x30); }
    }
}