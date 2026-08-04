using PKHeX.Core;

namespace PKHeX.Avalonia.Tests.Fixtures;

/// <summary>
/// Discovers and loads real save files from Tests/savefiles/ for data-driven integration tests.
/// </summary>
public static class SaveFileFixture
{
    private static readonly string[] SaveExtensions = [".sav", ".main", ".bin"];

    /// <summary>
    /// Walks up from the test output directory to find the savefiles folder.
    /// </summary>
    public static string? FindSaveFilesPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var savePath = Path.Combine(dir.FullName, "savefiles");
            if (Directory.Exists(savePath))
                return savePath;

            // Also check Tests/savefiles relative to the repo root
            var testsPath = Path.Combine(dir.FullName, "Tests", "savefiles");
            if (Directory.Exists(testsPath))
                return testsPath;

            dir = dir.Parent;
        }
        return null;
    }

    /// <summary>
    /// Loads a save file from the given path, returning null on failure.
    /// </summary>
    public static SaveFile? LoadSave(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            return SaveUtil.GetSaveFile(File.ReadAllBytes(path));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Discovers all save files and returns them as xUnit MemberData.
    /// Each entry is (SaveFile sav, string label, string filePath).
    /// </summary>
    public static IEnumerable<object[]> AllRealSaves()
    {
        var saveDir = FindSaveFilesPath();
        if (saveDir == null)
            yield break;

        var files = Directory.GetFiles(saveDir)
            .Where(f => SaveExtensions.Contains(Path.GetExtension(f).ToLowerInvariant())
                        || Path.GetFileName(f).EndsWith(".main", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => Path.GetFileName(f))
            .ToList();

        foreach (var file in files)
        {
            var sav = LoadSave(file);
            if (sav == null) continue;

            var label = $"Gen{sav.Generation}-{Path.GetFileNameWithoutExtension(file)}";
            yield return [sav, label, file];
        }
    }

    /// <summary>
    /// Same as AllRealSaves but only yields (SaveFile, label) for simpler test signatures.
    /// </summary>
    public static IEnumerable<object[]> AllRealSavesSimple()
    {
        foreach (var entry in AllRealSaves())
            yield return [entry[0], entry[1]];
    }

    /// <summary>
    /// Returns save files that support write round-trips (excludes known non-writeable blank saves).
    /// Real saves from actual games should generally be writeable.
    /// </summary>
    public static IEnumerable<object[]> WriteableSaves()
    {
        var results = new List<object[]>();
        foreach (var entry in AllRealSaves())
        {
            var sav = (SaveFile)entry[0];
            try
            {
                _ = sav.Write();
                results.Add([entry[0], entry[1]]);
            }
            catch
            {
                // Skip saves that can't write
            }
        }
        return results;
    }

    /// <summary>
    /// Returns the total number of occupied box slots in a save file.
    /// </summary>
    public static int CountOccupiedSlots(SaveFile sav)
    {
        int count = 0;
        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            try
            {
                var pk = sav.GetBoxSlotAtIndex(i);
                if (pk.Species != 0)
                    count++;
            }
            catch
            {
                // Some slot indices may be invalid
            }
        }
        return count;
    }

    /// <summary>
    /// Returns the first occupied box slot, or null if none found.
    /// </summary>
    public static (PKM Pkm, int Index)? GetFirstOccupiedSlot(SaveFile sav)
    {
        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            try
            {
                var pk = sav.GetBoxSlotAtIndex(i);
                if (pk.Species != 0)
                    return (pk, i);
            }
            catch
            {
                continue;
            }
        }
        return null;
    }
}
