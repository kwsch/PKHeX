using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Indicates the requirement of the player's lead Pokémon, first sent out when starting a battle.
    /// </summary>
    [Flags]
    public enum LeadRequired
    {
        /// <summary> No Lead ability effect is present, or is not checked for this type of frame. </summary>
        None = 0,

        /// <summary> <see cref="Ability.CuteCharm"/> </summary>
        CuteCharm = 1 << 0,
        /// <summary> <see cref="Ability.Synchronize"/> </summary>
        Synchronize = 1 << 1,

        // Slot Modifiers
        /// <summary> <see cref="Ability.Static"/> or <see cref="Ability.MagnetPull"/> </summary>
        StaticMagnet = 1 << 2,

        // Level Modifiers
        /// <summary> <see cref="Ability.Intimidate"/> or <see cref="Ability.KeenEye"/> </summary>
        IntimidateKeenEye = 1 << 3,
        /// <summary> <see cref="Ability.Pressure"/> or <see cref="Ability.Hustle"/> or <see cref="Ability.VitalSpirit"/> </summary>
        PressureHustleSpirit = 1 << 4,
        /// <summary> <see cref="Ability.SuctionCups"/> </summary>
        SuctionCups = 1 << 5,

        // Compatibility Flags
        UsesLevelCall = 1 << 6,
        Fail = 1 << 7,

        /// <summary> <inheritdoc cref="CuteCharm"/> </summary>
        CuteCharmFail = CuteCharm | Fail,
        /// <summary> <inheritdoc cref="Synchronize"/> </summary>
        SynchronizeFail = Synchronize | Fail,
        /// <summary> <inheritdoc cref="StaticMagnet"/> </summary>
        StaticMagnetFail = StaticMagnet | Fail,
        /// <summary> <inheritdoc cref="PressureHustleSpirit"/> </summary>
        PressureHustleSpiritFail = PressureHustleSpirit | Fail,

        AllFlags = UsesLevelCall | Fail,
    }

    public static partial class Extensions
    {
        internal static bool IsLevelOrSlotModified(this LeadRequired Lead) => Lead.RemoveFlags() > LeadRequired.Synchronize;
        internal static LeadRequired RemoveFlags(this LeadRequired Lead) => Lead & ~LeadRequired.AllFlags;
    }
}
