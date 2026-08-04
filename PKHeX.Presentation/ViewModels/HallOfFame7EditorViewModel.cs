using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class HallOfFame7EditorViewModel : ViewModelBase
{
    private readonly SAV7? _sav7;
    private readonly HallOfFame7? _fame;

    public HallOfFame7EditorViewModel(SaveFile sav)
    {
        _sav7 = sav as SAV7;
        IsSupported = _sav7?.EventWork?.Fame is not null;

        if (_sav7 is not null)
        {
            _fame = _sav7.EventWork.Fame;
            LoadSpeciesOptions();
            LoadEntries();

            // Handle USUM starter EC
            if (_sav7 is SAV7USUM usum)
            {
                _starterEc = usum.Misc.StarterEncryptionConstant.ToString("X8");
                HasStarterEc = true;
            }
        }
    }

    public bool IsSupported { get; }
    public bool HasStarterEc { get; }

    [ObservableProperty]
    private ObservableCollection<ComboItem> _speciesOptions = [];

    [ObservableProperty]
    private ObservableCollection<HallOfFameEntry7> _firstEntries = [];

    [ObservableProperty]
    private ObservableCollection<HallOfFameEntry7> _currentEntries = [];

    [ObservableProperty]
    private string _starterEc = "";

    partial void OnStarterEcChanged(string value)
    {
        if (_sav7 is SAV7USUM usum && uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var ec))
            usum.Misc.StarterEncryptionConstant = ec;
    }

    private void LoadSpeciesOptions()
    {
        SpeciesOptions.Clear();
        var species = GameInfo.FilteredSources.Species;
        foreach (var item in species)
            SpeciesOptions.Add(item);
    }

    private void LoadEntries()
    {
        if (_fame is null) return;

        FirstEntries.Clear();
        CurrentEntries.Clear();

        // First 6 = First clear, Last 6 = Current
        for (int i = 0; i < 6; i++)
        {
            FirstEntries.Add(new HallOfFameEntry7(i, _fame, SpeciesOptions));
            CurrentEntries.Add(new HallOfFameEntry7(i + 6, _fame, SpeciesOptions));
        }
    }
}

public class HallOfFameEntry7 : ViewModelBase
{
    private readonly int _index;
    private readonly HallOfFame7 _fame;

    public HallOfFameEntry7(int index, HallOfFame7 fame, ObservableCollection<ComboItem> speciesOptions)
    {
        _index = index;
        _fame = fame;
        SpeciesOptions = speciesOptions;
        _selectedSpecies = (int)_fame.GetEntry(index);
    }

    public ObservableCollection<ComboItem> SpeciesOptions { get; }

    public string Label => $"Slot {(_index % 6) + 1}";

    private int _selectedSpecies;
    public int SelectedSpecies
    {
        get => _selectedSpecies;
        set
        {
            if (SetProperty(ref _selectedSpecies, value))
                _fame.SetEntry(_index, (ushort)value);
        }
    }
}
