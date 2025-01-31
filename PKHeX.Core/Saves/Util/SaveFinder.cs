using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for detecting a <see cref="SaveFile"/> from various locations on the host machine.
/// </summary>
public static class SaveFinder
{
    /// <summary>
    /// Searches the provided <see cref="drives"/> to find a valid 3DS drive, usually from an inserted SD card.
    /// </summary>
    /// <param name="drives">List of drives on the host machine.</param>
    /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
    /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
    /// <returns>Folder path pointing to the Nintendo 3DS folder.</returns>
    public static string? Get3DSLocation(IEnumerable<string> drives, bool skipFirstDrive = true) =>
        FindConsoleRootFolder(drives, "Nintendo 3DS", skipFirstDrive);

    /// <summary>
    /// Searches the provided <see cref="drives"/> to find a valid Switch drive, usually from an inserted SD card.
    /// </summary>
    /// <param name="drives">List of drives on the host machine.</param>
    /// <param name="skipFirstDrive">Optional parameter to skip the first drive.
    /// The first drive is usually the system hard drive, or can be a floppy disk drive (slower to check, never has expected data).</param>
    /// <returns>Folder path pointing to the Nintendo folder.</returns>
    public static string? GetSwitchLocation(IEnumerable<string> drives, bool skipFirstDrive = true) =>
        FindConsoleRootFolder(drives, "Nintendo", skipFirstDrive);

    private static string? FindConsoleRootFolder(IEnumerable<string> drives, [ConstantExpected] string path, bool skipFirstDrive)
    {
        if (skipFirstDrive)
            drives = drives.Skip(1);

        var paths = drives.Select(drive => Path.Combine(drive, path));
        return paths.FirstOrDefault(Directory.Exists);
    }

    /// <summary>
    /// Gets a list of 3DS save backup paths for the storage device.
    /// </summary>
    /// <param name="root">Root location of device</param>
    /// <returns>List of possible 3DS save backup paths.</returns>
    public static IEnumerable<string> Get3DSBackupPaths(string root)
    {
        yield return Path.Combine(root, "saveDataBackup");
        yield return Path.Combine(root, "filer", "UserSaveData");
        yield return Path.Combine(root, "JKSV", "Saves");
        yield return Path.Combine(root, "TWLSaveTool");
        yield return Path.Combine(root, "fbi", "save");
        yield return Path.Combine(root, "gm9", "out");
        yield return Path.Combine(root, "3ds", "Checkpoint", "saves");
    }

    /// <summary>
    /// Gets a list of Switch save backup paths for the storage device.
    /// </summary>
    /// <param name="root">Root location of device</param>
    /// <returns>List of possible 3DS save backup paths.</returns>
    public static IEnumerable<string> GetSwitchBackupPaths(string root)
    {
        yield return Path.Combine(root, "switch", "Checkpoint", "saves");
        yield return Path.Combine(root, "JKSV");
    }

    /// <summary>
    /// Extra list of Backup Paths used for detecting a save file.
    /// </summary>
    public static readonly List<string> CustomBackupPaths = [];

    /// <summary>
    /// Finds a compatible save file that was most recently saved (by file write time).
    /// </summary>
    /// <param name="drives">List of drives on the host machine.</param>
    /// <param name="extra">Paths to check in addition to the default paths</param>
    /// <returns>Reference to a valid save file, if any.</returns>
    public static SaveFile? FindMostRecentSaveFile(IReadOnlyList<string> drives, params string[] extra)
        => FindMostRecentSaveFile(drives, (IEnumerable<string>)extra);

    /// <summary>
    /// Finds a compatible save file that was most recently saved (by file write time).
    /// </summary>
    /// <param name="drives">List of drives on the host machine.</param>
    /// <param name="extra">Paths to check in addition to the default paths</param>
    /// <returns>Reference to a valid save file, if any.</returns>
    public static SaveFile? FindMostRecentSaveFile(IReadOnlyList<string> drives, IEnumerable<string> extra)
    {
        var foldersToCheck = GetFoldersToCheck(drives, extra);
        var result = GetSaveFilePathsFromFolders(foldersToCheck, true, out var possiblePaths);
        if (!result)
            throw new FileNotFoundException(string.Join(Environment.NewLine, possiblePaths)); // `possiblePaths` contains the error message

        // return newest save file path that is valid
        var byMostRecent = possiblePaths.OrderByDescending(File.GetLastWriteTimeUtc);
        var saves = byMostRecent.Select(SaveUtil.GetVariantSAV);
        return saves.FirstOrDefault(z => z?.ChecksumsValid == true);
    }

