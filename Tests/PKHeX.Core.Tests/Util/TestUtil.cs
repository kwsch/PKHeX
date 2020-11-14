using System;
using System.IO;

namespace PKHeX.Tests
{
    internal static class TestUtil
    {
        public static string GetRepoPath()
        {
            var folder = Directory.GetCurrentDirectory();
            while (!folder.EndsWith(nameof(Tests)))
            {
                var dir = Directory.GetParent(folder);
                if (dir == null)
                    throw new ArgumentNullException(nameof(dir));
                folder = dir.FullName;
            }
            return folder;
        }
    }
}
