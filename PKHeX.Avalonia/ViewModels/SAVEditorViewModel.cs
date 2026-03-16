using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Avalonia.Converters;
using PKHeX.Avalonia.Services;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Avalonia;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the SAV editor panel. Manages box/party display.
/// </summary>
public partial class SAVEditorViewModel : ObservableObject
{
    private SaveFile? _sav;

    /// <summary>Gets the currently loaded <see cref="SaveFile"/>.</summary>
    public SaveFile? SAV => _sav;

    #region Undo/Redo

    /// <summary>Records a single slot change for undo/redo operations.</summary>
    public record SlotChange(int Box, int Slot, byte[] Data, bool IsParty);

    private readonly Stack<SlotChange> _undoStack = new();
    private readonly Stack<SlotChange> _redoStack = new();

    /// <summary>Whether there are any undo operations available.</summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>Whether there are any redo operations available.</summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Pushes the current state of a slot onto the undo stack before a modification.
    /// </summary>
    public void PushUndo(int box, int slot, PKM pokemon, bool isParty)
    {
        _undoStack.Push(new SlotChange(box, slot, pokemon.DecryptedBoxData, isParty));
        _redoStack.Clear();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    [RelayCommand]
    public void Undo()
    {
        if (_undoStack.Count == 0 || _sav is null) return;
        var change = _undoStack.Pop();

        // Save current state for redo
        var current = change.IsParty
            ? _sav.GetPartySlotAtIndex(change.Slot)
            : _sav.GetBoxSlotAtIndex(change.Box, change.Slot);
        _redoStack.Push(new SlotChange(change.Box, change.Slot, current.DecryptedBoxData, change.IsParty));

        // Restore old state
        var restored = EntityFormat.GetFromBytes(change.Data);
        if (restored is not null)
        {
            if (change.IsParty)
                _sav.SetPartySlotAtIndex(restored, change.Slot);
            else
                _sav.SetBoxSlotAtIndex(restored, change.Box, change.Slot);
        }

        RefreshBox();
        RefreshParty();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    [RelayCommand]
    public void Redo()
    {
        if (_redoStack.Count == 0 || _sav is null) return;
        var change = _redoStack.Pop();

        // Save current state for undo
        var current = change.IsParty
            ? _sav.GetPartySlotAtIndex(change.Slot)
            : _sav.GetBoxSlotAtIndex(change.Box, change.Slot);
        _undoStack.Push(new SlotChange(change.Box, change.Slot, current.DecryptedBoxData, change.IsParty));

        // Restore redo state
        var restored = EntityFormat.GetFromBytes(change.Data);
        if (restored is not null)
        {
            if (change.IsParty)
                _sav.SetPartySlotAtIndex(restored, change.Slot);
            else
                _sav.SetBoxSlotAtIndex(restored, change.Box, change.Slot);
        }

        RefreshBox();
        RefreshParty();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    #endregion

    [ObservableProperty]
    private string _boxName = "Box 1";

    [ObservableProperty]
    private int _currentBox;

    [ObservableProperty]
    private int _boxCount;

    [ObservableProperty]
    private bool _isLoaded;

    [ObservableProperty]
    private Bitmap? _boxWallpaper;

    // Box search/filter
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _searchResultText = string.Empty;

    // Extra slots (Other tab)
    public ObservableCollection<SlotModel> ExtraSlots { get; } = [];
    [ObservableProperty] private bool _hasExtraSlots;

    // Save slot info
    [ObservableProperty] private string _saveSlotInfo = string.Empty;
    [ObservableProperty] private bool _hasSaveSlotInfo;

    public ObservableCollection<SlotModel> BoxSlots { get; } = [];
    public ObservableCollection<SlotModel> PartySlots { get; } = [];
    public ObservableCollection<string> BoxNames { get; } = [];

    /// <summary>Dynamic save-type-specific tools populated from the registry.</summary>
    public ObservableCollection<SAVToolDescriptor> SavTools { get; } = [];

    /// <summary>Manages drag-and-drop operations between slots.</summary>
    public SlotChangeManager SlotManager { get; }

    // Daycare display
    [ObservableProperty] private bool _hasDaycare;
    [ObservableProperty] private string _daycareInfo = string.Empty;

    // Global tool command delegates — wired from MainWindowViewModel
    public ICommand? OpenSettingsEditorCommand { get; set; }
    public ICommand? OpenDatabaseCommand { get; set; }
    public ICommand? OpenBatchEditorCommand { get; set; }
    public ICommand? OpenEncountersCommand { get; set; }
    public ICommand? OpenReportGridCommand { get; set; }
    public ICommand? OpenBoxViewerCommand { get; set; }
    public ICommand? OpenMysteryGiftDBCommand { get; set; }
    public ICommand? OpenRibbonEditorCommand { get; set; }
    public ICommand? OpenMemoryAmieCommand { get; set; }
    public ICommand? OpenTechRecordEditorCommand { get; set; }

    public SAVEditorViewModel()
    {
        SlotManager = new SlotChangeManager(this);

        // Initialize 30 box slots
        for (int i = 0; i < 30; i++)
            BoxSlots.Add(new SlotModel { Slot = i });

        // Initialize 6 party slots
        for (int i = 0; i < 6; i++)
            PartySlots.Add(new SlotModel { Slot = i });
    }

    public void LoadSaveFile(SaveFile sav)
    {
        _sav = sav;
        BoxCount = sav.BoxCount;
        CurrentBox = 0;
        IsLoaded = true;
        _undoStack.Clear();
        _redoStack.Clear();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        RefreshBoxNames();
        RefreshBox();
        RefreshParty();
        UpdateToolVisibility(sav);
        RefreshDaycare(sav);
        RefreshExtraSlots(sav);
        RefreshSaveSlotInfo(sav);
        SearchText = string.Empty;
    }

    private void UpdateToolVisibility(SaveFile sav)
    {
        SavTools.Clear();
        var allTools = SAVToolRegistry.GetAllTools();
        foreach (var tool in allTools.Where(t => t.IsAvailable(sav)).OrderBy(t => t.Name))
            SavTools.Add(tool);
    }

    [RelayCommand]
    private async Task OpenToolAsync(SAVToolDescriptor? tool)
    {
        if (tool is null || _sav is null)
            return;
        try
        {
            var (vm, view) = tool.CreateEditor(_sav);
            var mainWindow = (global::Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
            if (tool.ReloadsSlots)
                ReloadSlots();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{tool.Name} error: {ex}");
            SetStatusMessage?.Invoke($"{tool.Name}: {ex.Message}");
        }
    }

    private void RefreshBoxNames()
    {
        BoxNames.Clear();
        if (_sav is null)
            return;

        for (int i = 0; i < _sav.BoxCount; i++)
        {
            var name = _sav is IBoxDetailNameRead n ? n.GetBoxName(i) : $"Box {i + 1}";
            BoxNames.Add(name);
        }
    }

    partial void OnCurrentBoxChanged(int value)
    {
        if (_sav is null)
            return;
        RefreshBox();
    }

    [RelayCommand]
    private void NextBox()
    {
        if (_sav is null)
            return;
        CurrentBox = (CurrentBox + 1) % BoxCount;
    }

    [RelayCommand]
    private void PreviousBox()
    {
        if (_sav is null)
            return;
        CurrentBox = (CurrentBox - 1 + BoxCount) % BoxCount;
    }

    private void RefreshBox()
    {
        if (_sav is null)
            return;

        BoxName = _sav is IBoxDetailNameRead n ? n.GetBoxName(CurrentBox) : $"Box {CurrentBox + 1}";

        // Update wallpaper
        try
        {
            var wpBitmap = _sav.WallpaperImage(CurrentBox);
            BoxWallpaper = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(wpBitmap);
        }
        catch
        {
            BoxWallpaper = null;
        }

        int slotCount = Math.Min(30, _sav.BoxSlotCount);
        for (int i = 0; i < slotCount && i < BoxSlots.Count; i++)
        {
            var pk = _sav.GetBoxSlotAtIndex(CurrentBox, i);
            BoxSlots[i].Entity = pk;
            if (pk.Species == 0)
            {
                BoxSlots[i].SetImage(SpriteUtil.Spriter.None);
                BoxSlots[i].IsEmpty = true;
            }
            else
            {
                var sprite = pk.Sprite(_sav, CurrentBox, i);
                BoxSlots[i].SetImage(sprite);
                BoxSlots[i].IsEmpty = false;
            }
        }
    }

    private void RefreshParty()
    {
        if (_sav is null)
            return;

        for (int i = 0; i < _sav.PartyCount && i < PartySlots.Count; i++)
        {
            var pk = _sav.GetPartySlotAtIndex(i);
            PartySlots[i].Entity = pk;
            if (pk.Species == 0)
            {
                PartySlots[i].SetImage(SpriteUtil.Spriter.None);
                PartySlots[i].IsEmpty = true;
            }
            else
            {
                var sprite = pk.Sprite();
                PartySlots[i].SetImage(sprite);
                PartySlots[i].IsEmpty = false;
            }
        }

        // Clear remaining party slots
        for (int i = _sav?.PartyCount ?? 0; i < PartySlots.Count; i++)
        {
            PartySlots[i].SetImage(null);
            PartySlots[i].Entity = null;
            PartySlots[i].IsEmpty = true;
        }
    }

    public void ReloadSlots()
    {
        RefreshBox();
        RefreshParty();
    }

    public PKM? GetSlotPKM(SlotModel slot)
    {
        if (_sav is null)
            return null;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
            return _sav.GetBoxSlotAtIndex(CurrentBox, boxIndex);

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
            return _sav.GetPartySlotAtIndex(partyIndex);

        return null;
    }

    /// <summary>Callback invoked when a slot is clicked to view its PKM.</summary>
    public Action<PKM>? SlotSelected { get; set; }

    /// <summary>Callback to get the PKM currently in the editor (for Set operations).</summary>
    public Func<PKM?>? GetEditorPKM { get; set; }

    /// <summary>Callback to update the status bar message in the main window.</summary>
    public Action<string>? SetStatusMessage { get; set; }

    [RelayCommand]
    private void ViewSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;
        var pk = GetSlotPKM(slot);
        if (pk is null || pk.Species == 0)
            return;
        SlotSelected?.Invoke(pk);
    }

    [RelayCommand]
    private void SetSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;
        var pk = GetEditorPKM?.Invoke();
        if (pk is null)
            return;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            var existing = _sav.GetBoxSlotAtIndex(CurrentBox, boxIndex);
            PushUndo(CurrentBox, boxIndex, existing, isParty: false);
            _sav.SetBoxSlotAtIndex(pk, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            var existing = _sav.GetPartySlotAtIndex(partyIndex);
            PushUndo(0, partyIndex, existing, isParty: true);
            _sav.SetPartySlotAtIndex(pk, partyIndex);
            RefreshParty();
        }
    }

    [RelayCommand]
    private void CheckLegalitySlot(SlotModel? slot)
    {
        if (slot?.Entity is not { Species: > 0 } pk)
            return;

        var la = new LegalityAnalysis(pk);
        var report = la.Report();
        var status = la.Valid ? "Legal" : "Illegal";
        SetStatusMessage?.Invoke($"Legality: {status} - {report}");
    }

    [RelayCommand]
    private void DeleteSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            var existing = _sav.GetBoxSlotAtIndex(CurrentBox, boxIndex);
            PushUndo(CurrentBox, boxIndex, existing, isParty: false);
            _sav.SetBoxSlotAtIndex(_sav.BlankPKM, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            var existing = _sav.GetPartySlotAtIndex(partyIndex);
            PushUndo(0, partyIndex, existing, isParty: true);
            _sav.DeletePartySlot(partyIndex);
            RefreshParty();
        }
    }

    private void RefreshDaycare(SaveFile sav)
    {
        if (sav is IDaycareStorage dc)
        {
            HasDaycare = true;
            var sb = new StringBuilder();
            for (int i = 0; i < dc.DaycareSlotCount; i++)
            {
                if (dc.IsDaycareOccupied(i))
                {
                    var data = dc.GetDaycareSlot(i);
                    var pk = EntityFormat.GetFromBytes(data, prefer: sav.Context);
                    if (pk is not null && pk.Species > 0)
                        sb.AppendLine($"Slot {i + 1}: {SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, pk.Format)} Lv.{pk.CurrentLevel}");
                    else
                        sb.AppendLine($"Slot {i + 1}: (empty)");
                }
                else
                {
                    sb.AppendLine($"Slot {i + 1}: (empty)");
                }
            }
            DaycareInfo = sb.ToString().TrimEnd();
        }
        else
        {
            HasDaycare = false;
            DaycareInfo = string.Empty;
        }
    }

    #region Box Search

    partial void OnSearchTextChanged(string value)
    {
        if (_sav is null) return;
        var searchLower = value.ToLowerInvariant().Trim();
        var isActive = !string.IsNullOrEmpty(searchLower);
        int count = 0;

        foreach (var slot in BoxSlots)
        {
            slot.IsSearchActive = isActive;
            if (slot.Entity is null || slot.Entity.Species == 0)
            {
                slot.IsHighlighted = false;
                continue;
            }
            if (!isActive)
            {
                slot.IsHighlighted = false;
                continue;
            }
            var name = SpeciesName.GetSpeciesNameGeneration(slot.Entity.Species, 2, slot.Entity.Format).ToLowerInvariant();
            var matches = name.Contains(searchLower);
            slot.IsHighlighted = matches;
            if (matches) count++;
        }

        SearchResultText = isActive ? $"{count} found" : string.Empty;
    }

    #endregion

    #region Extra Slots

    private void RefreshExtraSlots(SaveFile sav)
    {
        ExtraSlots.Clear();
        try
        {
            var extras = sav.GetExtraSlots(true);
            if (extras.Count == 0)
            {
                HasExtraSlots = false;
                return;
            }

            HasExtraSlots = true;
            foreach (var slotInfo in extras)
            {
                var pk = slotInfo.Read(sav);
                var model = new SlotModel
                {
                    Slot = slotInfo.Slot,
                    SlotLabel = GetSlotTypeLabel(slotInfo.Type, slotInfo.Slot),
                };
                model.Entity = pk;
                if (pk.Species == 0)
                {
                    model.SetImage(SpriteUtil.Spriter.None);
                    model.IsEmpty = true;
                }
                else
                {
                    var sprite = pk.Sprite();
                    model.SetImage(sprite);
                    model.IsEmpty = false;
                }
                ExtraSlots.Add(model);
            }
        }
        catch
        {
            HasExtraSlots = false;
        }
    }

    private static string GetSlotTypeLabel(StorageSlotType type, int slot) => type switch
    {
        StorageSlotType.Daycare => $"Daycare #{slot + 1}",
        StorageSlotType.GTS => "GTS Upload",
        StorageSlotType.BattleBox => $"Battle Box #{slot + 1}",
        StorageSlotType.FusedKyurem => "Fused Kyurem",
        StorageSlotType.FusedNecrozmaS => "Fused Necrozma (Solgaleo)",
        StorageSlotType.FusedNecrozmaM => "Fused Necrozma (Lunala)",
        StorageSlotType.FusedCalyrex => "Fused Calyrex",
        StorageSlotType.Resort => $"Resort #{slot + 1}",
        StorageSlotType.Ride => "Ride Legend",
        StorageSlotType.BattleAgency => $"Battle Agency #{slot + 1}",
        StorageSlotType.SurpriseTrade => $"Surprise Trade #{slot + 1}",
        StorageSlotType.Underground => $"Underground #{slot + 1}",
        StorageSlotType.Scripted => $"Scripted #{slot + 1}",
        StorageSlotType.PGL => "PGL Upload",
        StorageSlotType.Pokéwalker => "Pokewalker",
        StorageSlotType.Shiny => $"Shiny Cache #{slot + 1}",
        _ => $"{type} #{slot + 1}",
    };

    #endregion

    #region Save Slot Info

    private void RefreshSaveSlotInfo(SaveFile sav)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Type: {sav.GetType().Name}");
        sb.AppendLine($"Generation: {sav.Generation}");
        sb.AppendLine($"Version: {sav.Version}");
        sb.AppendLine($"Boxes: {sav.BoxCount} ({sav.BoxSlotCount} slots each)");
        if (sav.HasParty)
            sb.AppendLine($"Party: {sav.PartyCount} Pokemon");
        sb.AppendLine($"Checksums: {(sav.ChecksumsValid ? "Valid" : "Invalid")}");

        SaveSlotInfo = sb.ToString().TrimEnd();
        HasSaveSlotInfo = true;
    }

