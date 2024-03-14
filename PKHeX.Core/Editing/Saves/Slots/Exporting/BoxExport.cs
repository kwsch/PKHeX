using System;
using System.IO;

namespace PKHeX.Core;

/// <summary>
/// Logic for exporting a <see cref="SaveFile"/> to a folder of <see cref="PKM"/> files.
/// </summary>
public static class BoxExport
{
    /// <summary>
    /// File namer to use for exporting if none is provided.
    /// </summary>
    private static IFileNamer<PKM> Default => EntityFileNamer.Namer;

    /// <summary>
    /// Export a box in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="box">Box to export</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, int box, BoxExportSettings settings)
        => Export(sav, destPath, box, Default, settings);

    /// <summary>
    /// Export a box in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="box">Box to export</param>
    /// <param name="namer">File namer to use for exporting</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, int box, IFileNamer<PKM> namer, BoxExportSettings settings)
        => ExportBox(sav, destPath, namer, box, settings, sav.BoxSlotCount, sav.SlotCount);

    /// <summary>
    /// Export all boxes in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="settings">Settings to use for exporting</param>
    public static int Export(SaveFile sav, string destPath, BoxExportSettings settings)
        => Export(sav, destPath, Default, settings);

    /// <summary>
    /// Export all boxes in the <see cref="SaveFile"/> to the specified folder.
    /// </summary>
    /// <param name="sav">Save file to export</param>
    /// <param name="destPath">Folder to export to</param>
    /// <param name="namer">File namer to use for exporting</param>
    /// <param name="settings">Settings to use for exporting</param>
    /// <returns>Number of files exported</returns>
    public static int Export(SaveFile sav, string destPath, IFileNamer<PKM> namer, BoxExportSettings settings)
    {
        if (!sav.HasBox)
            return 0;

        int total = sav.SlotCount;
        int boxSlotCount = sav.BoxSlotCount;

        var startBox = settings.Scope == BoxExportScope.Current ? sav.CurrentBox : 0;
        var endBox = settings.Scope == BoxExportScope.Current ? startBox + 1 : sav.BoxCount;

        var ctr = 0;
        // Export each box specified.
        for (int box = startBox; box < endBox; box++)
        {
            var boxFolder = destPath;
            if (settings.FolderCreation == BoxExportFolderMode.FolderEachBox)
            {
                var folderName = GetFolderName(sav, box, settings.FolderPrefix);
                boxFolder = Path.Combine(destPath, folderName);
                Directory.CreateDirectory(boxFolder);
            }
            ctr += ExportBox(sav, boxFolder, namer, box, settings, boxSlotCount, total);
        }
        return ctr;
    }

    private static int ExportBox(SaveFile sav, string destPath, IFileNamer<PKM> namer, int box, BoxExportSettings settings,
        int boxSlotCount, int total)
    {
        if (!Directory.Exists(destPath))
            Directory.CreateDirectory(destPath);

        int count = GetSlotCountForBox(boxSlotCount, box, total);
        int ctr = 0;
        // Export each slot in the box.
        for (int slot = 0; slot < count; slot++)
        {
            var pk = sav.GetBoxSlotAtIndex(box, slot);
            if (IsUndesirableForExport(pk))
            {
                if (settings.EmptySlots == BoxExportEmptySlots.Skip)
                    continue;
            }

            var fileName = GetFileName(pk, settings.FileIndexPrefix, namer, box, slot, boxSlotCount);
            var fn = Path.Combine(destPath, fileName);
            File.WriteAllBytes(fn, pk.DecryptedPartyData);
            ctr++;
        }
        return ctr;
    }

    private static bool IsUndesirableForExport(PKM pk) => pk.Species == 0 || !pk.Valid;

    private static int GetSlotCountForBox(int boxSlotCount, int box, int total)
    {
        // Account for any jagged-boxes with less than the usual number of slots.
        int absoluteStart = boxSlotCount * box;
        int count = boxSlotCount;
        if (absoluteStart + count > total)
            count = total - absoluteStart;
        return count;
    }

    private static string GetFolderName(SaveFile sav, int box, BoxExportFolderNaming mode)
    {
        var boxName = sav is IBoxDetailNameRead r ? r.GetBoxName(box) : BoxDetailNameExtensions.GetDefaultBoxName(box);
        boxName = Util.CleanFileName(boxName);
        return mode switch
        {
            BoxExportFolderNaming.BoxName => boxName,
            BoxExportFolderNaming.Index => $"{box + 1:00}",
            BoxExportFolderNaming.IndexBoxName => $"{box + 1:00} {boxName}",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    private static string GetFileName(PKM pk, BoxExportIndexPrefix mode, IFileNamer<PKM> namer, int box, int slot, int boxSlotCount)
    {
        var slotName = GetInnerName(namer, pk);
        var fileName = Util.CleanFileName(slotName);
        var prefix = GetPrefix(mode, box, slot, boxSlotCount);

        return $"{prefix}{fileName}.{pk.Extension}";
    }

    private static string GetPrefix(BoxExportIndexPrefix mode, int box, int slot, int boxSlotCount) => mode switch
    {
        BoxExportIndexPrefix.None => string.Empty,
        BoxExportIndexPrefix.InAll => $"{(box * boxSlotCount) + slot:0000} - ",
        BoxExportIndexPrefix.InBox => $"{slot:00} - ",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    private static string GetInnerName(IFileNamer<PKM> namer, PKM pk)
    {
        try
        {
            var slotName = namer.GetName(pk);
            return Util.CleanFileName(slotName);
        }
        catch { return "Name Error"; }
    }
}
