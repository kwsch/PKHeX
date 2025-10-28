using System;

namespace PKHeX.Core;

/// <summary>
/// Flags describing special attributes for a <see cref="PKM"/> based on its origin from the parent <see cref="SaveFile"/>.
/// </summary>
/// <remarks>If <see cref="None"/>, then it's not a special slot.</remarks>
[Flags]
public enum StorageSlotSource
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

public static class StorageSlotSourceExtensions
{
    /// <summary>
    /// Checks to see if the <see cref="StorageSlotSource"/> prevents the corresponding slot from being overwritten.
    /// </summary>
    /// <param name="value">Flag value</param>
    /// <returns>True if write protected</returns>
    public static bool IsOverwriteProtected(this StorageSlotSource value)
    {
        if (value.HasFlag(StorageSlotSource.Locked))
            return true;

        if (value.HasFlag(StorageSlotSource.Starter))
            return true;

        return value.IsBattleTeam() >= 0;
    }

    /// <summary>
    /// Gets the Battle Team ID the <see cref="value"/> belongs to
    /// </summary>
    /// <param name="value">Flag value</param>
    /// <returns>Battle Team ID if valid, -1 otherwise.</returns>
    public static int IsBattleTeam(this StorageSlotSource value)
    {
        if (value.HasFlag(StorageSlotSource.BattleTeam1))
            return 0;
        if (value.HasFlag(StorageSlotSource.BattleTeam2))
            return 1;
        if (value.HasFlag(StorageSlotSource.BattleTeam3))
            return 2;
        if (value.HasFlag(StorageSlotSource.BattleTeam4))
            return 3;
        if (value.HasFlag(StorageSlotSource.BattleTeam5))
            return 4;
        if (value.HasFlag(StorageSlotSource.BattleTeam6))
            return 5;

        return -1;
    }

    /// <summary>
    /// Gets the Party Slot Index the <see cref="value"/> belongs to
    /// </summary>
    /// <param name="value">Flag value</param>
    /// <returns>[0,5] if valid, -1 otherwise.</returns>
    public static int IsParty(this StorageSlotSource value)
    {
        if (value.HasFlag(StorageSlotSource.Party1))
            return 0;
        if (value.HasFlag(StorageSlotSource.Party2))
            return 1;
        if (value.HasFlag(StorageSlotSource.Party3))
            return 2;
        if (value.HasFlag(StorageSlotSource.Party4))
            return 3;
        if (value.HasFlag(StorageSlotSource.Party5))
            return 4;
        if (value.HasFlag(StorageSlotSource.Party6))
            return 5;

        return -1;
    }
}
