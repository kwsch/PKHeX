using System;
using System.Collections.ObjectModel;
using System.Linq;
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

    public ObservableCollection<SlotModel> BoxSlots { get; } = [];
    public ObservableCollection<SlotModel> PartySlots { get; } = [];
    public ObservableCollection<string> BoxNames { get; } = [];

    /// <summary>Dynamic save-type-specific tools populated from the registry.</summary>
    public ObservableCollection<SAVToolDescriptor> SavTools { get; } = [];

    /// <summary>Manages drag-and-drop operations between slots.</summary>
    public SlotChangeManager SlotManager { get; }

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
        RefreshBoxNames();
        RefreshBox();
        RefreshParty();
        UpdateToolVisibility(sav);
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
            System.Diagnostics.Debug.WriteLine($"{tool.Name} error: {ex.Message}");
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
            _sav.SetBoxSlotAtIndex(pk, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
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
            _sav.SetBoxSlotAtIndex(_sav.BlankPKM, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            _sav.DeletePartySlot(partyIndex);
            RefreshParty();
        }
    }
}
