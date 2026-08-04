using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class DonutEditorViewModel : ViewModelBase
{
    private readonly SAV9ZA _sav;
    private readonly DonutPocket9a _pocket;

    public DonutEditorViewModel(SaveFile sav)
    {
        _sav = (SAV9ZA)sav;
        _pocket = _sav.Donuts;

        // The donut block only exists once the feature is unlocked in-game; on saves without it
        // the accessor substitutes an empty dummy block, and reading any slot would throw.
        IsSupported = _pocket.Data.Length >= DonutPocket9a.MaxCount * Donut9a.Size;
        if (!IsSupported)
            return;

        LoadDonuts();
        LoadFlavorOptions();
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<DonutEntryViewModel> _donuts = [];

    [ObservableProperty]
    private DonutEntryViewModel? _selectedDonut;

    // --- Generator ---
    [ObservableProperty]
    private ObservableCollection<DonutFlavorOptionViewModel> _flavorOptions = [];

    [ObservableProperty]
    private int _generateStart;

    [ObservableProperty]
    private int _generateEnd = DonutPocket9a.MaxCount;

    /// <summary>
    /// Builds the selectable flavor list, matching the upstream generator which keeps only flavors whose
    /// two-digit type index (characters 6-7 of the internal name, e.g. <c>sweet_03</c>) is in the range [3, 21].
    /// </summary>
    private void LoadFlavorOptions()
    {
        FlavorOptions.Clear();
        foreach (var (hash, name) in DonutInfo.Flavors)
        {
            if (!TryGetFlavorTypeIndex(name, out _))
                continue;
            FlavorOptions.Add(new DonutFlavorOptionViewModel(name, hash) { IsSelected = true });
        }
    }

    private static bool TryGetFlavorTypeIndex(string name, out int typeIndex)
    {
        typeIndex = -1;
        if (name.Length < 8 || !int.TryParse(name.AsSpan(6, 2), out var value) || value is < 3 or > 21)
            return false;

        typeIndex = value - 3;
        return true;
    }

    private void LoadDonuts()
    {
        Donuts.Clear();
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            var donut = _pocket.GetDonut(i);
            Donuts.Add(new DonutEntryViewModel(i, donut));
        }

        if (Donuts.Count > 0)
            SelectedDonut = Donuts[0];
    }

    [RelayCommand]
    private void RandomizeAll()
    {
        _pocket.SetAllRandomLv3();
        LoadDonuts();
    }

    [RelayCommand]
    private void CloneCurrent()
    {
        if (SelectedDonut == null) return;
        _pocket.CloneAllFromIndex(SelectedDonut.Index);
        LoadDonuts();
    }

    [RelayCommand]
    private void ShinyAssortment()
    {
        _pocket.SetAllAsShinyTemplate();
        LoadDonuts();
    }

    [RelayCommand]
    private void Compress()
    {
        _pocket.Compress();
        LoadDonuts();
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadDonuts();
    }

    [RelayCommand]
    private void Generate()
    {
        var hashes = FlavorOptions.Where(z => z.IsSelected).Select(z => z.Hash).ToArray();
        if (hashes.Length == 0)
            return;

        var start = Math.Clamp(GenerateStart, 0, DonutPocket9a.MaxCount);
        var end = Math.Clamp(GenerateEnd, 0, DonutPocket9a.MaxCount);
        if (start > end)
            return;

        _pocket.SetRandomShinyTemplateRange(hashes, start, end);
        _sav.State.Edited = true;
        LoadDonuts();
    }
}

public partial class DonutFlavorOptionViewModel : ViewModelBase
{
    public DonutFlavorOptionViewModel(string name, ulong hash)
    {
        Name = name;
        Hash = hash;
    }

    public string Name { get; }
    public ulong Hash { get; }

    [ObservableProperty]
    private bool _isSelected;
}

public partial class DonutEntryViewModel : ViewModelBase
{
    private readonly Donut9a _donut;

    public DonutEntryViewModel(int index, Donut9a donut)
    {
        Index = index;
        _donut = donut;

        _stars = donut.Stars;
        _calories = donut.Calories;
        _levelBoost = donut.LevelBoost;
        _donutType = donut.Donut;
        
        _berryName = donut.BerryName;
        _berry1 = donut.Berry1;
        _berry2 = donut.Berry2;
        _berry3 = donut.Berry3;
        _berry4 = donut.Berry4;
        _berry5 = donut.Berry5;
        _berry6 = donut.Berry6;
        _berry7 = donut.Berry7;
        _berry8 = donut.Berry8;

        _flavor0 = donut.Flavor0;
        _flavor1 = donut.Flavor1;
        _flavor2 = donut.Flavor2;
    }

    public int Index { get; }
    public string DisplayName => $"#{Index + 1:000} - {(_donut.IsEmpty ? "Empty" : $"{Stars}⭐")}";

    [ObservableProperty]
    private byte _stars;
    partial void OnStarsChanged(byte value) => _donut.Stars = value;

    [ObservableProperty]
    private ushort _calories;
    partial void OnCaloriesChanged(ushort value) => _donut.Calories = value;

    [ObservableProperty]
    private byte _levelBoost;
    partial void OnLevelBoostChanged(byte value) => _donut.LevelBoost = value;

    [ObservableProperty]
    private ushort _donutType;
    partial void OnDonutTypeChanged(ushort value) => _donut.Donut = value;

    [ObservableProperty] private ushort _berryName;
    [ObservableProperty] private ushort _berry1;
    [ObservableProperty] private ushort _berry2;
    [ObservableProperty] private ushort _berry3;
    [ObservableProperty] private ushort _berry4;
    [ObservableProperty] private ushort _berry5;
    [ObservableProperty] private ushort _berry6;
    [ObservableProperty] private ushort _berry7;
    [ObservableProperty] private ushort _berry8;

    partial void OnBerryNameChanged(ushort value) => _donut.BerryName = value;
    partial void OnBerry1Changed(ushort value) => _donut.Berry1 = value;
    partial void OnBerry2Changed(ushort value) => _donut.Berry2 = value;
    partial void OnBerry3Changed(ushort value) => _donut.Berry3 = value;
    partial void OnBerry4Changed(ushort value) => _donut.Berry4 = value;
    partial void OnBerry5Changed(ushort value) => _donut.Berry5 = value;
    partial void OnBerry6Changed(ushort value) => _donut.Berry6 = value;
    partial void OnBerry7Changed(ushort value) => _donut.Berry7 = value;
    partial void OnBerry8Changed(ushort value) => _donut.Berry8 = value;

    [ObservableProperty] private ulong _flavor0;
    [ObservableProperty] private ulong _flavor1;
    [ObservableProperty] private ulong _flavor2;

    partial void OnFlavor0Changed(ulong value) => _donut.Flavor0 = value;
    partial void OnFlavor1Changed(ulong value) => _donut.Flavor1 = value;
    partial void OnFlavor2Changed(ulong value) => _donut.Flavor2 = value;

    public string Flavor0Name => GetFlavorName(Flavor0);
    public string Flavor1Name => GetFlavorName(Flavor1);
    public string Flavor2Name => GetFlavorName(Flavor2);

    private string GetFlavorName(ulong hash)
    {
        if (DonutInfo.TryGetFlavorName(hash, out var name))
            return name;
        return hash == 0 ? "None" : $"Unknown ({hash:X16})";
    }

    [RelayCommand]
    private void Recalculate()
    {
        _donut.RecalculateDonutStats();
        Stars = _donut.Stars;
        Calories = _donut.Calories;
        LevelBoost = _donut.LevelBoost;
    }
}
