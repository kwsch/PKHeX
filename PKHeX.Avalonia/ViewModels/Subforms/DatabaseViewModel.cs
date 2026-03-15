using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the PKM Database browser subform.
/// Loads PKM files from a directory and displays them in a searchable grid.
/// </summary>
public partial class DatabaseViewModel : SaveEditorViewModelBase
{
    private List<SlotCache> _rawDb = [];
    private List<SlotCache> _results = [];
    private CancellationTokenSource _cts = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _filterSpeciesIndex;

    [ObservableProperty]
    private string _statusText = "Loading...";

    [ObservableProperty]
    private bool _isSearchEnabled;

    [ObservableProperty]
    private string _databasePath = string.Empty;

    [ObservableProperty]
    private int _currentPage;

    [ObservableProperty]
    private int _totalPages;

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
    /// Callback invoked when a slot is clicked to view its PKM.
    /// </summary>
    public Action<PKM>? SlotClicked { get; set; }

    public DatabaseViewModel(SaveFile sav, string databasePath) : base(sav)
    {
        DatabasePath = databasePath;

        // Initialize grid slots
        for (int i = 0; i < PageSize; i++)
            Slots.Add(new SlotModel { Slot = i });

        // Populate species list
        var speciesSource = GameInfo.FilteredSources.Source.SpeciesDataSource;
        SpeciesList.Add(new ComboItem("(Any)", 0));
        foreach (var item in speciesSource)
        {
            if (item.Value > 0)
                SpeciesList.Add(item);
        }
    }

    /// <summary>
    /// Loads the database from disk in the background.
    /// </summary>
    public async Task LoadDatabaseAsync()
    {
        IsSearchEnabled = false;
        StatusText = "Loading database...";

        try
        {
            var token = _cts.Token;
            await Task.Run(() => LoadDatabase(token), token);

            if (!token.IsCancellationRequested)
            {
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
        var db = new ConcurrentBag<SlotCache>();

        // Load from save file boxes
        SlotInfoLoader.AddFromSaveFile(SAV, db);

        // Load from database folder
        if (Directory.Exists(DatabasePath))
        {
            var files = Directory.EnumerateFiles(DatabasePath, "*", SearchOption.AllDirectories);
            var validExtensions = new HashSet<string>(
                EntityFileExtension.GetExtensionsAll().Select(z => $".{z}"));
            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                    break;
                SlotInfoLoader.AddFromLocalFile(file, db, SAV, validExtensions);
            }
        }

        _rawDb = [.. db];

        // Force party data for stats
        foreach (var entry in _rawDb)
            entry.Entity.ForcePartyData();
    }

    private ushort GetSelectedSpecies()
    {
        if (FilterSpeciesIndex < 0 || FilterSpeciesIndex >= SpeciesList.Count)
            return 0;
        return (ushort)SpeciesList[FilterSpeciesIndex].Value;
    }

    [RelayCommand]
    private void Search()
    {
        if (!IsSearchEnabled)
            return;

        var settings = new SearchSettings
        {
            Context = SAV.Context,
            Generation = SAV.Generation,
            Species = GetSelectedSpecies(),
        };

        var results = settings.Search(_rawDb);
        SetResults([.. results]);
    }

    [RelayCommand]
    private void ResetFilters()
    {
        FilterSpeciesIndex = 0;
        SearchText = string.Empty;
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

        int resultIndex = (CurrentPage * GridWidth) + index;
        if (resultIndex >= _results.Count)
            return;

        var entry = _results[resultIndex];
        var pk = entry.Entity;
        var converted = EntityConverter.ConvertToType(pk, SAV.PKMType, out _);
        if (converted is not null)
        {
            SAV.AdaptToSaveFile(converted);
            converted.RefreshChecksum();
            SlotClicked?.Invoke(converted);
        }
    }

    private void SetResults(List<SlotCache> results)
    {
        _results = results;
        CurrentPage = 0;
        TotalPages = Math.Max(1, (int)Math.Ceiling((double)_results.Count / GridWidth));
        StatusText = $"{_results.Count} results";
        FillGrid();
    }

    private void FillGrid()
    {
        int begin = CurrentPage * GridWidth;
        int end = Math.Min(PageSize, _results.Count - begin);

        for (int i = 0; i < end && i < Slots.Count; i++)
        {
            var entry = _results[i + begin];
            var pk = entry.Entity;
            var sprite = pk.Sprite();
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
