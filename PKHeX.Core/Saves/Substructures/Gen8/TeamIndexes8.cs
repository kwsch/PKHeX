using System;

namespace PKHeX.Core
{
    public sealed class TeamIndexes8 : SaveBlock
    {
        private const int TeamCount = 6;
        private const int NONE_SELECTED = -1;
        public readonly int[] TeamSlots = new int[TeamCount * 6];

        public TeamIndexes8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public void LoadBattleTeams()
        {
            if (!SAV.Exportable)
            {
                ClearBattleTeams();
                return;
            }

            for (int i = 0; i < TeamCount * 6; i++)
            {
                short val = BitConverter.ToInt16(Data, Offset + (i * 2));
                if (val < 0)
                {
                    TeamSlots[i] = NONE_SELECTED;
                    continue;
                }

                int box = val >> 8;
                int slot = val & 0xFF;
                int index = (SAV.BoxSlotCount * box) + slot;
                TeamSlots[i] = index & 0xFFFF;
            }
        }

        public void ClearBattleTeams()
        {
            for (int i = 0; i < TeamSlots.Length; i++)
                TeamSlots[i] = NONE_SELECTED;
        }

        public void UnlockAllTeams()
        {
            for (int i = 0; i < TeamCount; i++)
                SetIsTeamLocked(i, false);
        }

        public void SaveBattleTeams()
        {
            for (int i = 0; i < TeamCount * 6; i++)
            {
                int index = TeamSlots[i];
                if (index < 0)
                {
                    BitConverter.GetBytes((short)index).CopyTo(Data, Offset + (i * 2));
                    continue;
                }

                SAV.GetBoxSlotFromIndex(index, out var box, out var slot);
                int val = (box << 8) | slot;
                BitConverter.GetBytes((short)val).CopyTo(Data, Offset + (i * 2));
            }
        }

        public bool GetIsTeamLocked(int team) => true;

        public void SetIsTeamLocked(int team, bool value)
        {

        }
    }
}