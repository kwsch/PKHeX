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
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Encounter Generator subform.
/// Generates legal encounter templates for creating new PKM.
/// </summary>
public partial class EncountersViewModel : SaveEditorViewModelBase, IDisposable
{
    private List<IEncounterable> _results = [];
    private CancellationTokenSource _cts = new();

    [ObservableProperty]
    private int _filterSpeciesIndex;

    [ObservableProperty]
    private string _statusText = "Ready. Select a species and search.";

    [ObservableProperty]
    private bool _isSearchEnabled = true;

    [ObservableProperty]
    private int _currentPage;

    [ObservableProperty]
    private int _totalPages = 1;

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
    /// Callback invoked when a slot is clicked to load the encounter into the editor.
    /// </summary>
    public Action<PKM>? SlotClicked { get; set; }

    public EncountersViewModel(SaveFile sav) : base(sav)
    {
        // Initialize grid slots
        for (int i = 0; i < PageSize; i++)
            Slots.Add(new SlotModel { Slot = i });

        // Populate species list
        var speciesSource = GameInfo.FilteredSources.Source.SpeciesDataSource;
        foreach (var item in speciesSource)
        {
            if (item.Value > 0)
                SpeciesList.Add(item);
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (!IsSearchEnabled)
            return;

        if (FilterSpeciesIndex < 0 || FilterSpeciesIndex >= SpeciesList.Count)
        {
            StatusText = "Please select a species.";
            return;
        }

        var speciesItem = SpeciesList[FilterSpeciesIndex];
        var species = (ushort)speciesItem.Value;
        if (species == 0)
        {
            StatusText = "Please select a valid species.";
            return;
        }

        IsSearchEnabled = false;
        StatusText = "Searching...";

        // Cancel any previous search
        _cts.Cancel();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            var results = await Task.Run(() => SearchEncounters(species, token), token);
            if (!token.IsCancellationRequested)
            {
                SetResults(results);
                if (results.Count == 0)
                    StatusText = "No encounters found.";
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsSearchEnabled = true;
            EncounterMovesetGenerator.ResetFilters();
        }
    }

    private List<IEncounterable> SearchEncounters(ushort species, CancellationToken token)
    {
        var pk = SAV.BlankPKM;
        var pt = SAV.Personal;
        var versions = GameUtil.GetVersionsWithinRange(pk, pk.Context).ToArray();
        var moves = ReadOnlyMemory<ushort>.Empty;

        var results = new List<IEncounterable>();
        var seen = new HashSet<int>(); // deduplicate by reference hash code

        var pi = pt.GetFormEntry(species, 0);
        var fc = pi.FormCount;
        if (fc == 0)
            fc = 1;

        for (byte f = 0; f < fc; f++)
        {
            if (token.IsCancellationRequested)
                break;

            if (FormInfo.IsBattleOnlyForm(species, f, pk.Format))
                continue;

            pk.Species = species;
            pk.Form = f;
            pk.SetGender(pk.GetSaneGender());
            EncounterMovesetGenerator.OptimizeCriteria(pk, SAV);

            foreach (var enc in EncounterMovesetGenerator.GenerateEncounters(pk, moves, versions))
            {
                if (token.IsCancellationRequested)
                    break;
                var hash = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(enc);
                if (seen.Add(hash))
                    results.Add(enc);
            }
        }

        return results;
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

        try
        {
            var enc = _results[resultIndex];
            var criteria = EncounterCriteria.Unrestricted;
            var temp = enc.ConvertToPKM(SAV, criteria);
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
            // Conversion may fail for incompatible encounters
        }
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

    private void SetResults(List<IEncounterable> results)
    {
        _results = results;
        CurrentPage = 0;
        TotalPages = Math.Max(1, (int)Math.Ceiling((double)_results.Count / PageSize));
        StatusText = $"{_results.Count} encounters found";
        FillGrid();
    }

    private void FillGrid()
    {
        int begin = CurrentPage * PageSize;
        int end = Math.Min(PageSize, _results.Count - begin);

        for (int i = 0; i < end && i < Slots.Count; i++)
        {
            var enc = _results[i + begin];
            var sprite = enc.Sprite();
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

    public void CancelSearch()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}
