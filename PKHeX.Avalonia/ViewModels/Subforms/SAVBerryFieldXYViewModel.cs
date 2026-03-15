using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single berry plot entry.
/// </summary>
public partial class BerryPlotModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private int _berry;

    [ObservableProperty]
    private int _u1;

    [ObservableProperty]
    private int _u2;

    [ObservableProperty]
    private int _u3;

    [ObservableProperty]
    private int _u4;

    [ObservableProperty]
    private int _u5;

    [ObservableProperty]
    private int _u6;

    [ObservableProperty]
    private int _u7;

    public string DisplayName => $"Plot {Index + 1}";

    public BerryPlotModel(int index) => Index = index;
}

/// <summary>
/// ViewModel for the Gen 6 XY Berry Field viewer.
/// Read-only display of berry plots (no save, just viewing).
/// </summary>
public partial class SAVBerryFieldXYViewModel : SaveEditorViewModelBase
{
    private readonly SAV6XY _sav;

    public ObservableCollection<BerryPlotModel> Plots { get; } = [];

    [ObservableProperty]
    private BerryPlotModel? _selectedPlot;

    [ObservableProperty]
    private int _selectedPlotIndex = -1;

    public SAVBerryFieldXYViewModel(SAV6XY sav) : base(sav)
    {
        _sav = sav;

        for (int i = 0; i < BerryField6XY.Count; i++)
        {
            var model = new BerryPlotModel(i);
            Plots.Add(model);
        }

        if (Plots.Count > 0)
            SelectedPlotIndex = 0;
    }

    partial void OnSelectedPlotIndexChanged(int value)
    {
        if (value < 0 || value >= Plots.Count)
            return;
        LoadPlot(value);
        SelectedPlot = Plots[value];
    }

    private void LoadPlot(int index)
    {
        var span = _sav.BerryField.GetPlot(index);
        var model = Plots[index];
        model.Berry = ReadUInt16LittleEndian(span);
        model.U1 = ReadUInt16LittleEndian(span[(2 * 1)..]);
        model.U2 = ReadUInt16LittleEndian(span[(2 * 2)..]);
        model.U3 = ReadUInt16LittleEndian(span[(2 * 3)..]);
        model.U4 = ReadUInt16LittleEndian(span[(2 * 4)..]);
        model.U5 = ReadUInt16LittleEndian(span[(2 * 5)..]);
        model.U6 = ReadUInt16LittleEndian(span[(2 * 6)..]);
        model.U7 = ReadUInt16LittleEndian(span[(2 * 7)..]);
    }
}
