using System;
using System.IO;
using System.Linq;

namespace PKHeX
{
    public partial class Util
    {
        internal static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        internal static string GetTempFolder()
        {
            return Path.Combine(Path.GetTempPath(), "3DSSE");
        }
        internal static string GetCacheFolder()
        {
            return Path.Combine(GetBackupLocation(), "cache");
        }
        internal static string GetRegistryValue(string key)
        {
            Microsoft.Win32.RegistryKey currentUser = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey key3 = currentUser.OpenSubKey(GetRegistryBase());
            if (key3 == null)
                return null;

            string str = key3.GetValue(key) as string;
            key3.Close();
            currentUser.Close();
            return str;
        }
        internal static string GetRegistryBase()
        {
            return @"SOFTWARE\CYBER Gadget\3DSSaveEditor";
        }
        internal static string GetBackupLocation()
        {
            string registryValue = GetRegistryValue("Location");
            if (!string.IsNullOrEmpty(registryValue))
            {
                Directory.CreateDirectory(registryValue);
                return registryValue;
            }
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "3DSSaveBank");
            Directory.CreateDirectory(path);
            return path;
        }
        internal static string get3DSLocation()
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
        internal static string GetSDFLocation()
        {
            try
            {
                // Start by checking if the 3DS file path exists or not.
                string path_SDF = null;
                string[] DriveList = Environment.GetLogicalDrives();
                for (int i = 1; i < DriveList.Length; i++) // Skip first drive (some users still have floppy drives and would chew up time!)
                {
                    string potentialPath_SDF = NormalizePath(Path.Combine(DriveList[i], "filer", "UserSaveData"));
                    if (!Directory.Exists(potentialPath_SDF)) continue;

                    path_SDF = potentialPath_SDF; break;
                }
                if (path_SDF == null)
                    return null;
                // 3DS data found in SD card reader. Let's get the title folder location!
                string[] folders = Directory.GetDirectories(path_SDF, "*", SearchOption.TopDirectoryOnly);
                Array.Sort(folders); // Don't need Modified Date, sort by path names just in case.

                // Loop through all the folders in the Nintendo 3DS folder to see if any of them contain 'title'.
                for (int i = folders.Length - 1; i >= 0; i--)
                {
                    if (File.Exists(Path.Combine(folders[i], "000011c4", "main"))) return Path.Combine(folders[i], "000011c4"); // OR
                    if (File.Exists(Path.Combine(folders[i], "000011c5", "main"))) return Path.Combine(folders[i], "000011c5"); // AS
                    if (File.Exists(Path.Combine(folders[i], "0000055d", "main"))) return Path.Combine(folders[i], "0000055d"); // X
                    if (File.Exists(Path.Combine(folders[i], "0000055e", "main"))) return Path.Combine(folders[i], "0000055e"); // Y
                }
                return null; // Fallthrough
            }
            catch { return null; }
        }
        internal static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        internal static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            return index < 0 ? input : input.Substring(0, index);
        }
    }
}
