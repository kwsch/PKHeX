namespace PKHeX.Core
{
    /// <summary> Identification flair for what properties a <see cref="CheckResult"/> pertains to </summary>
    public enum CheckIdentifier
    {
        /// <summary>
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Moves"/>.
        /// </summary>
        Move,

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
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.EVs"/>.
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
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.OT_Name"/>, <see cref="PKM.TID"/>, or <see cref="PKM.SID"/>.
        /// </summary>
        Trainer,

        /// <summary>
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.IVs"/>.
        /// </summary>
        IVs,

        /// <summary>
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.Met_Level"/> or <see cref="PKM.CurrentLevel"/>.
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
        /// The <see cref="CheckResult"/> pertains to the <see cref="PKM.AltForm"/>.
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
        /// <remarks>This is used for parsing checks to ensure the <see cref="PKM"/> didn't debut on a future <see cref="PKM.GenNumber"/></remarks>
        /// </summary>
        GameOrigin,

        /// <summary>
        /// The CheckResult pertains to the <see cref="PKM.HeldItem"/>.
        /// </summary>
        HeldItem
    }
}
