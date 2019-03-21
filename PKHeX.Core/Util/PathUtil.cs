using System.IO;

namespace PKHeX.Core
{
    public static partial class Util
    {
        public static string CleanFileName(string fileName)
        {
            return string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
