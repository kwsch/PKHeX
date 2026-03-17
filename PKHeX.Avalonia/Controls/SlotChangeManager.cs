using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Core;

namespace PKHeX.Avalonia.Controls;

/// <summary>
/// Manages drag-and-drop operations between box/party slots in the Avalonia UI.
/// Equivalent of <c>SlotChangeManager</c> from WinForms.
/// </summary>
public sealed class SlotChangeManager
{
    /// <summary>Custom data format key for PKM drag payloads.</summary>
    private const string PKMFormat = "PKHeX_PKM";

    /// <summary>Custom data format key for the source slot model.</summary>
    private const string SlotFormat = "PKHeX_Slot";

    private readonly SAVEditorViewModel _editor;

    /// <summary>Registered BoxViewer VMs whose slots participate in drag-and-drop.</summary>
    private readonly List<BoxViewerViewModel> _boxViewers = [];

    /// <summary>Tracks the source slot when a drag operation is in progress.</summary>
    private SlotModel? _sourceSlot;

    /// <summary>Indicates whether a drag operation is currently in progress.</summary>
    public bool IsDragInProgress { get; private set; }

    /// <summary>Gets the <see cref="SAVEditorViewModel"/> that owns this manager.</summary>
    public SAVEditorViewModel Editor => _editor;

    public SlotChangeManager(SAVEditorViewModel editor)
    {
        _editor = editor;
    }

    /// <summary>
    /// Registers a <see cref="BoxViewerViewModel"/> so its slots can participate in drag-and-drop.
    /// </summary>
    public void RegisterBoxViewer(BoxViewerViewModel viewer) => _boxViewers.Add(viewer);

    /// <summary>
    /// Unregisters a <see cref="BoxViewerViewModel"/> when its window is closed.
    /// </summary>
    public void UnregisterBoxViewer(BoxViewerViewModel viewer) => _boxViewers.Remove(viewer);

    /// <summary>
    /// Initiates a drag operation from the given slot.
    /// Call this from <see cref="Avalonia.Input.Pointer"/> move after a press.
    /// </summary>
    /// <param name="slot">The slot model being dragged.</param>
    /// <param name="e">The pointer event that triggered the drag.</param>
    /// <returns>The drag-drop result, or <c>null</c> if the drag was not started.</returns>
    public async System.Threading.Tasks.Task<DragDropEffects?> StartDragAsync(SlotModel slot, PointerEventArgs e)
    {
        if (IsDragInProgress)
            return null;

        var pk = GetSlotPKM(slot);
        if (pk is null || pk.Species == 0)
            return null;

        IsDragInProgress = true;
        _sourceSlot = slot;

        try
        {
            var data = new DataObject();
            data.Set(PKMFormat, pk.DecryptedBoxData);
            data.Set(SlotFormat, slot);

            var result = await DragDrop.DoDragDrop(e, data, DragDropEffects.Move | DragDropEffects.Copy);
            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Drag error: {ex.Message}");
            return null;
        }
        finally
        {
            IsDragInProgress = false;
            _sourceSlot = null;
        }
    }

    /// <summary>
    /// Handles the DragOver event to provide visual feedback.
    /// </summary>
    public void HandleDragOver(DragEventArgs e)
    {
        // Allow drop if payload has our PKM data or is a file drop
        if (e.Data.Contains(PKMFormat) || e.Data.Contains(DataFormats.Files))
            e.DragEffects = DragDropEffects.Move | DragDropEffects.Copy;
        else
            e.DragEffects = DragDropEffects.None;
    }

    /// <summary>
    /// Handles the Drop event on a destination slot.
    /// </summary>
    /// <param name="destSlot">The slot model being dropped onto.</param>
    /// <param name="e">The drag event args.</param>
    public void HandleDrop(SlotModel destSlot, DragEventArgs e)
    {
        // Handle internal PKM drag-and-drop
        if (e.Data.Contains(PKMFormat))
        {
            var sourceSlot = e.Data.Get(SlotFormat) as SlotModel;
            if (sourceSlot is null)
                return;

            // Same slot — nothing to do
            if (ReferenceEquals(sourceSlot, destSlot))
                return;

            var mod = GetDropModifier(e.KeyModifiers);
            PerformSlotOperation(sourceSlot, destSlot, mod);
            return;
        }

        // Handle external file drop (.pk* files)
        if (e.Data.Contains(DataFormats.Files))
        {
            HandleFileDrop(destSlot, e);
        }
    }

