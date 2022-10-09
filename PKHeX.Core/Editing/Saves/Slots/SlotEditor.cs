namespace PKHeX.Core;

/// <summary>
/// Facilitates interaction with a <see cref="SaveFile"/> or other data location's slot data.
/// </summary>
public sealed class SlotEditor<T>
{
    private readonly SaveFile SAV;
    public readonly SlotChangelog Changelog;
    public readonly SlotPublisher<T> Publisher;

    public SlotEditor(SaveFile sav)
    {
        SAV = sav;
        Changelog = new SlotChangelog(sav);
        Publisher = new SlotPublisher<T>();
    }

    private void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pk) => Publisher.NotifySlotChanged(slot, type, pk);

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

        WriteSlot(slot, pk, type);
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

        var pk = DeleteSlot(slot);
        NotifySlotChanged(slot, SlotTouchType.Delete, pk);

        return SlotTouchResult.Success;
    }

    /// <summary>
    /// Swaps two slots.
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

        NotifySlotChanged(source, SlotTouchType.None, source.Read(SAV));
        NotifySlotChanged(dest, SlotTouchType.Swap, dest.Read(SAV));

        return SlotTouchResult.Success;
    }

    private void WriteSlot(ISlotInfo slot, PKM pk, SlotTouchType type = SlotTouchType.Set)
    {
        Changelog.AddNewChange(slot);
        var setDetail = type is SlotTouchType.Swap ? PKMImportSetting.Skip : PKMImportSetting.UseDefault;
        var result = slot.WriteTo(SAV, pk, setDetail);
        if (result)
            NotifySlotChanged(slot, type, pk);
    }

    private PKM DeleteSlot(ISlotInfo slot)
    {
        var pk = SAV.BlankPKM;
        WriteSlot(slot, pk, SlotTouchType.Delete);
        return pk;
    }

    public void Undo()
    {
        if (!Changelog.CanUndo)
            return;
        var slot = Changelog.Undo();
        NotifySlotChanged(slot, SlotTouchType.Delete, slot.Read(SAV));
    }

    public void Redo()
    {
        if (!Changelog.CanRedo)
            return;
        var slot = Changelog.Redo();
        NotifySlotChanged(slot, SlotTouchType.Delete, slot.Read(SAV));
    }
}
