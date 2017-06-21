using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.WinForms
{
    public static class PathUtilWindows
    {
        public static string Get3DSLocation()
        {
            try
            {
                string[] DriveList = Environment.GetLogicalDrives();
                for (int i = 1; i < DriveList.Length; i++) // Skip first drive (some users still have floppy drives and would chew up time!)
                {
                    string potentialPath = Path.Combine(DriveList[i], "Nintendo 3DS");
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
        public static string[] Get3DSBackupPaths(string root)
        {
            return new[]
            {
                Path.Combine(root, "saveDataBackup"),
                Path.Combine(root, "filer", "UserSaveData"),
                Path.Combine(root, "JKSV", "Saves"),
                Path.Combine(root, "TWLSaveTool"),
                Path.Combine(root, "fbi", "save"),
            };
        }

        /// <summary>
        /// Detects a save file.
        /// </summary>
        /// <param name="path">If this function returns true, full path of a save file or null if no path could be found. If this function returns false, this parameter will be set to the error message.</param>
        /// <param name="extra">Paths to check in addition to the default paths</param>
        /// <returns>A boolean indicating whether or not a file was detected</returns>
        public static bool DetectSaveFile(out string path, params string[] extra)
        {
            string path3DS = Path.GetPathRoot(Get3DSLocation());
            List<string> possiblePaths = new List<string>();
            List<string> foldersToCheck = new List<string>(extra.Where(f => f?.Length > 0));
            path = null;

            if (path3DS != null) // check for Homebrew/CFW backups
                foldersToCheck.AddRange(Get3DSBackupPaths(path3DS));

            foreach (var p in foldersToCheck)
            {
                if (!SaveUtil.GetSavesFromFolder(p, true, out IEnumerable<string> files))
                {
                    if (files != null) // Could be null if `p` doesn't exist
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
