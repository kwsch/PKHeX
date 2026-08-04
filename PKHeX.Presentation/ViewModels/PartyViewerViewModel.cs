using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Presentation.Models;
using PKHeX.Presentation.Localization;
using PKHeX.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PKHeX.Presentation.ViewModels;

public partial class PartyViewerViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly ISlotService? _slotService;
    private readonly IDialogService? _dialogService;

    [ObservableProperty]
    private int _selectedIndex;

    [ObservableProperty]
    private ObservableCollection<PartySlotData> _slots = [];

    /// <summary>
    /// The slot under the current keyboard/selection cursor, used to drive
    /// keyboard-only Ctrl+C/Ctrl+V/Delete slot operations (no mouse required).
    /// </summary>
    public PartySlotData? SelectedSlot => SelectedIndex >= 0 && SelectedIndex < Slots.Count ? Slots[SelectedIndex] : null;

    public event Action<int>? SlotActivated;
    public event Action<int>? ViewSlotRequested;
    public event Action<int>? SetSlotRequested;
    public event Action<int>? DeleteSlotRequested;

    public PartyViewerViewModel(SaveFile sav, ISpriteRenderer spriteRenderer, ISlotService? slotService = null, IDialogService? dialogService = null)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _slotService = slotService;
        _dialogService = dialogService;
        LoadParty();
    }

    partial void OnSelectedIndexChanged(int value)
    {
        for (int i = 0; i < Slots.Count; i++)
            Slots[i].IsSelected = i == value;
        OnPropertyChanged(nameof(SelectedSlot));
    }

    private void LoadParty()
    {
        var previousIndex = SelectedIndex;
        Slots.Clear();
        
        var partyCount = _sav.PartyCount;
        
        for (int i = 0; i < 6; i++)
        {
            // LGPE stores party in a PokeList structure inside boxes.
            // On blank saves GetPartySlotAtIndex throws if the list is empty.
            PKM pk;
            try { pk = _sav.GetPartySlotAtIndex(i); }
            catch { pk = _sav.BlankPKM; }
            var isEmpty = pk.Species == 0 || i >= partyCount;
            
            Slots.Add(new PartySlotData
            {
                Slot = i,
                Species = pk.Species,
                Sprite = _spriteRenderer.GetSprite(pk),
                IsEmpty = isEmpty,
                IsShiny = pk.IsShiny,
                Nickname = isEmpty ? string.Empty : pk.Nickname,
                SpeciesName = isEmpty ? string.Empty : StringResourceLookup.Species(pk.Species),
                Level = pk.CurrentLevel,
                Gender = (byte)pk.Gender,
                HeldItem = (ushort)pk.HeldItem,
                HeldItemName = pk.HeldItem > 0 ? StringResourceLookup.Item(pk.HeldItem) : string.Empty,
                IsEgg = pk.IsEgg,
                CurrentHp = (ushort)pk.Stat_HPCurrent,
                MaxHp = (ushort)pk.Stat_HPMax,
                ShowdownSummary = isEmpty ? string.Empty : new ShowdownSet(pk).Text,
                IsLegal = isEmpty || new LegalityAnalysis(pk).Valid,
                IsSelected = false
            });
        }
        
        // Restore selection position (clamped to valid range)
        SelectedIndex = Math.Clamp(previousIndex, 0, Slots.Count - 1);
    }

    [RelayCommand]
    private void SelectSlotByClick(PartySlotData? slot)
    {
        if (slot is null)
            return;

        SelectedIndex = slot.Slot;
    }

    [RelayCommand]
    private void ActivateSlot()
    {
        if (SelectedIndex < 0 || SelectedIndex >= Slots.Count)
            return;

        var slot = Slots[SelectedIndex];
        SlotActivated?.Invoke(slot.Slot);
    }

    [RelayCommand]
    private void MoveSelection(string direction)
    {
        if (Slots.Count == 0) return;

        int newIndex = direction switch
        {
            "Up" => SelectedIndex > 0 ? SelectedIndex - 1 : SelectedIndex,
            "Down" => SelectedIndex < Slots.Count - 1 ? SelectedIndex + 1 : SelectedIndex,
            _ => SelectedIndex
        };

        SelectedIndex = newIndex;
    }

    public void RefreshParty() => LoadParty();
    
    [RelayCommand]
    private void ViewSlot(PartySlotData? slot)
    {
        if (slot is null || slot.IsEmpty)
            return;
        
        if (_slotService is not null)
            _slotService.RequestView(SlotLocation.FromParty(slot.Slot));
        else
            ViewSlotRequested?.Invoke(slot.Slot);
    }
    
    [RelayCommand]
    private void SetSlot(PartySlotData? slot)
    {
        if (slot is null)
            return;
        
        if (_slotService is not null)
            _slotService.RequestSet(SlotLocation.FromParty(slot.Slot));
        else
            SetSlotRequested?.Invoke(slot.Slot);
    }
    
    [RelayCommand]
    private void DeleteSlot(PartySlotData? slot)
    {
        if (slot is null || slot.IsEmpty)
            return;
        
        if (_slotService is not null)
            _slotService.RequestDelete(SlotLocation.FromParty(slot.Slot));
        else
            DeleteSlotRequested?.Invoke(slot.Slot);
    }
    
    /// <summary>
    /// Gets the PKM at the specified slot.
    /// </summary>
    public PKM GetSlotPKM(int slot) => _sav.GetPartySlotAtIndex(slot);

    /// <summary>
    /// Sets the PKM at the specified party slot and refreshes the display.
    /// </summary>
    public void SetSlotPKM(int slot, PKM pk)
    {
        _sav.SetPartySlotAtIndex(pk, slot);
        RefreshParty();
    }

    [RelayCommand]
    private void RequestMove((SlotDragData data, PartySlotData dest, bool clone) param)
    {
        _slotService?.RequestMove(param.data.Source, param.dest.Location, param.clone);
    }

    /// <summary>Raised when a dropped OS file turns out to be a save file, so the host can open it.</summary>
    public event Action<string>? SaveFileDropRequested;

    /// <summary>
    /// Handles one or more OS files dropped onto a party slot. A single file replaces the target
    /// slot (subject to format compatibility); multiple files are placed into the party's next
    /// empty slots in order. Reuses the same detection/conversion pipeline as folder import.
    /// </summary>
    public async Task HandleFileDropAsync(IReadOnlyList<string> paths, int targetSlot)
    {
        if (paths.Count == 0)
            return;

        if (paths.Count == 1)
        {
            var result = new ImportEntityFileUseCase().Execute(_sav, paths[0]);
            switch (result.Kind)
            {
                case EntityFileDropKind.SaveFile:
                    SaveFileDropRequested?.Invoke(paths[0]);
                    return;
                case EntityFileDropKind.Entity:
                    SetSlotPKM(targetSlot, result.Entity!);
                    return;
                default:
                    if (_dialogService is not null)
                        await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["PartyViewer_ImportFailedTitle"], result.Message ?? LocalizedStrings.Instance["PartyViewer_FileCouldNotBeImported"]);
                    return;
            }
        }

        var candidates = new List<PKM>();
        foreach (var path in paths)
        {
            var result = new ImportEntityFileUseCase().Execute(_sav, path);
            if (result.Kind == EntityFileDropKind.Entity)
                candidates.Add(result.Entity!);
        }

        var placed = 0;
        var slot = 0;
        foreach (var pk in candidates)
        {
            while (slot < Slots.Count && !Slots[slot].IsEmpty)
                slot++;
            if (slot >= Slots.Count)
                break;

            SetSlotPKM(slot, pk);
            placed++;
            slot++;
        }

        if (_dialogService is not null)
        {
            var message = LocalizedStrings.Instance.Format("PartyViewer_PlacedPokemonIntoParty", placed, paths.Count);
            var skipped = paths.Count - placed;
            if (skipped > 0)
                message += "\n" + LocalizedStrings.Instance.Format("PartyViewer_FilesSkipped", skipped);
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["PartyViewer_ImportFilesTitle"], message);
        }
    }
}
