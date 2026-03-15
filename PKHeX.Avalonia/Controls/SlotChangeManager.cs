using System;
using System.Diagnostics;
using Avalonia.Input;
using PKHeX.Avalonia.ViewModels;
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

    /// <summary>Tracks the source slot when a drag operation is in progress.</summary>
    private SlotModel? _sourceSlot;

    /// <summary>Indicates whether a drag operation is currently in progress.</summary>
    public bool IsDragInProgress { get; private set; }

    public SlotChangeManager(SAVEditorViewModel editor)
    {
        _editor = editor;
    }

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

        var pk = _editor.GetSlotPKM(slot);
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
        // Allow drop if payload has our PKM data
        if (e.Data.Contains(PKMFormat))
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
        if (!e.Data.Contains(PKMFormat))
            return;

        var sourceSlot = e.Data.Get(SlotFormat) as SlotModel;
        if (sourceSlot is null)
            return;

        // Same slot — nothing to do
        if (ReferenceEquals(sourceSlot, destSlot))
            return;

        var mod = GetDropModifier(e.KeyModifiers);
        PerformSlotOperation(sourceSlot, destSlot, mod);
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
    /// Executes the slot move/swap/clone operation.
    /// </summary>
    private void PerformSlotOperation(SlotModel source, SlotModel dest, DropModifier mod)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        var sourcePkm = _editor.GetSlotPKM(source);
        if (sourcePkm is null || sourcePkm.Species == 0)
            return;

        var destPkm = _editor.GetSlotPKM(dest);
        bool destIsEmpty = destPkm is null || destPkm.Species == 0;

        // Write source PKM to destination
        WriteSlot(dest, sourcePkm);

        // Handle source slot based on modifier
        switch (mod)
        {
            case DropModifier.Clone:
                // Clone: source stays intact, destination gets a copy
                break;

            case DropModifier.Overwrite:
                // Overwrite: clear source
                ClearSlot(source);
                break;

            default: // None
                if (destIsEmpty)
                {
                    // Move: clear source since destination was empty
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
    }

    /// <summary>
    /// Writes a PKM to the slot identified by the given <see cref="SlotModel"/>.
    /// </summary>
    private void WriteSlot(SlotModel slot, PKM pk)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        int boxIndex = _editor.BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            sav.SetBoxSlotAtIndex(pk, _editor.CurrentBox, boxIndex);
            return;
        }

        int partyIndex = _editor.PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            sav.SetPartySlotAtIndex(pk, partyIndex);
        }
    }

    /// <summary>
    /// Clears the slot identified by the given <see cref="SlotModel"/>.
    /// </summary>
    private void ClearSlot(SlotModel slot)
    {
        var sav = _editor.SAV;
        if (sav is null)
            return;

        int boxIndex = _editor.BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            sav.SetBoxSlotAtIndex(sav.BlankPKM, _editor.CurrentBox, boxIndex);
            return;
        }

        int partyIndex = _editor.PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            sav.DeletePartySlot(partyIndex);
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
