using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the donut flavor profile display.
/// Shows the five flavor stats (Spicy, Fresh, Sweet, Bitter, Sour).
/// </summary>
public partial class DonutFlavorProfile9aViewModel : ObservableObject
{
    [ObservableProperty] private int _spicy;
    [ObservableProperty] private int _fresh;
    [ObservableProperty] private int _sweet;
    [ObservableProperty] private int _bitter;
    [ObservableProperty] private int _sour;

    public void LoadFromDonut(Donut9a donut)
    {
        Span<int> stats = stackalloc int[5];
        donut.RecalculateDonutFlavors(stats);

        Spicy = stats[0];
        Fresh = stats[1];
        Sweet = stats[2];
        Bitter = stats[3];
        Sour = stats[4];
    }
}