    #endregion

    #region Box Management (Sort / Clear)

    [RelayCommand]
    private void SortBoxBySpecies()
    {
        if (_sav is null) return;
        try
        {
            var param = new BoxManipParam(CurrentBox, CurrentBox);
            _sav.SortBoxes(param.Start, param.Stop);
            RefreshBox();
            SetStatusMessage?.Invoke("Box sorted by species.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Sort error: {ex.Message}"); }
    }

    [RelayCommand]
    private void SortBoxByLevel()
    {
        if (_sav is null) return;
        try
        {
            var param = new BoxManipParam(CurrentBox, CurrentBox);
            _sav.SortBoxes(param.Start, param.Stop, (pkms, _) => pkms.OrderByLevel());
            RefreshBox();
            SetStatusMessage?.Invoke("Box sorted by level.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Sort error: {ex.Message}"); }
    }

    [RelayCommand]
    private void SortAllBoxes()
    {
        if (_sav is null) return;
        try
        {
            _sav.SortBoxes();
            RefreshBox();
            SetStatusMessage?.Invoke("All boxes sorted.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Sort error: {ex.Message}"); }
    }

    [RelayCommand]
    private void ClearBox()
    {
        if (_sav is null) return;
        try
        {
            _sav.ClearBoxes(CurrentBox, CurrentBox);
            RefreshBox();
            SetStatusMessage?.Invoke("Box cleared.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Clear error: {ex.Message}"); }
    }

