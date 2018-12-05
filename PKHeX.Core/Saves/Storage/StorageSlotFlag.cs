using System;

namespace PKHeX.Core
{
    [Flags]
    public enum StorageSlotFlag
    {
        None,
        Party1 = 1 << 0,
        Party2 = 1 << 1,
        Party3 = 1 << 2,
        Party4 = 1 << 3,
        Party5 = 1 << 4,
        Party6 = 1 << 5,
        BattleTeam1 = 1 << 6,
        BattleTeam2 = 1 << 7,
        BattleTeam3 = 1 << 8,
        BattleTeam4 = 1 << 9,
        BattleTeam5 = 1 << 10,
        BattleTeam6 = 1 << 11,

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

            return value.IsBattleTeam();
        }

        public static bool IsBattleTeam(this StorageSlotFlag value)
        {
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam1))
                return true;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam2))
                return true;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam3))
                return true;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam4))
                return true;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam5))
                return true;
            if (value.HasFlagFast(StorageSlotFlag.BattleTeam6))
                return true;

            return false;
        }
    }
}
