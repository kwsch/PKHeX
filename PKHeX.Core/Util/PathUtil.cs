using System.IO;

namespace PKHeX.Core
{
    public static partial class Util
    {
        public static string CleanFileName(string fileName)
        {
            return string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        }

        public static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            return index < 0 ? input : input.Substring(0, index);
        }
    }
}
