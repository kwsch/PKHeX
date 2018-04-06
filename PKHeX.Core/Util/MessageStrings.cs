// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PKHeX.Core
{
    /// <summary>
    /// Generic Message Strings used for messages shown to the user.
    /// </summary>
    public static class MessageStrings
    {
        #region Generic Program Messages

        public static string MsgProgramRestart { get; set; } = "Please restart the program.";
        public static string MsgProgramIllegalModeBehave { get; set; } = "Please behave.";
        public static string MsgProgramIllegalModeActive { get; set; } = "Illegal mode activated.";
        public static string MsgProgramUpdateAvailable { get; set; } = "New Update Available!";
        public static string MsgProgramCloseUnsaved { get; set; } = "Any unsaved changes will be lost.";
        public static string MsgProgramCloseConfirm { get; set; } = "Are you sure you want to close PKHeX?";
        public static string MsgProgramError { get; set; } = "ERROR";
        public static string MsgProgramErrorExpectedHex { get; set; } = "Expected Text containing only the following characters (0-9, A-F).";

        public static string MsgSettingsLoadFail { get; set; } = "Unable to load settings.";
        public static string MsgSettingsResetCorrupt { get; set; } = "Program settings are corrupt. Would you like to reset the settings?";
        public static string MsgSettingsResetPrompt { get; set; } = "Yes to delete the program settings, or No to close the program.";
        public static string MsgSettingsResetSuccess { get; set; } = "Program settings have been deleted.";

        public static string MsgYes { get; set; } = "Yes";
        public static string MsgNo { get; set; } = "No";

        public static string MsgGameColosseum { get; set; } = "Colosseum";
        public static string MsgGameXD { get; set; } = "XD";
        public static string MsgGameRSBOX { get; set; } = "RS Box";

        #endregion

        #region Main Window

        public static string MsgFileLoad { get; set; } = "File Loaded:";
        // ReSharper disable once UnusedMember.Global -- used only for release
        public static string MsgFileLoadFail { get; set; } = "Unable to load file.";
        public static string MsgFileLoadFailAuto { get; set; } = "An error occurred while attempting to auto-load your save file.";
        public static string MsgFileLoadVersionSelect { get; set; } = "Select version.";
        public static string MsgFileLoadVersionDetect { get; set; } = "Generation {0} Save File detected.";
        public static string MsgFileLoadEncrypted { get; set; } = "PKHeX only edits decrypted save files.";
        public static string MsgFileLoadEncryptedFail { get; set; } = "This save file is not decrypted.";
        public static string MsgFileLoadXorpad1 { get; set; } = "Press Yes to load the sav at 0x3000.";
        public static string MsgFileLoadXorpad2 { get; set; } = "Press No to load the sav at 0x82000.";
        public static string MsgFileLoadIncompatible { get; set; } = "Binary is not compatible with save file.";
        
        public static string MsgFileLoadSaveFail { get; set; } = "The data file is not a valid save file.";
        public static string MsgFileLoadSaveDetected { get; set; } = "Save File Detected.";
        public static string MsgFileLoadSaveLoadFail { get; set; } = "Invalid save file loaded. Aborting.";
        public static string MsgFileLoadSaveDetectReload { get; set; } = "Open save file from the following location?";
        public static string MsgFileLoadSaveSelectVersion { get; set; } = "Select the version.";
        public static string MsgFileLoadSaveSelectGame { get; set; } = "Select a game to edit.";
        public static string MsgFileLoadSaveMultiple { get; set; } = "Multiple games detected";
        public static string MsgFileGameCubeBad { get; set; } = "Invalid or corrupted GC Memory Card. Aborting.";
        public static string MsgFileGameCubeDuplicate { get; set; } = "GC Memory Card with duplicated game save files. Aborting.";
        public static string MsgFileGameCubeNoGames { get; set; } = "GC Memory Card without any Pokémon save file. Aborting.";

        public static string MsgFileSize { get; set; } = "File Size: {0} bytes";
        public static string MsgFileSizeLarge { get; set; } = "Input file is too large.";
        public static string MsgFileSizeSmall { get; set; } = "Input file is too small.";
        public static string MsgFileWriteProtected { get; set; } = "File's location is write protected:";
        public static string MsgFileWriteProtectedAdvice { get; set; } = "If the path is a removable disk (SD card), please ensure the write protection switch is not set.";
        public static string MsgFileInUse { get; set; } = "Unable to load file. It could be in use by another program.";
        public static string MsgFileUnsupported { get; set; } = "Attempted to load an unsupported file type/size.";
        public static string MsgPKMConvertFail { get; set; } = "Conversion failed.";
        public static string MsgPKMMysteryGiftFail { get; set; } = "Mystery Gift is not a Pokémon.";

        public static string MsgClipboardLegalityExport { get; set; } = "Copy report to Clipboard?";
        public static string MsgClipboardFailRead { get; set; } = "Clipboard does not contain text.";
        public static string MsgClipboardFailWrite { get; set; } = "Unable to set text to Clipboard.";

        public static string MsgSimulatorFailClipboard { get; set; } = "Set data not found in clipboard.";
        public static string MsgSimulatorLoad { get; set; } = "Import this set?";
        public static string MsgSimulatorInvalid { get; set; } = "Invalid lines detected:";
        public static string MsgSimulatorExportBadFields { get; set; } = "Fix data before exporting.";
        public static string MsgSimulatorExportFail { get; set; } = "Try exporting again.";
        public static string MsgSimulatorExportSuccess { get; set; } = "Exported Showdown Set to Clipboard:";
        public static string MsgSimulatorExportParty { get; set; } = "Showdown Team (Party) set to Clipboard.";
        public static string MsgSimulatorExportBattleBox { get; set; } = "Showdown Team (Battle Box) set to Clipboard.";

        public static string MsgLegalityAllowTradebacks { get; set; } = "Generation 1 Save File detected. Allow tradebacks from Generation 2 for legality purposes?";
        public static string MsgLegalityAllowTradebacksYes { get; set; } = "Yes: Allow Generation 2 tradeback learnsets";
        public static string MsgLegalityAllowTradebacksNo { get; set; } = "No: Don't allow Generation 2 tradeback learnsets";

        public static string MsgQRDecodeFail { get; set; } = "Decoded data not a valid PKM/Gift.";
        public static string MsgQRDecodeSize { get; set; } = "QR Data Size: {0} bytes";
        public static string MsgQRDeprecated { get; set; } = "QR codes are deprecated in favor of other methods.";
        public static string MsgQRAlternative { get; set; } = "Consider utilizing homebrew or on-the-fly RAM editing custom firmware (PKMN-NTR).";

        public static string MsgBackupCreateLocation { get; set; } = "PKHeX can perform automatic backups if you create a folder with the name '{0}' in the same folder as PKHeX's executable.";
        public static string MsgBackupCreateQuestion { get; set; } = "Would you like to create the backup folder now?";
        public static string MsgBackupDelete { get; set; } = "If you wish to no longer automatically back up save files, delete the '{0}' folder.";
        public static string MsgBackupUnable { get; set; } = "Unable to create backup folder";
        public static string MsgBackupSuccess { get; set; } = "Backup folder created!";

        public static string MsgDatabase { get; set; } = "PKHeX's database was not found.";
        public static string MsgDatabaseAdvice { get; set; } = "Please dump all boxes from a save file, then ensure the '{0}' folder exists.";
        public static string MsgDatabaseExport { get; set; } = "Save to PKHeX's database?";
        public static string MsgDatabaseLoad { get; set; } = "Load from PKHeX's database?";

        #endregion

        #region PKM Editor

        public static string MsgPKMLoadNull { get; set; } = "Attempted to load a null file.";
        public static string MsgPKMSuggestionFormat { get; set; } = "Suggestions are not enabled for this PKM format.";
        public static string MsgPKMSuggestionMoves { get; set; } = "Apply suggested current moves?";
        public static string MsgPKMSuggestionRelearn { get; set; } = "Apply suggested relearn moves?";
        public static string MsgPKMSuggestionNone { get; set; } = "Unable to provide a suggestion.";
        public static string MsgPKMSuggestionStart { get; set; } = "Suggested:";
        public static string MsgPKMSuggestionMetLocation { get; set; } = "Met Location:";
        public static string MsgPKMSuggestionMetLevel { get; set; } = "Met Level:";
        public static string MsgPKMSuggestionLevel { get; set; } = "Current Level:";

        #endregion

        #region Save Editor
        public static string MsgSaveBackup { get; set; } = "Saved Backup of current SAV to:";
        public static string MsgSaveCurrentGeneration { get; set; } = "Current SAV Generation: {0}";

        public static string MsgSaveBoxCloneFromTabs { get; set; } = "Clone Pokemon from Editing Tabs to all slots in {0}?";
        public static string MsgSaveBoxSortCurrent { get; set; } = "Sort Current Box?";
        public static string MsgSaveBoxSortCurrentFailBattle { get; set; } = "Battle Box slots prevent the sorting of box.";
        public static string MsgSaveBoxSortCurrentSuccess { get; set; } = "Current Box sorted!";
        public static string MsgSaveBoxSortAll { get; set; } = "Sort ALL Boxes?!";
        public static string MsgSaveBoxSortAllFailBattle { get; set; } = "Battle Box slots prevent the sorting of all boxes.";
        public static string MsgSaveBoxSortAllSuccess { get; set; } = "Boxes sorted!";
        public static string MsgSaveBoxClearCurrent { get; set; } = "Clear Current Box?";
        public static string MsgSaveBoxClearCurrentFailBattle { get; set; } = "Battle Box slots prevent the clearing of box.";
        public static string MsgSaveBoxClearCurrentSuccess { get; set; } = "Current Box cleared!";
        public static string MsgSaveBoxClearAll { get; set; } = "Clear ALL Boxes?!";
        public static string MsgSaveBoxClearAllFailBattle { get; set; } = "Battle Box slots prevent the clearing of all boxes.";
        public static string MsgSaveBoxClearAllSuccess { get; set; } = "Boxes cleared!";

        public static string MsgSaveBoxExportFailNone { get; set; } = "The currently loaded save file does not have boxes to export from!";
        public static string MsgSaveBoxImportModifyIntro { get; set; } = "PKM Loading overrides:";
        public static string MsgSaveBoxImportModifyYes { get; set; } = "Yes - Modify .pk* when set to SAV";
        public static string MsgSaveBoxImportModifyCurrent { get; set; } = "Cancel - Use current settings ({0})";
        public static string MsgSaveBoxImportModifyNo { get; set; } = "No - Don't modify .pk*";
        public static string MsgSaveBoxImportClear { get; set; } = "Clear subsequent boxes when importing data?";
        public static string MsgSaveBoxImportClearNo { get; set; } = "If you only want to overwrite for new data, press no.";
        public static string MsgSaveBoxImportPCBinary { get; set; } = "PC Binary loaded.";
        public static string MsgSaveBoxImportPCFailBattle { get; set; } = "Battle Box slots prevent loading of PC data.";
        public static string MsgSaveBoxImportBoxBinary { get; set; } = "Box Binary loaded.";
        public static string MsgSaveBoxImportBoxFailBattle { get; set; } = "Battle Box slots in box prevent loading of box data.";
        public static string MsgSaveBoxImportVideo { get; set; } = "Load Battle Video Pokémon data to the current box ({0})?";
        public static string MsgSaveBoxImportVideoSuccess { get; set; } = "Battle Video data loaded to box slots.";
        public static string MsgSaveBoxImportVideoFailGeneration { get; set; } = "Cannot load the Battle Video to a different generation save file.";
        public static string MsgSaveBoxImportOverwrite { get; set; } = "The current box will be overwritten.";
        public static string MsgSaveBoxImportSkippedLocked { get; set; } = "Skipped {0} locked slot(s).";

        public static string MsgSaveSwitchDaycareView { get; set; } = "Would you like to switch the view to the other Daycare?";
        public static string MsgSaveSwitchDaycareCurrent { get; set; } = "Currently viewing daycare {0}.";
        public static string MsgSaveGen6OPower { get; set; } = "No editing support for ORAS :(";
        public static string MsgSaveGen6OPowerCheatDesc { get; set; } = "Max O-Powers with a working code?";
        public static string MsgSaveGen6Passerby { get; set; } = "Export Passerby Info to Clipboard?";
        public static string MsgSaveGen6FriendSafari { get; set; } = "No editing support for Friend Safari :(";
        public static string MsgSaveGen6FriendSafariCheatDesc { get; set; } = "Unlock all 3 slots for each friend?";
        public static string MsgSaveGen2RTCResetPassword { get; set; } = "RTC Reset Password: {0:00000}";
        public static string MsgSaveJPEGExportFail { get; set; } = "No picture data found in the save file!";

        public static string MsgSaveChecksumFailEdited { get; set; } = "Save has been edited. Cannot integrity check.";
        public static string MsgSaveChecksumValid { get; set; } = "Checksums are valid.";
        public static string MsgSaveChecksumFailExport { get; set; } = "Export Checksum Info to Clipboard?";

        #endregion
    }
}
