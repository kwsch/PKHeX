using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Settings for Parsing Legality
/// </summary>
/// <remarks><see cref="LegalityAnalysis"/></remarks>
public static class ParseSettings
{
    internal static ITrainerInfo ActiveTrainer { get; set; } = new SimpleTrainerInfo(GameVersion.Any) { OT = string.Empty, Language = -1 };

    /// <summary>
    /// Master settings configuration for legality analysis.
    /// </summary>
    /// <remarks>Allows configuring severities away from the default settings for users who want to deviate.</remarks>
    public static LegalitySettings Settings { get; set; } = new();

    /// <summary>
    /// Setting to specify if an analysis should permit data sourced from the physical cartridge era of Game Boy games.
    /// </summary>
    /// <remarks>If false, indicates to use Virtual Console rules (which are transferable to Gen7+)</remarks>
    public static bool AllowGBCartEra { private get; set; }

    /// <summary>
    /// Setting to specify if an analysis should permit trading a Generation 1 origin file to Generation 2, then back. Useful for checking RBY Metagame rules.
    /// </summary>
    public static bool AllowGen1Tradeback => Settings.Tradeback.AllowGen1Tradeback;

    public static void Initialize(LegalitySettings settings)
    {
        Settings = settings;
    }

    public static IReadOnlyList<string> MoveStrings { get; private set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
    public static IReadOnlyList<string> SpeciesStrings { get; private set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
    public static string GetMoveName(ushort move) => move >= MoveStrings.Count ? LegalityCheckStrings.L_AError : MoveStrings[move];

    public static void ChangeLocalizationStrings(IReadOnlyList<string> moves, IReadOnlyList<string> species)
    {
        SpeciesStrings = species;
        MoveStrings = moves;
    }

    /// <summary>
    /// Checks to see if Crystal is available to visit/originate from.
    /// </summary>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2Crystal(PKM pk) => !pk.Korean;

    /// <summary>
    /// Checks to see if the Move Reminder (Relearner) is available.
    /// </summary>
    /// <remarks> Pokémon Stadium 2 was never released in Korea.</remarks>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2MoveReminder(PKM pk) => !pk.Korean && AllowGBStadium2;

    public static bool AllowGen2OddEgg(PKM pk) => !pk.Japanese || AllowGBCartEra;

    public static bool AllowGBVirtualConsole3DS => !AllowGBCartEra;
    public static bool AllowGBEraEvents => AllowGBCartEra;
    public static bool AllowGBStadium2 => AllowGBCartEra;

    internal static bool IsFromActiveTrainer(PKM pk) => ActiveTrainer.IsFromTrainer(pk);

    /// <summary>
    /// Initializes certain settings
    /// </summary>
    /// <param name="sav">Newly loaded save file</param>
    /// <returns>Save file is Physical GB cartridge save file (not Virtual Console)</returns>
    public static bool InitFromSaveFileData(SaveFile sav)
    {
        ActiveTrainer = sav;
        return AllowGBCartEra = sav switch
        {
            SAV1 { IsVirtualConsole: true } => false,
            SAV2 { IsVirtualConsole: true } => false,
            { Generation: 1 or 2 } => true,
            _ => false,
        };
    }

    internal static bool IgnoreTransferIfNoTracker => Settings.HOMETransfer.HOMETransferTrackerNotPresent == Severity.Invalid;
}
