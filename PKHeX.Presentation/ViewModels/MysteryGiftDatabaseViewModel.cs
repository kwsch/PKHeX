using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MysteryGiftDatabaseViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<MysteryGiftDatabaseEntry> _results = [];

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private string _statusText = "Ready";

    // Filtering properties
    [ObservableProperty] private int _species;
    [ObservableProperty] private int _generation;
    [ObservableProperty] private bool _showItems = true;
    [ObservableProperty] private bool _showPokemon = true;

    // Data Sources for View
    public IReadOnlyList<ComboItem> SpeciesList { get; }
    public IReadOnlyList<ComboItem> GenerationList { get; }

    public MysteryGiftDatabaseViewModel(SaveFile sav, ISpriteRenderer spriteRenderer, IDialogService dialogService)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _dialogService = dialogService;

        SpeciesList = GameInfo.Sources.SpeciesDataSource;
        GenerationList = new List<ComboItem>
        {
            new("Any", 0),
            new("Gen 4", 4),
            new("Gen 5", 5),
            new("Gen 6", 6),
            new("Gen 7", 7),
            new("Gen 8", 8),
            new("Gen 9", 9),
        };

        // Initialize with all events
        SearchDatabase();
    }

    [RelayCommand]
    private void SearchDatabase()
    {
        IsSearching = true;
        StatusText = "Searching internal database...";

        try
        {
            var gifts = EncounterEvent.GetAllEvents(sorted: true);

            // Apply filters
            if (Species > 0)
                gifts = gifts.Where(g => g.Species == Species);

            if (Generation > 0)
                gifts = gifts.Where(g => g.Generation == Generation);

            if (!ShowItems)
                gifts = gifts.Where(g => !g.IsItem);

            if (!ShowPokemon)
                gifts = gifts.Where(g => !g.IsEntity);

            Results.Clear();
            foreach (var gift in gifts)
            {
                Results.Add(new MysteryGiftDatabaseEntry(gift, _spriteRenderer));
            }

            StatusText = $"Found {Results.Count} gifts.";
        }
        catch (Exception ex)
        {
            StatusText = $"Error searching database: {ex.Message}";
            _ = _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftDatabase_SearchErrorTitle"], ex.Message);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task LoadFolder()
    {
        var folder = await _dialogService.OpenFolderAsync(LocalizedStrings.Instance["MysteryGiftDatabase_SelectFolderTitle"]);
        if (string.IsNullOrEmpty(folder))
            return;

        IsSearching = true;
        StatusText = "Scanning folder...";

        try
        {
            var matches = await Task.Run(() =>
            {
                var gifts = MysteryUtil.GetGiftsFromFolder(folder);

                // Apply current filters to loaded gifts
                if (Species > 0)
                    gifts = gifts.Where(g => g.Species == Species).ToList();
                else
                    gifts = gifts.ToList();

                if (Generation > 0)
                    gifts = gifts.Where(g => g.Generation == Generation).ToList();

                if (!ShowItems)
                    gifts = gifts.Where(g => !g.IsItem).ToList();

                if (!ShowPokemon)
                    gifts = gifts.Where(g => !g.IsEntity).ToList();

                return gifts.ToList();
            });

            Results.Clear();
            foreach (var gift in matches)
            {
                Results.Add(new MysteryGiftDatabaseEntry(gift, _spriteRenderer));
            }

            StatusText = $"Found {Results.Count} gifts in folder.";
        }
        catch (Exception ex)
        {
            StatusText = $"Error scanning folder: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>Raised with the converted <see cref="PKM"/> once a selected gift is a Pokémon entity.</summary>
    public event Action<PKM>? GiftSelected;

    [RelayCommand]
    private void SelectGift(MysteryGiftDatabaseEntry? entry)
    {
        // Item-only gifts (IsEntity: false) have no Pokémon to convert; guard so they never reach
        // ConvertToPKM (mirrors EncounterDatabaseViewModel.SelectEncounter's guard/try-catch shape).
        if (entry?.Gift is not { IsEntity: true } gift)
            return;

        try
        {
            var pk = gift.ConvertToPKM(_sav);
            GiftSelected?.Invoke(pk);
        }
        catch (Exception ex)
        {
            _ = _dialogService.ShowErrorAsync(
                LocalizedStrings.Instance["MysteryGiftDatabase_ConversionErrorTitle"],
                LocalizedStrings.Instance.Format("MysteryGiftDatabase_ConversionErrorMessage", ex.Message));
        }
    }
}

public class MysteryGiftDatabaseEntry
{
    public MysteryGift Gift { get; }
    public byte[]? Sprite { get; }
    public string SpeciesName => Gift.IsItem ? GameInfo.Strings.Item[Gift.ItemID] : GameInfo.Strings.Species[Gift.Species];
    public string Level => Gift.IsEntity ? Gift.Level.ToString() : "-";
    public string CardTitle => Gift.CardTitle;
    public string Format => $"Gen {Gift.Generation}";
    public string Type => Gift.IsItem ? "Item" : "Pokémon";

    public MysteryGiftDatabaseEntry(MysteryGift gift, ISpriteRenderer renderer)
    {
        Gift = gift;
        if (gift.IsEntity)
        {
            // Create a temp PKM for the sprite renderer. A malformed/edge-case gift can throw
            // during conversion; skip the sprite rather than crashing the whole database open.
            try
            {
                var pk = gift.ConvertToPKM(new SimpleTrainerInfo(gift.Version));
                Sprite = renderer.GetSprite(pk);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning($"Failed to render sprite for mystery gift '{gift.CardTitle}': {ex.Message}");
            }
        }
    }
}
