using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the individual donut detail editor (embedded in Donut9a).
/// Tracks per-donut fields like berries, flavors, stars, calories.
/// </summary>
public partial class DonutEditor9aViewModel : ObservableObject
{
    [ObservableProperty] private int _stars;
    [ObservableProperty] private int _calories;
    [ObservableProperty] private int _levelBoost;
    [ObservableProperty] private int _donutIndex;

    [ObservableProperty] private int _berry0;
    [ObservableProperty] private int _berry1;
    [ObservableProperty] private int _berry2;
    [ObservableProperty] private int _berry3;
    [ObservableProperty] private int _berry4;
    [ObservableProperty] private int _berry5;
    [ObservableProperty] private int _berry6;
    [ObservableProperty] private int _berry7;
    [ObservableProperty] private int _berry8;

    [ObservableProperty] private int _flavor0;
    [ObservableProperty] private int _flavor1;
    [ObservableProperty] private int _flavor2;

    [ObservableProperty] private string _milliseconds = "0";

    private Donut9a? _donut;
    private bool _loading;

    public void LoadDonut(Donut9a donut)
    {
        _loading = true;
        _donut = donut;

        Stars = donut.Stars;
        Calories = donut.Calories;
        LevelBoost = donut.LevelBoost;
        DonutIndex = donut.Donut;

        Berry0 = donut.BerryName;
        Berry1 = donut.Berry1;
        Berry2 = donut.Berry2;
        Berry3 = donut.Berry3;
        Berry4 = donut.Berry4;
        Berry5 = donut.Berry5;
        Berry6 = donut.Berry6;
        Berry7 = donut.Berry7;
        Berry8 = donut.Berry8;

        Milliseconds = donut.MillisecondsSince1970.ToString();

        _loading = false;
    }

    public void SaveDonut()
    {
        if (_donut is not { } donut)
            return;

        donut.Stars = (byte)Stars;
        donut.Calories = (ushort)Calories;
        donut.LevelBoost = (byte)LevelBoost;
        donut.Donut = (ushort)DonutIndex;

        donut.BerryName = (ushort)Berry0;
        donut.Berry1 = (ushort)Berry1;
        donut.Berry2 = (ushort)Berry2;
        donut.Berry3 = (ushort)Berry3;
        donut.Berry4 = (ushort)Berry4;
        donut.Berry5 = (ushort)Berry5;
        donut.Berry6 = (ushort)Berry6;
        donut.Berry7 = (ushort)Berry7;
        donut.Berry8 = (ushort)Berry8;

        donut.MillisecondsSince1970 = ulong.TryParse(Milliseconds, out var ms) ? ms : 0;
    }

    public void Reset()
    {
        _donut?.Clear();
        if (_donut is { } donut)
            LoadDonut(donut);
    }
}
