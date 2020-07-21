using System;

namespace PKHeX.Core
{
    [Flags]
    public enum LeadRequired
    {
        None = 0,
        CuteCharm = 1 << 0,
        Synchronize = 1 << 1,

        // Slot Modifiers
        StaticMagnet = 1 << 2,

        // Level Modifiers
        IntimidateKeenEye = 1 << 3,
        PressureHustleSpirit = 1 << 4,
        SuctionCups = 1 << 5,

        // Compatibility Flags
        UsesLevelCall = 1 << 6,
        Fail = 1 << 7,

        CuteCharmFail = CuteCharm | Fail,
        SynchronizeFail = Synchronize | Fail,
        StaticMagnetFail = StaticMagnet | Fail,
        PressureHustleSpiritFail = PressureHustleSpirit | Fail,

        AllFlags = UsesLevelCall | Fail,
    }

    public static partial class Extensions
    {
        internal static bool IsLevelOrSlotModified(this LeadRequired Lead) => Lead.RemoveFlags() > LeadRequired.Synchronize;
        internal static LeadRequired RemoveFlags(this LeadRequired Lead) => Lead & ~LeadRequired.AllFlags;
    }
}