    [RelayCommand]
    private void ClearAllBoxes()
    {
        if (_sav is null) return;
        try
        {
            _sav.ClearBoxes();
            RefreshBox();
            SetStatusMessage?.Invoke("All boxes cleared.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Clear error: {ex.Message}"); }
    }

    #endregion

    [RelayCommand]
    private void VerifyAllPkm()
    {
        if (_sav is null) return;
        try
        {
            int total = 0, legal = 0, illegal = 0;
            for (int box = 0; box < _sav.BoxCount; box++)
            {
                for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
                {
                    var pk = _sav.GetBoxSlotAtIndex(box, slot);
                    if (pk.Species == 0) continue;
                    total++;
                    var la = new LegalityAnalysis(pk);
                    if (la.Valid) legal++; else illegal++;
                }
            }
            SetStatusMessage?.Invoke($"Checked {total} Pokemon: {legal} legal, {illegal} illegal.");
        }
        catch (Exception ex) { SetStatusMessage?.Invoke($"Verify error: {ex.Message}"); }
    }

    [RelayCommand]
    private void VerifyChecksums()
    {
        if (_sav is null) return;
        var valid = _sav.ChecksumsValid;
        var info = _sav.ChecksumInfo;
        if (valid)
            SetStatusMessage?.Invoke("Checksums valid.");
        else
            SetStatusMessage?.Invoke($"Checksum issues: {info}");
    }

    [RelayCommand]
    private void ExportBackup()
    {
        if (_sav is null) return;
        try
        {
            var data = _sav.Write();
            var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PKHeX", "backups");
            Directory.CreateDirectory(backupDir);
            var filename = $"{_sav.GetType().Name}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            File.WriteAllBytes(Path.Combine(backupDir, filename), data.ToArray());
            SetStatusMessage?.Invoke($"Backup saved: {filename}");
        }
        catch (Exception ex)
        {
            SetStatusMessage?.Invoke($"Backup failed: {ex.Message}");
        }
    }
}
