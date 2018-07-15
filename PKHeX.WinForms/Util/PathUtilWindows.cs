using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public static class PathUtilWindows
    {
        /// <summary>
        /// Gets the 3DS's root folder, usually from an inserted SD card.
        /// </summary>
        /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
        /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
        /// <returns>Folder path pointing to the Nintendo 3DS folder.</returns>
        public static string Get3DSLocation(bool skipFirstDrive = true) => FindConsoleRootFolder("Nintendo 3DS", skipFirstDrive);

        /// <summary>
        /// Gets the Switch's root folder, usually from an inserted SD card.
        /// </summary>
        /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
        /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
        /// <returns>Folder path pointing to the Nintendo folder.</returns>
        public static string GetSwitchLocation(bool skipFirstDrive = true) => FindConsoleRootFolder("Nintendo", skipFirstDrive);

        private static string FindConsoleRootFolder(string path, bool skipFirstDrive)
        {
            try
            {
                // Skip first drive (some users still have floppy drives and would chew up time!)
                IEnumerable<string> DriveList = Environment.GetLogicalDrives();
                if (skipFirstDrive)
                    DriveList = DriveList.Skip(1);

                return DriveList
                    .Select(drive => Path.Combine(drive, path))
                    .FirstOrDefault(Directory.Exists);
            }
            catch { }
            return null;
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
        /// Finds a compatible save file that was most recently saved (by file write time).
        /// </summary>
        /// <param name="error">If this function does not return a save file, this parameter will be set to the error message.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>Reference to a valid save file, if any.</returns>
        public static SaveFile DetectSaveFile(ref string error, params string[] extra)
        {
            var foldersToCheck = GetFoldersToCheck(extra);
            var result = GetSaveFilePathsFromFolders(foldersToCheck, out var possiblePaths);
            if (!result)
            {
                error = string.Join(Environment.NewLine, possiblePaths); // `possiblePaths` contains the error message
                return null;
            }

            // return newest save file path that is valid
            var byMostRecent = possiblePaths.OrderByDescending(f => new FileInfo(f).LastWriteTime);
            var saves = byMostRecent.Select(SaveUtil.GetVariantSAV);
            return saves.FirstOrDefault(z => z?.ChecksumsValid == true);
        }

        /// <summary>
        /// Gets all detectable save files ordered by most recently saved (by file write time).
        /// </summary>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>Valid save files, if any.</returns>
        public static IEnumerable<SaveFile> GetSaveFiles(params string[] extra)
        {
            var foldersToCheck = GetFoldersToCheck(extra);
            var result = GetSaveFilePathsFromFolders(foldersToCheck, out var possiblePaths);
            if (!result)
                return Enumerable.Empty<SaveFile>();

            var byMostRecent = possiblePaths.OrderByDescending(f => new FileInfo(f).LastWriteTime);
            return byMostRecent.Select(SaveUtil.GetVariantSAV);
        }

        private static IEnumerable<string> GetFoldersToCheck(IEnumerable<string> extra)
        {
            var foldersToCheck = extra.Where(f => f?.Length > 0);

            string path3DS = Path.GetPathRoot(Get3DSLocation());
            if (path3DS != null) // check for Homebrew/CFW backups
                foldersToCheck = foldersToCheck.Concat(Get3DSBackupPaths(path3DS));

            string pathNX = Path.GetPathRoot(GetSwitchLocation());
            if (pathNX != null) // check for Homebrew/CFW backups
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
                    if (files == null)
                        continue;
                    possible = files;
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
