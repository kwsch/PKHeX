using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class BerryFieldEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly BerryField6AO? _berryField;

    public BerryFieldEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV6AO ao)
        {
            _berryField = ao.BerryField;
            IsSupported = true;
        }

        if (IsSupported)
            LoadData();
    }

    public bool IsSupported { get; }
    public int PlotCount => BerryField6AO.Count;
    public string GameInfo => $"{_sav.Version} - Berry Field (90 plots)";

    [ObservableProperty]
    private ObservableCollection<BerryPlotViewModel> _plots = [];

    private void LoadData()
    {
        if (_berryField is null) return;

        Plots.Clear();
        var berryNames = Core.GameInfo.Strings.GetItemStrings(_sav.Context, _sav.Version);

        for (int i = 0; i < BerryField6AO.Count; i++)
        {
            var plot = _berryField.GetPlot(i);
            Plots.Add(new BerryPlotViewModel(i, plot.Berry, plot.GrowthStage, plot.Count, berryNames, SetBerry));
        }
    }

    private void SetBerry(int index, ushort berry, byte growth, float count)
    {
        if (_berryField is null) return;
        var plot = _berryField.GetPlot(index);
        plot.Berry = berry;
        plot.GrowthStage = growth;
        plot.Count = count;
    }

    [RelayCommand]
    private void SetAllFullyGrown()
    {
        foreach (var plot in Plots)
        {
            if (plot.Berry > 0 && plot.Berry != BerryPlot6AO.NoBerry)
            {
                plot.GrowthStage = 5;
                plot.Count = 20;
            }
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var plot in Plots)
        {
            plot.Berry = 0;
            plot.GrowthStage = 0;
            plot.Count = 0;
        }
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public partial class BerryPlotViewModel : ViewModelBase
{
    private readonly System.Action<int, ushort, byte, float> _onChanged;
    private readonly string[] _berryNames;

    public BerryPlotViewModel(int index, ushort berry, byte growthStage, float count, string[] berryNames, System.Action<int, ushort, byte, float> onChanged)
    {
        Index = index;
        _berry = berry;
        _growthStage = growthStage;
        _count = count;
        _berryNames = berryNames;
        _onChanged = onChanged;
    }

    public int Index { get; }
    public string PlotLabel => $"Plot {Index + 1}";

    [ObservableProperty]
    private ushort _berry;

    [ObservableProperty]
    private byte _growthStage;

    [ObservableProperty]
    private float _count;

    partial void OnBerryChanged(ushort value) => NotifyChange();
    partial void OnGrowthStageChanged(byte value) => NotifyChange();
    partial void OnCountChanged(float value) => NotifyChange();

    private void NotifyChange()
    {
        _onChanged(Index, Berry, GrowthStage, Count);
        OnPropertyChanged(nameof(BerryName));
    }

    public string BerryName => Berry > 0 && Berry < _berryNames.Length ? _berryNames[Berry] : "(Empty)";
    public string GrowthText => GrowthStage switch
    {
        0 => "Empty",
        1 => "Planted",
        2 => "Sprouting",
        3 => "Growing",
        4 => "Flowering",
        5 => "Berry!",
        _ => $"Stage {GrowthStage}"
    };
}
