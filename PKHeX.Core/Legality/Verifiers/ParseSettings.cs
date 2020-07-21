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
        public static bool AllowGen1Tradeback { get; set; }

        public static Severity NicknamedTrade { get; set; } = Severity.Invalid;
        public static Severity NicknamedMysteryGift { get; set; } = Severity.Fishy;
        public static Severity RNGFrameNotFound { get; set; } = Severity.Fishy;
        public static Severity Gen8MemoryLocationTextVariable { get; set; } = Severity.Fishy;
        public static Severity Gen8TransferTrackerNotPresent { get; set; } = Severity.Fishy;

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
            bool vc = !sav.Exportable || (sav.FileName?.EndsWith("dat") ?? false); // default to true for non-exportable
            return AllowGBCartEra = !vc; // physical cart selected
        }
    }
}
