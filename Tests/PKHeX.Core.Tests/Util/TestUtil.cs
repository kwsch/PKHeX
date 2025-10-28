using System;
using System.IO;
using System.Threading;

namespace PKHeX.Core.Tests;

internal static class TestUtil
{
    public static string GetRepoPath()
    {
        var folder = Directory.GetCurrentDirectory();
        while (!folder.EndsWith(nameof(Tests)))
        {
            var dir = Directory.GetParent(folder);
            ArgumentNullException.ThrowIfNull(dir);
            folder = dir.FullName;
        }
        return folder;
    }

    private static readonly Lock InitLock = new();
    private static bool IsInitialized;

    public static void InitializeLegality()
    {
        lock (InitLock)
        {
            if (IsInitialized)
                return;
            // There used to be a lot of static initialization in the legality code, which is now removed.
            IsInitialized = true;
        }
    }
}
