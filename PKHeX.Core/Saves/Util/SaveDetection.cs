using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility logic for detecting a <see cref="SaveFile"/> from various locations on the host machine.
    /// </summary>
    public static class SaveDetection
    {
        /// <summary>
        /// Gets the 3DS's root folder, usually from an inserted SD card.
        /// </summary>
        /// <param name="drives">List of drives on the host machine.</param>
        /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
        /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
        /// <returns>Folder path pointing to the Nintendo 3DS folder.</returns>
        public static string? Get3DSLocation(IEnumerable<string> drives, bool skipFirstDrive = true) =>
            FindConsoleRootFolder(drives, "Nintendo 3DS", skipFirstDrive);

        /// <summary>
        /// Gets the Switch's root folder, usually from an inserted SD card.
        /// </summary>
        /// <param name="drives">List of drives on the host machine.</param>
        /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
        /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
        /// <returns>Folder path pointing to the Nintendo folder.</returns>
        public static string? GetSwitchLocation(IEnumerable<string> drives, bool skipFirstDrive = true) =>
            FindConsoleRootFolder(drives, "Nintendo", skipFirstDrive);

        private static string? FindConsoleRootFolder(IEnumerable<string> drives, string path, bool skipFirstDrive)
        {
            if (skipFirstDrive)
                drives = drives.Skip(1);

            var paths = drives.Select(drive => Path.Combine(drive, path));
            return paths.FirstOrDefault(Directory.Exists);
        }

        /// <summary>
        /// Gets a list of 3DS save backup paths for the storage device.
        /// </summary>
        /// <param name="root">Root location of device</param>
        /// <returns>List of possible 3DS save backup paths.</returns>
        public static IEnumerable<string> Get3DSBackupPaths(string root)
        {
            yield return Path.Combine(root, "saveDataBackup");
            yield return Path.Combine(root, "filer", "UserSaveData");
            yield return Path.Combine(root, "JKSV", "Saves");
            yield return Path.Combine(root, "TWLSaveTool");
            yield return Path.Combine(root, "fbi", "save");
            yield return Path.Combine(root, "gm9", "out");
            yield return Path.Combine(root, "3ds", "Checkpoint", "saves");
        }

        /// <summary>
        /// Gets a list of Switch save backup paths for the storage device.
        /// </summary>
        /// <param name="root">Root location of device</param>
        /// <returns>List of possible 3DS save backup paths.</returns>
        public static IEnumerable<string> GetSwitchBackupPaths(string root)
        {
            yield return Path.Combine(root, "switch", "Checkpoint", "saves");
        }

        /// <summary>
        /// Extra list of Backup Paths used for detecting a save file.
        /// </summary>
        public static readonly List<string> CustomBackupPaths = new List<string>();

        /// <summary>
        /// Finds a compatible save file that was most recently saved (by file write time).
        /// </summary>
        /// <param name="drives">List of drives on the host machine.</param>
        /// <param name="error">If this function does not return a save file, this parameter will be set to the error message.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>Reference to a valid save file, if any.</returns>
        public static SaveFile? DetectSaveFile(IReadOnlyList<string> drives, ref string error, params string[] extra)
        {
            var foldersToCheck = GetFoldersToCheck(drives, extra);
            var result = GetSaveFilePathsFromFolders(foldersToCheck, out var possiblePaths);
            if (!result)
            {
                error = string.Join(Environment.NewLine, possiblePaths); // `possiblePaths` contains the error message
                return null;
            }

            // return newest save file path that is valid
            var byMostRecent = possiblePaths.OrderByDescending(File.GetLastWriteTimeUtc);
            var saves = byMostRecent.Select(SaveUtil.GetVariantSAV);
            return saves.FirstOrDefault(z => z?.ChecksumsValid == true);
        }

        /// <summary>
        /// Gets all detectable save files ordered by most recently saved (by file write time).
        /// </summary>
        /// <param name="drives">List of drives on the host machine.</param>
        /// <param name="detect">Detect save files stored in common SD card homebrew locations.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>Valid save files, if any.</returns>
        public static IEnumerable<SaveFile> GetSaveFiles(IReadOnlyList<string> drives, bool detect, params string[] extra) => GetSaveFiles(drives, detect, (IEnumerable<string>)extra);

        /// <summary>
        /// Gets all detectable save files ordered by most recently saved (by file write time).
        /// </summary>
        /// <param name="drives">List of drives on the host machine.</param>
        /// <param name="detect">Detect save files stored in common SD card homebrew locations.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>Valid save files, if any.</returns>
        public static IEnumerable<SaveFile> GetSaveFiles(IReadOnlyList<string> drives, bool detect, IEnumerable<string> extra)
        {
            var paths = detect ? GetFoldersToCheck(drives, extra) : extra;
            var result = GetSaveFilePathsFromFolders(paths, out var possiblePaths);
            if (!result)
                yield break;

            var byMostRecent = possiblePaths.OrderByDescending(File.GetLastWriteTimeUtc);
            foreach (var s in byMostRecent)
            {
                var sav = SaveUtil.GetVariantSAV(s);
                if (sav != null)
                    yield return sav;
            }
        }

        public static IEnumerable<string> GetFoldersToCheck(IReadOnlyList<string> drives, IEnumerable<string> extra)
        {
            var foldersToCheck = extra.Where(f => !string.IsNullOrWhiteSpace(f)).Concat(CustomBackupPaths);

            string path3DS = Path.GetPathRoot(Get3DSLocation(drives));
            if (!string.IsNullOrEmpty(path3DS)) // check for Homebrew/CFW backups
                foldersToCheck = foldersToCheck.Concat(Get3DSBackupPaths(path3DS));

            string pathNX = Path.GetPathRoot(GetSwitchLocation(drives));
            if (!string.IsNullOrEmpty(pathNX)) // check for Homebrew/CFW backups
                foldersToCheck = foldersToCheck.Concat(GetSwitchBackupPaths(pathNX));

            return foldersToCheck;
        }

        private static bool GetSaveFilePathsFromFolders(IEnumerable<string> foldersToCheck, out IEnumerable<string> possible)
        {
            var possiblePaths = new List<string>();
            foreach (var folder in foldersToCheck)
            {
                if (!SaveUtil.GetSavesFromFolder(folder, true, out IEnumerable<string> files))
                {
                    if (!(files is string[] msg)) // should always return string[]
                        continue;
                    if (msg.Length == 0) // folder doesn't exist
                        continue;
                    possible = msg;
                    return false;
                }
                if (files != null)
                    possiblePaths.AddRange(files);
            }
            possible = possiblePaths;
            return true;
        }
    }
}
