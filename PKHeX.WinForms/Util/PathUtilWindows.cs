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
        public static string Get3DSLocation(bool skipFirstDrive = true)
        {
            try
            {
                IEnumerable<string> DriveList = Environment.GetLogicalDrives();

                // Skip first drive (some users still have floppy drives and would chew up time!)
                if (skipFirstDrive)
                    DriveList = DriveList.Skip(1);

                foreach (var drive in DriveList)
                {
                    string potentialPath = Path.Combine(drive, "Nintendo 3DS");
                    if (Directory.Exists(potentialPath))
                        return potentialPath;
                }
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
        }

        /// <summary>
        /// Finds a compatible save file that was most recently saved (by file write time).
        /// </summary>
        /// <param name="path">If this function returns true, full path of a save file or null if no path could be found. If this function returns false, this parameter will be set to the error message.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>A boolean indicating whether or not a file was detected</returns>
        public static bool DetectSaveFile(out string path, params string[] extra)
        {
            var foldersToCheck = extra.Where(f => f?.Length > 0);

            string path3DS = Path.GetPathRoot(Get3DSLocation());
            if (path3DS != null) // check for Homebrew/CFW backups
                foldersToCheck = foldersToCheck.Concat(Get3DSBackupPaths(path3DS));

            path = null;
            List<string> possiblePaths = new List<string>();
            foreach (var folder in foldersToCheck)
            {
                if (!SaveUtil.GetSavesFromFolder(folder, true, out IEnumerable<string> files))
                {
                    if (files != null) // can be null if folder doesn't exist
                    {
                        path = string.Join(Environment.NewLine, files); // `files` contains the error message
                        return false;
                    }
                }
                if (files != null)
                    possiblePaths.AddRange(files);
            }

            // return newest save file path that is valid
            foreach (var file in possiblePaths.OrderByDescending(f => new FileInfo(f).LastWriteTime))
            {
                try
                {
                    var data = File.ReadAllBytes(file);
                    var sav = SaveUtil.GetVariantSAV(data);
                    if (sav?.ChecksumsValid != true)
                        continue;

                    path = file;
                    return true;
                }
                catch (Exception e)
                {
                    path = e.Message + Environment.NewLine + file;
                    return false;
                }
            }
            return true;
        }
    }
}
