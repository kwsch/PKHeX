using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public static class ParseSettings
{
    internal static ITrainerInfo ActiveTrainer { get; set; } = new SimpleTrainerInfo(GameVersion.Any) { OT = string.Empty, Language = -1 };

    /// <summary>
    /// Toggles whether or not the word filter should be used when checking the data.
    /// </summary>
    public static bool CheckWordFilter { get; set; } = true;

    /// <summary>
    /// Setting to specify if an analysis should permit data sourced from the physical cartridge era of GameBoy games.
    /// </summary>
    /// <remarks>If false, indicates to use Virtual Console rules (which are transferable to Gen7+)</remarks>
    public static bool AllowGBCartEra { get; set; }

    /// <summary>
    /// Setting to specify if an analysis should permit trading a Generation 1 origin file to Generation 2, then back. Useful for checking RBY Metagame rules.
    /// </summary>
    public static bool AllowGen1Tradeback { get; set; } = true;

    public static Severity NicknamedTrade { get; private set; } = Severity.Invalid;
    public static Severity NicknamedMysteryGift { get; private set; } = Severity.Fishy;
    public static Severity RNGFrameNotFound { get; private set; } = Severity.Fishy;
    public static Severity Gen7TransferStarPID { get; private set; } = Severity.Fishy;
    public static Severity Gen8MemoryMissingHT { get; private set; } = Severity.Fishy;
    public static Severity Gen8TransferTrackerNotPresent { get; private set; } = Severity.Fishy;
    public static Severity NicknamedAnotherSpecies { get; private set; } = Severity.Fishy;
    public static Severity ZeroHeightWeight { get; private set; } = Severity.Fishy;
    public static Severity CurrentHandlerMismatch { get; private set; } = Severity.Invalid;
    public static bool CheckActiveHandler { get; set; }

    public static IReadOnlyList<string> MoveStrings { get; private set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
    public static IReadOnlyList<string> SpeciesStrings { get; private set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
    public static string GetMoveName(int move) => (uint)move >= MoveStrings.Count ? LegalityCheckStrings.L_AError : MoveStrings[move];
    public static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => (uint)m >= MoveStrings.Count ? LegalityCheckStrings.L_AError : MoveStrings[m]);

    public static void ChangeLocalizationStrings(IReadOnlyList<string> moves, IReadOnlyList<string> species)
    {
        SpeciesStrings = species;
        MoveStrings = moves;
    }

    /// <summary>
    /// Checks to see if Crystal is available to visit/originate from.
    /// </summary>
    /// <remarks>Pokemon Crystal was never released in Korea.</remarks>
    /// <param name="Korean">Korean data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2Crystal(bool Korean) => !Korean;

    /// <summary>
    /// Checks to see if Crystal is available to visit/originate from.
    /// </summary>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2Crystal(PKM pk) => !pk.Korean;

    /// <summary>
    /// Checks to see if the Move Reminder (Relearner) is available.
    /// </summary>
    /// <remarks> Pokemon Stadium 2 was never released in Korea.</remarks>
    /// <param name="pk">Data being checked</param>
    /// <returns>True if Crystal data is allowed</returns>
    public static bool AllowGen2MoveReminder(PKM pk) => !pk.Korean && AllowGBCartEra;

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

    internal static bool IgnoreTransferIfNoTracker => Gen8TransferTrackerNotPresent == Severity.Invalid;

    public static void InitFromSettings(IParseSettings settings)
    {
        CheckWordFilter = settings.CheckWordFilter;
        AllowGen1Tradeback = settings.AllowGen1Tradeback;
        NicknamedTrade = settings.NicknamedTrade;
        NicknamedMysteryGift = settings.NicknamedMysteryGift;
        RNGFrameNotFound = settings.RNGFrameNotFound;
        Gen7TransferStarPID = settings.Gen7TransferStarPID;
        Gen8TransferTrackerNotPresent = settings.Gen8TransferTrackerNotPresent;
        Gen8MemoryMissingHT = settings.Gen8MemoryMissingHT;
        NicknamedAnotherSpecies = settings.NicknamedAnotherSpecies;
        ZeroHeightWeight = settings.ZeroHeightWeight;
        CurrentHandlerMismatch = settings.CurrentHandlerMismatch;
        CheckActiveHandler = settings.CheckActiveHandler;
    }
}

public interface IParseSettings
{
    bool CheckWordFilter { get; }
    bool CheckActiveHandler { get; }
    bool AllowGen1Tradeback { get; }

    Severity NicknamedTrade { get; }
    Severity NicknamedMysteryGift { get; }
    Severity RNGFrameNotFound { get; }
    Severity Gen7TransferStarPID { get; }
    Severity Gen8MemoryMissingHT { get; }
    Severity Gen8TransferTrackerNotPresent { get; }
    Severity NicknamedAnotherSpecies { get; }
    Severity ZeroHeightWeight { get; }
    Severity CurrentHandlerMismatch { get; }
}

public interface IBulkAnalysisSettings
{
    bool CheckActiveHandler { get; }
}
