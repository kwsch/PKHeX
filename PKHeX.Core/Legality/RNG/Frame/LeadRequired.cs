namespace PKHeX.Core;

/// <summary>
/// Indicates the requirement of the player's lead Pokémon, first sent out when starting a battle.
/// </summary>
public enum LeadRequired : byte
{
    /// <summary> No Lead ability effect is present, or is not checked for this type of frame. </summary>
    None = 0,

    /// <summary> <see cref="Ability.Synchronize"/> </summary>
    Synchronize,
    /// <summary> <see cref="Ability.CuteCharm"/> </summary>
    CuteCharm,
    /// <summary> <see cref="Ability.Static"/> or <see cref="Ability.MagnetPull"/> </summary>
    StaticMagnet,
    /// <summary> Internal tag -- passing this will abort the encounter. </summary>
    IntimidateKeenEye,
    /// <summary> <see cref="Ability.Pressure"/> or <see cref="Ability.Hustle"/> or <see cref="Ability.VitalSpirit"/> </summary>
    PressureHustleSpirit,
    /// <summary> <see cref="Ability.SuctionCups"/> or <see cref="Ability.StickyHold"/> </summary>
    SuctionCups,

    /// <summary> Internal tag </summary>
    Fail = 1 << 7,

    /// <summary> <inheritdoc cref="Synchronize"/> </summary>
    SynchronizeFail = Synchronize | Fail,
    /// <summary> <inheritdoc cref="CuteCharm"/> </summary>
    CuteCharmFail = CuteCharm | Fail,
    /// <summary> <inheritdoc cref="StaticMagnet"/> </summary>
    StaticMagnetFail = StaticMagnet | Fail,
    /// <summary> <see cref="Ability.Intimidate"/> or <see cref="Ability.KeenEye"/> failed to activate. </summary>
    IntimidateKeenEyeFail = IntimidateKeenEye | Fail,
    /// <summary> <inheritdoc cref="PressureHustleSpirit"/> </summary>
    PressureHustleSpiritFail = PressureHustleSpirit | Fail,

    // Suction cups failing will fail to yield the encounter since it's the first call.

    /// <summary> Sentinel value for invalid/impossible to yield lead conditions. </summary>
    Invalid = byte.MaxValue,
}
