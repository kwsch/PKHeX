using System;

namespace PKHeX.Core
{
    public sealed class BattleTree7 : SaveBlock
    {
        public BattleTree7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public BattleTree7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public int GetTreeStreak(int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            var offset = GetStreakOffset(battletype, super, max);
            return BitConverter.ToUInt16(Data, Offset + offset);
        }

        public void SetTreeStreak(int value, int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            if (value > ushort.MaxValue)
                value = ushort.MaxValue;

            var offset = GetStreakOffset(battletype, super, max);
            BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + offset);
        }

        private static int GetStreakOffset(int battletype, bool super, bool max)
        {
            int offset = 8 * battletype;
            if (super)
                offset += 2;
            if (max)
                offset += 4;
            return offset;
        }
    }
}