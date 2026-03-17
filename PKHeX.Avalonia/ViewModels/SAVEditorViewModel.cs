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
using PKHeX.Avalonia.Views;
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

    /// <summary>Records a single slot's state for undo/redo operations.</summary>
    public record SlotChangeEntry(int Box, int Slot, byte[] Data, bool IsParty);

    /// <summary>Groups one or more slot changes into an atomic undo/redo operation.</summary>
    private record SlotChange(IReadOnlyList<SlotChangeEntry> Entries);

    private const int MaxUndoSize = 100;

    private readonly Stack<SlotChange> _undoStack = new();
    private readonly Stack<SlotChange> _redoStack = new();

    /// <summary>Whether there are any undo operations available.</summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>Whether there are any redo operations available.</summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Pushes one or more slot states onto the undo stack as a single atomic operation.
    /// </summary>
    public void PushUndo(params SlotChangeEntry[] entries)
    {
        if (entries.Length == 0) return;
        _undoStack.Push(new SlotChange(entries));
        _redoStack.Clear();

        // Cap undo stack size to prevent unbounded memory growth
        if (_undoStack.Count > MaxUndoSize)
        {
            var items = _undoStack.Take(MaxUndoSize).Reverse().ToArray();
            _undoStack.Clear();
            foreach (var item in items)
                _undoStack.Push(item);
        }

        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
    }

    [RelayCommand]
    public void Undo()
    {
        if (_undoStack.Count == 0 || _sav is null) return;
        var change = _undoStack.Pop();

        var redoEntries = new List<SlotChangeEntry>();
        foreach (var entry in change.Entries)
        {
            // Save current state for redo
            var current = entry.IsParty
                ? _sav.GetPartySlotAtIndex(entry.Slot)
                : _sav.GetBoxSlotAtIndex(entry.Box, entry.Slot);
            if (current is not null)
                redoEntries.Add(new SlotChangeEntry(entry.Box, entry.Slot, current.DecryptedBoxData, entry.IsParty));

            // Restore the saved state
            var restored = EntityFormat.GetFromBytes(entry.Data, prefer: _sav.Context);
            if (restored is null) continue;
            if (entry.IsParty)
                _sav.SetPartySlotAtIndex(restored, entry.Slot);
            else
                _sav.SetBoxSlotAtIndex(restored, entry.Box, entry.Slot);
        }

        _redoStack.Push(new SlotChange(redoEntries));
        ReloadSlots();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnModified?.Invoke();
    }

    [RelayCommand]
    public void Redo()
    {
        if (_redoStack.Count == 0 || _sav is null) return;
        var change = _redoStack.Pop();

        var undoEntries = new List<SlotChangeEntry>();
        foreach (var entry in change.Entries)
        {
            // Save current state for undo
            var current = entry.IsParty
                ? _sav.GetPartySlotAtIndex(entry.Slot)
                : _sav.GetBoxSlotAtIndex(entry.Box, entry.Slot);
            if (current is not null)
                undoEntries.Add(new SlotChangeEntry(entry.Box, entry.Slot, current.DecryptedBoxData, entry.IsParty));

            // Restore the redo state
            var restored = EntityFormat.GetFromBytes(entry.Data, prefer: _sav.Context);
            if (restored is null) continue;
            if (entry.IsParty)
                _sav.SetPartySlotAtIndex(restored, entry.Slot);
            else
                _sav.SetBoxSlotAtIndex(restored, entry.Box, entry.Slot);
        }

        _undoStack.Push(new SlotChange(undoEntries));
        ReloadSlots();
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnModified?.Invoke();
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

    [RelayCommand(AllowConcurrentExecutions = false)]
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
            if (view is SubformWindow { Modified: true })
            {
                if (tool.ReloadsSlots)
                    ReloadSlots();
                OnModified?.Invoke();
            }
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
            var oldWp = BoxWallpaper;
            BoxWallpaper = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmapAndDispose(wpBitmap);
            oldWp?.Dispose();
        }
        catch
        {
            var oldWp = BoxWallpaper;
            BoxWallpaper = null;
            oldWp?.Dispose();
        }

        int slotCount = Math.Min(30, _sav.BoxSlotCount);
        for (int i = 0; i < slotCount && i < BoxSlots.Count; i++)
        {
            var pk = _sav.GetBoxSlotAtIndex(CurrentBox, i);
            if (pk is null) continue;
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

        // Clear any remaining slots beyond the save's box slot count
        for (int i = slotCount; i < BoxSlots.Count; i++)
        {
            BoxSlots[i].Entity = null;
            BoxSlots[i].SetImage(SpriteUtil.Spriter.None);
            BoxSlots[i].IsEmpty = true;
        }
    }

    private void RefreshParty()
    {
        if (_sav is null)
            return;

        for (int i = 0; i < _sav.PartyCount && i < PartySlots.Count; i++)
        {
            var pk = _sav.GetPartySlotAtIndex(i);
            if (pk is null) continue;
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

    /// <summary>Callback invoked whenever the save file is modified (slot set/delete, undo/redo, sort, clear).</summary>
    public Action? OnModified { get; set; }

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
            if (existing is not null)
                PushUndo(new SlotChangeEntry(CurrentBox, boxIndex, existing.DecryptedBoxData, false));
            _sav.SetBoxSlotAtIndex(pk, CurrentBox, boxIndex);
            RefreshBox();
            OnModified?.Invoke();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            var existing = _sav.GetPartySlotAtIndex(partyIndex);
            if (existing is not null)
                PushUndo(new SlotChangeEntry(0, partyIndex, existing.DecryptedBoxData, true));
            _sav.SetPartySlotAtIndex(pk, partyIndex);
            RefreshParty();
            OnModified?.Invoke();
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
            if (existing is not null)
                PushUndo(new SlotChangeEntry(CurrentBox, boxIndex, existing.DecryptedBoxData, false));
            _sav.SetBoxSlotAtIndex(_sav.BlankPKM, CurrentBox, boxIndex);
            RefreshBox();
            OnModified?.Invoke();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            if (_sav.PartyCount <= 1)
                return; // Cannot delete the last party member
            var existing = _sav.GetPartySlotAtIndex(partyIndex);
            if (existing is not null)
                PushUndo(new SlotChangeEntry(0, partyIndex, existing.DecryptedBoxData, true));
            _sav.DeletePartySlot(partyIndex);
            RefreshParty();
            OnModified?.Invoke();
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
                if (pk is null) continue;
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

    /// <summary>
    /// Saves the current state of all slots in a single box for undo.
    /// </summary>
    private List<SlotChangeEntry> SnapshotBox(int box)
    {
        var entries = new List<SlotChangeEntry>();
        if (_sav is null) return entries;
        for (int i = 0; i < _sav.BoxSlotCount; i++)
        {
            var pk = _sav.GetBoxSlotAtIndex(box, i);
            if (pk is not null)
                entries.Add(new SlotChangeEntry(box, i, pk.DecryptedBoxData, false));
        }
        return entries;
    }

    /// <summary>
    /// Saves the current state of all slots in all boxes for undo.
    /// </summary>
    private List<SlotChangeEntry> SnapshotAllBoxes()
    {
        var entries = new List<SlotChangeEntry>();
        if (_sav is null) return entries;
        for (int box = 0; box < _sav.BoxCount; box++)
        {
            for (int i = 0; i < _sav.BoxSlotCount; i++)
            {
                var pk = _sav.GetBoxSlotAtIndex(box, i);
                if (pk is not null)
                    entries.Add(new SlotChangeEntry(box, i, pk.DecryptedBoxData, false));
            }
        }
        return entries;
    }

    [RelayCommand]
    private void SortBoxBySpecies()
    {
        if (_sav is null) return;
        try
        {
            var entries = SnapshotBox(CurrentBox);
            if (entries.Count > 0)
                PushUndo(entries.ToArray());

            var param = new BoxManipParam(CurrentBox, CurrentBox);
            _sav.SortBoxes(param.Start, param.Stop);
            RefreshBox();
            OnModified?.Invoke();
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
            var entries = SnapshotBox(CurrentBox);
            if (entries.Count > 0)
                PushUndo(entries.ToArray());

            var param = new BoxManipParam(CurrentBox, CurrentBox);
            _sav.SortBoxes(param.Start, param.Stop, (pkms, _) => pkms.OrderByLevel());
            RefreshBox();
            OnModified?.Invoke();
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
            var entries = SnapshotAllBoxes();
            if (entries.Count > 0)
                PushUndo(entries.ToArray());

            _sav.SortBoxes();
            RefreshBox();
            OnModified?.Invoke();
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
            var entries = SnapshotBox(CurrentBox);
            if (entries.Count > 0)
                PushUndo(entries.ToArray());

            _sav.ClearBoxes(CurrentBox, CurrentBox);
            RefreshBox();
            OnModified?.Invoke();
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
            var entries = SnapshotAllBoxes();
            if (entries.Count > 0)
                PushUndo(entries.ToArray());

            _sav.ClearBoxes();
            RefreshBox();
            OnModified?.Invoke();
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
                    if (pk is null || pk.Species == 0) continue;
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
