using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 Link event editor.
/// Edits link data including items, quantities, battle points, and pokemiles.
/// </summary>
public partial class SAVLink6ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly ISaveBlock6Main _clone;
    private readonly PL6 _gifts;

    public List<ComboItem> ItemsList { get; }

    [ObservableProperty]
    private string _linkSource = string.Empty;

    [ObservableProperty]
    private bool _linkAvailable;

    [ObservableProperty]
    private int _battlePoints;

    [ObservableProperty]
    private int _pokemiles;

    [ObservableProperty]
    private int _item1;

    [ObservableProperty]
    private int _item2;

    [ObservableProperty]
    private int _item3;

    [ObservableProperty]
    private int _item4;

    [ObservableProperty]
    private int _item5;

    [ObservableProperty]
    private int _item6;

    [ObservableProperty]
    private int _quantity1;

    [ObservableProperty]
    private int _quantity2;

    [ObservableProperty]
    private int _quantity3;

    [ObservableProperty]
    private int _quantity4;

    [ObservableProperty]
    private int _quantity5;

    [ObservableProperty]
    private int _quantity6;

    [ObservableProperty]
    private string _pkm1 = string.Empty;

    [ObservableProperty]
    private string _pkm2 = string.Empty;

    [ObservableProperty]
    private string _pkm3 = string.Empty;

    [ObservableProperty]
    private string _pkm4 = string.Empty;

    [ObservableProperty]
    private string _pkm5 = string.Empty;

    [ObservableProperty]
    private string _pkm6 = string.Empty;

    public SAVLink6ViewModel(SaveFile sav) : base(sav)
    {
        _origin = sav;
        var clone = sav.Clone();
        _clone = (ISaveBlock6Main)clone;
        _gifts = _clone.Link.Gifts;

        ItemsList = GameInfo.FilteredSources.Items.ToList();

        LoadLinkData();
    }

    private void LoadLinkData()
    {
        LinkSource = _gifts.Origin;
        LinkAvailable = _gifts.Enabled;

        BattlePoints = _gifts.BattlePoints;
        Pokemiles = _gifts.Pokemiles;

        Item1 = _gifts.Item1;
        Item2 = _gifts.Item2;
        Item3 = _gifts.Item3;
        Item4 = _gifts.Item4;
        Item5 = _gifts.Item5;
        Item6 = _gifts.Item6;

        Quantity1 = _gifts.Quantity1;
        Quantity2 = _gifts.Quantity2;
        Quantity3 = _gifts.Quantity3;
        Quantity4 = _gifts.Quantity4;
        Quantity5 = _gifts.Quantity5;
        Quantity6 = _gifts.Quantity6;

        Pkm1 = GetSpecies(_gifts.Entity1.Species);
        Pkm2 = GetSpecies(_gifts.Entity2.Species);
        Pkm3 = GetSpecies(_gifts.Entity3.Species);
        Pkm4 = GetSpecies(_gifts.Entity4.Species);
        Pkm5 = GetSpecies(_gifts.Entity5.Species);
        Pkm6 = GetSpecies(_gifts.Entity6.Species);
    }

    private static string GetSpecies(ushort species)
    {
        var arr = GameInfo.Strings.Species;
        if (species < arr.Count)
            return arr[species];
        return species.ToString();
    }

    [RelayCommand]
    private void Save()
    {
        _gifts.Origin = LinkSource;
        _gifts.Enabled = LinkAvailable;
        _gifts.BattlePoints = (ushort)BattlePoints;
        _gifts.Pokemiles = (ushort)Pokemiles;

        _gifts.Item1 = (ushort)Item1;
        _gifts.Item2 = (ushort)Item2;
        _gifts.Item3 = (ushort)Item3;
        _gifts.Item4 = (ushort)Item4;
        _gifts.Item5 = (ushort)Item5;
        _gifts.Item6 = (ushort)Item6;

        _gifts.Quantity1 = (ushort)Quantity1;
        _gifts.Quantity2 = (ushort)Quantity2;
        _gifts.Quantity3 = (ushort)Quantity3;
        _gifts.Quantity4 = (ushort)Quantity4;
        _gifts.Quantity5 = (ushort)Quantity5;
        _gifts.Quantity6 = (ushort)Quantity6;

        _clone.Link.RefreshChecksum();
        _origin.CopyChangesFrom((SaveFile)_clone);
        Modified = true;
    }
}
