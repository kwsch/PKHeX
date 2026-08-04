using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    private void OnBoxSlotActivated(int box, int slot) => OnBoxViewSlot(box, slot);
    private void OnPartySlotActivated(int slot) => OnPartyViewSlot(slot);

    private void OnViewRequested(SlotLocation location)
    {
        if (location.IsParty) OnPartyViewSlot(location.Slot);
        else OnBoxViewSlot(location.Box, location.Slot);
    }

    private void OnSetRequested(SlotLocation location)
    {
        if (location.IsParty) OnPartySetSlot(location.Slot);
        else OnBoxSetSlot(location.Box, location.Slot);
    }

    private void OnDeleteRequested(SlotLocation location)
    {
        if (location.IsParty) OnPartyDeleteSlot(location.Slot);
        else OnBoxDeleteSlot(location.Box, location.Slot);
    }

    private void OnMoveRequested(SlotLocation source, SlotLocation destination, bool clone)
    {
        if (CurrentSave is null) return;

        var pkSource = source.IsParty
            ? CurrentSave.GetPartySlotAtIndex(source.Slot)
            : CurrentSave.GetBoxSlotAtIndex(source.Box, source.Slot);

        if (pkSource.Species == 0) return;

        if (clone)
        {
            if (destination.IsParty)
                CurrentSave.SetPartySlotAtIndex(pkSource.Clone(), destination.Slot);
            else
                CurrentSave.SetBoxSlotAtIndex(pkSource.Clone(), destination.Box, destination.Slot);
        }
        else
        {
            var pkDest = destination.IsParty
                ? CurrentSave.GetPartySlotAtIndex(destination.Slot)
                : CurrentSave.GetBoxSlotAtIndex(destination.Box, destination.Slot);

            if (source.IsParty)
                CurrentSave.SetPartySlotAtIndex(pkDest, source.Slot);
            else
                CurrentSave.SetBoxSlotAtIndex(pkDest, source.Box, source.Slot);

            if (destination.IsParty)
                CurrentSave.SetPartySlotAtIndex(pkSource, destination.Slot);
            else
                CurrentSave.SetBoxSlotAtIndex(pkSource, destination.Box, destination.Slot);
        }

        BoxViewer?.RefreshCurrentBox();
        PartyViewer?.RefreshParty();
    }

    private void OnBoxViewSlot(int box, int slot)
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;
        var pk = CurrentSave.GetBoxSlotAtIndex(box, slot);
        if (pk.Species != 0) CurrentPokemonEditor.LoadPKM(pk);
    }

    private void OnBoxSetSlot(int box, int slot)
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;
        var prepared = CurrentPokemonEditor.PreparePKM();
        if (!TryGetCompatible(prepared, out var pk))
        {
            _ = _dialogService.ShowErrorAsync(T("Slot_IncompatibleFormatTitle"), T("Slot_IncompatibleFormatMessage"));
            return;
        }
        CurrentSave.SetBoxSlotAtIndex(pk, box, slot);
        BoxViewer?.RefreshCurrentBox();
    }

    private void OnBoxDeleteSlot(int box, int slot)
    {
        if (CurrentSave is null) return;
        if (CurrentSave.GetBoxSlotAtIndex(box, slot).Species == 0) return;
        CurrentSave.SetBoxSlotAtIndex(CurrentSave.BlankPKM, box, slot);
        BoxViewer?.RefreshCurrentBox();
    }

    private void OnPartyViewSlot(int slot)
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;
        var pk = CurrentSave.GetPartySlotAtIndex(slot);
        if (pk.Species != 0) CurrentPokemonEditor.LoadPKM(pk);
    }

    private void OnPartySetSlot(int slot)
    {
        if (CurrentSave is null || PartyViewer is null || CurrentPokemonEditor is null) return;
        var prepared = CurrentPokemonEditor.PreparePKM();
        if (!TryGetCompatible(prepared, out var pk))
        {
            _ = _dialogService.ShowErrorAsync(T("Slot_IncompatibleFormatTitle"), T("Slot_IncompatibleFormatMessage"));
            return;
        }
        CurrentSave.SetPartySlotAtIndex(pk, slot);
        PartyViewer.RefreshParty();
    }

    /// <summary>
    /// Ensures <paramref name="pk"/> is a type the currently open save file accepts, converting
    /// cross-format entities (e.g. a PK3 loaded via the Encounter Database into a Gen-4 save) when
    /// possible. Returns false if no compatible conversion exists, so the caller can show an error
    /// instead of letting <see cref="SaveFile.SetBoxSlotAtIndex"/>/<see cref="SaveFile.SetPartySlotAtIndex"/>
    /// throw on a format mismatch (see GitHub issue #163).
    /// </summary>
    private bool TryGetCompatible(PKM pk, out PKM result)
    {
        if (CurrentSave is null || pk.GetType() == CurrentSave.PKMType)
        {
            result = pk;
            return true;
        }

        var converted = CurrentSave.GetCompatiblePKM(pk);
        if (converted.Species == 0 && pk.Species != 0)
        {
            result = pk;
            return false;
        }

        result = converted;
        return true;
    }

    private void OnPartyDeleteSlot(int slot)
    {
        if (CurrentSave is null || PartyViewer is null) return;
        _ = _dialogService.ShowErrorAsync(T("Common_Delete"), T("Msg_CannotDeletePartyPokemon"));
    }

    private void OnBatchEditCompleted()
    {
        BoxViewer?.RefreshCurrentBox();
        PartyViewer?.RefreshParty();
    }

    private void OnUndoRedoPerformed(ISlotInfo info)
    {
        if (info is SlotInfoBox) BoxViewer?.RefreshCurrentBox();
        else if (info is SlotInfoParty) PartyViewer?.RefreshParty();
    }
}
