using System;

namespace PKHeX.Core
{
    [Flags]
    public enum StorageSlotFlag
    {
        None,

        Party = 1 << 0,
        Party1 = Party << 0,
        Party2 = Party << 1,
        Party3 = Party << 2,
        Party4 = Party << 3,
        Party5 = Party << 4,
        Party6 = Party << 5,

        BattleTeam = 1 << 6,
        BattleTeam1 = BattleTeam << 0,
        BattleTeam2 = BattleTeam << 1,
        BattleTeam3 = BattleTeam << 2,
        BattleTeam4 = BattleTeam << 3,
        BattleTeam5 = BattleTeam << 4,
        BattleTeam6 = BattleTeam << 5,

        Starter = 1 << 29,
        Locked = 1 << 30,
    }

    public static class StorageSlotFlagExtensions
    {
        public static bool HasFlagFast(this StorageSlotFlag value, StorageSlotFlag flag) => (value & flag) != 0;

        public static bool IsOverwriteProtected(this StorageSlotFlag value)
        {
            if (value.HasFlagFast(StorageSlotFlag.Locked))
                return true;

            if (value.HasFlagFast(StorageSlotFlag.Starter))
                return true;

            return value.IsBattleTeam() >= 0;
        }

        public static int IsBattleTeam(this StorageSlotFlag value)
        {
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam1))
                return 0;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam2))
                return 1;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam3))
                return 2;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam4))
                return 3;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam5))
                return 4;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam6))
                return 5;

            return -1;
        }

        public static int IsParty(this StorageSlotFlag value)
        {
            if (value.HasFlagFast(StorageSlotFlag.Party1))
                return 0;
            if (value.HasFlagFast(StorageSlotFlag.Party2))
                return 1;
            if (value.HasFlagFast(StorageSlotFlag.Party3))
                return 2;
            if (value.HasFlagFast(StorageSlotFlag.Party4))
                return 3;
            if (value.HasFlagFast(StorageSlotFlag.Party5))
                return 4;
            if (value.HasFlagFast(StorageSlotFlag.Party6))
                return 5;

            return -1;
        }
    }
}
