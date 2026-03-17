using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a donut in the list.
/// </summary>
public partial class DonutListItemModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty] private string _displayName = string.Empty;

    public DonutListItemModel(int index) => Index = index;
}

/// <summary>
/// ViewModel for the ZA Donut management editor.
/// </summary>
public partial class Donut9aViewModel : SaveEditorViewModelBase
{
    private readonly SAV9ZA _origin;
    private readonly SAV9ZA _sav;
    private readonly DonutPocket9a _donuts;

    public ObservableCollection<DonutListItemModel> DonutList { get; } = [];

    [ObservableProperty] private DonutListItemModel? _selectedDonut;

    // Current donut detail fields
    [ObservableProperty] private int _stars;
    [ObservableProperty] private int _calories;
    [ObservableProperty] private int _levelBoost;
    [ObservableProperty] private int _donutType;

    private bool _loading;

    public Donut9aViewModel(SAV9ZA sav) : base(sav)
    {
        _sav = (SAV9ZA)(_origin = sav).Clone();
        _donuts = _sav.Donuts;

        _loading = true;
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            var item = new DonutListItemModel(i) { DisplayName = GetDonutDisplayName(i) };
            DonutList.Add(item);
        }

        if (DonutList.Count > 0)
            SelectedDonut = DonutList[0];
        _loading = false;

        if (SelectedDonut != null)
            LoadDonut(SelectedDonut.Index);
    }

    private string GetDonutDisplayName(int i)
    {
        var donut = _donuts.GetDonut(i);
        var flavorString = new string('*', donut.FlavorCount);
        return $"#{i + 1:000} {donut.Stars}* @ {donut.Calories:0000} cal {flavorString}";
    }

    partial void OnSelectedDonutChanged(DonutListItemModel? value)
    {
        if (_loading || value is null)
            return;
        SaveCurrentDonut();
        LoadDonut(value.Index);
    }

    private int _currentIndex;

    private void LoadDonut(int index)
    {
        _loading = true;
        _currentIndex = index;
        var donut = _donuts.GetDonut(index);
        Stars = donut.Stars;
        Calories = donut.Calories;
        LevelBoost = donut.LevelBoost;
        DonutType = donut.Donut;
        _loading = false;
    }

    private void SaveCurrentDonut()
    {
        if (_loading)
            return;
        var donut = _donuts.GetDonut(_currentIndex);
        donut.Stars = (byte)Math.Clamp(Stars, 0, 255);
        donut.Calories = (ushort)Math.Clamp(Calories, 0, ushort.MaxValue);
        donut.LevelBoost = (byte)Math.Clamp(LevelBoost, 0, 255);
        donut.Donut = (ushort)Math.Clamp(DonutType, 0, ushort.MaxValue);
    }

    private void ReloadDonutNames()
    {
        for (int i = 0; i < DonutList.Count; i++)
            DonutList[i].DisplayName = GetDonutDisplayName(i);
    }

    [RelayCommand]
    private void RandomizeAll()
    {
        _donuts.SetAllRandomLv3();
        ReloadDonutNames();
        if (SelectedDonut != null)
            LoadDonut(SelectedDonut.Index);
    }

    [RelayCommand]
    private void CloneCurrent()
    {
        SaveCurrentDonut();
        _donuts.CloneAllFromIndex(_currentIndex);
        ReloadDonutNames();
    }

    [RelayCommand]
    private void ShinyAssortment()
    {
        _donuts.SetAllAsShinyTemplate();
        ReloadDonutNames();
        if (SelectedDonut != null)
            LoadDonut(SelectedDonut.Index);
    }

    [RelayCommand]
    private void ResetCurrent()
    {
        var donut = _donuts.GetDonut(_currentIndex);
        donut.Clear();
        LoadDonut(_currentIndex);
        DonutList[_currentIndex].DisplayName = GetDonutDisplayName(_currentIndex);
    }

    [RelayCommand]
    private void Save()
    {
        SaveCurrentDonut();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