    /// <summary>
    /// Handles dropping a .pk* file onto a slot from an external source (file manager, PKM editor, etc.).
    /// </summary>
    private void HandleFileDrop(SlotModel destSlot, DragEventArgs e)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        try
        {
            var files = e.Data.GetFiles()?.Select(f => f.Path.LocalPath).ToArray();
            if (files is not { Length: > 0 })
                return;

            var filePath = files[0];
            if (!File.Exists(filePath))
                return;

            var data = File.ReadAllBytes(filePath);
            var pk = EntityFormat.GetFromBytes(data, prefer: sav.Context);
            if (pk is null)
            {
                _editor.SetStatusMessage?.Invoke($"Could not parse PKM from: {Path.GetFileName(filePath)}");
                return;
            }

            // Convert to the save file's expected format if needed
            pk = EntityConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out var result);
            if (pk is null)
            {
                _editor.SetStatusMessage?.Invoke($"Conversion failed: {result}");
                return;
            }

            var undoEntry = CreateUndoEntry(destSlot);
            if (undoEntry is not null)
                _editor.PushUndo(undoEntry);
            WriteSlot(destSlot, pk);
            _editor.ReloadSlots();
            // Refresh box viewer if the drop target was in one
            foreach (var bv in _boxViewers)
            {
                if (bv.BoxSlots.Contains(destSlot))
                    bv.RefreshBox();
            }
            _editor.SetStatusMessage?.Invoke($"Loaded {Path.GetFileName(filePath)} into slot.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"File drop error: {ex.Message}");
            _editor.SetStatusMessage?.Invoke($"File drop error: {ex.Message}");
        }
    }

    /// <summary>
    /// Determines the drop modifier based on keyboard state.
    /// Shift = clone (copy), Alt = overwrite (move without swap).
    /// Default = swap if destination is occupied, otherwise move.
    /// </summary>
    private static DropModifier GetDropModifier(KeyModifiers keys)
    {
        if ((keys & KeyModifiers.Shift) != 0)
            return DropModifier.Clone;
        if ((keys & KeyModifiers.Alt) != 0)
            return DropModifier.Overwrite;
        return DropModifier.None;
    }

    /// <summary>
    /// Resolved location of a <see cref="SlotModel"/> within the save file.
    /// </summary>
    private readonly record struct ResolvedSlot(int Box, int Index, bool IsParty, BoxViewerViewModel? BoxViewer);

    /// <summary>
    /// Resolves a <see cref="SlotModel"/> to its (box, index) coordinate by checking
    /// the main editor's box/party slots and all registered box viewers.
    /// </summary>
    private ResolvedSlot? ResolveSlot(SlotModel slot)
    {
        // Check main editor box slots
        int boxIndex = _editor.BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
            return new ResolvedSlot(_editor.CurrentBox, boxIndex, false, null);

        // Check main editor party slots
        int partyIndex = _editor.PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
            return new ResolvedSlot(0, partyIndex, true, null);

        // Check registered box viewers
        foreach (var bv in _boxViewers)
        {
            int bvIndex = bv.BoxSlots.IndexOf(slot);
            if (bvIndex >= 0)
                return new ResolvedSlot(bv.CurrentBox, bvIndex, false, bv);
        }

        return null;
    }

    /// <summary>
    /// Gets the PKM at the resolved slot location.
    /// </summary>
    private PKM? GetSlotPKM(SlotModel slot)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return null;

        var resolved = ResolveSlot(slot);
        if (resolved is null)
            return null;

        var r = resolved.Value;
        if (r.IsParty)
            return sav.GetPartySlotAtIndex(r.Index);
        return sav.GetBoxSlotAtIndex(r.Box, r.Index);
    }

    /// <summary>
    /// Executes the slot move/swap/clone operation.
    /// </summary>
    private void PerformSlotOperation(SlotModel source, SlotModel dest, DropModifier mod)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        var sourcePkm = GetSlotPKM(source);
        if (sourcePkm is null || sourcePkm.Species == 0)
            return;

        var destPkm = GetSlotPKM(dest);
        bool destIsEmpty = destPkm is null || destPkm.Species == 0;

        // Collect undo entries and push as a single atomic group
        var entries = new List<SAVEditorViewModel.SlotChangeEntry>();
        var destEntry = CreateUndoEntry(dest);
        if (destEntry is not null)
            entries.Add(destEntry);
        if (mod != DropModifier.Clone)
        {
            var srcEntry = CreateUndoEntry(source);
            if (srcEntry is not null)
                entries.Add(srcEntry);
        }
        if (entries.Count > 0)
            _editor.PushUndo(entries.ToArray());

        // Write source PKM to destination
        WriteSlot(dest, sourcePkm);

        // Handle source slot based on modifier
        switch (mod)
        {
            case DropModifier.Clone:
                // Clone: source stays intact, destination gets a copy
                break;

            case DropModifier.Overwrite:
                // Don't clear source if it's the last party member
                {
                    var sourceResolved = ResolveSlot(source);
                    bool isParty = sourceResolved is { IsParty: true };
                    if (!isParty || sav.PartyCount > 1)
                        ClearSlot(source);
                }
                break;

            default: // None
                if (destIsEmpty)
                {
                    // Don't clear source if it's the last party member
                    var sourceResolved = ResolveSlot(source);
                    bool isParty = sourceResolved is { IsParty: true };
                    if (!isParty || sav.PartyCount > 1)
                        ClearSlot(source);
                }
                else
                {
                    // Swap: put destination PKM into source
                    WriteSlot(source, destPkm!);
                }
                break;
        }

        _editor.ReloadSlots();
        // Refresh any box viewers that were involved
        RefreshBoxViewers(source, dest);
    }

    /// <summary>
    /// Refreshes box viewers whose slots were involved in a drag-drop operation.
    /// </summary>
    private void RefreshBoxViewers(SlotModel source, SlotModel dest)
    {
        foreach (var bv in _boxViewers)
        {
            if (bv.BoxSlots.Contains(source) || bv.BoxSlots.Contains(dest))
                bv.RefreshBox();
        }
    }

    /// <summary>
    /// Creates a <see cref="SAVEditorViewModel.SlotChangeEntry"/> capturing the current state of the given slot,
    /// or <c>null</c> if the slot cannot be resolved.
    /// </summary>
    private SAVEditorViewModel.SlotChangeEntry? CreateUndoEntry(SlotModel slot)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return null;

        var resolved = ResolveSlot(slot);
        if (resolved is null)
            return null;

        var r = resolved.Value;
        if (r.IsParty)
        {
            var existing = sav.GetPartySlotAtIndex(r.Index);
            if (existing is null) return null;
            return new SAVEditorViewModel.SlotChangeEntry(0, r.Index, existing.DecryptedBoxData, true);
        }
        else
        {
            var existing = sav.GetBoxSlotAtIndex(r.Box, r.Index);
            if (existing is null) return null;
            return new SAVEditorViewModel.SlotChangeEntry(r.Box, r.Index, existing.DecryptedBoxData, false);
        }
    }

    /// <summary>
    /// Writes a PKM to the slot identified by the given <see cref="SlotModel"/>.
    /// </summary>
    private void WriteSlot(SlotModel slot, PKM pk)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        var resolved = ResolveSlot(slot);
        if (resolved is null)
            return;

        var r = resolved.Value;
        if (r.IsParty)
            sav.SetPartySlotAtIndex(pk, r.Index);
        else
            sav.SetBoxSlotAtIndex(pk, r.Box, r.Index);
    }

    /// <summary>
    /// Clears the slot identified by the given <see cref="SlotModel"/>.
    /// </summary>
    private void ClearSlot(SlotModel slot)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        var resolved = ResolveSlot(slot);
        if (resolved is null)
            return;

        var r = resolved.Value;
        if (r.IsParty)
        {
            if (sav.PartyCount <= 1)
                return;
            sav.DeletePartySlot(r.Index);
        }
        else
        {
            sav.SetBoxSlotAtIndex(sav.BlankPKM, r.Box, r.Index);
        }
    }
}

/// <summary>
/// Specifies the modifier for a PKM drag-and-drop operation.
/// </summary>
public enum DropModifier
{
    /// <summary>No modifier — swap if occupied, move if empty.</summary>
    None,

    /// <summary>Overwrite the target slot and clear the source.</summary>
    Overwrite,

    /// <summary>Clone the source slot to the destination (source remains).</summary>
    Clone,
}
