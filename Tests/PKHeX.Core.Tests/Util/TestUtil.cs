using System.IO;

namespace PKHeX.Tests
{
    internal static class TestUtil
    {
        public static string GetRepoPath()
        {
            var folder = Directory.GetCurrentDirectory();
            while (!folder.EndsWith(nameof(Tests)))
                folder = Directory.GetParent(folder).FullName;
            return folder;
        }
    }
}
