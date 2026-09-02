using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Facilitates interaction with a <see cref="SaveFile"/> or other data location's slot data.
/// </summary>
public sealed class SlotEditor<T>(SaveFile SAV)
{
    public readonly SlotChangelog Changelog = new(SAV);
    public readonly SlotPublisher<T> Publisher = new();

    private void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pk) => Publisher.NotifySlotChanged(slot, type, pk);

    /// <summary>
    /// Notifies subscribers that a slot was modified externally.
    /// </summary>
    public void UpdateSlot(ISlotInfo slot)
    {
        var pk = slot.Read(SAV);
        NotifySlotChanged(slot, SlotTouchType.Set, pk);
    }

    /// <summary>
    /// Gets data from a slot.
    /// </summary>
    /// <param name="slot">Slot to retrieve from.</param>
    /// <returns>Operation succeeded or not via enum value.</returns>
    public PKM Get(ISlotInfo slot)
    {
        // Reading from a slot is always allowed.
        var pk = slot.Read(SAV);
        NotifySlotChanged(slot, SlotTouchType.Get, pk);
        return pk;
    }

    /// <summary>
    /// Sets data to a slot.
    /// </summary>
    /// <param name="slot">Slot to be set to.</param>
    /// <param name="pk">Data to set.</param>
    /// <param name="type">Type of slot action</param>
    /// <returns>Operation succeeded or not via enum value.</returns>
    public SlotTouchResult Set(ISlotInfo slot, PKM pk, SlotTouchType type = SlotTouchType.Set)
    {
        if (!slot.CanWriteTo(SAV))
            return SlotTouchResult.FailWrite;

        using var change = Changelog.Begin(slot);
        var settings = type != SlotTouchType.Set ? EntityImportSettings.None : default;
        if (!slot.WriteTo(SAV, pk, settings))
            return SlotTouchResult.FailWrite;

        change.Commit();
        NotifySlotChanged(slot, type, pk);
        return SlotTouchResult.Success;
    }

    /// <summary>
    /// Deletes a slot.
    /// </summary>
    /// <param name="slot">Slot to be deleted.</param>
    /// <returns>Operation succeeded or not via enum value.</returns>
    public SlotTouchResult Delete(ISlotInfo slot)
    {
        if (!slot.CanWriteTo(SAV))
            return SlotTouchResult.FailDelete;

        var pk = SAV.BlankPKM;
        var settings = EntityImportSettings.None;

        using var change = Changelog.Begin(slot);

        if (!slot.WriteTo(SAV, pk, settings))
            return SlotTouchResult.FailDelete;

        change.Commit();
        NotifySlotChanged(slot, SlotTouchType.Delete, pk);
        return SlotTouchResult.Success;
    }

    /// <summary>
    /// Swaps two slots as one undoable operation.
    /// </summary>
    /// <param name="source">Source slot to be switched with <see cref="dest"/>.</param>
    /// <param name="dest">Destination slot to be switched with <see cref="source"/>.</param>
    /// <returns>Operation succeeded or not via enum value.</returns>
    public SlotTouchResult Swap(ISlotInfo source, ISlotInfo dest)
    {
        if (!source.CanWriteTo(SAV))
            return SlotTouchResult.FailSource;
        if (!dest.CanWriteTo(SAV))
            return SlotTouchResult.FailDestination;

        var settings = EntityImportSettings.None;

        var sourcePK = source.Read(SAV);
        var destPK = dest.Read(SAV);

        using var change = Changelog.Begin([source, dest]);

        if (!source.WriteTo(SAV, destPK, settings))
            return SlotTouchResult.FailSource;

        if (!dest.WriteTo(SAV, sourcePK, settings))
            return SlotTouchResult.FailDestination;

        change.Commit();

        NotifySlotChanged(source, SlotTouchType.Swap, destPK);
        NotifySlotChanged(dest, SlotTouchType.Swap, sourcePK);

        return SlotTouchResult.Success;
    }

    /// <summary>
    /// Performs a batch operation against multiple slots as one undoable operation.
    /// </summary>
    /// <param name="slots">Slots affected by the operation.</param>
    /// <param name="action">
    /// Action which performs the actual modifications. The slots have already
    /// been captured by the changelog when this action executes.
    /// </param>
    public bool Batch(IEnumerable<ISlotInfo> slots, Action<IReadOnlyList<ISlotInfo>> action)
    {
        var affected = slots.ToArray();

        if (affected.Length == 0)
            return false;

        foreach (var slot in affected)
        {
            if (!slot.CanWriteTo(SAV))
                return false;
        }

        using var change = Changelog.Begin(affected);

        action(affected);
        // Disposing `change` rolls back the captured state, even if the action throws.

        change.Commit();
        foreach (var slot in affected)
        {
            var pk = slot.Read(SAV);
            NotifySlotChanged(slot, SlotTouchType.Set, pk);
        }

        return true;
    }

    /// <summary>
    /// Undoes the last change and notifies every affected slot.
    /// </summary>
    public void Undo()
    {
        if (!Changelog.CanUndo)
            return;

        var slots = Changelog.Undo();

        foreach (var slot in slots)
        {
            var pk = slot.Read(SAV);
            NotifySlotChanged(slot, SlotTouchType.Undo, pk);
        }
    }

    /// <summary>
    /// Redoes the last undone change and notifies every affected slot.
    /// </summary>
    public void Redo()
    {
        if (!Changelog.CanRedo)
            return;

        var slots = Changelog.Redo();

        foreach (var slot in slots)
        {
            var pk = slot.Read(SAV);
            NotifySlotChanged(slot, SlotTouchType.Redo, pk);
        }
    }
}
