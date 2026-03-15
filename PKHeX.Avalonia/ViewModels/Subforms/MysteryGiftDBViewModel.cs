using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Avalonia;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Mystery Gift Database browser subform.
/// Loads all event mystery gifts and displays them in a searchable grid.
/// </summary>
public partial class MysteryGiftDBViewModel : SaveEditorViewModelBase
{
    private List<MysteryGift> _rawDb = [];
    private List<MysteryGift> _results = [];
    private CancellationTokenSource _cts = new();

    [ObservableProperty]
    private int _filterSpeciesIndex;

    [ObservableProperty]
    private string _statusText = "Loading...";

    [ObservableProperty]
    private bool _isSearchEnabled;

    [ObservableProperty]
    private int _currentPage;

    [ObservableProperty]
    private int _totalPages;

    [ObservableProperty]
    private string _detailText = string.Empty;

    private const int GridWidth = 6;
    private const int GridHeight = 11;
    private const int PageSize = GridWidth * GridHeight;

    /// <summary>
    /// Slots displayed in the PokeGrid.
    /// </summary>
    public ObservableCollection<SlotModel> Slots { get; } = [];

    /// <summary>
    /// Available species for filtering.
    /// </summary>
    public ObservableCollection<ComboItem> SpeciesList { get; } = [];

    /// <summary>
    /// Callback invoked when a slot is clicked to view the gift as a PKM.
    /// </summary>
    public Action<PKM>? SlotClicked { get; set; }

    public MysteryGiftDBViewModel(SaveFile sav) : base(sav)
    {
        // Initialize grid slots
        for (int i = 0; i < PageSize; i++)
            Slots.Add(new SlotModel { Slot = i });
    }

    /// <summary>
    /// Loads the mystery gift database in the background.
    /// </summary>
    public async Task LoadDatabaseAsync()
    {
        IsSearchEnabled = false;
        StatusText = "Loading mystery gift database...";

        try
        {
            var token = _cts.Token;
            await Task.Run(() => LoadDatabase(token), token);

            if (!token.IsCancellationRequested)
            {
                PopulateSpeciesList();
                SetResults(_rawDb);
                IsSearchEnabled = true;
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled
        }
        catch (Exception ex)
        {
            StatusText = $"Failed to load: {ex.Message}";
        }
    }

    private void LoadDatabase(CancellationToken token)
    {
        var db = EncounterEvent.GetAllEvents();
        if (token.IsCancellationRequested)
            return;

        _rawDb = [.. db];

        // Mark all as unused for display
        foreach (var mg in _rawDb)
            mg.GiftUsed = false;
    }

    private void PopulateSpeciesList()
    {
        SpeciesList.Clear();
        SpeciesList.Add(new ComboItem("(Any)", -1));

        var speciesInDb = _rawDb
            .Select(mg => mg.Species)
            .Where(s => s > 0)
            .Distinct()
            .OrderBy(s => s);

        var names = GameInfo.Strings.Species;
        foreach (var species in speciesInDb)
        {
            var name = species < names.Count ? names[species] : $"Species {species}";
            SpeciesList.Add(new ComboItem(name, species));
        }
    }

    [RelayCommand]
    private void Search()
    {
        if (!IsSearchEnabled)
            return;

        IEnumerable<MysteryGift> res = _rawDb;

        // Apply species filter
        var species = GetSelectedSpecies();
        if (species > 0)
            res = res.Where(mg => mg.Species == species);

        var results = res.ToList();
        if (results.Count == 0)
            StatusText = "No results found.";

        SetResults(results);
    }

    [RelayCommand]
    private void ResetFilters()
    {
        FilterSpeciesIndex = 0;
        SetResults(_rawDb);
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages - 1)
        {
            CurrentPage++;
            FillGrid();
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 0)
        {
            CurrentPage--;
            FillGrid();
        }
    }

    [RelayCommand]
    private void ClickSlot(SlotModel? slot)
    {
        if (slot is null)
            return;

        int index = Slots.IndexOf(slot);
        if (index < 0 || index >= PageSize)
            return;

        int resultIndex = (CurrentPage * PageSize) + index;
        if (resultIndex >= _results.Count)
            return;

        var gift = _results[resultIndex];

        // Show gift details
        var lines = gift.GetDescription();
        DetailText = string.Join("\n", lines);

        // Convert to PKM and notify
        try
        {
            var temp = gift.ConvertToPKM(SAV, EncounterCriteria.Unrestricted);
            var pk = EntityConverter.ConvertToType(temp, SAV.PKMType, out _);
            if (pk is not null)
            {
                SAV.AdaptToSaveFile(pk);
                pk.RefreshChecksum();
                SlotClicked?.Invoke(pk);
            }
        }
        catch
        {
            // Conversion may fail for incompatible gifts
        }
    }

    private ushort GetSelectedSpecies()
    {
        if (FilterSpeciesIndex < 0 || FilterSpeciesIndex >= SpeciesList.Count)
            return 0;
        var val = SpeciesList[FilterSpeciesIndex].Value;
        return val < 0 ? (ushort)0 : (ushort)val;
    }

    private void SetResults(List<MysteryGift> results)
    {
        _results = results;
        CurrentPage = 0;
        TotalPages = Math.Max(1, (int)Math.Ceiling((double)_results.Count / PageSize));
        StatusText = $"{_results.Count} gifts found";
        FillGrid();
    }

    private void FillGrid()
    {
        int begin = CurrentPage * PageSize;
        int end = Math.Min(PageSize, _results.Count - begin);

        for (int i = 0; i < end && i < Slots.Count; i++)
        {
            var gift = _results[i + begin];
            var sprite = gift.Sprite();
            Slots[i].SetImage(sprite);
            Slots[i].IsEmpty = false;
        }

        // Clear remaining slots
        for (int i = Math.Max(0, end); i < Slots.Count; i++)
        {
            Slots[i].SetImage(null);
            Slots[i].IsEmpty = true;
        }
    }

    public void CancelLoad()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
    }
}
