using System.Configuration;
using System.IO;

namespace PKHeX.WinForms
{
    public static class ConfigUtil
    {
        public static bool checkConfig()
        {
            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                return true;
            }
            catch (ConfigurationErrorsException e)
            {
                string path = (e.InnerException as ConfigurationErrorsException)?.Filename;
                if (path != null && File.Exists(path))
                    File.Delete(path);
                return false;
            }
        }
    }
}
