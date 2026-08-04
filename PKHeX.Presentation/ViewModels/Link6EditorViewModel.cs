using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;
using System.IO;

namespace PKHeX.Presentation.ViewModels;

public partial class Link6EditorViewModel : ViewModelBase
{
    private readonly SAV6? _sav;
    private readonly ISaveBlock6Main? _savBlock;
    private readonly PL6? _gifts;
    private readonly IDialogService _dialogService;
    private readonly string[] _itemNames;
    private readonly System.Collections.Generic.IReadOnlyList<string> _speciesNames;

    public Link6EditorViewModel(SaveFile sav, IDialogService dialogService)
    {
        _sav = sav as SAV6;
        _savBlock = sav as ISaveBlock6Main;
        _dialogService = dialogService;
        IsSupported = _sav is not null && _savBlock is not null;

        if (IsSupported)
        {
            _gifts = _savBlock!.Link.Gifts;
            _itemNames = GameInfo.Strings.GetItemStrings(_sav!.Context, _sav.Version);
            _speciesNames = GameInfo.Strings.Species;
            LoadData();
        }
        else
        {
            _itemNames = [];
            _speciesNames = [];
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty] private string _origin = string.Empty;
    [ObservableProperty] private ushort _battlePoints;
    [ObservableProperty] private ushort _pokeMiles;
    [ObservableProperty] private bool _isEnabled;

    public ObservableCollection<LinkItemViewModel> Items { get; } = [];
    public ObservableCollection<LinkPokemonViewModel> Pokemon { get; } = [];

    private void LoadData()
    {
        if (_gifts is null) return;

        Origin = _gifts.Origin;
        BattlePoints = _gifts.BattlePoints;
        PokeMiles = _gifts.Pokemiles;
        IsEnabled = _gifts.Enabled;

        Items.Clear();
        Items.Add(new LinkItemViewModel(1, () => _gifts.Item1, v => _gifts.Item1 = v, () => _gifts.Quantity1, v => _gifts.Quantity1 = v, _itemNames));
        Items.Add(new LinkItemViewModel(2, () => _gifts.Item2, v => _gifts.Item2 = v, () => _gifts.Quantity2, v => _gifts.Quantity2 = v, _itemNames));
        Items.Add(new LinkItemViewModel(3, () => _gifts.Item3, v => _gifts.Item3 = v, () => _gifts.Quantity3, v => _gifts.Quantity3 = v, _itemNames));
        Items.Add(new LinkItemViewModel(4, () => _gifts.Item4, v => _gifts.Item4 = v, () => _gifts.Quantity4, v => _gifts.Quantity4 = v, _itemNames));
        Items.Add(new LinkItemViewModel(5, () => _gifts.Item5, v => _gifts.Item5 = v, () => _gifts.Quantity5, v => _gifts.Quantity5 = v, _itemNames));
        Items.Add(new LinkItemViewModel(6, () => _gifts.Item6, v => _gifts.Item6 = v, () => _gifts.Quantity6, v => _gifts.Quantity6 = v, _itemNames));

        Pokemon.Clear();
        Pokemon.Add(new LinkPokemonViewModel(1, _gifts.Entity1, _speciesNames));
        Pokemon.Add(new LinkPokemonViewModel(2, _gifts.Entity2, _speciesNames));
        Pokemon.Add(new LinkPokemonViewModel(3, _gifts.Entity3, _speciesNames));
        Pokemon.Add(new LinkPokemonViewModel(4, _gifts.Entity4, _speciesNames));
        Pokemon.Add(new LinkPokemonViewModel(5, _gifts.Entity5, _speciesNames));
        Pokemon.Add(new LinkPokemonViewModel(6, _gifts.Entity6, _speciesNames));
    }

    [RelayCommand]
    private void Save()
    {
        if (_sav is null || _savBlock is null || _gifts is null) return;

        _gifts.Origin = Origin;
        _gifts.BattlePoints = BattlePoints;
        _gifts.Pokemiles = PokeMiles;
        _gifts.Enabled = IsEnabled;
        
        // Items are updated directly via delegates
        
        // Write back to save
        _gifts.Data.CopyTo(_savBlock.Link.PL6.Span);
        _savBlock.Link.RefreshChecksum();
        _sav.State.Edited = true;
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        if (_gifts is null) return;
        var path = await _dialogService.OpenFileAsync("Import Link Data", ["*.pl6"]);
        if (path is null) return;

        byte[] data;
        try
        {
            data = await File.ReadAllBytesAsync(path);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("File_CouldNotReadFile", ex.Message));
            return;
        }

        if (data.Length != PL6.Size)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance["Link6Editor_InvalidFileSize"]);
            return;
        }

        data.CopyTo(_gifts.Data);
        LoadData();
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (_gifts is null) return;
        var path = await _dialogService.SaveFileAsync(LocalizedStrings.Instance["Link6Editor_ExportLinkDataTitle"], "pokemon.pl6", ["*.pl6"]);
        if (path is null) return;

        try
        {
            await File.WriteAllBytesAsync(path, _gifts.Data.ToArray());
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("File_CouldNotWriteFile", ex.Message));
        }
    }
}

public partial class LinkItemViewModel : ViewModelBase
{
    private readonly System.Action<ushort> _setItem;
    private readonly System.Action<ushort> _setQty;
    public string[] ItemNames { get; }

    public LinkItemViewModel(int index, System.Func<ushort> getItem, System.Action<ushort> setItem, System.Func<ushort> getQty, System.Action<ushort> setQty, string[] itemNames)
    {
        Index = index;
        _setItem = setItem;
        _setQty = setQty;
        ItemNames = itemNames;
        _item = getItem();
        _quantity = getQty();
    }

    public int Index { get; }

    [ObservableProperty]
    private ushort _item;

    partial void OnItemChanged(ushort value) => _setItem(value);

    [ObservableProperty]
    private ushort _quantity;

    partial void OnQuantityChanged(ushort value) => _setQty(value);
}

public partial class LinkPokemonViewModel : ViewModelBase
{
    private readonly LinkEntity6 _entity;

    public LinkPokemonViewModel(int index, LinkEntity6 entity, System.Collections.Generic.IReadOnlyList<string> speciesNames)
    {
        Index = index;
        _entity = entity;
        SpeciesName = GetSpeciesName(entity.Species, speciesNames);
        Nickname = entity.Nickname;
        Level = entity.Level;
    }

    public int Index { get; }
    public string SpeciesName { get; }
    public string Nickname { get; }
    public int Level { get; }

    private static string GetSpeciesName(ushort species, System.Collections.Generic.IReadOnlyList<string> speciesNames)
    {
        if (species == 0) return "(None)";
        if (species < speciesNames.Count) return speciesNames[species];
        return $"Species {species}";
    }
}
