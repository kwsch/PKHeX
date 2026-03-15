using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model representing a single ribbon on the entity.
/// </summary>
public partial class RibbonModel : ObservableObject
{
    private readonly RibbonInfo _info;

    /// <summary>Property name of the ribbon (e.g. "RibbonChampionKalos").</summary>
    public string PropertyName => _info.Name;

    /// <summary>Localized display name.</summary>
    public string DisplayName { get; }

    /// <summary>Whether this is a boolean or byte-count ribbon.</summary>
    public bool IsBooleanType => _info.Type is RibbonValueType.Boolean;

    /// <summary>Maximum ribbon count for byte-type ribbons.</summary>
    public int MaxCount => IsBooleanType ? 1 : _info.MaxCount;

    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty]
    private byte _count;

    public RibbonModel(RibbonInfo info, string displayName)
    {
        _info = info;
        DisplayName = displayName;
        _isChecked = info.HasRibbon;
        _count = info.RibbonCount;
    }

    /// <summary>
    /// Writes the current state back to the underlying <see cref="RibbonInfo"/>.
    /// </summary>
    public void WriteBack()
    {
        _info.HasRibbon = IsChecked;
        _info.RibbonCount = Count;
    }

    partial void OnIsCheckedChanged(bool value)
    {
        if (IsBooleanType)
            _info.HasRibbon = value;
    }

    partial void OnCountChanged(byte value)
    {
        if (!IsBooleanType)
        {
            _info.RibbonCount = value;
            _info.HasRibbon = value > 0;
        }
    }
}

/// <summary>
/// ViewModel for the Ribbon Editor subform.
/// Loads all ribbons/marks from the PKM entity and exposes them for editing.
/// </summary>
public partial class RibbonEditorViewModel : ObservableObject
{
    private readonly PKM _entity;
    private readonly IReadOnlyList<RibbonInfo> _ribbonInfoList;

    [ObservableProperty]
    private bool _modified;

    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>All ribbon models.</summary>
    public ObservableCollection<RibbonModel> AllRibbons { get; } = [];

    /// <summary>Filtered ribbon models based on search text.</summary>
    [ObservableProperty]
    private ObservableCollection<RibbonModel> _filteredRibbons = [];

    public RibbonEditorViewModel(PKM pk)
    {
        _entity = pk;
        _ribbonInfoList = RibbonInfo.GetRibbonInfo(pk);

        var ribbonStrings = GameInfo.Strings.Ribbons;
        foreach (var rib in _ribbonInfoList)
        {
            string displayName;
            if (!ribbonStrings.GetNameSafe(rib.Name, out var name))
                displayName = rib.Name;
            else
                displayName = name;

            AllRibbons.Add(new RibbonModel(rib, displayName));
        }

        FilteredRibbons = new ObservableCollection<RibbonModel>(AllRibbons);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredRibbons = new ObservableCollection<RibbonModel>(AllRibbons);
            return;
        }

        var filtered = AllRibbons
            .Where(r => r.DisplayName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                     || r.PropertyName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));
        FilteredRibbons = new ObservableCollection<RibbonModel>(filtered);
    }

    /// <summary>
    /// Saves all ribbon states back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        foreach (var ribbon in AllRibbons)
            ribbon.WriteBack();

        // Write ribbon values back to the entity
        foreach (var rib in _ribbonInfoList)
        {
            if (rib.Type is RibbonValueType.Boolean)
                ReflectUtil.SetValue(_entity, rib.Name, rib.HasRibbon);
            else
                ReflectUtil.SetValue(_entity, rib.Name, rib.RibbonCount);
        }

        Modified = true;
    }

    /// <summary>
    /// Sets all ribbons to their maximum values.
    /// </summary>
    [RelayCommand]
    private void SelectAll()
    {
        foreach (var ribbon in AllRibbons)
        {
            if (ribbon.IsBooleanType)
                ribbon.IsChecked = true;
            else
                ribbon.Count = (byte)ribbon.MaxCount;
        }
    }

    /// <summary>
    /// Clears all ribbons.
    /// </summary>
    [RelayCommand]
    private void SelectNone()
    {
        foreach (var ribbon in AllRibbons)
        {
            if (ribbon.IsBooleanType)
                ribbon.IsChecked = false;
            else
                ribbon.Count = 0;
        }
    }
}
