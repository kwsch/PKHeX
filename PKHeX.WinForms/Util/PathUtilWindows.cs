using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.WinForms
{
    public static class PathUtilWindows
    {
        public static string get3DSLocation()
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
        /// Detects a save file.
        /// </summary>
        /// <returns>Full path of a save file. Returns null if unable to find any.</returns>
        public static bool detectSaveFile(out string path, params string[] extra)
        {
            string path3DS = Path.GetPathRoot(get3DSLocation());
            List<string> possiblePaths = new List<string>();
            List<string> foldersToCheck = new List<string>(extra.Where(f => f?.Length > 0));
            path = null;

            // Homebrew/CFW
            if (path3DS != null)
            {
                foldersToCheck.AddRange(new[]
                {
                    Path.Combine(path3DS, "saveDataBackup"),
                    Path.Combine(path3DS, "filer", "UserSaveData"),
                    Path.Combine(path3DS, "JKSV", "Saves"),
                    Path.Combine(path3DS, "TWLSaveTool"),
                    Path.Combine(path3DS, "fbi", "save"),
                });
            }

            foreach (var p in foldersToCheck)
            {
                IEnumerable<string> files;
                if (!SaveUtil.getSavesFromFolder(p, true, out files))
                {
                    if (files == null)
                        continue;
                    path = files.First(); // error
                    return false;
                }
                possiblePaths.AddRange(files);
            }

            // return newest save file path that is valid
            foreach (var file in possiblePaths.OrderByDescending(f => new FileInfo(f).LastWriteTime))
            {
                try
                {
                    var data = File.ReadAllBytes(file);
                    var sav = SaveUtil.getVariantSAV(data);
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
