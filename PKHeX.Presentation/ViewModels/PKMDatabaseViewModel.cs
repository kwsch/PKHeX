using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class PKMDatabaseViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<PKMDatabaseEntry> _results = [];

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private int _searchProgress;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private PKMDatabaseEntry? _selectedResult;

    /// <summary>Shared entity filter inputs + combo data sources (bound by the view).</summary>
    public EntityFilterViewModel Filter { get; }

    public PKMDatabaseViewModel(SaveFile sav, ISpriteRenderer spriteRenderer, IDialogService dialogService)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _dialogService = dialogService;

        Filter = new EntityFilterViewModel(sav);

        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) => RefreshLanguage());
    }

    [RelayCommand]
    private async Task SearchSaveAsync()
    {
        Results.Clear();
        IsSearching = true;
        StatusText = LocalizedStrings.Instance["PKMDatabase_SearchingCurrentSave"];

        try
        {
            var settings = Filter.GetSearchSettings();
            var allPkms = _sav.BoxData.Concat(_sav.PartyData).ToList();

            int totalMons = allPkms.Count(p => p.Species != 0);
            if (totalMons == 0)
            {
                StatusText = LocalizedStrings.Instance["PKMDatabase_SaveHasNoPokemon"];
                return;
            }

            var matches = await Task.Run(() => settings.Search(allPkms).Where(p => p.Species != 0).ToList());

            foreach (var pk in matches)
                Results.Add(new PKMDatabaseEntry(pk, _spriteRenderer));

            StatusText = LocalizedStrings.Instance.Format("PKMDatabase_FoundMatchesInSave", Results.Count);
        }
        catch (Exception ex)
        {
            StatusText = LocalizedStrings.Instance.Format("PKMDatabase_SearchErrorStatus", ex.Message);
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["PKMDatabase_SearchErrorTitle"], ex.Message);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task LoadFolderAsync()
    {
        var path = await _dialogService.OpenFolderAsync(LocalizedStrings.Instance["PKMDatabase_SelectFolderToScanTitle"]);
        if (string.IsNullOrEmpty(path)) return;

        Results.Clear();
        IsSearching = true;
        StatusText = LocalizedStrings.Instance["PKMDatabase_ScanningFolder"];

        try 
        {
            var settings = Filter.GetSearchSettings();
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            
            var matches = await Task.Run(() => 
            {
                var found = new List<PKM>();
                foreach (var file in files)
                {
                    var data = File.ReadAllBytes(file);
                    if (SaveUtil.IsSizeValid(data.Length))
                    {
                        var sav = SaveUtil.GetSaveFile(data);
                        if (sav != null)
                        {
                            var pkms = sav.BoxData.Concat(sav.PartyData);
                            found.AddRange(settings.Search(pkms).Where(p => p.Species != 0));
                        }
                    }
                    else
                    {
                        var pk = EntityFormat.GetFromBytes(data, _sav.Context);
                        if (pk != null && settings.Search([pk]).Any())
                            found.Add(pk);
                    }
                }
                return found;
            });
    
            foreach (var pk in matches)
                Results.Add(new PKMDatabaseEntry(pk, _spriteRenderer));

            StatusText = LocalizedStrings.Instance.Format("PKMDatabase_FoundMatchesInFolder", Results.Count);
        }
        catch (Exception ex)
        {
            StatusText = LocalizedStrings.Instance.Format("PKMDatabase_SearchErrorStatus", ex.Message);
        }
        finally
        {
            IsSearching = false;
        }
    }

    public event Action<PKM>? PokemonSelected;

    public void RefreshLanguage()
    {
        Filter.RefreshLanguage();
        foreach (var entry in Results)
            entry.Refresh();
    }

    [RelayCommand]
    private void SelectPokemon(PKMDatabaseEntry entry)
    {
        PokemonSelected?.Invoke(entry.PKM);
    }
}

public partial class PKMDatabaseEntry : ObservableObject
{
    public PKM PKM { get; }
    public byte[]? Sprite { get; }
    
    public string SpeciesName
    {
        get
        {
            if (PKM.Species == 0 || PKM.Species >= GameInfo.Strings.Species.Count)
                return "---";

            var name = GameInfo.Strings.Species[PKM.Species];
            if (PKM.Form > 0)
            {
                var formList = FormConverter.GetFormList(PKM.Species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, PKM.Context);
                if (formList != null && PKM.Form < formList.Length && !string.IsNullOrEmpty(formList[PKM.Form]))
                    name += $" ({formList[PKM.Form]})";
            }
            return name;
        }
    }

    public string Level => PKM.CurrentLevel.ToString();
    public string NatureName => StringResourceLookup.Nature((int)PKM.Nature);
    public string Gender => PKM.Gender switch { 0 => "♂", 1 => "♀", _ => "-" };

    public PKMDatabaseEntry(PKM pkm, ISpriteRenderer renderer)
    {
        PKM = pkm;
        Sprite = renderer.GetSprite(pkm);
    }

    public void Refresh()
    {
        OnPropertyChanged(string.Empty);
    }
}
