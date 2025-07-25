namespace PKHeX.Core;

/// <summary> Identification flair for what properties a <see cref="CheckResult"/> pertains to </summary>
public enum CheckIdentifier : byte
{
    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Moves"/>.
    /// </summary>
    CurrentMove,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.RelearnMoves"/>.
    /// </summary>
    RelearnMove,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/>'s matched encounter information.
    /// </summary>
    Encounter,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.IsShiny"/> status.
    /// </summary>
    Shiny,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.EncryptionConstant"/>.
    /// </summary>
    EC,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.PID"/>.
    /// </summary>
    PID,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Gender"/>.
    /// </summary>
    Gender,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="EffortValues"/>.
    /// </summary>
    EVs,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Language"/>.
    /// </summary>
    Language,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Nickname"/>.
    /// </summary>
    Nickname,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.OriginalTrainerName"/>, <see cref="PKM.TID16"/>, or <see cref="PKM.SID16"/>.
    /// </summary>
    Trainer,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.IVs"/>.
    /// </summary>
    IVs,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.MetLevel"/> or <see cref="PKM.CurrentLevel"/>.
    /// </summary>
    Level,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Ball"/>.
    /// </summary>
    Ball,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> memory data.
    /// </summary>
    Memory,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> geography data.
    /// </summary>
    Geography,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Form"/>.
    /// </summary>
    Form,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.IsEgg"/> status.
    /// </summary>
    Egg,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> miscellaneous properties.
    /// </summary>
    Misc,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.FatefulEncounter"/>.
    /// </summary>
    Fateful,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> ribbon data.
    /// </summary>
    Ribbon,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> super training data.
    /// </summary>
    Training,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Ability"/>.
    /// </summary>
    Ability,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> evolution chain relative to the matched encounter.
    /// </summary>
    Evolution,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Nature"/>.
    /// </summary>
    Nature,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/>'s <see cref="PKM.Version"/> compatibility.
    /// <remarks>This is used for parsing checks to ensure the <see cref="PKM"/> didn't debut on a future <see cref="PKM.Generation"/></remarks>
    /// </summary>
    GameOrigin,

    /// <summary>
    /// The CheckResult pertains to the <see cref="PKM.HeldItem"/>.
    /// </summary>
    HeldItem,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> <see cref="IRibbonSetMark8"/>.
    /// </summary>
    RibbonMark,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="IGanbaru"/> values.
    /// </summary>
    GVs,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to <see cref="IAppliedMarkings"/>.
    /// </summary>
    Marking,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> <see cref="IAwakened"/> values.
    /// </summary>
    AVs,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to string <see cref="TrashBytes"/>.
    /// </summary>
    TrashBytes,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the <see cref="PKM"/> <see cref="StorageSlotType"/>.
    /// </summary>
    SlotType,

    /// <summary>
    /// The <see cref="CheckResult"/> pertains to the Current Handler (not OT) of the <see cref="PKM"/> data.
    /// </summary>
    Handler,
}