    /// <summary>
    /// Gets all detectable save files ordered by most recently saved (by file write time).
    /// </summary>
    /// <param name="drives">List of drives on the host machine.</param>
    /// <param name="detect">Detect save files stored in common SD card homebrew locations.</param>
    /// <param name="extra">Paths to check in addition to the default paths</param>
    /// <param name="ignoreBackups">Option to ignore backup files.</param>
    /// <returns>Valid save files, if any.</returns>
    public static IEnumerable<SaveFile> GetSaveFiles(IReadOnlyList<string> drives, bool detect, IEnumerable<string> extra, bool ignoreBackups)
    {
        var paths = detect ? GetFoldersToCheck(drives, extra) : extra;
        var result = GetSaveFilePathsFromFolders(paths, ignoreBackups, out var possiblePaths);
        if (!result)
            yield break;

        var byMostRecent = possiblePaths.OrderByDescending(File.GetLastWriteTimeUtc);
        foreach (var s in byMostRecent)
        {
            var sav = SaveUtil.GetVariantSAV(s);
            if (sav is not null)
                yield return sav;
        }
    }

    public static IEnumerable<string> GetFoldersToCheck(IReadOnlyList<string> drives, IEnumerable<string> extra)
    {
        var foldersToCheck = extra.Where(f => !string.IsNullOrWhiteSpace(f)).Concat(CustomBackupPaths);

        string? path3DS = Path.GetPathRoot(Get3DSLocation(drives));
        if (!string.IsNullOrEmpty(path3DS)) // check for Homebrew/CFW backups
            foldersToCheck = foldersToCheck.Concat(Get3DSBackupPaths(path3DS));

        string? pathNX = Path.GetPathRoot(GetSwitchLocation(drives));
        if (!string.IsNullOrEmpty(pathNX)) // check for Homebrew/CFW backups
            foldersToCheck = foldersToCheck.Concat(GetSwitchBackupPaths(pathNX));

        return foldersToCheck;
    }

    private static bool GetSaveFilePathsFromFolders(IEnumerable<string> foldersToCheck, bool ignoreBackups, out IEnumerable<string> possible)
    {
        var possiblePaths = new List<string>();
        foreach (var folder in foldersToCheck)
        {
            if (!SaveUtil.GetSavesFromFolder(folder, true, out IEnumerable<string> files, ignoreBackups))
            {
                if (files is not string[] msg) // should always return string[]
                    continue;
                if (msg.Length == 0) // folder doesn't exist
                    continue;
                possible = msg;
                return false;
            }
            possiblePaths.AddRange(files);
        }
        possible = possiblePaths;
        return true;
    }

    /// <inheritdoc cref="FindMostRecentSaveFile(IReadOnlyList{string},string[])"/>
    public static SaveFile? FindMostRecentSaveFile() => FindMostRecentSaveFile(Environment.GetLogicalDrives(), CustomBackupPaths);

    /// <inheritdoc cref="GetSaveFiles"/>
    public static IEnumerable<SaveFile> DetectSaveFiles() => GetSaveFiles(Environment.GetLogicalDrives(), true, CustomBackupPaths, true);

    /// <returns>
    /// True if a valid save file was found, false otherwise.
    /// </returns>
    /// <inheritdoc cref="FindMostRecentSaveFile(IReadOnlyList{string},string[])"/>
    public static bool TryDetectSaveFile([NotNullWhen(true)] out SaveFile? sav) => TryDetectSaveFile(Environment.GetLogicalDrives(), out sav);

    /// <inheritdoc cref="TryDetectSaveFile(out SaveFile)"/>
    public static bool TryDetectSaveFile(IReadOnlyList<string> drives, [NotNullWhen(true)] out SaveFile? sav)
    {
        sav = FindMostRecentSaveFile(drives, CustomBackupPaths);
        var path = sav?.Metadata.FilePath;
        return File.Exists(path);
    }
}
