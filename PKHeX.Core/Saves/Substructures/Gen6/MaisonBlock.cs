using System;

namespace PKHeX.Core
{
    public sealed class MaisonBlock : SaveBlock
    {
        public MaisonBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public MaisonBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        // 5 * [u16*4: normal,super,normalStreak,superStreak]
        public const int MaisonStatCount = 20;

        public ushort GetMaisonStat(int index) => BitConverter.ToUInt16(Data, Offset + 0x1C0 + (2 * index));
        public void SetMaisonStat(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C0 + (2 * index));

        private static int GetMaisonStatIndex(BattleStyle6 type, bool streak, bool super) => ((int)type << 2) | (streak ? 2 : 0) | (super ? 1 : 0);
        public ushort GetMaisonStat(BattleStyle6 type, bool streak, bool super) => GetMaisonStat(GetMaisonStatIndex(type, streak, super));
        public void SetMaisonStat(BattleStyle6 type, bool streak, bool super, ushort value) => SetMaisonStat(GetMaisonStatIndex(type, streak, super), value);
    }
}
