using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PKHeX.Core
{
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
        public static bool AllowGBCartEra { get; set; }

        /// <summary>
        /// Setting to specify if an analysis should permit trading a Generation 1 origin file to Generation 2, then back. Useful for checking RBY Metagame rules.
        /// </summary>
        public static bool AllowGen1Tradeback { get; set; } = true;

        public static Severity NicknamedTrade { get; set; } = Severity.Invalid;
        public static Severity NicknamedMysteryGift { get; set; } = Severity.Fishy;
        public static Severity RNGFrameNotFound { get; set; } = Severity.Fishy;
        public static Severity Gen7TransferStarPID { get; set; } = Severity.Fishy;
        public static Severity Gen8MemoryLocationTextVariable { get; set; } = Severity.Fishy;
        public static Severity Gen8TransferTrackerNotPresent { get; set; } = Severity.Fishy;
        public static Severity NicknamedAnotherSpecies { get; set; } = Severity.Fishy;

        public static IReadOnlyList<string> MoveStrings = Util.GetMovesList(GameLanguage.DefaultLanguage);
        public static IReadOnlyList<string> SpeciesStrings = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
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
        /// <param name="pkm">Data being checked</param>
        /// <returns>True if Crystal data is allowed</returns>
        public static bool AllowGen2Crystal(PKM pkm) => !pkm.Korean;

        /// <summary>
        /// Checks to see if the Move Reminder (Relearner) is available.
        /// </summary>
        /// <remarks> Pokemon Stadium 2 was never released in Korea.</remarks>
        /// <param name="pkm">Data being checked</param>
        /// <returns>True if Crystal data is allowed</returns>
        public static bool AllowGen2MoveReminder(PKM pkm) => !pkm.Korean && AllowGBCartEra;

        internal static bool IsFromActiveTrainer(PKM pkm) => ActiveTrainer.IsFromTrainer(pkm);

        /// <summary>
        /// Initializes certain settings
        /// </summary>
        /// <param name="sav">Newly loaded save file</param>
        /// <returns>Save file is Physical GB cartridge save file (not Virtual Console)</returns>
        public static bool InitFromSaveFileData(SaveFile sav)
        {
            ActiveTrainer = sav;
            if (sav.Generation >= 3)
                return AllowGBCartEra = false;
            bool vc = !sav.State.Exportable || (sav.Metadata.FileName?.EndsWith("dat") ?? false); // default to true for non-exportable
            return AllowGBCartEra = !vc; // physical cart selected
        }

        public static void InitFromSettings(LegalitySettings settings)
        {
            AllowGen1Tradeback = settings.AllowGen1Tradeback;
            NicknamedTrade = settings.NicknamedTrade;
            NicknamedMysteryGift = settings.NicknamedMysteryGift;
            RNGFrameNotFound = settings.RNGFrameNotFound;
            Gen7TransferStarPID = settings.Gen7TransferStarPID;
            Gen8MemoryLocationTextVariable = settings.Gen8MemoryLocationTextVariable;
            Gen8TransferTrackerNotPresent = settings.Gen8TransferTrackerNotPresent;
            NicknamedAnotherSpecies = settings.NicknamedAnotherSpecies;
        }
    }

    [Serializable]
    public sealed class LegalitySettings
    {
        [Description("GB: Allow Generation 2 tradeback learnsets for PK1 formats. Disable when checking RBY Metagame rules.")]
        public bool AllowGen1Tradeback { get; set; } = true;

        [Description("Severity to flag a Legality Check if it is a nicknamed In-Game Trade the player cannot normally nickname.")]
        public Severity NicknamedTrade { get; set; } = Severity.Invalid;

        [Description("Severity to flag a Legality Check if it is a nicknamed Mystery Gift the player cannot normally nickname.")]
        public Severity NicknamedMysteryGift { get; set; } = Severity.Fishy;

        [Description("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match.")]
        public Severity RNGFrameNotFound { get; set; } = Severity.Fishy;

        [Description("Severity to flag a Legality Check if Pokémon from Gen1/2 has a Star Shiny PID.")]
        public Severity Gen7TransferStarPID { get; set; } = Severity.Fishy;

        [Description("Severity to flag a Legality Check if a Gen8 Location Memory text variable is present.")]
        public Severity Gen8MemoryLocationTextVariable { get; set; } = Severity.Fishy;

        [Description("Severity to flag a Legality Check if the HOME Tracker is Missing")]
        public Severity Gen8TransferTrackerNotPresent { get; set; } = Severity.Fishy;

        [Description("Severity to flag a Legality Check if Pokémon has a Nickname matching another Species.")]
        public Severity NicknamedAnotherSpecies { get; set; } = Severity.Fishy;
    }
}
