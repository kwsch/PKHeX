using System;
using System.IO;

namespace PKHeX.WinForms
{
    public static class CyberGadgetUtil
    {
        public static string GetTempFolder()
        {
            return Path.Combine(Path.GetTempPath(), "3DSSE");
        }
        public static string GetCacheFolder()
        {
            return Path.Combine(GetBackupLocation(), "cache");
        }
        private static string GetRegistryValue(string key)
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
        private static string GetRegistryBase()
        {
            return @"SOFTWARE\CYBER Gadget\3DSSaveEditor";
        }
        private static string GetBackupLocation()
        {
            string registryValue = GetRegistryValue("Location");
            if (!string.IsNullOrEmpty(registryValue))
            {
                Directory.CreateDirectory(registryValue);
                return registryValue;
            }
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "3DSSaveBank");
            if (Directory.Exists(GetRegistryBase()))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
