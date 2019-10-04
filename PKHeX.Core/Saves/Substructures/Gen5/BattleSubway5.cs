using System;

namespace PKHeX.Core
{
    public sealed class BattleSubway5 : SaveBlock
    {
        public BattleSubway5(SAV5 sav, int offset) : base(sav) => Offset = offset;

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset);
        }
    }
}