using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PoffinCaseEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly PoffinCase4? _poffinCase;

    public PoffinCaseEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV4Sinnoh sinnoh)
        {
            _poffinCase = new PoffinCase4(sinnoh);
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Poffin Case (100 slots)";

    [ObservableProperty]
    private ObservableCollection<PoffinViewModel> _poffins = [];

    [ObservableProperty]
    private int _filledCount;

    private void LoadData()
    {
        if (_poffinCase is null) return;

        Poffins.Clear();
        FilledCount = 0;

        for (int i = 0; i < _poffinCase.Poffins.Length; i++)
        {
            var poffin = _poffinCase.Poffins[i];
            var vm = new PoffinViewModel(i, poffin);
            Poffins.Add(vm);
            if (poffin.Type != PoffinFlavor4.None) FilledCount++;
        }
    }

    [RelayCommand]
    private void FillAll()
    {
        _poffinCase?.FillCase();
        LoadData();
    }

    [RelayCommand]
    private void DeleteAll()
    {
        _poffinCase?.DeleteAll();
        LoadData();
    }

    [RelayCommand]
    private void Save()
    {
        _poffinCase?.Save();
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public class PoffinViewModel
{
    private readonly Poffin4 _poffin;

    public PoffinViewModel(int slot, Poffin4 poffin)
    {
        Slot = slot;
        _poffin = poffin;
    }

    public int Slot { get; }
    public string SlotLabel => $"#{Slot + 1:00}";

    public string FlavorName => _poffin.Type switch
    {
        PoffinFlavor4.None => "(Empty)",
        PoffinFlavor4.Foul => "Foul",
        PoffinFlavor4.Rich => "Rich",
        PoffinFlavor4.Overripe => "Overripe",
        PoffinFlavor4.Mild => "Mild (All)",
        PoffinFlavor4.Spicy => "Spicy",
        PoffinFlavor4.Dry => "Dry",
        PoffinFlavor4.Sweet => "Sweet",
        PoffinFlavor4.Bitter => "Bitter",
        PoffinFlavor4.Sour => "Sour",
        _ => _poffin.Type.ToString()
    };

    public byte Level => _poffin.Level;
    public byte Smoothness => _poffin.Smoothness;

    public byte Spicy => _poffin.BoostSpicy;
    public byte Dry => _poffin.BoostDry;
    public byte Sweet => _poffin.BoostSweet;
    public byte Bitter => _poffin.BoostBitter;
    public byte Sour => _poffin.BoostSour;

    public bool IsEmpty => _poffin.Type == PoffinFlavor4.None;

    public string Stats => IsEmpty ? "" : $"Lv.{Level} | ♥{Smoothness}";
}
