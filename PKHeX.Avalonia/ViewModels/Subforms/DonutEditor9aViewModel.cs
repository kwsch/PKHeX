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

        donut.Stars = (byte)Math.Clamp(Stars, 0, 255);
        donut.Calories = (ushort)Math.Clamp(Calories, 0, ushort.MaxValue);
        donut.LevelBoost = (byte)Math.Clamp(LevelBoost, 0, 255);
        donut.Donut = (ushort)Math.Clamp(DonutIndex, 0, ushort.MaxValue);

        donut.BerryName = (ushort)Math.Clamp(Berry0, 0, ushort.MaxValue);
        donut.Berry1 = (ushort)Math.Clamp(Berry1, 0, ushort.MaxValue);
        donut.Berry2 = (ushort)Math.Clamp(Berry2, 0, ushort.MaxValue);
        donut.Berry3 = (ushort)Math.Clamp(Berry3, 0, ushort.MaxValue);
        donut.Berry4 = (ushort)Math.Clamp(Berry4, 0, ushort.MaxValue);
        donut.Berry5 = (ushort)Math.Clamp(Berry5, 0, ushort.MaxValue);
        donut.Berry6 = (ushort)Math.Clamp(Berry6, 0, ushort.MaxValue);
        donut.Berry7 = (ushort)Math.Clamp(Berry7, 0, ushort.MaxValue);
        donut.Berry8 = (ushort)Math.Clamp(Berry8, 0, ushort.MaxValue);

        donut.MillisecondsSince1970 = ulong.TryParse(Milliseconds, out var ms) ? ms : 0;
    }

    public void Reset()
    {
        _donut?.Clear();
        if (_donut is { } donut)
            LoadDonut(donut);
    }
}
