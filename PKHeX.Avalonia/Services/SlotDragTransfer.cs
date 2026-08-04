using System;
using System.IO;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using PKHeX.Application.Abstractions;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using PKHeX.Presentation.Models;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Builds and reads the <see cref="DataTransfer"/> payload used for Pokémon slot
/// drag-and-drop. Avalonia 11.3's data-transfer model only supports byte/string
/// application formats (it no longer carries arbitrary CLR objects), so the
/// <see cref="SlotLocation"/> is serialized to a compact string.
/// </summary>
internal static class SlotDragTransfer
{
    private static readonly DataFormat<string> Format =
        DataFormat.CreateStringApplicationFormat("PKHeX.SlotDragData");

    /// <summary>Creates the drag payload for the given slot data.</summary>
    public static DataTransfer Create(SlotDragData data)
    {
        var transfer = new DataTransfer();
        transfer.Add(DataTransferItem.Create(Format, Serialize(data.Source)));
        return transfer;
    }

    /// <summary>
    /// Creates the drag payload for the given slot, additionally attaching a decrypted entity
    /// file (e.g. ".pk9") so the OS receives a real file when the slot is dragged out to the
    /// desktop/Finder/Explorer.
    ///
    /// This is intentionally synchronous: on macOS, <c>DragDrop.DoDragDropAsync</c> must be
    /// invoked from within the live pointer-moved frame with no prior <c>await</c>, otherwise
    /// AppKit's <c>[NSApp currentEvent]</c> is no longer the originating mouse-down event and the
    /// native drag session silently fails to start. The temp file write is a few hundred bytes
    /// (fine as a blocking call), and <see cref="IStorageProvider.TryGetFileFromPathAsync"/> is
    /// only consulted if it happens to complete synchronously; otherwise the OS file attachment
    /// is skipped and the payload degrades gracefully to an in-app-only drag (no exception).
    /// </summary>
    public static DataTransfer Create(SlotDragData data, PKM? pk, IStorageProvider? storageProvider)
    {
        var transfer = Create(data);

        if (pk is null || storageProvider is null)
            return transfer;

        var exported = new ExportEntityToFileUseCase().Execute(pk);
        if (exported is not { } file)
            return transfer;

        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), file.FileName);
            File.WriteAllBytes(tempPath, file.Data);

            var fileTask = storageProvider.TryGetFileFromPathAsync(new Uri(tempPath));
            var storageFile = fileTask.IsCompletedSuccessfully ? fileTask.Result : null;
            if (storageFile is not null)
                transfer.Add(DataTransferItem.CreateFile(storageFile));
        }
        catch
        {
            // OS file drag-out isn't supported on this platform/backend; the in-app drag payload
            // added above still allows box <-> party moves to work.
        }

        return transfer;
    }

    /// <summary>Reads the slot data back from a drop, or null if it isn't present/valid.</summary>
    public static SlotDragData? TryGet(IDataTransfer? transfer)
    {
        if (transfer?.TryGetValue(Format) is { } raw && TryDeserialize(raw, out var source))
            return new SlotDragData(source);
        return null;
    }

    private static string Serialize(SlotLocation loc)
        => $"{(loc.IsParty ? 1 : 0)}:{loc.Box}:{loc.Slot}";

    private static bool TryDeserialize(string raw, out SlotLocation source)
    {
        source = default;
        var parts = raw.Split(':');
        if (parts.Length != 3
            || !int.TryParse(parts[0], out var isParty)
            || !int.TryParse(parts[1], out var box)
            || !int.TryParse(parts[2], out var slot))
        {
            return false;
        }

        source = new SlotLocation { Box = box, Slot = slot, IsParty = isParty != 0 };
        return true;
    }
}
