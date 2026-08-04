using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class EncounterDatabaseViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IDialogService _dialogService;
    private readonly Action<PKM> _onSelect;

    public EncounterDatabaseViewModel(SaveFile sav, ISpriteRenderer spriteRenderer, IDialogService dialogService, Action<PKM> onSelect)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _dialogService = dialogService;
        _onSelect = onSelect;

        LoadSpeciesList();
    }

    [ObservableProperty]
    private ObservableCollection<ComboItem> _speciesList = [];

    [ObservableProperty]
    private ushort _selectedSpecies;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private ObservableCollection<EncounterResultViewModel> _results = [];

    private void LoadSpeciesList()
    {
        var names = GameInfo.Strings.Species;
        for (ushort i = 1; i <= _sav.MaxSpeciesID; i++)
        {
            var name = i < names.Count ? names[i] : $"Species #{i}";
            SpeciesList.Add(new ComboItem(name, i));
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (SelectedSpecies == 0)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["EncounterDatabase_SearchErrorTitle"], LocalizedStrings.Instance["EncounterDatabase_SelectSpeciesFirst"]);
            return;
        }

        IsSearching = true;
        Results.Clear();

        try
        {
            var foundEncounters = await Task.Run(() =>
            {
                var blank = _sav.BlankPKM;
                blank.Species = SelectedSpecies;
                blank.Form = 0;
                blank.Gender = 0;

                var versions = GameUtil.GetVersionsWithinRange(blank, _sav.Context).ToArray();
                return EncounterMovesetGenerator.GenerateEncounters(blank, ReadOnlyMemory<ushort>.Empty, versions)
                    .Take(100)
                    .ToList();
            });

            foreach (var enc in foundEncounters)
            {
                var pk = enc.ConvertToPKM(_sav);
                Results.Add(new EncounterResultViewModel(enc, pk, _spriteRenderer));
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["EncounterDatabase_SearchErrorTitle"], ex.Message);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task SelectEncounterAsync(EncounterResultViewModel? result)
    {
        if (result?.Encounter == null) return;

        try
        {
            var pk = result.Encounter.ConvertToPKM(_sav);
            pk.ResetPartyStats();
            _onSelect(pk);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["EncounterDatabase_ConversionErrorTitle"], LocalizedStrings.Instance.Format("EncounterDatabase_ConversionErrorMessage", ex.Message));
        }
    }
}

public class EncounterResultViewModel // Removed 'partial' as it's not needed unless using ObservableProperty
{
    private readonly IEncounterable _encounter;
    
    public EncounterResultViewModel(IEncounterable encounter, PKM pkm, ISpriteRenderer renderer)
    {
        _encounter = encounter;
        Encounter = encounter;
        Sprite = renderer.GetSprite(pkm);
    }
    
    public IEncounterable Encounter { get; }
    public byte[]? Sprite { get; }
    public string Species => GameInfo.Strings.Species.Count > _encounter.Species ? GameInfo.Strings.Species[_encounter.Species] : $"#{_encounter.Species}";
    public string Level => $"Lv. {_encounter.LevelMin}" + (_encounter.LevelMin != _encounter.LevelMax ? $"-{_encounter.LevelMax}" : "");
    public string Version => _encounter.Version.ToString();
    public string Type => _encounter.GetType().Name.Replace("Encounter", "");
    public string Location
    {
        get
        {
            var version = _encounter.Version;
            var context = _encounter.Context;
            var locationId = _encounter.Location;
            var names = GameInfo.GetLocationList(version, context, false);
            return names.FirstOrDefault(x => x.Value == locationId)?.Text ?? $"#{locationId}";
        }
    }

    /// <summary>Screen-reader-friendly summary for this result card, e.g. "Pikachu, Lv. 5, Route 1, Yellow".</summary>
    public string AccessibleName => $"{Species}, {Level}, {Location}, {Version}";
}
