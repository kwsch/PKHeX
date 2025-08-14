namespace PKHeX.Core;

public sealed class EntityConverterSettings
{
    [LocalizedDescription("Allow PKM file conversion paths that are not possible via official methods. Individual properties will be copied sequentially.")]
    public EntityCompatibilitySetting AllowIncompatibleConversion { get; set; } = EntityCompatibilitySetting.DisallowIncompatible;

    [LocalizedDescription("Allow PKM file conversion paths to guess the legal original encounter data that is not stored in the format that it was converted from.")]
    public EntityRejuvenationSetting AllowGuessRejuvenateHOME { get; set; } = EntityRejuvenationSetting.MissingDataHOME;

    [LocalizedDescription("Default version to set when transferring from Generation 1 3DS Virtual Console to Generation 7.")]
    public GameVersion VirtualConsoleSourceGen1 { get; set; } = GameVersion.RD;

    [LocalizedDescription("Default version to set when transferring from Generation 2 3DS Virtual Console to Generation 7.")]
    public GameVersion VirtualConsoleSourceGen2 { get; set; } = GameVersion.SI;

    [LocalizedDescription("Retain the Met Date when transferring from Generation 4 to Generation 5.")]
    public bool RetainMetDateTransfer45 { get; set; }
}
