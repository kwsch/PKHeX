using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Tracks information about where the <see cref="SAV"/> originated from, and provides logic for saving to a file.
/// </summary>
public sealed record SaveFileMetadata(SaveFile SAV)
{
    public SaveFile SAV { private get; init; } = SAV;

    /// <summary>
    /// Full path where the <see cref="SAV"/> originated from.
    /// </summary>
    public string? FilePath { get; private set; }

    /// <summary>
    /// File Name of the <see cref="SAV"/>.
    /// </summary>
    /// <remarks>This is not always the original file name. We try to strip out the Backup name markings to get the original filename.</remarks>
    public string? FileName { get; private set; }

    /// <summary>
    /// Directory in which the <see cref="SAV"/> was saved in.
    /// </summary>
    public string? FileFolder { get; private set; }

    private byte[] Footer = []; // .dsv
    private byte[] Header = []; // .gci
    private ISaveHandler? Handler;

    private string BAKSuffix => $" [{SAV.ShortSummary}].bak";

    /// <summary>
    /// Simple summary of the save file, to help differentiate it from other save files with the same filename.
    /// </summary>
    public string BAKName => FileName + BAKSuffix;

    public bool HasHeader => Header.Length != 0;
    public bool HasFooter => Footer.Length != 0;

    /// <summary>
    /// File Dialog filter to help save the file.
    /// </summary>
    public string Filter => $"{SAV.GetType().Name}|{GetSuggestedExtension()}|All Files|*.*";

    /// <summary>
    /// Writes the input <see cref="data"/> and appends the <see cref="Header"/> and <see cref="Footer"/> if requested.
    /// </summary>
    /// <param name="data">Finalized save file data (with fixed checksums) to be written to a file</param>
    /// <param name="setting">Toggle flags </param>
    /// <returns>Final save file data.</returns>
    public byte[] Finalize(byte[] data, BinaryExportSetting setting)
    {
        if (HasFooter && !setting.HasFlag(BinaryExportSetting.ExcludeFooter))
            data = [..data, ..Footer];
        if (HasHeader && !setting.HasFlag(BinaryExportSetting.ExcludeHeader))
            data = [..Header, ..data];
        if (!setting.HasFlag(BinaryExportSetting.ExcludeFinalize))
            Handler?.Finalize(data);
        return data;
    }

    /// <summary>
    /// Sets the details of any trimmed header and footer arrays to a <see cref="SaveFile"/> object.
    /// </summary>
    public void SetExtraInfo(byte[] header, byte[] footer, ISaveHandler handler)
    {
        Header = header;
        Footer = footer;
        Handler = handler;
    }

    /// <summary>
    /// Sets the details of a path to a <see cref="SaveFile"/> object.
    /// </summary>
    /// <param name="path">Full Path of the file</param>
    public void SetExtraInfo(string path)
    {
        var sav = SAV;
        if (!sav.State.Exportable || string.IsNullOrWhiteSpace(path)) // Blank save file
        {
            sav.Metadata.SetAsBlank();
            return;
        }

        SetAsLoadedFile(path);
    }

    private void SetAsLoadedFile(string path)
    {
        FilePath = path;
        FileFolder = Path.GetDirectoryName(path);
        FileName = GetFileName(path, BAKSuffix);
    }

    private static string GetFileName(string path, string bak)
    {
        var fileName = Path.GetFileName(path);

        // Trim off existing backup name if present
        var bakName = Util.CleanFileName(bak);
        if (fileName.EndsWith(bakName, StringComparison.Ordinal))
            fileName = fileName[..^bakName.Length];

        if (fileName.StartsWith("savedata ", StringComparison.OrdinalIgnoreCase) && fileName.Contains(".bin"))
            return fileName[..8] + ".bin";
        if (fileName.StartsWith("main"))
            return "main";

        var extensions = CollectionsMarshal.AsSpan(CustomSaveExtensions);
        return TrimNames(fileName, extensions);
    }

    public static readonly List<string> CustomSaveExtensions =
    [
        "sav", // standard
        "dat", // VC data
        "gci", // Dolphin GameCubeImage
        "dsv", // DeSmuME
        "srm", // RetroArch save files
        "fla", // flash
        "SaveRAM", // BizHawk
    ];

    private static string TrimNames(string fileName, ReadOnlySpan<string> extensions)
    {
        foreach (var ext in extensions)
        {
            var index = fileName.LastIndexOf(ext, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;
            // Check for a period before the extension
            if (index == 0 || fileName[index - 1] != '.')
                continue;

            var result = fileName.AsSpan();
            result = result[..(index-1)];

            // Files can have (#) appended to them, so we need to trim that off
            var open = result.LastIndexOf('(');
            var close = result.LastIndexOf(')');
            if (open != -1 && close != -1 && close > open && char.IsDigit(result[open + 1]))
                result = result[..open].Trim();
            var copy = result.IndexOf(" - Copy", StringComparison.OrdinalIgnoreCase);
            if (copy == -1)
                copy = result.IndexOf("_-_Copy", StringComparison.OrdinalIgnoreCase);
            if (copy != -1)
                result = result[..copy].Trim();

            // Re-add the extension
            return $"{result}.{ext}";
        }
        return fileName;
    }

    public string GetBackupFileName(string destDir)
    {
        return Path.Combine(destDir, Util.CleanFileName(BAKName));
    }

    private void SetAsBlank()
    {
        FileFolder = FilePath = string.Empty;
        FileName = "Blank Save File";
    }

    /// <summary>
    /// Gets the suggested file extension when writing to a saved file.
    /// </summary>
    public string GetSuggestedExtension()
    {
        var sav = SAV;
        var fn = sav.Metadata.FileName;
        if (fn != null)
            return Path.GetExtension(fn);

        if ((sav.Generation is 4 or 5) && sav.Metadata.HasFooter)
            return ".dsv";
        return sav.Extension;
    }

    /// <summary>
    /// Gets suggested export options for the save file.
    /// </summary>
    /// <param name="ext">Selected export extension</param>
    public BinaryExportSetting GetSuggestedFlags(string? ext = null)
    {
        // Do everything as default
        var flags = BinaryExportSetting.None;

        if (FileName is not null)
        {
            // Try to support a couple formats changes that the user wants to remove from the file
            if (FileName.EndsWith(".dsv") && ext is not ".dsv")
                flags |= BinaryExportSetting.ExcludeFooter;
            else if (FileName.EndsWith(".gci") && ext is not ".gci")
                flags |= BinaryExportSetting.ExcludeHeader;
        }
        return flags;
    }
}
