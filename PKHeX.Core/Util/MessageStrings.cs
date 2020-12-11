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
        public static string MsgProgramSaveFileConfirm { get; set; } = "Are you sure you want to load a new save file?";
        public static string MsgProgramError { get; set; } = "ERROR";
        public static string MsgProgramErrorExpectedHex { get; set; } = "Expected Text containing only the following characters (0-9, A-F).";

        public static string MsgSettingsLoadFail { get; set; } = "Unable to load settings.";
        public static string MsgSettingsResetCorrupt { get; set; } = "Program settings are corrupt. Would you like to reset the settings?";
        public static string MsgSettingsResetPrompt { get; set; } = "Yes to delete the program settings, or No to close the program.";
        public static string MsgSettingsResetSuccess { get; set; } = "Program settings have been deleted.";

        public static string MsgAny { get; set; } = "Any";
        public static string MsgAll { get; set; } = "All";
        public static string MsgYes { get; set; } = "Yes";
        public static string MsgNo { get; set; } = "No";
        public static string MsgContinue { get; set; } = "Continue?";

        public static string MsgGameColosseum { get; set; } = "Colosseum";
        public static string MsgGameXD { get; set; } = "XD";
        public static string MsgGameRSBOX { get; set; } = "RS Box";

        public static string MsgFileDeleteCount { get; set; } = "{0} files deleted.";
        public static string MsgFolderNotFound { get; set; } = "Can't find folder:";
        public static string MsgWindowClose { get; set; } = "The current window will now close.";

        public static string MsgResearchRequired { get; set; } = "Needs more research.";

        #endregion

        #region Main Window

        public static string MsgFileLoad { get; set; } = "File Loaded:";
        public static string MsgFileLoadFail { get; set; } = "Unable to load file.";
        public static string MsgFileLoadFailAuto { get; set; } = "An error occurred while attempting to auto-load your save file.";
        public static string MsgFileLoadFailAutoAdvise { get; set; } = "It is advised to manually remove bad filenames from the folder.";
        public static string MsgFileLoadFailAutoCause { get; set; } = "This is likely caused by Homebrew creating invalid filenames.";
        public static string MsgFileLoadVersionDetect { get; set; } = "Generation {0} Save File detected.";
        public static string MsgFileLoadEncrypted { get; set; } = "PKHeX only edits decrypted save files.";
        public static string MsgFileLoadEncryptedFail { get; set; } = "This save file is not decrypted.";
        public static string MsgFileLoadIncompatible { get; set; } = "Binary is not compatible with save file.";

        public static string MsgFileLoadSaveFail { get; set; } = "The data file is not a valid save file.";
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
        public static string MsgFileWriteFail { get; set; } = "Unable to save file.";
        public static string MsgFileWriteProtected { get; set; } = "File's location is write protected:";
        public static string MsgFileWriteProtectedAdvice { get; set; } = "If the file is on a removable disk (SD card), please ensure the write protection switch is not set.";
        public static string MsgFileInUse { get; set; } = "Unable to load file. It could be in use by another program.";
        public static string MsgFileUnsupported { get; set; } = "Attempted to load an unsupported file type/size. This could mean PKHeX doesn't support your save file or your save file is corrupt.";
        public static string MsgPKMUnsupported { get; set; } = "Attempted to load an unsupported file type/size. This could be caused by loading a different generation Pokemon file on an unsupported generation or your file is corrupt.";

        public static string MsgPKMConvertSuccess { get; set; } = "Converted from {0} to {1}.";
        public static string MsgPKMConvertFail { get; set; } = "Conversion failed.";
        public static string MsgPKMMysteryGiftFail { get; set; } = "Mystery Gift is not a Pokémon.";
        public static string MsgPKMConvertFailFormat { get; set; } = "Cannot convert a {0} to {1}";
        public static string MsgPKMConvertFailBackwards { get; set; } = "Can't load {0}s to Gen{1} saves.";
        public static string MsgPKMConvertFailForm { get; set; } = "Form cannot be transferred to future games.";
        public static string MsgPKMConvertFailNoMethod { get; set; } = "Cannot transfer this format to the requested format.";
        public static string MsgPKMConvertIncompatible { get; set; } = "Cannot load {0} {1}s to {2} saves.";
        public static string MsgPKMConvertInternational { get; set; } = "International";
        public static string MsgPKMConvertJapanese { get; set; } = "Japanese";

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
        public static string MsgSimulatorExportList { get; set; } = "Showdown Sets copied to Clipboard.";

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

        public static string MsgSaveExportSuccessPath { get; set; } = "SAV exported to:";
        public static string MsgSaveExportContinue { get; set; } = "Continue saving?";

        public static string MsgSaveSlotEmpty { get; set; } = "Can't have an empty/egg party.";
        public static string MsgSaveSlotLocked { get; set; } = "Can't modify a locked slot.";
        public static string MsgSaveSlotBadData { get; set; } = "Unable to set to this slot.";

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

        public static string MsgSaveBoxFailNone { get; set; } = "The currently loaded save file does not have boxes!";
        public static string MsgSaveBoxExportYes { get; set; } = "Yes: Export All Boxes";
        public static string MsgSaveBoxExportNo { get; set; } = "No: Export {0} (Box {1})";
        public static string MsgSaveBoxExportCancel { get; set; } = "Cancel: Abort";
        public static string MsgSaveBoxExportInvalid { get; set; } = "Invalid Box Data, unable to dump.";
        public static string MsgSaveBoxExportPathCount { get; set; } = "Dumped Box(es) ({0} pkm) to path:";
        public static string MsgSaveBoxExportPathInvalid { get; set; } = "Invalid path specified.";
        public static string MsgSaveBoxImportModifyIntro { get; set; } = "PKM Loading overrides:";
        public static string MsgSaveBoxImportModifyYes { get; set; } = "Yes - Modify .pk* when set to SAV";
        public static string MsgSaveBoxImportModifyCurrent { get; set; } = "Cancel - Use current settings ({0})";
        public static string MsgSaveBoxImportModifyNo { get; set; } = "No - Don't modify .pk*";
        public static string MsgSaveBoxImportNoFiles { get; set; } = "No files loaded.";
        public static string MsgSaveBoxImportSuccess { get; set; } = "Loaded {0} files to boxes.";
        public static string MsgSaveBoxImportClear { get; set; } = "Clear subsequent boxes when importing data?";
        public static string MsgSaveBoxImportClearNo { get; set; } = "If you only want to overwrite for new data, press no.";
        public static string MsgSaveBoxImportPCBinary { get; set; } = "PC Binary loaded.";
        public static string MsgSaveBoxImportPCFailBattle { get; set; } = "Battle Box slots prevent loading of PC data.";
        public static string MsgSaveBoxImportBoxBinary { get; set; } = "Box Binary loaded.";
        public static string MsgSaveBoxImportBoxFailBattle { get; set; } = "Battle Box slots in box prevent loading of box data.";
        public static string MsgSaveBoxImportGroup { get; set; } = "Load Pokémon data from the file to the current box ({0})?";
        public static string MsgSaveBoxImportGroupSuccess { get; set; } = "Pokémon data from the file was loaded to box slots.";
        public static string MsgSaveBoxImportOverwrite { get; set; } = "The current box will be overwritten.";
        public static string MsgSaveBoxImportSkippedLocked { get; set; } = "Skipped {0} locked slot(s).";

        public static string MsgSaveSwitchDaycareView { get; set; } = "Would you like to switch the view to the other Daycare?";
        public static string MsgSaveSwitchDaycareCurrent { get; set; } = "Currently viewing daycare {0}.";
        public static string MsgSaveGen6Passerby { get; set; } = "Export Passerby Info to Clipboard?";
        public static string MsgSaveGen6FriendSafari { get; set; } = "No editing support for Friend Safari :(";
        public static string MsgSaveGen6FriendSafariCheatDesc { get; set; } = "Unlock all 3 slots for each friend?";
        public static string MsgSaveGen2RTCResetPassword { get; set; } = "RTC Reset Password: {0:00000}";
        public static string MsgSaveGen2RTCResetBitflag { get; set; } = "Would you like to reset the RTC?";
        public static string MsgSaveJPEGExportFail { get; set; } = "No picture data found in the save file!";

        public static string MsgSaveChecksumFailEdited { get; set; } = "Save has been edited. Cannot integrity check.";
        public static string MsgSaveChecksumValid { get; set; } = "Checksums are valid.";
        public static string MsgSaveChecksumFailExport { get; set; } = "Export Checksum Info to Clipboard?";

        public static string MsgIndexItemRange { get; set; } = "Item Index beyond range:";
        public static string MsgIndexItemGame { get; set; } = "Game can't obtain item:";
        public static string MsgIndexItemHeld { get; set; } = "Game can't hold item:";
        public static string MsgIndexSpeciesRange { get; set; } = "Species Index beyond range:";
        public static string MsgIndexSpeciesGame { get; set; } = "Game can't obtain species:";
        public static string MsgIndexMoveRange { get; set; } = "Move Index beyond range:";
        public static string MsgIndexMoveGame { get; set; } = "Game can't obtain move:";
        public static string MsgIndexAbilityRange { get; set; } = "Ability Index beyond range:";
        public static string MsgIndexAbilityGame { get; set; } = "Ability Index beyond range:";

        #endregion

        #region QR Codes

        public static string MsgQRDecodeFail { get; set; } = "Decoded data not a valid PKM/Gift.";
        public static string MsgQRDecodeSize { get; set; } = "QR Data Size: {0} bytes";
        public static string MsgQRDeprecated { get; set; } = "QR codes are deprecated in favor of other methods.";
        public static string MsgQRAlternative { get; set; } = "Consider utilizing homebrew or on-the-fly RAM editing custom firmware (PKMN-NTR).";
        public static string MsgQRClipboardUrl { get; set; } = "Copy QR URL to Clipboard?";
        public static string MsgQRClipboardImage { get; set; } = "Copy QR Image to Clipboard?";
        public static string MsgQRClipboardFail { get; set; } = "Failed to set Image to Clipboard";
        public static string MsgQRUrlFailPath { get; set; } = "Clipboard text is not a valid URL:";
        public static string MsgQRUrlFailImage { get; set; } = "Reader could not find QR data in the image.";
        public static string MsgQRUrlFailType { get; set; } = "Input URL is not valid. Double check that it is an image (jpg/png).";
        public static string MsgQRUrlFailConnection { get; set; } = "Unable to connect to the internet to decode QR code.";
        public static string MsgQRUrlFailConvert { get; set; } = "QR string to Data failed.";

        #endregion

        #region Database

        public static string MsgDBDeleteFailBackup { get; set; } = "Can't delete from a backup save.";
        public static string MsgDBDeleteFailModified { get; set; } = "Database slot data does not match save data!";
        public static string MsgDBDeleteFailWarning { get; set; } = "Don't move Pokémon after initializing the Database, please re-open the Database viewer.";
        public static string MsgDBAddFailExistsFile { get; set; } = "File already exists in database!";
        public static string MsgDBAddFailExistsPKM { get; set; } = "Pokémon already exists in database.";
        public static string MsgDBAddFromTabsSuccess { get; set; } = "Added Pokémon from tabs to database.";
        public static string MsgDBCreateReportPrompt { get; set; } = "Generate a Report on all data?";
        public static string MsgDBCreateReportWarning { get; set; } = "This may take a while...";
        public static string MsgDBCreateReportFail { get; set; } = "No results to export.";
        public static string MsgDBExportResultsPrompt { get; set; } = "Export results to a folder?";
        public static string MsgDBSearchLegalityWordfilter { get; set; } = "Check wordfilter legality?";
        public static string MsgDBSearchFail { get; set; } = "No data source to search!";
        public static string MsgDBSearchNone { get; set; } = "No results found!";
        public static string MsgDBDeleteCloneWarning { get; set; } = "Deleting clones from database is not reversible.";
        public static string MsgDBDeleteCloneAdvice { get; set; } = "If a PKM is deemed a clone, only the newest file (date modified) will be kept.";
        public static string MsgDBDeleteCloneFail { get; set; } = "Unable to delete clone:";
        public static string MsgDBDeleteCloneNone { get; set; } = "No clones detected or deleted.";
        public static string MsgExportWC3DataFail { get; set; } = "Unable to save WC3 data. No data to save!";

        #endregion

        #region Batch Editor

        public static string MsgBEToolTipPropName { get; set; } = "Property of a given PKM to modify.";
        public static string MsgBEToolTipPropType { get; set; } = "PropertyType of the currently loaded PKM in the main window.";
        public static string MsgBEToolTipPropValue { get; set; } = "PropertyValue of the currently loaded PKM in the main window.";

        public static string MsgBEFilterEmpty { get; set; } = "Empty Filter Value detected.";
        public static string MsgBEPropertyEmpty { get; set; } = "Empty Property Value(s) detected:";
        public static string MsgBEPropertyInvalid { get; set; } = "Invalid property selected.";
        public static string MsgBEInstructionInvalid { get; set; } = "Line length error in instruction list.";
        public static string MsgBEInstructionNone { get; set; } = "No instructions defined for a modification set.";

        public static string MsgBEModifySuccess { get; set; } = "Modified {0}{1}/{2} files.";
        public static string MsgBEModifyFailBlocked { get; set; } = "Skipped a file due to disallowed value:";
        public static string MsgBEModifyFailError { get; set; } = "{0} files ignored due to an internal error.";
        public static string MsgBEModifyFail { get; set; } = "Unable to modify {0} to {1}.";
        public static string MsgBEModifyFailCompare { get; set; } = "Unable to compare {0} to {1}.";

        public static string MsgExportFolder { get; set; } = "Please select the folder where the files will be saved to.";
        public static string MsgExportFolderAdvice { get; set; } = "This can be the same folder as the source of PKM files.";

        #endregion

        #region Misc

        public static string MsgReportExportTable { get; set; } = "Copy as formatted table?";
        public static string MsgReportExportCSV { get; set; } = "Save all the data to CSV?";
        public static string MsgReportColumnHide { get; set; } = "Hide Column";
        public static string MsgReportColumnRestore { get; set; } = "Restore Columns";
        public static string MsgReportColumnHideFail { get; set; } = "No cells/Columns selected.";
        public static string MsgReportColumnRestoreSuccess { get; set; } = "Column visibility restored.";

        #endregion

        #region Mystery Gift

        public static string MsgMsyteryGiftUsedAlert { get; set; } = "Mystery Gift is marked as USED and will not be able to be picked up in-game.";
        public static string MsgMysteryGiftUsedFix { get; set; } = "Do you want to remove the USED flag so that it is UNUSED?";
        public static string MsgMysteryGiftInvalid { get; set; } = "File is not a Mystery Gift:";
        public static string MsgMysteryGiftTypeUnexpected { get; set; } = "Expected Mystery Gift Type(s):";
        public static string MsgMysteryGiftTypeDetails { get; set; } = "Gift Details are not compatible with the save file.";
        public static string MsgMysteryGiftTypeIncompatible { get; set; } = "Gift type is not compatible with the save file.";
        public static string MsgMysteryGiftParseFail { get; set; } = "Unable to create gift description.";
        public static string MsgMysteryGiftParseTypeUnknown { get; set; } = "Unknown Mystery Gift Type!";

        public static string MsgMysteryGiftSlotEmpty { get; set; } = "Empty Slot. No data!";
        public static string MsgMysteryGiftSlotFail { get; set; } = "Unable to insert the Mystery Gift.";
        public static string MsgMysteryGiftSlotNone { get; set; } = "No Mystery Gift data found in loaded slot!";
        public static string MsgMysteryGiftSlotSpecialReject { get; set; } = "Does this Mystery Gift really belong to this game?";
        public static string MsgMysteryGiftSlotAlternate { get; set; } = "{0} gift was set to to a {1} slot instead.";
        public static string MsgMysteryGiftSlotFailSwap { get; set; } = "Can't swap a {0} with a {1}.";

        public static string MsgMysteryGiftQRTypeLength { get; set; } = "Decoded data length not valid for loaded save file.";
        public static string MsgMysteryGiftQRReceived { get; set; } = "QR Gift Type:";
        public static string MsgMysteryGiftQREonTicket { get; set; } = "Eon Ticket Mystery Gift will not import correctly when received via QR.";
        public static string MsgMysteryGiftQREonTicketAdvice { get; set; } = "Inject the Eon Ticket Mystery Gift to the save file instead.";

        #endregion

        #region Inventory

        public static string MsgItemPouchUnknown { get; set; } = "Unknown item detected.";
        public static string MsgItemPouchCountUpdated { get; set; } = "Item count modified.";
        public static string MsgItemCleared { get; set; } = "Items cleared.";
        public static string MsgItemPouchSizeSmall { get; set; } = "Pouch is too small for all items.";
        public static string MsgItemPouchRandom { get; set; } = "Yes: Give by Item ID{0}No: Random assortment";
        public static string MsgItemPouchWarning { get; set; } = "If you save changes, the item(s) will no longer be in the pouch.";
        public static string MsgItemPouchRemoved { get; set; } = "The following item(s) have been removed from the {0} pouch.";

        #endregion

        #region Misc

        public static string MsgSaveDifferentTypes { get; set; } = "Save File types are different.";
        public static string MsgSaveDifferentVersions { get; set; } = "Save File versions are not the same.";
        public static string MsgSaveNumberInvalid { get; set; } = "Save File {0} is not valid.";

        #endregion
    }
}
